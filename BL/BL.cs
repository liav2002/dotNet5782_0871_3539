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
            //Singleton design pattern
            private static BL instance = null;

            public static IBL GetInstance()
            {
                if (instance == null)
                    instance = new BL();
                return instance;
            }

            private IDAL.IDAL _dalObj;
            private BO.SysLog syslog = new BO.SysLog();

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
            private int _GetNearestStation(IDAL.DO.Location location)
            {
                int stationId = -1;
                var stations = _dalObj.GetStationsList();

                if (!stations.Any() || stations == null) throw new NonItems("Stations");

                var station = stations.GetEnumerator();

                if (station.MoveNext())
                {
                    double min = station.Current.Location.Distance(location);
                    stationId = station.Current.Id;

                    while (station.MoveNext())
                    {
                        double distance = station.Current.Location.Distance(location);

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
                    drone.Location = station.Location;
                }

                else if (parcel.Delivered == default(DateTime)) // the parcel is not delivered yet (but it's picked up)
                {
                    var sender = _dalObj.GetCostumerById(parcel.SenderId);
                    drone.Location = station.Location;
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

                if (drone.Status == IDAL.DO.DroneStatuses.Shipping)
                {
                    station = this._dalObj.GetStationById(_GetNearestStation(target.Location));


                    // first fly - base to parcel (maybe the drone is already in base, it doesn't affect)
                    d1 = drone.Location.Distance(sender.Location);

                    // second fly - sender to target with the parcel
                    d2 = sender.Location.Distance(target.Location);

                    // last fly - returning to the nearest base from target, without the parcel
                    d3 = target.Location.Distance(station.Location);

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

                else if (drone.Status == IDAL.DO.DroneStatuses.Available)
                {
                    minBattery = drone.Location.Distance(station.Location) *
                                 DataSource.Config.avilablePPK;
                }

                // set the drone's battery random value between minBattery to 100
                drone.Battery = (rand.NextDouble() * (100 - minBattery)) + minBattery;
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

            private void HandleAssignParcel(IDAL.DO.Parcel parcel, IDAL.DO.Drone drone)
            {
                syslog.HandleAssignParcel(parcel.Id, drone.Id);
                parcel.Scheduled = DateTime.Now;
                parcel.Status = IDAL.DO.ParcelStatuses.Assign;

                parcel.DroneId = drone.Id;

                var sender = _dalObj.GetCostumerById(parcel.SenderId);
                var nearStation =
                    _dalObj.GetStationById(_GetNearestStation(sender.Location));

                syslog.ChangeDroneStatus(drone.Id, IDAL.DO.DroneStatuses.Shipping);
                drone.Status = IDAL.DO.DroneStatuses.Shipping; // change the drone status to Shipping

                syslog.InitDroneLocation(drone.Id);
                _InitDroneLocation(drone, parcel, nearStation); // set drone's location

                syslog.InitDroneBattery(drone.Id);
                _InitBattery(drone, parcel, nearStation); // set drone's battery
            }

            private void SetDroneDetails(IDAL.DO.Drone drone)
            {
                if (drone.Status != IDAL.DO.DroneStatuses.Shipping) //for all the drones that not in shiping.
                    //I dont care from shiping drones, because I already init there value (battery, location).
                {
                    Random rand = new Random();
                    int status = rand.Next(0, 1);

                    // get random staus avilable or maintance.
                    if (status == 0)
                    {
                        syslog.ChangeDroneStatus(drone.Id, IDAL.DO.DroneStatuses.Available);
                        drone.Status = IDAL.DO.DroneStatuses.Available;
                    }
                    else
                    {
                        syslog.ChangeDroneStatus(drone.Id, IDAL.DO.DroneStatuses.Maintenance);
                        drone.Status = IDAL.DO.DroneStatuses.Maintenance;
                    }
                }

                if (drone.Status == IDAL.DO.DroneStatuses.Maintenance) // handle the maintance drones.
                {
                    Random rand = new Random();
                    var stations = _dalObj.GetStationsList();
                    int counter = _LengthIEnumerator<IDAL.DO.Station>(stations);
                    int index = rand.Next(0, counter - 1);

                    //get random base station
                    var station = stations.GetEnumerator();
                    for (int _ = 0;
                        _ < index;
                        _++)
                    {
                        station.MoveNext();
                    }

                    //set location
                    syslog.InitDroneLocation(drone.Id);
                    drone.Location = station.Current.Location;


                    //set battery
                    syslog.InitDroneBattery(drone.Id);
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
                    syslog.InitDroneLocation(drone.Id);

                    drone.Location = randTarget.Location;

                    //get the nearest base station to the target
                    IDAL.DO.Station nearStation =
                        _dalObj.GetStationById(_GetNearestStation(drone.Location));

                    //according to the nearest base station, get random battery and set it in drone's battery
                    syslog.InitDroneBattery(drone.Id);
                    _InitBattery(drone, randParcel, nearStation);
                }
            }

            private void CheckWaitingList(IDAL.DO.Drone drone)
            {
                //check if there is any waiting parcels, for assign them to the current drone.
                syslog.TryHandleWaitingParcels();
                IDAL.DO.Parcel parcel = this._dalObj.GetNextParcel();

                if (parcel != null) //if there is a waiting parcel
                {
                    this.HandleAssignParcel(parcel, drone);
                }
            }

            public BL()
            {
                // handle all drones, init their values (location, battery, etc):

                this._dalObj = DalObject.DalObject.GetInstance(); // Singleton
                IEnumerable<IDAL.DO.Drone> drones = _dalObj.GetDroneList();

                syslog.HandleAssignParcels();

                //find all the drones which was assign to a parcel, and change there status to Shiping
                IEnumerable<IDAL.DO.Parcel> parcels = _dalObj.GetParcelsList();

                foreach (var parcel in parcels)
                {
                    if (
                        parcel.DroneId !=
                        0) // the parcel has been assigned to drone, in this part I handle all the shiping drones.
                    {
                        HandleAssignParcel(parcel, _dalObj.GetDroneById(parcel.DroneId));
                    }
                }

                foreach (var drone in drones)
                {
                    SetDroneDetails(drone);
                }
            }

            /*
       	    *Description: add new Station to stations. check the logic in the parameters, then use dalObject for adding.
       	    *Parameters: station's id, station's name, station's longitude, station's latitude, charge's slots.
       	    *Return: None.
            */
            public void AddStation(int id, string name, double longitude, double latitude, int charge_solts)
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

                syslog.AddStation(id);
                this._dalObj.AddStation(id, name, new IDAL.DO.Location(latitude, longitude), charge_solts);
            }

            /*
   	        *Description: add new Drone to drones. check logic, then use dalObject for adding.
       	    *Parameters: drone's details.	
       	    *Return: true - if added successfully, false - else.
            */
            public void AddDrone(int id, string model, int maxWeight, int stationId)
            {
                IEnumerable<IDAL.DO.Drone> drones = _dalObj.GetDroneList();
                IEnumerable<IDAL.DO.Station> stations = _dalObj.GetStationsList();

                foreach (var drone in drones)
                {
                    if (drone.Id == id)
                    {
                        throw new BO.NonUniqueID("Drone's id");
                    }
                }

                bool flag = false;
                foreach (var station in stations)
                {
                    if (station.Id == stationId)
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag == false) // station id does not exist
                {
                    throw new BO.NonItems("Station with id " + id);
                }

                if (id < 0)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                if (maxWeight < 0 || maxWeight > 2)
                {
                    throw new BO.WrongEnumValuesRange("maxWeight", "0", "2");
                }


                Random random = new Random();
                double battery = random.NextDouble() * 20 + 20; // random double in range (20, 40)
                syslog.AddDrone(id);
                this._dalObj.AddDrone(id, model, (IDAL.DO.WeightCategories) maxWeight, battery);

                // after the drone created we send he to the station:
                SendDroneToCharge(id, stationId);
            }

            /*
            *Description: add new Costumer to costumers. check logic, then use dalObject for adding.
            *Parameters: costumer's details.
            *Return: None.
            */
            public void AddCostumer(int id, string name, string phone, double longitude, double latitude)
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

                syslog.AddCostumer(id);
                this._dalObj.AddCostumer(id, name, phone, new IDAL.DO.Location(longitude, latitude));
            }

            /*
            *Description: add new Parcel to parcels. check logic, then use dalObject for adding.
            *Parameters: parcel's detatils.
            *Return: None.
            */
            public void AddParcel(int senderId, int targetId, int weight, int priority, int droneId)
            {
                IEnumerable<IDAL.DO.Parcel> parcels = _dalObj.GetParcelsList();

                if (targetId == senderId)
                {
                    throw new BO.SelfDelivery();
                }

                if (0 > targetId || 0 > senderId)
                {
                    throw new BO.NegetiveValue("Id");
                }

                int id = DataSource.ParcelsLength() + 1;
                syslog.AddParcel(id);
                this._dalObj.AddParcel(id, senderId, targetId, weight,
                    priority, DateTime.Now, droneId, default(DateTime),
                    default(DateTime), default(DateTime));

                syslog.MoveParcelToWaitingList(id);
                this._dalObj.MoveParcelToWaitingList(this._dalObj.GetParcelById(id));
            }

            /*
            *Description: find avaliable drone for deliverd a parcel.
            *Parameters: a parcel.
            *Return: None.
            */
            public void AssignParcelToDrone(int parcelId)
            {
                IEnumerable<IDAL.DO.Drone> drones = this._dalObj.GetDroneList();

                if (0 > parcelId)
                {
                    throw new BO.NegetiveValue("Parcel's id");
                }

                IDAL.DO.Parcel parcel = this._dalObj.GetParcelById(parcelId);

                if (parcel.DroneId != 0)
                {
                    throw new BO.ParcelAlreadyAssign(parcel.DroneId);
                }

                foreach (var drone in drones)
                {
                    if (drone.Status == IDAL.DO.DroneStatuses.Available && // if the drone is avalible
                        (int) drone.MaxWeight >=
                        (int) parcel.Weight) // and the drone maxWeight is qual or bigger to the parcel weight
                    {
                        this.HandleAssignParcel(parcel, drone);
                        return; //operation complete - we find an avilable drone, so exit the function.
                    }
                }

                //if there is no any avilable drone
                syslog.MoveParcelToWaitingList(parcel.Id);
                this._dalObj.MoveParcelToWaitingList(parcel);
                throw new IDAL.DO.NonAvilableDrones();
            }

            /*
            *Description: update PickedUp time to NOW. check logic.
            *Parameters: a parcel.
            *Return: None.
            */
            public void ParcelCollection(int parcelId)
            {
                if (0 > parcelId)
                {
                    throw new BO.NegetiveValue("Parcel's id");
                }

                IDAL.DO.Parcel parcel = this._dalObj.GetParcelById(parcelId);
                IDAL.DO.Drone drone = this.GetDroneById(parcel.DroneId);
                IDAL.DO.Costumer costumer = this.GetCostumerById(parcel.SenderId);

                syslog.ParcelCollection(parcelId, drone.Id);

                drone.Location = costumer.Location;

                parcel.PickedUp = DateTime.Now;
                parcel.Status = IDAL.DO.ParcelStatuses.PickedUp;

            }

            /*
            *Description: update costumer details.
            *Parameters: costumer's id, new name, new phone number.
            *Return: None.
            */
            public void UpdateCostumer(int costumerId, string name, string phoneNumber)
            {
                //check logic
                if (0 > costumerId)
                {
                    throw new BO.NegetiveValue("Costumer's id");
                }

                IDAL.DO.Costumer costumer = this.GetCostumerById(costumerId);
                if (costumer.Name == name)
                {
                    throw new BO.NotNewValue("Costumer Name", name);
                }

                if (costumer.Phone == phoneNumber)
                {
                    throw new BO.NotNewValue("Costumer phone", phoneNumber);
                }

                if (name != null)
                {
                    costumer.Name = name;
                    syslog.ChangeCostumerName(costumerId, name);
                }

                if (phoneNumber != null)
                {
                    costumer.Phone = phoneNumber;
                    syslog.ChangeCostumerPhone(costumerId, phoneNumber);
                }
            }

            /*
            *Description: update station details.
            *Parameters: station's id, new name, new charge slots.
            *Return: None.
            */

            public void UpdateStation(int stationId, string name, int chargeSlots)
            {
                //check logic
                if (0 > stationId)
                {
                    throw new BO.NegetiveValue("Station's id");
                }

                IDAL.DO.Station station = this.GetStationById(stationId);
                if (station.Name == name)
                {
                    throw new BO.NotNewValue("Station name", name);
                }

                if (station.ChargeSlots == chargeSlots)
                {
                    throw new BO.NotNewValue("Station charge slots", chargeSlots.ToString());
                }

                if (name != null)
                {
                    station.Name = name;
                    syslog.ChangeStationName(stationId, name);
                }

                if (chargeSlots != 0)
                {
                    station.ChargeSlots = chargeSlots;
                    syslog.ChangeStationChargeSlots(stationId, chargeSlots);
                }
            }

            /*
            *Description: update drone name.
            *Parameters: drone's id, new name.
            *Return: None.
            */
            public void UpdateDroneName(int droneId, string name)
            {
                //check logic
                if (0 > droneId)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                IDAL.DO.Drone drone = this.GetDroneById(droneId);

                if (drone.Model == name)
                {
                    throw new BO.NotNewValue("Drone model", name);
                }

                drone.Model = name;
                syslog.ChangeDroneModelName(droneId, name);
            }

            /*
            *Description: update delivered time to NOW. check logic.
            *Parameters: a parcel.
            *Return: None.
            */
            public void ParcelDelivered(int parcelId)
            {
                //check logic
                if (0 > parcelId)
                {
                    throw new BO.NegetiveValue("Parcel's id");
                }

                IDAL.DO.Parcel parcel = this.GetParcelById(parcelId);

                if (parcel.PickedUp == default(DateTime))
                {
                    throw new Exception("Parcel is not picked up yet, therefore can't be in delivered status.\n");
                }

                //update parcel's details

                parcel.Delivered = DateTime.Now;
                parcel.Status = IDAL.DO.ParcelStatuses.Delivered;

                    //update drone's details

                IDAL.DO.Drone drone = this.GetDroneById(parcel.DroneId);
                IDAL.DO.Costumer target = this.GetCostumerById(parcel.TargetId);

                syslog.ParcelDelivered(parcelId, drone.Id);

                syslog.CalculateNearStation("target costumer");
                IDAL.DO.Station station =
                    this._dalObj.GetStationById(this._GetNearestStation(target.Location));

                syslog.ChangeDroneStatus(drone.Id, IDAL.DO.DroneStatuses.Available);
                drone.Status = IDAL.DO.DroneStatuses.Available;

                syslog.InitDroneLocation(drone.Id);

                drone.Location = station.Location;

                //check if there is any waiting parcels, for assign them to the current drone.

                this.CheckWaitingList(drone);
            }

            /*
            *Description: Send drone to charge's station. check logic.
            *Parameters: drone's id, station's id.
            *Return: None.
            */
            public void SendDroneToCharge(int droneId, int stationId)
            {
                if (0 > droneId)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                if (0 > stationId)
                {
                    throw new BO.NegetiveValue("Station's id");
                }

                IDAL.DO.Drone drone = this._dalObj.GetDroneById(droneId);

                IDAL.DO.Station station = this._dalObj.GetStationById(stationId);


                if (station.ChargeSlots <= 0)
                {
                    throw new BO.NegetiveValue("Charge's slots");
                }

                station.ChargeSlots--;

                syslog.InitDroneLocation(droneId);

                drone.Location = station.Location;

                syslog.InitDroneBattery(droneId);
                this._dalObj.AddDroneToCharge(drone.Id, station.Id);

                syslog.ChangeDroneStatus(droneId, IDAL.DO.DroneStatuses.Maintenance);
                drone.Status = IDAL.DO.DroneStatuses.Maintenance;
            }

            /*
            *Description: release drone from station. check logic.
            *Parameters: a drone.
            *Return: None.
            */
            public void DroneRelease(int droneId)
            {
                if (0 > droneId)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                syslog.DroneRelease(droneId);
                this._dalObj.DroneRelease(droneId);

                //check if there is any waiting parcels, for assign them to the current drone.
                this.CheckWaitingList(this._dalObj.GetDroneById(droneId));
            }

            //getters:

            public IDAL.DO.Parcel GetParcelById(int id)
            {
                if (0 > id)
                {
                    throw new BO.NegetiveValue("Parcel's id");
                }

                return this._dalObj.GetParcelById(id);
            }

            public IDAL.DO.Costumer GetCostumerById(int id)
            {
                if (0 > id)
                {
                    throw new BO.NegetiveValue("Costumer's id");
                }

                return this._dalObj.GetCostumerById(id);
            }

            public IDAL.DO.Station GetStationById(int id)
            {
                if (0 >= id)
                {
                    throw new BO.NegetiveValue("Station's id");
                }

                return this._dalObj.GetStationById(id);
            }

            public IDAL.DO.Drone GetDroneById(int id)
            {
                if (0 > id)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                return this._dalObj.GetDroneById(id);
            }

            public IDAL.DO.DroneCharge GetDroneChargeByDroneId(int id)
            {
                if (0 > id)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

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

            public SysLog Sys()
            {
                return this.syslog;
            }
        }
    }
}