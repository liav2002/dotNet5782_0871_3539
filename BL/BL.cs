using System;
using IDAL;
using System.Collections.Generic;
using System.Linq;
using DalObject;

namespace IBL
{
    namespace BO
    {
        class BL : IBL
        {
            private IDAL.IDAL _dalObj;
            private IEnumerable<IDAL.DO.Drone> _drones;

            private double _chargeRate = DalObject.DataSource.Config.chargeRatePH; // to all the drones

            private double _lightPPK = DalObject.DataSource.Config.lightPPK;
            private double _mediumPPK = DalObject.DataSource.Config.mediumPPK;
            private double _heavyPPK = DalObject.DataSource.Config.heavyPPK;
            private double _avilablePPK = DalObject.DataSource.Config.avilablePPK;

            /**********************************************************************************************
             * Details: this function find the nearest station to drone.                                  *
             * Parameters: drone's current location (lattidue and longtiube).                             *
             * Return: station's id.                                                                      *
             **********************************************************************************************/
            private int _GetNearestStation(double lat, double lon)
            {
                var stations = _dalObj.GetStationsList();

                if (!stations.Any() || stations == null) throw new NonItems("Stations");

                var station = stations.GetEnumerator();

                double min = _Distance(station.Current.Latitude, station.Current.Longitude, lat,
                    lon);
                int stationId = station.Current.Id;

                while (station.MoveNext())
                {
                    double distance = _Distance(station.Current.Latitude, station.Current.Longitude, lat,
                        lon);

                    if (min > distance)
                    {
                        min = distance;
                        stationId = station.Current.Id;
                    }
                }

                return stationId;
            }


            /**********************************************************************************************
             * Details: this function initalized drone's location according to some conditions.           *
             * Parameters: drone, parcel (the drone carried), station (the drone came from).              *
             * Return: None.                                                                              *
             **********************************************************************************************/
            private void _InitDroneLocation(IDAL.DO.Drone drone, IDAL.DO.Parcel parcel, IDAL.DO.Station station)
            {
                if (parcel.PickedUp == default(DateTime)) //the parcel is not pickedup yet
                {
                    drone.Latitude = station.Latitude;
                    drone.Longitube = station.Longitude;
                }

                else if (parcel.Delivered == default(DateTime)) // the parcel is not delivered yet (but it's picked up)
                {
                    var sender = _dalObj.GetCostumerById(parcel.SenderId);
                    drone.Latitude = sender.Latitude;
                    drone.Longitube = sender.Longitude;
                }
            }

            /**********************************************************************************************
             * Details: this function initalized drone's battery according to some conditions.            *
             * Parameters: drone, parcel (the drone carried), station (the drone came from, or move to).  *
             * Return: None.                                                                              *
             **********************************************************************************************/
            private void _InitBattery(IDAL.DO.Drone drone, IDAL.DO.Parcel parcel, IDAL.DO.Station station)
            {
                Random rand = new Random();
                double minBattery = 0;
                double d1 = 0;
                double d2 = 0;
                double d3 = 0;
                var sender = _dalObj.GetCostumerById(parcel.SenderId);
                var target = _dalObj.GetCostumerById(parcel.TargetId);

                if(drone.Status == IDAL.DO.DroneStatuses.Shipping)
                {
                    // first fly - base to parcel (maybe the drone is already in base, it doesn't affect)
                    d1 = _Distance(drone.Latitude, drone.Longitube, sender.Latitude, sender.Longitude);

                    // second fly - sender to target with the parcel
                    d2 = _Distance(sender.Latitude, sender.Longitude, target.Latitude, target.Longitude);

                    // last fly - returning to the nearest base from target, without the parcel
                    station = this._dalObj.GetStationById(_GetNearestStation(target.Latitude, target.Longitude));
                    d3 = _Distance(target.Latitude, target.Longitude, station.Latitude, station.Longitude);

                    minBattery = (d1 + d3) * DataSource.Config.avilablePPK;

                    switch (parcel.Weight)
                    {
                        case IDAL.DO.WeightCategories.Heavy:
                            {
                                minBattery += d2 * DataSource.Config.heavyPPK;
                                break;
                            }
                        case IDAL.DO.WeightCategories.Medium:
                            {
                                minBattery += d2 * DataSource.Config.mediumPPK;
                                break;
                            }
                        case IDAL.DO.WeightCategories.Light:
                            {
                                minBattery += d2 * DataSource.Config.lightPPK;
                                break;
                            }
                    }
                }

                else if(drone.Status == IDAL.DO.DroneStatuses.Available)
                {
                    minBattery = _Distance(drone.Latitude, drone.Longitube, station.Latitude, station.Longitude) * DataSource.Config.avilablePPK;
                }

                // set the drone's battery random value between minBattery to 100
                drone.Battery = (rand.NextDouble() * (100 - minBattery)) + minBattery;
            }

