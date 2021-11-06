using System;
using IDAL;
using System.Collections.Generic;
using System.Linq;
using DalObject;

namespace IBL
{
    namespace BO
    {
        public class BL : IBL
        {
            private IDAL.IDAL _dalObj;

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
                int stationId = -1;
                var stations = _dalObj.GetStationsList();

                if (!stations.Any() || stations == null) throw new NonItems("Stations");

                var station = stations.GetEnumerator();

                if(station.MoveNext())
                {
                    double min = _Distance(station.Current.Latitude, station.Current.Longitube, lat,
                    lon);
                    stationId = station.Current.Id;

                    while (station.MoveNext())
                    {
                        double distance = _Distance(station.Current.Latitude, station.Current.Longitube, lat,
                            lon);

                        if (min > distance)
                        {
                            min = distance;
                            stationId = station.Current.Id;
                        }
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
                    drone.Longitube = station.Longitube;
                }

                else if (parcel.Delivered == default(DateTime)) // the parcel is not delivered yet (but it's picked up)
                {
                    var sender = _dalObj.GetCostumerById(parcel.SenderId);
                    drone.Latitude = sender.Latitude;
                    drone.Longitube = sender.Longitube;
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
                    d1 = _Distance(drone.Latitude, drone.Longitube, sender.Latitude, sender.Longitube);

                    // second fly - sender to target with the parcel
                    d2 = _Distance(sender.Latitude, sender.Longitube, target.Latitude, target.Longitube);

                    // last fly - returning to the nearest base from target, without the parcel
                    station = this._dalObj.GetStationById(_GetNearestStation(target.Latitude, target.Longitube));
                    d3 = _Distance(target.Latitude, target.Longitube, station.Latitude, station.Longitube);

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
                    minBattery = _Distance(drone.Latitude, drone.Longitube, station.Latitude, station.Longitube) * DataSource.Config.avilablePPK;
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

            private void HandleAssignParcels()
            {
                //find all the drones which was assign to a parcel, and change there status to Shiping
                IEnumerable<IDAL.DO.Parcel> parcels = _dalObj.GetParcelsList();

                foreach (var parcel in parcels)
                {
                    if (parcel.DroneId != 0) // the parcel has been assigned to drone, in this part I handle all the shiping drones.
                    {
                        HandleAssignParcel(parcel);
                    }
                }
            }

            private void HandleAssignParcel(IDAL.DO.Parcel parcel)
            {
                var drone = _dalObj.GetDroneById(parcel.DroneId);
                var sender = _dalObj.GetCostumerById(parcel.SenderId);
                var nearStation =
                    _dalObj.GetStationById(_GetNearestStation(sender.Latitude, sender.Longitube));

                drone.Status = IDAL.DO.DroneStatuses.Shipping; // change the drone status to Shipping

                _InitDroneLocation(drone, parcel, nearStation); // set drone's location

                _InitBattery(drone, parcel, nearStation); // set drone's battery
            }

            private void SetDroneDetails(IDAL.DO.Drone drone)
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
                    drone.Longitube = enumerator.Current.Longitube;

                    //set battery
                    drone.Battery = rand.NextDouble() * 20;
                }

                else if (drone.Status == IDAL.DO.DroneStatuses.Available) // handle the avilable drones.
                {
                    IEnumerable<IDAL.DO.Parcel> parcels = _dalObj.GetParcelsList();
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
                    drone.Longitube = randTarget.Longitube;

                    //get the nearest base station to the target
                    IDAL.DO.Station nearStation = _dalObj.GetStationById(_GetNearestStation(drone.Latitude, drone.Longitube));

                    //according to the nearest base station, get random battery and set it in drone's battery
                    _InitBattery(drone, randParcel, nearStation);
                }
            }

            public BL()
            {
                // handle all drones, init their values (location, battery, etc):

                this._dalObj = new DalObject.DalObject();
                IEnumerable<IDAL.DO.Drone> drones = _dalObj.GetDroneList();
                HandleAssignParcels();

                foreach(var drone in drones)
                {
                    SetDroneDetails(drone);
                }
            }

            /*
       	    *Description: add new Station to stations. check the logic in the parameters, then use dalObject for adding.
       	    *Parameters: station's id, station's name, station's longitude, station's latitude, charge's slots.
       	    *Return: None.
            */
            public void AddStation(int id, string name, double longitube, double latitude, int charge_solts)
            {
                IEnumerable<IDAL.DO.Station> stations = this._dalObj.GetStationsList();

                foreach (var station in stations)
                {
                    if (station.Id == id)
                    {
                        throw new BO.NonUniqueID("Staion's id");
                    }
                }

                if (id < 0)
                {
                    throw new BO.NegetiveValue("Station's id");
                }

                if (charge_solts < 0)
                {
                    throw new BO.NegetiveValue("Charge's slots");
                }

                this._dalObj.AddStation(id, name, longitube, latitude, charge_solts);
            }

