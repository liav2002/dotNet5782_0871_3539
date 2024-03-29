﻿using System;
using DalApi;
using System.Collections.Generic;
using System.Linq;
using Dal;
using DO;
using System.Threading;

namespace BO
{
    public sealed class BL : BlApi.IBL
    {
        //Singleton design pattern
        // internal static BL _instance = null;

        internal static readonly Lazy<BlApi.IBL> _instance = new Lazy<BlApi.IBL>(() => new BL());

        internal static BlApi.IBL GetInstance
        {
            get { return _instance.Value; }
        }


        private PriorityQueue<DO.Parcel> _waitingParcels = new PriorityQueue<DO.Parcel>();

        private DalApi.IDal _idalObj;

        private double _chargeRate = Dal.DataSource.Config.chargeRatePH; // to all the drones


        private double _lightPPK = Dal.DataSource.Config.lightPPK;
        private double _mediumPPK = Dal.DataSource.Config.mediumPPK;
        private double _heavyPPK = Dal.DataSource.Config.heavyPPK;
        private double _avilablePPK = Dal.DataSource.Config.avilablePPK;

        /**********************************************************************************************
         * Details: this function find the nearest avilable station to drone.                         *
         * Parameters: drone's current location (lattidue and longtiube).                             *
         * Return: station's id.                                                                      *
         **********************************************************************************************/
        private int _GetNearestAvilableStation(DO.Location location)
        {
            int stationId = -1;
            var stations = _idalObj.GetStationsList();
            double min = -1;
            int numberOfIteration = 0;
            bool stopSearch = false;

            if (!stations.Any() || stations == null) throw new NonItems("Stations");

            double minToAvoid = -1;
            double saveMin = 0;

            while (!stopSearch)
            {
                var station = stations.GetEnumerator();
                while (station.MoveNext())
                {
                    double distance = station.Current.Location.Distance(location);

                    if ((min > distance || min == -1) && distance > minToAvoid)
                    {
                        saveMin = distance;
                        min = station.Current.ChargeSlots > 0 ? distance : -1;
                        stationId = station.Current.ChargeSlots > 0 ? station.Current.Id : -1;
                    }
                }

                if (min == -1) // if the nearest station is not avilable
                {
                    if (numberOfIteration == stations.Count()) // if there is no another station to check
                    {
                        throw new BO.NotAvilableStation(); // tell the user
                    }

                    else
                    {
                        minToAvoid = saveMin; // mark this minimum as unAvailable station, so next time avoid him
                        numberOfIteration++; // count number of tests
                    }
                }

                else
                {
                    stopSearch = true;
                }
            }


            return stationId;
        }