            private double _Radians(double x)
            {
                return x * Math.PI / 180;
            }

            /**********************************************************************************************
             * Details: this function calculate distance between to points with lattiude and longitube.   *
             * Parameters: 2 points values (lattidue and longitube)                                       *
             * Return: distatnce in km.                                                                   *
             **********************************************************************************************/
            private double _Distance(double lat1, double lon1, double lat2,
                double lon2) // the return value is in km
            {
                const double RADIUS = 6371.0088; // the average Radius of earth


                double dlon = _Radians(lon2 - lon1);
                double dlat = _Radians(lat2 - lat1);

                double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(_Radians(lat1)) *
                    Math.Cos(_Radians(lat2)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
                double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                return angle * RADIUS;
            }

            /**********************************************************************************************
             * Details: this function calculate size of enumurator from type T.                           *
             * Parameters: IEnumrator object type T.                                                      *
             * Return: size - numer of elements in the object.                                            *
             **********************************************************************************************/
            private int _LengthIEnumerator<T>(IEnumerable<T> ienumerable)
            {
                int count = 0;
                foreach (var item in ienumerable)
                    count++;
                return count;
            }

            public BL()
            {
                // handle all drones, init their values (location, battery, etc):

                this._dalObj = new DalObject.DalObject();
                this._drones = _dalObj.GetDroneList();
                
                //find all the drones which was assign to a parcel, and change there status to Shiping
                IEnumerable<IDAL.DO.Parcel> parcels = _dalObj.GetParcelsList();
                foreach (var parcel in parcels)
                {
                    if (parcel.DroneId != 0) // the parcel has been assigned to drone, in this part I handle all the shiping drones.
                    {
                        // for every parcel that have a drone
                        var drone = _dalObj.GetDroneById(parcel.DroneId);
                        var sender = _dalObj.GetCostumerById(parcel.SenderId);
                        var nearStation =
                            _dalObj.GetStationById(_GetNearestStation(sender.Latitude, sender.Longitude));

                        drone.Status = IDAL.DO.DroneStatuses.Shipping; // change the drone status to Shipping

                        _InitDroneLocation(drone, parcel, nearStation); // set drone's location

                        _InitBattery(drone, parcel, nearStation); // set drone's battery
                    }
                }

                //move all the drone, set there values.
                foreach (var drone in this._drones)
                {
                    if (drone.Status != IDAL.DO.DroneStatuses.Shipping) //for all the drones that not in shiping.
                                                                        //I dont care from shiping drones, because I already init there value (battery, location).
                    {
                        Random rand = new Random();
                        int status = rand.Next(0, 1);
                        drone.Status = (status == 0) // get random staus avilable or maintance.
                            ? IDAL.DO.DroneStatuses.Available
                            : IDAL.DO.DroneStatuses.Maintenance;
                    }

                    if (drone.Status == IDAL.DO.DroneStatuses.Maintenance) // handle the maintance drones.
                    {
                        Random rand = new Random();
                        var stations = _dalObj.GetStationsList();
                        int counter = _LengthIEnumerator<IDAL.DO.Station>(stations);
                        int index = rand.Next(0, counter - 1);

                        //get random base station
                        var enumerator = stations.GetEnumerator();
                        for (int _ = 0;
                            _ < index;
                            _++)
                        {
                            enumerator.MoveNext();
                        }

                        //set location
                        drone.Latitude = enumerator.Current.Latitude;
                        drone.Longitube = enumerator.Current.Longitude;

                        //set battery
                        drone.Battery = rand.NextDouble() * 20;
                    }

                    else if (drone.Status == IDAL.DO.DroneStatuses.Available) // handle the avilable drones.
                    {
                        Random rand = new Random();

                        // get a list of delivered parcels.
                        List<IDAL.DO.Parcel> deliveredParcels = new List<IDAL.DO.Parcel>();
                        foreach (var parcel in parcels)
                        {
                            if (parcel.Delivered != default(DateTime))
                            {
                                deliveredParcels.Add(parcel);
                            }
                        }

                        // get random parcel from the list.
                        int index = rand.Next(0, deliveredParcels.Count - 1);
                        IDAL.DO.Parcel randParcel = deliveredParcels[index];

                        // get the target from the random delivered parcel
                        IDAL.DO.Costumer randTarget = _dalObj.GetCostumerById(randParcel.TargetId);

                        // set drone's location
                        drone.Latitude = randTarget.Latitude;
                        drone.Longitube = randTarget.Longitude;

                        //get the nearest base station to the target
                        IDAL.DO.Station nearStation = _dalObj.GetStationById(_GetNearestStation(drone.Latitude, drone.Longitube));

                        //according to the nearest base station, get random battery and set it in drone's battery
                        _InitBattery(drone, randParcel, nearStation);
                    }
                }
            }
        }
    }
}