using System;
using IDAL;
using System.Collections.Generic;
using System.Linq;
using DalObject;
using IDAL.DO;

namespace IBL
{
    namespace BO
    {
        public class BL : IBL
        {
            //Singleton design pattern
            private static BL _instance = null;
            public static BL GetInstance() => _instance ?? (_instance = new BL());


            private PriorityQueue<IDAL.DO.Parcel> _waitingParcels = new PriorityQueue<IDAL.DO.Parcel>();

            private IDAL.IDAL _dalObj;

            private double _chargeRate = DalObject.DataSource.Config.chargeRatePH; // to all the drones

            private double _lightPPK = DalObject.DataSource.Config.lightPPK;
            private double _mediumPPK = DalObject.DataSource.Config.mediumPPK;
            private double _heavyPPK = DalObject.DataSource.Config.heavyPPK;
            private double _avilablePPK = DalObject.DataSource.Config.avilablePPK;

            /**********************************************************************************************
             * Details: this function find the nearest avilable station to drone.                         *
             * Parameters: drone's current location (lattidue and longtiube).                             *
             * Return: station's id.                                                                      *
             **********************************************************************************************/
            private int _GetNearestAvilableStation(IDAL.DO.Location location)
            {
                int stationId = -1;
                var stations = _dalObj.GetStationsList();
                double min = -1;

                if (!stations.Any() || stations == null) throw new NonItems("Stations");

                var station = stations.GetEnumerator();

                while (station.MoveNext())
                {
                    double distance = station.Current.Location.Distance(location);

                    if (min > distance || min == -1)
                    {
                        min = station.Current.ChargeSlots > 0 ? distance : -1;
                        stationId = station.Current.ChargeSlots > 0 ? station.Current.Id : -1;
                    }
                }

                if (min == -1)
                {
                    throw new BO.NotAvilableStation();
                }

                return stationId;
            }

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