        /**********************************************************************************************
         * Details: this function find the nearest station to drone.                                  *
         * Parameters: drone's current location (lattidue and longtiube).                             *
         * Return: station's id.                                                                      *
         **********************************************************************************************/
        private int _GetNearestStation(DO.Location location)
        {
            int stationId = -1;
            var stations = _idalObj.GetStationsList();

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
        private void _InitDroneLocation(DO.Drone drone, DO.Parcel parcel, DO.Station station)
        {
            if (parcel.PickedUp == null) //the parcel is not picked up yet
            {
                drone.Location = station.Location;
            }

            else if (parcel.Delivered == null) // the parcel is not delivered yet (but it's picked up)
            {
                var sender = _idalObj.GetCostumerById(parcel.SenderId);
                drone.Location = station.Location;
            }

            _idalObj.UpdateDrone(drone);
        }

        /**********************************************************************************************
         * Details: this function initalized drone's battery according to some conditions.            *
         * Parameters: drone, parcel (the drone carried), station (the drone came from, or move to).  *
         * Return: None.                                                                              *
         **********************************************************************************************/
        private void _InitBattery(DO.Drone drone, DO.Parcel parcel, DO.Station station)
        {
            Random rand = new Random();
            double minBattery = 0;
            double d1 = 0;
            double d2 = 0;
            double d3 = 0;
            var sender = _idalObj.GetCostumerById(parcel.SenderId);
            var target = _idalObj.GetCostumerById(parcel.TargetId);

            if (drone.Status == DO.DroneStatuses.Shipping)
            {
                station = this._idalObj.GetStationById(_GetNearestStation(target.Location));


                // first fly - base to parcel (maybe the drone is already in base, it doesn't affect)
                d1 = drone.Location.Distance(sender.Location);

                // second fly - sender to target with the parcel
                d2 = sender.Location.Distance(target.Location);

                // last fly - returning to the nearest base from target, without the parcel
                d3 = target.Location.Distance(station.Location);

                minBattery = (d1 + d3) * DataSource.Config.avilablePPK;

                switch (parcel.Weight)
                {
                    case DO.WeightCategories.Heavy:
                    {
                        minBattery += d2 * DataSource.Config.heavyPPK;
                        break;
                    }
                    case DO.WeightCategories.Medium:
                    {
                        minBattery += d2 * DataSource.Config.mediumPPK;
                        break;
                    }
                    case DO.WeightCategories.Light:
                    {
                        minBattery += d2 * DataSource.Config.lightPPK;
                        break;
                    }
                }
            }

            else if (drone.Status == DO.DroneStatuses.Available)
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
            _idalObj.UpdateDrone(drone);
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
        private DO.Parcel findSuitableParcel(int droneId, int lessPriority = 0)
        {
            //first create a priority queue with all the parcels with highest priority
            PriorityQueue<DO.Parcel> parcelsAccordingWeight = new PriorityQueue<DO.Parcel>();
            PriorityQueue<DO.Parcel>
                saveParcels = new PriorityQueue<DO.Parcel>(); // in case we call the function not in the first time,
            // we want to search parcel with the next highest prioriry,
            // so, I save the real highest prioirty in different queue.
            DO.Drone drone = this._idalObj.GetDroneById(droneId);
            DO.Parcel parcelForAssign = this._waitingParcels.Dequeue();
            DO.Priorities priority = parcelForAssign.Priority - lessPriority;

            if (priority == parcelForAssign.Priority)
            {
                parcelsAccordingWeight.Enqueue(parcelForAssign, (int) parcelForAssign.Weight);
            }
            else if (priority < parcelForAssign.Priority)
            {
                saveParcels.Enqueue(parcelForAssign, (int) parcelForAssign.Weight);
            }
            else
            {
                this._waitingParcels.Enqueue(parcelForAssign, (int) parcelForAssign.Priority);
            }

            bool search = true;

            while (search)
            {
                if (this._waitingParcels.Count != 0)
                {
                    parcelForAssign = this._waitingParcels.Dequeue();

                    if (parcelForAssign.Priority == priority)
                    {
                        parcelsAccordingWeight.Enqueue(parcelForAssign, (int) parcelForAssign.Weight);
                    }
                    else if (priority < parcelForAssign.Priority)
                    {
                        saveParcels.Enqueue(parcelForAssign, (int) parcelForAssign.Weight);
                    }
                    else
                    {
                        search = false;
                        this._waitingParcels.Enqueue(parcelForAssign, (int) parcelForAssign.Priority);
                    }
                }

                else
                {
                    search = false;
                }
            }

            //and check which from the parcels in the list
            //the drone can carry consider the parcel's weight
            search = true;

            while (search)
            {
                if (parcelsAccordingWeight.Count != 0)
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

                else
                {
                    search = false;
                }
            }

            if (parcelsAccordingWeight.Count == 0)
            {
                if (lessPriority >= 0 && lessPriority < 2)
                {
                    for (int i = 0; i < saveParcels.Count; ++i)
                    {
                        DO.Parcel savedParcel = saveParcels.Dequeue();
                        _waitingParcels.Enqueue(savedParcel, (int) savedParcel.Priority);
                    }

                    return findSuitableParcel(drone.Id, lessPriority + 1);
                }

                else
                {
                    for (int i = 0; i < saveParcels.Count; ++i)
                    {
                        DO.Parcel savedParcel = saveParcels.Dequeue();
                        _waitingParcels.Enqueue(savedParcel, (int) savedParcel.Priority);
                    }

                    throw new BO.NoSuitableParcelForDrone(drone.Id);
                }
            }

            //second create list with the heaviest parcels, the drone can carry
            List<DO.Parcel> suitableParcels = new List<DO.Parcel>();
            parcelForAssign = parcelsAccordingWeight.Dequeue();
            suitableParcels.Add(parcelForAssign);
            DO.WeightCategories maxWeight = parcelForAssign.Weight;
            search = true;

            while (search)
            {
                if (parcelsAccordingWeight.Count != 0)
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

                else
                {
                    search = false;
                }
            }

            //third, if there is more than one heaviest parcel,
            //check which one from them is the nearest to drone
            parcelForAssign = suitableParcels[0];

            if (suitableParcels.Count > 1)
            {
                double minDistance =
                    drone.Location.Distance(this._idalObj.GetCostumerById(parcelForAssign.SenderId).Location);

                for (int i = 1; i < suitableParcels.Count; ++i)
                {
                    DO.Parcel parcel = suitableParcels[i];
                    double currentDistance =
                        drone.Location.Distance(this._idalObj.GetCostumerById(parcel.SenderId).Location);

                    if (currentDistance < minDistance)
                    {
                        parcelForAssign = parcel;
                        minDistance = currentDistance;
                    }
                }

                for (int i = 0; i < suitableParcels.Count; ++i)
                {
                    DO.Parcel parcel = suitableParcels[i];
                    if (parcel.Id != parcelForAssign.Id)
                    {
                        this._waitingParcels.Enqueue(parcel, (int) parcel.Priority);
                    }
                }
            }

            for (int i = 0; i < saveParcels.Count; ++i)
            {
                DO.Parcel savedParcel = saveParcels.Dequeue();
                _waitingParcels.Enqueue(savedParcel, (int) savedParcel.Priority);
            }

            return parcelForAssign;
        }

        private void HandleAssignParcel(int parcelId, int droneId, bool isFromTheConatiner = false)
        {
            DO.Parcel parcel = this._idalObj.GetParcelById(parcelId);
            DO.Drone drone = this._idalObj.GetDroneById(droneId);

            parcel.Scheduled = DateTime.Now;
            parcel.Status = DO.ParcelStatuses.Assign;

            parcel.DroneId = drone.Id;

            var sender = _idalObj.GetCostumerById(parcel.SenderId);
            var startNearStation =
                _idalObj.GetStationById(_GetNearestStation(sender.Location));

            var target = _idalObj.GetCostumerById(parcel.TargetId);
            var endNearestStation =
                _idalObj.GetStationById(_GetNearestStation(target.Location));

            drone.Status = DO.DroneStatuses.Shipping; // change the drone status to Shipping

            _InitDroneLocation(drone, parcel, startNearStation); // set drone's location

            //check drone's battery

            if (isFromTheConatiner)
            {
                _InitBattery(drone, parcel, startNearStation);
            }

            else
            {
                double
                    d1 = startNearStation.Location
                        .Distance(sender
                            .Location); // first distance: drone (from start station point) --> sender (without parcel == without weight)

                double
                    d2 = sender.Location.Distance(target
                        .Location); //second distance: drone(from sender) --> target (with parcel == with weight)

                double
                    d3 = target.Location.Distance(endNearestStation
                        .Location); //third distance: drone(from target) --> end station point. (without parcel == without weight)

                double batteryConsumption = (d1 + d3) * DataSource.Config.avilablePPK;

                if (parcel.Weight == WeightCategories.Light)
                {
                    batteryConsumption += d2 * DataSource.Config.lightPPK;
                }

                else if (parcel.Weight == WeightCategories.Medium)
                {
                    batteryConsumption += d2 * DataSource.Config.mediumPPK;
                }

                else
                {
                    batteryConsumption += d3 * DataSource.Config.heavyPPK;
                }

                if (drone.Battery - batteryConsumption <= 0)
                {
                    if (drone.Battery != 100)
                    {
                        throw new DroneNotEnoughBattery(droneId);
                    }

                    else
                    {
                        throw new Exception("ERROR: the distance to long for the drone.\n");
                    }
                }
            }

            _idalObj.UpdateDrone(drone);
            _idalObj.UpdateParcel(parcel);
        }

        private void SetDroneDetails(DO.Drone drone)
        {
            if (drone.Status != DO.DroneStatuses.Shipping) //for all the drones that not in shiping.
                //I dont care from shiping drones, because I already init there value (battery, location).
            {
                Random rand = new Random();
                int status = rand.Next(0, 1);

                // get random staus avilable or maintance.
                if (status == 0)
                {
                    drone.Status = DO.DroneStatuses.Available;
                }
                else
                {
                    drone.Status = DO.DroneStatuses.Maintenance;
                }
            }

            if (drone.Status == DO.DroneStatuses.Maintenance) // handle the maintance drones.
            {
                Random rand = new Random();
                var stations = _idalObj.GetStationsList();
                int counter = _LengthIEnumerator<DO.Station>(stations);
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

                _idalObj.UpdateDrone(drone);
            }

            else if (drone.Status == DO.DroneStatuses.Available) // handle the avilable drones.
            {
                IEnumerable<DO.Parcel> parcels = _idalObj.GetParcelsList();
                Random rand = new Random();

                // get a list of delivered parcels.
                List<DO.Parcel> deliveredParcels = new List<DO.Parcel>();
                foreach (var parcel in parcels)
                {
                    if (parcel.Delivered != null)
                    {
                        deliveredParcels.Add(parcel);
                    }
                }

                if (deliveredParcels.Count > 0)
                {
                    // get random parcel from the list.
                    int index = rand.Next(0, deliveredParcels.Count - 1);
                    DO.Parcel randParcel = deliveredParcels[index];

                    // get the target from the random delivered parcel
                    DO.Costumer randTarget = _idalObj.GetCostumerById(randParcel.TargetId);

                    // set drone's location

                    drone.Location = randTarget.Location;

                    //get the nearest base station to the target
                    DO.Station nearStation =
                        _idalObj.GetStationById(_GetNearestStation(drone.Location));

                    //according to the nearest base station, get random battery and set it in drone's battery
                    _InitBattery(drone, randParcel, nearStation);
                }
            }

            _idalObj.UpdateDrone(drone);
        }

        private BL() //Constructor
        {
            this._idalObj = DalFactory.GetDal();

            if (this._idalObj.type() ==
                DalTypes.DalObj) // handle all drones, init their values (location, battery, etc): 
            {
                IEnumerable<DO.Drone> drones = _idalObj.GetDroneList();

                //find all the drones which was assign to a parcel, and change there status to Shiping
                IEnumerable<DO.Parcel> parcels = _idalObj.GetParcelsList();

                foreach (var parcel in parcels)
                {
                    if (
                        parcel.DroneId !=
                        0) // the parcel has been assigned to drone, in this part I handle all the shiping drones.
                    {
                        HandleAssignParcel(parcel.Id, parcel.DroneId, true);
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

            else if (this._idalObj.type() == DalTypes.DalXml)
            {
                IEnumerable<DO.Parcel> waitParcelsList =
                    _idalObj.GetParcelsList(parcel => parcel.Status == ParcelStatuses.Created);

                foreach (var parcel in waitParcelsList)
                {
                    _waitingParcels.Enqueue(parcel, (int) parcel.Priority);
                }
            }
        }

        /*
           *Description: add new Station to stations. check the logic in the parameters, then use dalObject for adding.
           *Parameters: station's id, station's name, station's longitude, station's latitude, charge's slots.
           *Return: None.
        */
        public void AddStation(int id, string name, double longitude, double latitude, int charge_solts)
        {
            IEnumerable<DO.Station> stations = this._idalObj.GetStationsList();

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

            this._idalObj.AddStation(id, name, new DO.Location(latitude, longitude), charge_solts);
        }

        /*
           *Description: Remove station from data.
           *Parameters: station's id
           *Return: None.
        */
        public void RemoveStation(int stationId)
        {
            StationBL station = GetStationById(stationId);

            if (!station.IsAvailable)
            {
                throw new NoAvailable("station");
            }

            DO.Station dalStation = _idalObj.GetStationById(stationId);
            station.SetAvailability(false); //for dalObject mode. (without database).
            dalStation.IsAvailable = false;
            _idalObj.UpdateStation(dalStation);
        }

        /*
           *Description: Restore station from data.
           *Parameters: station's id
           *Return: None.
        */
        public void RestoreStation(int stationId)
        {
            StationBL station = GetStationById(stationId);
            station.SetAvailability(true); //for dalObject mode. (without database).
            DO.Station dalStation = _idalObj.GetStationById(stationId);
            dalStation.IsAvailable = true;
            _idalObj.UpdateStation(dalStation);
        }

        /*
           *Description: add new Drone to drones. check logic, then use dalObject for adding.
           *Parameters: drone's details.	
           *Return: true - if added successfully, false - else.
        */
        public void AddDrone(int id, string model, int maxWeight, int stationId)
        {
            IEnumerable<DO.Drone> drones = _idalObj.GetDroneList();
            IEnumerable<DO.Station> stations = _idalObj.GetStationsList();

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
                throw new BO.NonItems("Station with id " + stationId);
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
            this._idalObj.AddDrone(id, model, (DO.WeightCategories) maxWeight, battery);

            // after the drone created we send he to the station:
            SendDroneToCharge(id, stationId);
        }

        /*
           *Description: Remove drone from data.
           *Parameters: drone's id
           *Return: None.
        */
        public void RemoveDrone(int droneId)
        {
            DroneBL drone = GetDroneById(droneId);

            if (!drone.IsAvaliable)
            {
                throw new NoAvailable("drone");
            }

            drone.SetAvailability(false); //for dalObject mode. (without database).
            DO.Drone dalDrone = _idalObj.GetDroneById(droneId);
            dalDrone.IsAvaliable = false;
            _idalObj.UpdateDrone(dalDrone);
        }

        /*
           *Description: Restore drone from data.
           *Parameters: drone's id
           *Return: None.
        */
        public void RestoreDrone(int droneId)
        {
            DroneBL drone = GetDroneById(droneId);

            drone.SetAvailability(true); //for dalObject mode. (without database).

            DO.Drone dalDrone = _idalObj.GetDroneById(droneId);
            dalDrone.IsAvaliable = true;
            _idalObj.UpdateDrone(dalDrone);
        }

        /*
        *Description: add new Costumer to costumers. check logic, then use dalObject for adding.
        *Parameters: costumer's details.
        *Return: None.
        */
        public void AddCostumer(int id, string name, string phone, double longitude, double latitude, string email,
            string password)
        {
            IEnumerable<DO.Costumer> costumers = _idalObj.GetCostumerList();

            foreach (var costumer in costumers)
            {
                if (costumer.Id == id)
                {
                    throw new BO.NonUniqueID("Costumer's id");
                }

                else if (costumer.Name == name)
                {
                    throw new BO.NonUniqueID("Costumer's name");
                }
            }

            if (id < 0)
            {
                throw new BO.NegetiveValue("Costumer's id");
            }

            this._idalObj.AddCostumer(id, name, phone, new DO.Location(longitude, latitude), email, password);
        }

        /*
            *Description: Remove costumer from data.
            *Parameters: costumer's id
            *Return: None.
        */
        public void RemoveCostumer(int costumerId)
        {
            CostumerBL costumer = GetCostumerById(costumerId);

            if (!costumer.IsAvaliable)
            {
                throw new NoAvailable("costumer");
            }

            costumer.SetAvailability(false); //for dalObject mode. (without database).
            DO.Costumer dalCostumer = _idalObj.GetCostumerById(costumerId);
            dalCostumer.IsAvaliable = false;
            _idalObj.UpdateCostumer(dalCostumer);
        }

        /*
           *Description: Restore costumer from data.
           *Parameters: costumer's id
           *Return: None.
        */
        public void RestoreCostumer(int costumerId)
        {
            CostumerBL costumer = GetCostumerById(costumerId);

            costumer.SetAvailability(true); //for dalObject mode. (without database).

            DO.Costumer dalCostumer = _idalObj.GetCostumerById(costumerId);
            dalCostumer.IsAvaliable = true;
            _idalObj.UpdateCostumer(dalCostumer);
        }

        /*
           *Description: Set costumer as manager.
           *Parameters: costumer's id
           *Return: None.
        */
        public void SetAsManager(int costumerId)
        {
            CostumerBL costumer = GetCostumerById(costumerId);

            costumer.SetMangementStatus(true); //for dalObject mode. (without database).

            DO.Costumer dalCostumer = _idalObj.GetCostumerById(costumerId);
            dalCostumer.IsManger = true;
            _idalObj.UpdateCostumer(dalCostumer);
        }

        /*
           *Description: Set costumer as regular costumer.
           *Parameters: costumer's id
           *Return: None.
        */
        public void NonManagement(int costumerId)
        {
            CostumerBL costumer = GetCostumerById(costumerId);

            costumer.SetMangementStatus(false); //for dalObject mode. (without database).

            DO.Costumer dalCostumer = _idalObj.GetCostumerById(costumerId);
            dalCostumer.IsManger = false;
            _idalObj.UpdateCostumer(dalCostumer);
        }

        /*
        *Description: add new Parcel to parcels. check logic, then use dalObject for adding.
        *Parameters: parcel's detatils.
        *Return: None.
        */
        public void AddParcel(int senderId, int targetId, int weight, int priority, int droneId)
        {
            IEnumerable<DO.Parcel> parcels = _idalObj.GetParcelsList();

            if (targetId == senderId)
            {
                throw new BO.SelfDelivery();
            }

            if (0 > targetId || 0 > senderId)
            {
                throw new BO.NegetiveValue("Id");
            }

            int id = _idalObj.GetParcelsList().Count() + 1;
            this._idalObj.AddParcel(id, senderId, targetId, weight,
                priority, DateTime.Now, droneId, null,
                null, null);

            this._waitingParcels.Enqueue(this._idalObj.GetParcelById(id), priority);
        }

        /*
            *Description: Remove drone from data.
            *Parameters: drone's id
            *Return: None.
        */
        public void RemoveParcel(int parcelId)
        {
            ParcelBL parcel = GetParcelById(parcelId);

            if (!parcel.IsAvailable)
                throw new NoAvailable("parcel");
            if (parcel.Status != ParcelStatuses.Created)
                throw new RemoveError("parcel");

            parcel.SetAvailability(false); //for dalObject mode. (without database).
            DO.Parcel dalParcel = _idalObj.GetParcelById(parcelId);
            dalParcel.IsAvailable = false;
            _idalObj.UpdateParcel(dalParcel);
        }

        /*
            *Description: Remove drone from data.
            *Parameters: parcel's id
            *Return: None.
        */
        public void RestoreParcel(int parcelId)
        {
            ParcelBL parcel = GetParcelById(parcelId);

            parcel.SetAvailability(true); //for dalObject mode. (without database).

            DO.Parcel dalParcel = _idalObj.GetParcelById(parcelId);
            dalParcel.IsAvailable = true;
            _idalObj.UpdateParcel(dalParcel);
        }

        /*
        *Description: find available drone for deliverd a parcel.
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

            var drone = this._idalObj.GetDroneById(droneId);

            if (drone.IsAvaliable == false)
            {
                throw new BO.NoAvailable("drone");
            }

            if (drone.Status != DO.DroneStatuses.Available)
            {
                throw new BO.DroneNotAvliable(droneId);
            }

            if (this._waitingParcels.Count == 0)
            {
                throw new BO.NoParcelsForAssign();
            }

            //check which parcel we prefer to assign to these drone.
            DO.Parcel parcel = findSuitableParcel(drone.Id);

            //making the assign
            HandleAssignParcel(parcel.Id, drone.Id);
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


            DO.Parcel parcel = this._idalObj.GetParcelByDroneId(droneId);
            DO.Drone drone = this._idalObj.GetDroneById(droneId);
            DO.Costumer sender = this._idalObj.GetCostumerById(parcel.SenderId);

            if (parcel.Status == ParcelStatuses.Created)
            {
                throw new Exception("ERROR: The parcel (id: " + parcel.Id +
                                    ") is not Assign (not supposed to happen check [AssignParcelToDrone] method)");
            }

            if (parcel.Status == ParcelStatuses.PickedUp)
            {
                throw new ParcelIsAlreadyPickedUp(parcel.Id);
            }

            double distance = drone.Location.Distance(sender.Location) * DataSource.Config.avilablePPK;
            drone.Location = sender.Location;
            drone.Battery -= distance;

            parcel.PickedUp = DateTime.Now;
            parcel.Status = DO.ParcelStatuses.PickedUp;

            _idalObj.UpdateParcel(parcel);
            _idalObj.UpdateDrone(drone);
        }

        /*
        *Description: update costumer details.
        *Parameters: costumer's id, new name, new phone number.
        *Return: None.
        */
        public void UpdateCostumer(int costumerId, string name, string phoneNumber, string email, string password)
        {
            //check logic
            if (0 > costumerId)
            {
                throw new BO.NegetiveValue("Costumer's id");
            }

            DO.Costumer costumer = this._idalObj.GetCostumerById(costumerId);

            if (name != "")
            {
                costumer.Name = name;
            }

            if (phoneNumber != "")
            {
                costumer.Phone = phoneNumber;
            }

            if (email != "")
            {
                costumer.Email = email;
            }

            if (password != "")
            {
                costumer.Password = password;
            }

            _idalObj.UpdateCostumer(costumer);
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

            DO.Station station = this._idalObj.GetStationById(stationId);

            if (name != "")
            {
                station.Name = name;
            }

            if (chargeSlots != -1)
            {
                if (chargeSlots < 0)
                {
                    throw new BO.NegetiveValue("Charge slots");
                }

                station.ChargeSlots = chargeSlots;
            }

            _idalObj.UpdateStation(station);
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

            if (GetDroneById(droneId).IsAvaliable == false)
            {
                throw new BO.NoAvailable("drone");
            }

            DO.Drone drone = this._idalObj.GetDroneById(droneId);

            if (name != "")
            {
                drone.Model = name;
            }

            _idalObj.UpdateDrone(drone);
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

            DO.Drone drone = _idalObj.GetDroneById(droneId);
            if (drone.Status != DroneStatuses.Shipping) throw new DroneNotShipping(droneId);
            DO.Parcel parcel = _idalObj.GetParcelByDroneId(droneId);

            if (parcel.Status == ParcelStatuses.Delivered)
            {
                throw new ParcelIsAlreadyDelivered(parcel.Id);
            }

            if (parcel.Status != ParcelStatuses.PickedUp)
            {
                throw new ParcelNotPickedUp(parcel.Id);
            }

            //update parcel's details
            parcel.Delivered = DateTime.Now;
            parcel.Status = DO.ParcelStatuses.Delivered;

            //update drone's details


            DO.Costumer target = this._idalObj.GetCostumerById(parcel.TargetId);
            DO.Station station =
                this._idalObj.GetStationById(this._GetNearestStation(target.Location));

            drone.Status = DO.DroneStatuses.Available;

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

            // drone.Location = target.Location;

            // the second fly
            double sD = target.Location.Distance(station.Location) * DataSource.Config.avilablePPK;
            drone.Location = station.Location;
            drone.Battery -= (fD + sD);

            _idalObj.UpdateDrone(drone);
            _idalObj.UpdateParcel(parcel);
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

            DO.Drone drone = this._idalObj.GetDroneById(droneId);

            if (drone.IsAvaliable == false)
            {
                throw new BO.NoAvailable("drone");
            }

            if (drone.Status != DO.DroneStatuses.Available)
            {
                throw new BO.DroneNotAvliable(droneId);
            }

            DO.Station station;

            if (stationId == -1)
            {
                station = this._idalObj.GetStationById(this._GetNearestAvilableStation(drone.Location));
                double distance = drone.Location.Distance(station.Location);
                double minBattery = distance * Dal.DataSource.Config.avilablePPK;

                if (minBattery > drone.Battery) // the drone 
                {
                    //throw new BO.DroneNotEnoughBattery(droneId);
                    drone.Battery = minBattery + 1;
                }

                drone.Battery -= minBattery;

                drone.Location = station.Location;

                drone.Status = DO.DroneStatuses.Maintenance;

                station.ChargeSlots--;

                this._idalObj.AddDroneToCharge(droneId, station.Id);
            }

            else // only when we add new drone. we add him to particular station.
            {
                station = this._idalObj.GetStationById(stationId);

                if (station.ChargeSlots <= 0)
                {
                    throw new BO.NegetiveValue("Charge's slots");
                }

                station.ChargeSlots--;

                drone.Location = station.Location;

                this._idalObj.AddDroneToCharge(drone.Id, station.Id);

                drone.Status = DO.DroneStatuses.Maintenance;
            }

            _idalObj.UpdateDrone(drone);
            _idalObj.UpdateStation(station);
        }

        /*
        *Description: Chargine the drone in the simulator.
        *Parameters: drone's id.
        *Return: None.
        */
        public void DroneCharging(int droneId)
        {
            if (0 > droneId)
            {
                throw new BO.NegetiveValue("Drone's id");
            }

            DO.Drone drone = this._idalObj.GetDroneById(droneId);

            if (drone.Status != DO.DroneStatuses.Maintenance)
            {
                throw new BO.DroneNotInMaintenance(droneId);
            }

            DO.DroneCharge dc = this._idalObj.GetDroneChargeByDroneId(droneId);

            TimeSpan time = DateTime.Now.Subtract(dc.StartTime);
            double hours = time.TotalHours;

            this._idalObj.DroneCharging(drone.Id, hours);
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

            DO.Drone drone = this._idalObj.GetDroneById(droneId);

            if (drone.Status != DO.DroneStatuses.Maintenance)
            {
                throw new BO.DroneNotInMaintenance(droneId);
            }

            DO.DroneCharge dc = this._idalObj.GetDroneChargeByDroneId(droneId);

            TimeSpan time = DateTime.Now.Subtract(dc.StartTime);
            double hours = time.TotalHours;

            this._idalObj.DroneRelease(droneId, hours);
        }

        public void SignIn(string username, string password)
        {
            CostumerBL costumer = GetCostumerByUsername(username);

            if (!costumer.IsAvaliable)
            {
                throw new UserBlocked();
            }

            if (costumer.Password == password)
            {
                this._idalObj.SignIn(costumer.Id);
            }

            else
            {
                throw new BO.FailedSignIn();
            }
        }

        public void SignOut()
        {
            this._idalObj.SignOut();
        }


        public void StartSimulator(BO.DroneBL drone)
        {
            switch (drone.Status)
            {
                case DroneStatuses.Available:
                {
                    try
                    {
                        AssignParcelToDrone(drone.Id);
                        throw new Exception("SYS_LOG: parcel succesfully assigned.\n");
                    }
                    catch (Exception ex)
                    {
                        if (ex is BO.DroneNotEnoughBattery)
                        {
                            //Thread.Sleep((int)((drone.Location.Distance(station.Location) / DataSource.Config.droneSpeedKMPH)) * 60 * 60 * 1000); //wait for the drone for arriving the station.  --> to much time, it's simulator not real time
                            Thread.Sleep(2000);
                            SendDroneToCharge(drone.Id);
                            throw new Exception("SYS_LOG: drone succesfully connected for charging.\n");
                        }
                        else
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }

                case DroneStatuses.Maintenance:
                {
                    if (drone.Battery == 100)
                    {
                        DroneRelease(drone.Id);
                        throw new Exception("SYS_LOG: drone succesfully released from charge.\n");
                    }

                    else
                    {
                        DroneCharging(drone.Id);
                    }

                    break;
                }

                case DroneStatuses.Shipping:
                {
                    if (!drone.Parcel.IsOnTheWay && !drone.Parcel.IsDelivered)
                    {
                        //Thread.Sleep((int)((drone.Location.Distance(sender.Location) / DataSource.Config.droneSpeedKMPH)) * 60 * 60 * 1000); //wait for the drone for arriving the sender person. --> to much time, it's simulator not real time
                        Thread.Sleep(2000);
                        ParcelCollection(drone.Id);
                        throw new Exception("SYS_LOG: parcel succesfully collected.\n");
                    }

                    else if (drone.Parcel.IsOnTheWay)
                    {
                        //Thread.Sleep((int)((drone.Location.Distance(target.Location) / DataSource.Config.droneSpeedKMPH)) * 60 * 60 * 1000); //wait for the drone for arriving the target.  --> to much time, it's simulator not real time
                        Thread.Sleep(2000);

                        //Thread.Sleep((int)((target.Location.Distance(station.Location) / DataSource.Config.droneSpeedKMPH)) * 60 * 60 * 1000); //wait for the drone for arriving the station (end point).  --> to much time, it's simulator not real time
                        Thread.Sleep(1000);

                        ParcelDelivered(drone.Id);
                        throw new Exception("SYS_LOG: parcel succesfully delivered.\n");
                    }

                    else
                    {
                        DO.Drone droneDal = this._idalObj.GetDroneById(drone.Id);
                        droneDal.Status = DroneStatuses.Available;
                        this._idalObj.UpdateDrone(droneDal);
                        throw new Exception("ERROR: drone was in status: Shipping althrogh he was avilable.\n");
                    }
                }
            }
        }

        public void SetDroneStartTimeOfCharge(int droneId)
        {
            DO.DroneCharge dc = this._idalObj.GetDroneChargeByDroneId(droneId);
            dc.StartTime = DateTime.Now;
            this._idalObj.UpdateDroneCharge(dc);
        }

        //getters:
        public BO.CostumerBL GetLoggedUser()
        {
            CostumerBL loggedUser = null;

            if (this._idalObj.GetLoggedUser() != null)
            {
                loggedUser = GetCostumerById(this._idalObj.GetLoggedUser().Id);
            }

            return loggedUser;
        }

        public BO.ParcelBL GetParcelById(int parcelId)
        {
            if (0 > parcelId)
            {
                throw new BO.NegetiveValue("Parcel's id");
            }

            return new BO.ParcelBL(this._idalObj.GetParcelById(parcelId));
        }

        public BO.CostumerBL GetCostumerById(int id)
        {
            if (0 > id)
            {
                throw new BO.NegetiveValue("Costumer's id");
            }

            return new BO.CostumerBL(this._idalObj.GetCostumerById(id));
        }

        public BO.StationBL GetStationById(int id)
        {
            if (0 >= id)
            {
                throw new BO.NegetiveValue("Station's id");
            }

            return new BO.StationBL(this._idalObj.GetStationById(id));
        }

        public BO.DroneBL GetDroneById(int droneId)
        {
            if (0 > droneId)
            {
                throw new BO.NegetiveValue("Drone's id");
            }

            //get the parcel which the drone is assign to...
            DO.Drone drone = this._idalObj.GetDroneById(droneId);
            IEnumerable<DO.Parcel> parcels = this._idalObj.GetParcelsList();
            int parcelOfDroneId = 0;

            foreach (var parcel in parcels)
            {
                if (parcel.DroneId == droneId && (parcel.Delivered == null || parcel.Delivered == default(DateTime)))
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

            return new BO.DroneChargeBL(this._idalObj.GetDroneById(id));
        }

        public IEnumerable<BO.StationListBL> GetStationsList(Func<DO.Station, bool> filter = null)
        {
            List<BO.StationListBL> stationList = new List<BO.StationListBL>();
            IEnumerable<DO.Station> stations = this._idalObj.GetStationsList(filter);

            foreach (var station in stations)
            {
                stationList.Add(new StationListBL(station));
            }

            return stationList;
        }

        public IEnumerable<BO.CostumerListBL> GetCostumerList(Func<DO.Costumer, bool> filter = null)
        {
            List<BO.CostumerListBL> costumerList = new List<BO.CostumerListBL>();
            IEnumerable<DO.Costumer> costumers = this._idalObj.GetCostumerList(filter);

            foreach (var costumer in costumers)
            {
                costumerList.Add(new BO.CostumerListBL(costumer));
            }

            return costumerList;
        }

        public List<DO.Parcel> GetWaitingParcels()
        {
            List<DO.Parcel> parcels = new List<DO.Parcel>();
            DO.Parcel parcel;
            for (int _ = 0; _ < _waitingParcels.Count; _++)
            {
                parcel = _waitingParcels.Dequeue();
                parcels.Add(parcel);
                _waitingParcels.Enqueue(parcel, (int) parcel.Priority);
            }

            return parcels;
        }

        public IEnumerable<BO.ParcelListBL> GetParcelsList(Func<DO.Parcel, bool> filter = null)
        {
            List<BO.ParcelListBL> parcelList = new List<BO.ParcelListBL>();
            IEnumerable<DO.Parcel> parcels = this._idalObj.GetParcelsList(filter);

            foreach (var parcel in parcels)
            {
                parcelList.Add(new BO.ParcelListBL(parcel));
            }

            return parcelList;
        }


        public IEnumerable<BO.DroneListBL> GetDroneList(Func<DO.Drone, bool> filter = null)
        {
            List<BO.DroneListBL> droneList = new List<BO.DroneListBL>();
            IEnumerable<DO.Drone> drones = this._idalObj.GetDroneList(filter);

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

        private DO.Parcel _bestParcel(List<DO.Parcel> parcels, DO.Drone drone)
        {
            // filter the parcels list to contain just the one's who the drone can carry
            parcels = parcels.Where(parcel => parcel.Weight <= drone.MaxWeight)
                .ToList(); // where the weight is valid

            // firstly: we sort by the distance => the latest influencing parameter
            parcels = parcels.OrderBy(parcel =>
                _idalObj.GetCostumerById(parcel.SenderId).Location.Distance(drone.Location)).ToList();

            // secondly: we sort by the [parcel.Weight] => the second most influencing parameter
            parcels = parcels.OrderBy(parcel => -1 * (int) parcel.Weight).ToList(); // in desc order: 3, 2, 1...

            // third: we sort by the [parcel.Priority] => the most influencing parameter
            parcels = parcels.OrderBy(parcel => -1 * (int) parcel.Priority).ToList(); // in desc order: 3, 2, 1...

            if (parcels.Count == 0) return null; // ERROR, throwing suitable exception
            return parcels[0];
        }

        private CostumerBL GetCostumerByUsername(string name)
        {
            int id = 0;
            IEnumerable<BO.CostumerListBL> costumers = GetCostumerList();

            foreach (var costumer in costumers)
            {
                if (costumer.Name == name)
                {
                    id = costumer.Id;
                    return new BO.CostumerBL(this._idalObj.GetCostumerById(id));
                }
            }

            throw new BO.FailedSignIn();
        }
    }
}