            /*
   	        *Description: add new Drone to drones. check logic, then use dalObject for adding.
       	    *Parameters: drone's details.	
       	    *Return: true - if added successfully, false - else.
            */
            public void AddDrone(int id, string model, int maxWeight)
            {
                IEnumerable<IDAL.DO.Drone> drones = _dalObj.GetDroneList();

                foreach (var drone in drones)
                {
                    if (drone.Id == id)
                    {
                        throw new BO.NonUniqueID("Drone's id");
                    }
                }

                if (id < 0)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                if (maxWeight < 0 || maxWeight > 2)
                {
                    throw new BO.WrongEnumValuesRange("maxWeight", "0", "2");
                }

                this._dalObj.AddDrone(id, model, maxWeight);

                this.SetDroneDetails(this._dalObj.GetDroneById(id));
            }

            /*
            *Description: add new Costumer to costumers. check logic, then use dalObject for adding.
            *Parameters: costumer's details.
            *Return: None.
            */
            public void AddCostumer(int id, string name, string phone, double longitube, double latitude)
            {
                IEnumerable<IDAL.DO.Costumer> costumers = _dalObj.GetCostumerList();

                foreach (var costumer in costumers)
                {
                    if (costumer.Id == id)
                    {
                        throw new BO.NonUniqueID("Costumer's id");
                    }
                }

                if (id < 0)
                {
                    throw new BO.NegetiveValue("Costumer's id");
                }

                this._dalObj.AddCostumer(id, name, phone, longitube, latitude);
            }

            /*
            *Description: add new Paracel to paracels. check logic, then use dalObject for adding.
            *Parameters: paracel's detatils.
            *Return: None.
            */
            public void AddParcel(int id, int senderId, int targetId, int weight, int priority, int droneId)
            {
                IEnumerable<IDAL.DO.Parcel> parcels = _dalObj.GetParcelsList();

                foreach (var parcel in parcels)
                {
                    if (parcel.Id == id)
                    {
                        throw new BO.NonUniqueID("Parcel's id");
                    }
                }

                if (targetId == senderId)
                {
                    throw new BO.SelfDelivery();
                }

                if (0 > targetId || 0 > senderId)
                {
                    throw new BO.NegetiveValue("Id");
                }

                if (id < 0)
                {
                    throw new BO.NegetiveValue("Parcel's id");
                }

                this._dalObj.AddParcel(id, senderId, targetId, weight, 
                    priority, DateTime.Now, droneId, default(DateTime), 
                    default(DateTime), default(DateTime));
            }

            /*
            *Description: find avaliable drone for deliverd a paracel.
            *Parameters: a paracel.
            *Return: None.
            */
            public void AssignParcelToDrone(int parcelId)
            {
                IEnumerable<IDAL.DO.Drone> drones = this._dalObj.GetDroneList();
                IDAL.DO.Parcel parcel = this._dalObj.GetParcelById(parcelId);

                if(0 > parcelId) { throw new BO.NegetiveValue("Parcel's id"); }

                foreach (var drone in drones)
                {
                    if (drone.Status == IDAL.DO.DroneStatuses.Available && // if the drone is avalible
                        (int)drone.MaxWeight >=
                        (int)parcel.Weight) // and the drone maxWeight is qual or bigger to the parcel weight
                    {
                        parcel.Scheduled = DateTime.Now;
                        parcel.DroneId = drone.Id;
                        this.HandleAssignParcel(this._dalObj.GetParcelById(parcelId));
                        return; //operation complete - we find an avilable drone, so exit the function.
                    }
                }

                //if there is no any avilable drone
                this._dalObj.MoveParcelToWaitingList(parcel);
            }

            /*
            *Description: update PickedUp time to NOW. check logic.
            *Parameters: a paracel.
            *Return: None.
            */
            public void ParcelCollection(int parcelId)
            {
                if (0 > parcelId) { throw new BO.NegetiveValue("Parcel's id"); }

                IDAL.DO.Parcel parcel = this._dalObj.GetParcelById(parcelId);
                IDAL.DO.Drone drone = this.GetDroneById(parcel.DroneId);
                IDAL.DO.Costumer costumer = this.GetCostumerById(parcel.SenderId);
                drone.Latitude = costumer.Latitude;
                drone.Longitube = costumer.Longitube;
                parcel.PickedUp = DateTime.Now;
            }