                if (minBattery > 100)
                {
                    throw new BO.DroneNotEnoughBattery(drone.Id);
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

            /*
             *I use this function only on AssignParcelToDrone() metohd. 
             */
            private IDAL.DO.Parcel findSuitableParcel(IDAL.DO.Drone drone, int lessPriority = 0)
            {
                //first create a priority queue with all the parcels with highest priority
                PriorityQueue<IDAL.DO.Parcel> parcelsAccordingWeight = new PriorityQueue<IDAL.DO.Parcel>();
                IDAL.DO.Parcel parcelForAssign = this._waitingParcels.Dequeue();
                IDAL.DO.Priorities priority = parcelForAssign.Priority - lessPriority;

                if (priority == parcelForAssign.Priority)
                {
                    parcelsAccordingWeight.Enqueue(parcelForAssign, (int) parcelForAssign.Weight);
                }
                else
                {
                    this._waitingParcels.Enqueue(parcelForAssign, (int) parcelForAssign.Priority);
                }

                bool search = true;

                while (search)
                {
                    parcelForAssign = this._waitingParcels.Dequeue();

                    if (parcelForAssign.Priority == priority)
                    {
                        parcelsAccordingWeight.Enqueue(parcelForAssign, (int) parcelForAssign.Weight);
                    }

                    else
                    {
                        search = false;
                        this._waitingParcels.Enqueue(parcelForAssign, (int) parcelForAssign.Priority);
                    }
                }

                //and check which from the parcels in the list
                //the drone can carry consider the parcel's weight
                search = true;

                while (search)
                {
                    parcelForAssign = parcelsAccordingWeight.Dequeue();

                    if (parcelForAssign.Weight > drone.MaxWeight)
                    {
                        this._waitingParcels.Enqueue(parcelForAssign, (int) parcelForAssign.Priority);
                    }

                    else
                    {
                        parcelsAccordingWeight.Enqueue(parcelForAssign, (int) parcelForAssign.Weight);
                        search = false;
                    }
                }

                if (parcelsAccordingWeight.Count == 0)
                {
                    if (lessPriority >= 0 && lessPriority <= 2)
                    {
                        return findSuitableParcel(drone, lessPriority + 1);
                    }

                    else
                    {
                        throw new BO.NoSuitableParcelForDrone(drone.Id);
                    }
                }

                //second create list with the heaviest parcels, the drone can carry
                List<IDAL.DO.Parcel> suitableParcels = new List<IDAL.DO.Parcel>();
                parcelForAssign = parcelsAccordingWeight.Dequeue();
                suitableParcels.Add(parcelForAssign);
                IDAL.DO.WeightCategories maxWeight = parcelForAssign.Weight;
                search = true;

                while (search)
                {
                    parcelForAssign = parcelsAccordingWeight.Dequeue();

                    if (parcelForAssign.Weight == maxWeight)
                    {
                        suitableParcels.Add(parcelForAssign);
                    }

                    else
                    {
                        this._waitingParcels.Enqueue(parcelForAssign, (int) parcelForAssign.Priority);
                        search = false;
                    }
                }

                //third, if there is more than one heaviest parcel,
                //check which one from them is the nearest to drone
                parcelForAssign = suitableParcels[0];

                if (suitableParcels.Count > 1)
                {
                    double minDistance =
                        drone.Location.Distance(this._dalObj.GetCostumerById(parcelForAssign.SenderId).Location);

                    for (int i = 1; i < suitableParcels.Count; ++i)
                    {
                        IDAL.DO.Parcel parcel = suitableParcels[i];
                        double currentDistance =
                            drone.Location.Distance(this._dalObj.GetCostumerById(parcel.SenderId).Location);

                        if (currentDistance < minDistance)
                        {
                            parcelForAssign = parcel;
                            minDistance = currentDistance;
                        }
                    }

                    for (int i = 0; i < suitableParcels.Count; ++i)
                    {
                        IDAL.DO.Parcel parcel = suitableParcels[i];
                        if (parcel.Id != parcelForAssign.Id)
                        {
                            this._waitingParcels.Enqueue(parcel, (int) parcel.Priority);
                        }
                    }
                }

                return parcelForAssign;
            }

            private void HandleAssignParcel(IDAL.DO.Parcel parcel, IDAL.DO.Drone drone)
            {
                parcel.Scheduled = DateTime.Now;
                parcel.Status = IDAL.DO.ParcelStatuses.Assign;

                parcel.DroneId = drone.Id;

                var sender = _dalObj.GetCostumerById(parcel.SenderId);
                var nearStation =
                    _dalObj.GetStationById(_GetNearestStation(sender.Location));

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

                    // get random staus avilable or maintance.
                    if (status == 0)
                    {
                        drone.Status = IDAL.DO.DroneStatuses.Available;
                    }
                    else
                    {
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
                    drone.Location = station.Current.Location;


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

                    drone.Location = randTarget.Location;

                    //get the nearest base station to the target
                    IDAL.DO.Station nearStation =
                        _dalObj.GetStationById(_GetNearestStation(drone.Location));

                    //according to the nearest base station, get random battery and set it in drone's battery
                    _InitBattery(drone, randParcel, nearStation);
                }
            }

            private BL()
            {
                // handle all drones, init their values (location, battery, etc):

                this._dalObj = DalObject.DalObject.GetInstance(); // Singleton
                IEnumerable<IDAL.DO.Drone> drones = _dalObj.GetDroneList();

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

                    else
                    {
                        this._waitingParcels.Enqueue(parcel, (int) parcel.Priority);
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
                this._dalObj.AddParcel(id, senderId, targetId, weight,
                    priority, DateTime.Now, droneId, default(DateTime),
                    default(DateTime), default(DateTime));

                SysLog.SysLog.GetInstance().MoveParcelToWaitingList(id);
                this._waitingParcels.Enqueue(this._dalObj.GetParcelById(id), priority);
            }

            /*
            *Description: find avaliable drone for deliverd a parcel.
            *Parameters: a parcel.
            *Return: None.
            */
            public void AssignParcelToDrone(int droneId)
            {
                //check exceptions

                if (droneId < 0)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                var drone = this._dalObj.GetDroneById(droneId);

                if (drone.Status != IDAL.DO.DroneStatuses.Available)
                {
                    throw new BO.DroneNotAvliable(droneId);
                }

                if (this._waitingParcels.Count == 0)
                {
                    throw new BO.NoParcelsForAssign();
                }

                //check which parcel we prefer to assign to these drone.
                IDAL.DO.Parcel parcelForAssign = findSuitableParcel(drone);

                //making the assign
                HandleAssignParcel(parcelForAssign, drone);
            }

            /*
            *Description: update PickedUp time to NOW. check logic.
            *Parameters: a parcel.
            *Return: None.
            */
            public void ParcelCollection(int droneId)
            {
                if (0 > droneId)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }


                IDAL.DO.Parcel parcel = this._dalObj.GetParcelByDroneId(droneId);
                IDAL.DO.Drone drone = this._dalObj.GetDroneById(droneId);
                IDAL.DO.Costumer sender = this._dalObj.GetCostumerById(parcel.SenderId);

                if (parcel.Scheduled == default(DateTime))
                {
                    throw new Exception("ERROR: The parcel (id: " + parcel.Id +
                                        ") is not Assign (not supposed to happen check [AssignParcelToDrone] method)");
                }

                if (parcel.PickedUp != default(DateTime))
                {
                    throw new ParcelIsAlreadyPickedUp(parcel.Id);
                }

                double distance = drone.Location.Distance(sender.Location) * DataSource.Config.avilablePPK;
                drone.Location = sender.Location;
                drone.Battery -= distance;

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

                IDAL.DO.Costumer costumer = this._dalObj.GetCostumerById(costumerId);
                if (costumer.Name == name)
                {
                    throw new BO.NotNewValue("Costumer Name", name);
                }

                if (costumer.Phone == phoneNumber)
                {
                    throw new BO.NotNewValue("Costumer phone", phoneNumber);
                }

                if (name != "")
                {
                    costumer.Name = name;
                }

                if (phoneNumber != "")
                {
                    costumer.Phone = phoneNumber;
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

                IDAL.DO.Station station = this._dalObj.GetStationById(stationId);
                if (station.Name == name)
                {
                    throw new BO.NotNewValue("Station name", name);
                }

                if (station.ChargeSlots == chargeSlots)
                {
                    throw new BO.NotNewValue("Station charge slots", chargeSlots.ToString());
                }

                if (name != "")
                {
                    station.Name = name;
                }

                if (chargeSlots != -1)
                {
                    station.ChargeSlots = chargeSlots;
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

                IDAL.DO.Drone drone = this._dalObj.GetDroneById(droneId);

                if (drone.Model == name)
                {
                    throw new BO.NotNewValue("Drone model", name);
                }

                if (name != "")
                {
                    drone.Model = name;
                }
            }

            /*
            *Description: update delivered time to NOW. check logic.
            *Parameters: a parcel.
            *Return: None.
            */
            public void ParcelDelivered(int droneId)
            {
                //check logic
                if (0 > droneId)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                IDAL.DO.Drone drone = _dalObj.GetDroneById(droneId);
                if (drone.Status != DroneStatuses.Shipping) throw new DroneNotShipping(droneId);
                IDAL.DO.Parcel parcel = _dalObj.GetParcelByDroneId(droneId);


                if (parcel.PickedUp == default(DateTime))
                {
                    throw new ParcelNotPickedUp(parcel.Id);
                }

                if (parcel.Delivered != default(DateTime))
                {
                    throw new ParcelIsAlreadyDelivered(parcel.Id);
                }

                //update parcel's details
                parcel.Delivered = DateTime.Now;
                parcel.Status = IDAL.DO.ParcelStatuses.Delivered;

                //update drone's details


                IDAL.DO.Costumer target = this._dalObj.GetCostumerById(parcel.TargetId);
                IDAL.DO.Station station =
                    this._dalObj.GetStationById(this._GetNearestStation(target.Location));

                drone.Status = IDAL.DO.DroneStatuses.Available;

                // the km of the first fly
                double fD = drone.Location.Distance(target.Location);

                switch (parcel.Weight)
                {
                    case WeightCategories.Heavy:
                        fD *= DataSource.Config.heavyPPK;
                        break;
                    case WeightCategories.Medium:
                        fD *= DataSource.Config.mediumPPK;
                        break;
                    case WeightCategories.Light:
                        fD *= DataSource.Config.lightPPK;
                        break;
                }

                drone.Location = target.Location;

                // the second fly
                double sD = target.Location.Distance(station.Location) * DataSource.Config.avilablePPK;
                drone.Location = station.Location;
                drone.Battery -= (fD + sD);
            }

            /*
            *Description: Send drone to charge's station. check logic.
            *Parameters: drone's id, station's id.
            *Return: None.
            */
            public void SendDroneToCharge(int droneId, int stationId = -1)
            {
                if (0 > droneId)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                if (0 > stationId && stationId != -1)
                {
                    throw new BO.NegetiveValue("Station's id");
                }

                IDAL.DO.Drone drone = this._dalObj.GetDroneById(droneId);
                if (drone.Status != IDAL.DO.DroneStatuses.Available)
                {
                    throw new BO.DroneNotAvliable(droneId);
                }

                IDAL.DO.Station station;

                if (stationId == -1)
                {
                    station = this._dalObj.GetStationById(this._GetNearestAvilableStation(drone.Location));
                    double distance = drone.Location.Distance(station.Location);
                    double minBattery = distance * DalObject.DataSource.Config.avilablePPK;

                    if (minBattery > drone.Battery)
                    {
                        throw new BO.DroneNotEnoughBattery(droneId);
                    }

                    drone.Battery -= minBattery;

                    drone.Location = station.Location;

                    drone.Status = IDAL.DO.DroneStatuses.Maintenance;

                    station.ChargeSlots--;

                    this._dalObj.AddDroneToCharge(droneId, stationId);
                }

                else
                {
                    station = this._dalObj.GetStationById(stationId);

                    if (station.ChargeSlots <= 0)
                    {
                        throw new BO.NegetiveValue("Charge's slots");
                    }

                    station.ChargeSlots--;

                    drone.Location = station.Location;

                    this._dalObj.AddDroneToCharge(drone.Id, station.Id);

                    drone.Status = IDAL.DO.DroneStatuses.Maintenance;

                    this._dalObj.AddDroneToCharge(droneId, stationId);
                }
            }

            /*
            *Description: release drone from station. check logic.
            *Parameters: a drone.
            *Return: None.
            */
            public void DroneRelease(int droneId, double hours)
            {
                if (0 > droneId)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                IDAL.DO.Drone drone = this._dalObj.GetDroneById(droneId);

                if (drone.Status != IDAL.DO.DroneStatuses.Maintenance)
                {
                    throw new BO.DroneNotInMaintenance(droneId);
                }

                this._dalObj.DroneRelease(droneId, hours);
            }

            //getters:

            public BO.ParcelBL GetParcelById(int parcelId)
            {
                if (0 > parcelId)
                {
                    throw new BO.NegetiveValue("Parcel's id");
                }

                return new BO.ParcelBL(this._dalObj.GetParcelById(parcelId));
            }

            public BO.CostumerBL GetCostumerById(int id)
            {
                if (0 > id)
                {
                    throw new BO.NegetiveValue("Costumer's id");
                }

                return new BO.CostumerBL(this._dalObj.GetCostumerById(id));
            }

            public BO.StationBL GetStationById(int id)
            {
                if (0 >= id)
                {
                    throw new BO.NegetiveValue("Station's id");
                }

                return new BO.StationBL(this._dalObj.GetStationById(id));
            }

            public BO.DroneBL GetDroneById(int droneId)
            {
                if (0 > droneId)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                //get the parcel which the drone is assign to...
                IDAL.DO.Drone drone = this._dalObj.GetDroneById(droneId);
                IEnumerable<IDAL.DO.Parcel> parcels = this._dalObj.GetParcelsList();
                int parcelOfDroneId = 0;

                foreach (var parcel in parcels)
                {
                    if (parcel.DroneId == droneId)
                    {
                        parcelOfDroneId = parcel.Id;
                        break;
                    }
                }

                return new BO.DroneBL(droneId, parcelOfDroneId);
            }

            public BO.DroneChargeBL GetDroneChargeByDroneId(int id)
            {
                if (0 > id)
                {
                    throw new BO.NegetiveValue("Drone's id");
                }

                return new BO.DroneChargeBL(this._dalObj.GetDroneById(id));
            }

            public IEnumerable<BO.StationListBL> GetStationsList()
            {
                List<BO.StationListBL> stationList = new List<BO.StationListBL>();
                IEnumerable<IDAL.DO.Station> stations = this._dalObj.GetStationsList();

                foreach (var station in stations)
                {
                    stationList.Add(new StationListBL(station));
                }

                return stationList;
            }

            public IEnumerable<BO.CostumerListBL> GetCostumerList()
            {
                List<BO.CostumerListBL> costumerList = new List<BO.CostumerListBL>();
                IEnumerable<IDAL.DO.Costumer> costumers = this._dalObj.GetCostumerList();

                foreach (var costumer in costumers)
                {
                    costumerList.Add(new BO.CostumerListBL(costumer));
                }

                return costumerList;
            }

            public List<IDAL.DO.Parcel> GetWaitingParcels()
            {
                List<IDAL.DO.Parcel> parcels = new List<IDAL.DO.Parcel>();
                IDAL.DO.Parcel parcel;
                for (int _ = 0; _ < _waitingParcels.Count; _++)
                {
                    parcel = _waitingParcels.Dequeue();
                    parcels.Add(parcel);
                    _waitingParcels.Enqueue(parcel, (int) parcel.Priority);
                }

                return parcels;
            }

            public IEnumerable<BO.ParcelListBL> GetParcelsList()
            {
                List<BO.ParcelListBL> parcelList = new List<BO.ParcelListBL>();
                IEnumerable<IDAL.DO.Parcel> parcels = this._dalObj.GetParcelsList();

                foreach (var parcel in parcels)
                {
                    parcelList.Add(new BO.ParcelListBL(parcel));
                }

                return parcelList;
            }

            public IEnumerable<BO.DroneListBL> GetDroneList()
            {
                List<BO.DroneListBL> droneList = new List<BO.DroneListBL>();
                IEnumerable<IDAL.DO.Drone> drones = this._dalObj.GetDroneList();

                foreach (var drone in drones)
                {
                    droneList.Add(new BO.DroneListBL(drone));
                }

                return droneList;
            }

            public SysLog.SysLog Sys()
            {
                return SysLog.SysLog.GetInstance();
            }

            private IDAL.DO.Parcel _bestParcel(List<IDAL.DO.Parcel> parcels, IDAL.DO.Drone drone)
            {
                // filter the parcels list to contain just the one's who the drone can carry
                parcels = parcels.Where(parcel => parcel.Weight <= drone.MaxWeight)
                    .ToList(); // where the weight is valid

                // firstly: we sort by the distance => the latest influencing parameter
                parcels = parcels.OrderBy(parcel =>
                    _dalObj.GetCostumerById(parcel.SenderId).Location.Distance(drone.Location)).ToList();

                // secondly: we sort by the [parcel.Weight] => the second most influencing parameter
                parcels = parcels.OrderBy(parcel => -1 * (int) parcel.Weight).ToList(); // in desc order: 3, 2, 1...

                // third: we sort by the [parcel.Priority] => the most influencing parameter
                parcels = parcels.OrderBy(parcel => -1 * (int) parcel.Priority).ToList(); // in desc order: 3, 2, 1...

                if (parcels.Count == 0) return null; // ERROR, throwing suitable exception
                return parcels[0];
            }
        }
    }
}