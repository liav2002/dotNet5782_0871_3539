using System;
using IDAL;
using System.Collections.Generic;
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

            private int _GetNearestStation(IDAL.DO.Parcel parcel, bool NearToSender) //TODO: there is two options, handle the according to flag.
            {
                var station = _dalObj._getStationList().GetEnumerator();
                var sender = _dalObj._getCostumerById(parcel.SenderId);
                double min = _Distance(station.Current.Latitude, station.Current.Longitude, sender.Latitude, sender.Longitube);
                int stationId = station.Current.Id;

                while (station.MoveNext())
                {
                    double distance = _Distance(station.Current.Latitude, station.Current.Longitude, sender.Latitude, sender.Longitube);

                    if (min > distance)
                    {
                        min = distance;
                        stationId = station.Current.Id;
                    }
                }

                return stationId;
            }

            private void _InitDroneLocation(IDAL.DO.Drone drone, IDAL.DO.Parcel parcel, IDAL.DO.Station station)
            {
                if (parcel.PickedUp == default(DateTime)) //the parcel is not pickedup yet
                {
                    drone.Latitude = station.Latitude;
                    drone.Longitube = station.Longitude;
                }

                else if (parcel.Delivered == default(DateTime)) // the parcel is not delivered yet (but it's picked up)
                {
                    var sender = _dalObj._getCostumerById(parcel.SenderId);
                    drone.Latitude = sender.Latitude;
                    drone.Longitube = sender.Longitube;
                }
            }

            private void _InitBattery(IDAL.DO.Drone drone, IDAL.DO.Parcel parcel, IDAL.DO.Station station)
            {

                Random rand = new Random();

                double minBattery = 0;

                var sender = _dalObj._getCostumerById(parcel.SenderId);
                var target = _dalObj._getCostumerById(parcel.TargetId);

                // first fly - base to parcel (maybe the drone is already in base, it doesn't affect)
                double d1 = _Distance(drone.Latitude, drone.Longitube, sender.Latitude, sender.Longitube);

                // second fly - sender to target with the parcel
                double d2 = _Distance(sender.Latitude, sender.Longitube, target.Latitude, target.Longitube);

                // last fly - returning to base from target, without the parcel
                double d3 = _Distance(target.Latitude, target.Longitube, station.Latitude, station.Longitude);

                minBattery = (d1 + d3) * DataSource.Config.avilablePPK;

                switch (parcel.Weight)
                {
                    case IDAL.DO.WeightCategories.Heavy: { minBattery += d2 * DataSource.Config.heavyPPK; break; }
                    case IDAL.DO.WeightCategories.Medium: { minBattery += d2 * DataSource.Config.mediumPPK; break; }
                    case IDAL.DO.WeightCategories.Light: { minBattery += d2 * DataSource.Config.lightPPK; break; }
                }

                // set the drone's battery random value between minBattery to 100
                drone.Battery = (rand.NextDouble() * (100 - minBattery)) + minBattery;
            }

            private double _Distance(double x1, double y1, double x2, double y2) //TODO: Make sure that the return value is in km
            {
                return Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
            }

            private int _LengthIEnumerator<T>(IEnumerable<T> ienumerable)
            {
                int count = 0;
                foreach (var item in ienumerable)
                    count++;
                return count;
            }

            public BL()
            {
                this._dalObj = new DalObject.DalObject();
                this._drones = _dalObj._getDroneList();

                IEnumerable<IDAL.DO.Parcel> parcels = _dalObj._getParceList();

                foreach (var parcel in parcels)
                {
                    if (parcel.DroneId != 0) // the parcel has been assigned to drone
                    {
                        // for every parcel that have a drone
                        var drone = _dalObj._getDroneById(parcel.DroneId);
                        var nearStation = _dalObj._getStationById(_GetNearestStation(parcel));

                        drone.Status = IDAL.DO.DroneStatuses.Shipping; // change the drone status to shipping

                        _InitDroneLocation(drone, parcel, nearStation);

                        _InitBattery(drone, parcel, nearStation);
                    }
                }

                foreach (var drone in this._drones)
                {
                    if (drone.Status != IDAL.DO.DroneStatuses.Shipping)
                    {
                        Random rand = new Random();
                        int status = rand.Next(0, 1);
                        drone.Status = (status == 0) ? IDAL.DO.DroneStatuses.Available : IDAL.DO.DroneStatuses.Maintenance;
                    }

                    if (drone.Status == IDAL.DO.DroneStatuses.Maintenance)
                    {
                        Random rand = new Random();
                        var stations = _dalObj._getStationList();
                        int counter = _LengthIEnumerator<IDAL.DO.Station>(stations);
                        int index = rand.Next(0, counter - 1);

                        var enumerator = stations.GetEnumerator();
                        for (int _ = 0; _ < index; _++) { enumerator.MoveNext(); }

                        drone.Latitude = enumerator.Current.Latitude;
                        drone.Longitube = enumerator.Current.Longitude;

                        drone.Battery = rand.NextDouble() * 20;
                    }

                    else if (drone.Status == IDAL.DO.DroneStatuses.Available)
                    {
                        Random rand = new Random();
                        List < IDAL.DO.Parcel > deliveredParcels = new List<IDAL.DO.Parcel>();

                        foreach (var parcel in parcels)
                        {
                            if(parcel.Delivered != default(DateTime))
                            {
                                deliveredParcels.Add(parcel);
                            }
                        }

                        int index = rand.Next(0, deliveredParcels.Count - 1);

                        IDAL.DO.Parcel randParcel = deliveredParcels[index];
                        IDAL.DO.Station nearStation = _dalObj._getStationById(_GetNearestStation(randParcel));
                        IDAL.DO.Costumer randTarget = _dalObj._getCostumerById(randParcel.TargetId);

                        drone.Latitude = randTarget.Latitude;
                        drone.Longitube = randTarget.Longitube;

                        //TODO: get min battery for the road: target to near base with avilable ppk)
                    }
                }
            }
        }
    }
}