            /*
            *Description: update delivered time to NOW. check logic.
            *Parameters: a paracel.
            *Return: None.
            */
            public void ParcelDelivered(int parcelId)
            {
                //check logic
                if (0 > parcelId) { throw new BO.NegetiveValue("Parcel's id"); }

                IDAL.DO.Parcel parcel = this.GetParcelById(parcelId);

                if(parcel.PickedUp == default(DateTime)) { throw new Exception("Parcel is not picked up yet, therefore can't be in delivered status.\n"); }

                //update parcel's details

                parcel.Delivered = DateTime.Now;

                //update drone's details

                IDAL.DO.Drone drone = this.GetDroneById(parcel.DroneId);
                IDAL.DO.Costumer target = this.GetCostumerById(parcel.TargetId);
                IDAL.DO.Station station = this._dalObj.GetStationById(this._GetNearestStation(target.Latitude, target.Longitube));
                drone.Status = IDAL.DO.DroneStatuses.Available;
                drone.Latitude = station.Latitude;
                drone.Longitube = station.Longitube;

                //check if there is any waiting parcels, for assign them to the current drone.

                parcel = this._dalObj.GetNextParcel();

                if(parcel != null) //if there is a waiting parcel
                {
                    this.AssignParcelToDrone(parcel.Id);
                }
            }

            /*
            *Description: Send drone to charge's station. check logic.
            *Parameters: drone's id, station's id.
            *Return: None.
            */
            public void SendDroneToCharge(int droneId, int stationId)
            {
                if (0 > droneId) { throw new BO.NegetiveValue("Drone's id"); }
                if (0 > stationId) { throw new BO.NegetiveValue("Station's id"); }

                IDAL.DO.Drone drone = this._dalObj.GetDroneById(droneId); ;
                IDAL.DO.Station station = this._dalObj.GetStationById(stationId); ;

                if (station.ChargeSolts <= 0)
                {
                    throw new BO.NegetiveValue("Charge's slots");
                }

                station.ChargeSolts--;
                this._dalObj.AddDroneToCharge(drone.Id, station.Id);
                drone.Status = IDAL.DO.DroneStatuses.Maintenance;
            }

            /*
            *Description: release drone from station. check logic.
            *Parameters: a drone.
            *Return: None.
            */
            public void DroneRelease(int droneId)
            {
                if (0 > droneId) { throw new BO.NegetiveValue("Drone's id"); }
                
                this._dalObj.DroneRelease(droneId);

                //check if there is any waiting parcels, for assign them to the current drone.
                IDAL.DO.Parcel nextParcel = this._dalObj.GetNextParcel();

                if (nextParcel != null) //if there is a wairing parcel
                {
                    this.AssignParcelToDrone(nextParcel.Id);
                }
            }

            //getters:

            public IDAL.DO.Parcel GetParcelById(int id)
            {
                if (0 > id) { throw new BO.NegetiveValue("Parcel's id"); }
                return this._dalObj.GetParcelById(id);
            }

            public IDAL.DO.Costumer GetCostumerById(int id)
            {
                if (0 > id) { throw new BO.NegetiveValue("Costumer's id"); }
                return this._dalObj.GetCostumerById(id);
            }

            public IDAL.DO.Station GetStationById(int id)
            {
                if (0 > id) { throw new BO.NegetiveValue("Station's id"); }
                return this._dalObj.GetStationById(id);
            }

            public IDAL.DO.Drone GetDroneById(int id)
            {
                if (0 > id) { throw new BO.NegetiveValue("Drone's id"); }
                return this._dalObj.GetDroneById(id);
            }

            public IDAL.DO.DroneCharge GetDroneChargeByDroneId(int id)
            {
                if (0 > id) { throw new BO.NegetiveValue("Drone's id"); }
                return this._dalObj.GetDroneChargeByDroneId(id);
            }

            public IEnumerable<IDAL.DO.Station> GetStationsList()
            {
                return this._dalObj.GetStationsList();
            }

            public IEnumerable<IDAL.DO.Costumer> GetCostumerList()
            {
                return this._dalObj.GetCostumerList();
            }

            public IEnumerable<IDAL.DO.Parcel> GetParcelsList()
            {
                return this._dalObj.GetParcelsList();
            }

            public IEnumerable<IDAL.DO.Drone> GetDroneList()
            {
                return this._dalObj.GetDroneList();
            }

            public Queue<IDAL.DO.Parcel> GetWaitingParcels()
            {
                return this._dalObj.GetWaitingParcels();
            }
        }
    }
}