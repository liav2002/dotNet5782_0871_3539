using System;
using DalApi;
using System.Collections.Generic;
using System.Linq;
using DalObject;
using DO;


namespace BO
{
    public sealed class BL : BlApi.IBL
    {
        //Singleton design pattern
        // internal static BL _instance = null;

        internal static readonly Lazy<BlApi.IBL> _instance = new Lazy<BlApi.IBL>(() => new BL());

        public static  BlApi.IBL GetInstance
        {
            get { return _instance.Value; }
        }


        private PriorityQueue<DO.Parcel> _waitingParcels = new PriorityQueue<DO.Parcel>();

        private DalApi.IDAL _idalObj;

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
        private int _GetNearestAvilableStation(DO.Location location)
        {
            int stationId = -1;
            var stations = _idalObj.GetStationsList();
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
        private DO.Parcel findSuitableParcel(DO.Drone drone, int lessPriority = 0)
        {
            //first create a priority queue with all the parcels with highest priority
            PriorityQueue<DO.Parcel> parcelsAccordingWeight = new PriorityQueue<DO.Parcel>();
            DO.Parcel parcelForAssign = this._waitingParcels.Dequeue();
            DO.Priorities priority = parcelForAssign.Priority - lessPriority;

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
                if (this._waitingParcels.Count != 0)
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

            return parcelForAssign;
        }

        private void HandleAssignParcel(DO.Parcel parcel, DO.Drone drone)
        {
            parcel.Scheduled = DateTime.Now;
            parcel.Status = DO.ParcelStatuses.Assign;

            parcel.DroneId = drone.Id;

            var sender = _idalObj.GetCostumerById(parcel.SenderId);
            var nearStation =
                _idalObj.GetStationById(_GetNearestStation(sender.Location));

            drone.Status = DO.DroneStatuses.Shipping; // change the drone status to Shipping

            _InitDroneLocation(drone, parcel, nearStation); // set drone's location

            _InitBattery(drone, parcel, nearStation); // set drone's battery
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
        }

        private BL()
        {
            // handle all drones, init their values (location, battery, etc):
            this._idalObj = DalFactory.GetDal(DO.DalTypes.DalObj); // Singleton

            IEnumerable<DO.Drone> drones = _idalObj.GetDroneList();

            //find all the drones which was assign to a parcel, and change there status to Shiping
            IEnumerable<DO.Parcel> parcels = _idalObj.GetParcelsList();

            foreach (var parcel in parcels)
            {
                if (
                    parcel.DroneId !=
                    0) // the parcel has been assigned to drone, in this part I handle all the shiping drones.
                {
                    HandleAssignParcel(parcel, _idalObj.GetDroneById(parcel.DroneId));
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
        *Description: add new Costumer to costumers. check logic, then use dalObject for adding.
        *Parameters: costumer's details.
        *Return: None.
        */
        public void AddCostumer(int id, string name, string phone, double longitude, double latitude)
        {
            IEnumerable<DO.Costumer> costumers = _idalObj.GetCostumerList();

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

            this._idalObj.AddCostumer(id, name, phone, new DO.Location(longitude, latitude));
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

            int id = DataSource.ParcelsLength() + 1;
            this._idalObj.AddParcel(id, senderId, targetId, weight,
                priority, DateTime.Now, droneId, null,
                null, null);

            SysLog.SysLog.GetInstance().MoveParcelToWaitingList(id);
            this._waitingParcels.Enqueue(this._idalObj.GetParcelById(id), priority);
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

            var drone = this._idalObj.GetDroneById(droneId);

            if (drone.Status != DO.DroneStatuses.Available)
            {
                throw new BO.DroneNotAvliable(droneId);
            }

            if (this._waitingParcels.Count == 0)
            {
                throw new BO.NoParcelsForAssign();
            }

            //check which parcel we prefer to assign to these drone.
            DO.Parcel parcelForAssign = findSuitableParcel(drone);

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


            DO.Parcel parcel = this._idalObj.GetParcelByDroneId(droneId);
            DO.Drone drone = this._idalObj.GetDroneById(droneId);
            DO.Costumer sender = this._idalObj.GetCostumerById(parcel.SenderId);

            if (parcel.Scheduled == null)
            {
                throw new Exception("ERROR: The parcel (id: " + parcel.Id +
                                    ") is not Assign (not supposed to happen check [AssignParcelToDrone] method)");
            }

            if (parcel.PickedUp != null)
            {
                throw new ParcelIsAlreadyPickedUp(parcel.Id);
            }

            double distance = drone.Location.Distance(sender.Location) * DataSource.Config.avilablePPK;
            drone.Location = sender.Location;
            drone.Battery -= distance;

            parcel.PickedUp = DateTime.Now;
            parcel.Status = DO.ParcelStatuses.PickedUp;
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

            DO.Costumer costumer = this._idalObj.GetCostumerById(costumerId);
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

            DO.Station station = this._idalObj.GetStationById(stationId);
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

            DO.Drone drone = this._idalObj.GetDroneById(droneId);

            //if (drone.Model == name)
            //{
            //    throw new BO.NotNewValue("Drone model", name);
            //}

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

            DO.Drone drone = _idalObj.GetDroneById(droneId);
            if (drone.Status != DroneStatuses.Shipping) throw new DroneNotShipping(droneId);
            DO.Parcel parcel = _idalObj.GetParcelByDroneId(droneId);


            if (parcel.PickedUp == null)
            {
                throw new ParcelNotPickedUp(parcel.Id);
            }

            if (parcel.Delivered != null)
            {
                throw new ParcelIsAlreadyDelivered(parcel.Id);
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
            if (drone.Status != DO.DroneStatuses.Available)
            {
                throw new BO.DroneNotAvliable(droneId);
            }

            DO.Station station;

            if (stationId == -1)
            {
                station = this._idalObj.GetStationById(this._GetNearestAvilableStation(drone.Location));
                double distance = drone.Location.Distance(station.Location);
                double minBattery = distance * DalObject.DataSource.Config.avilablePPK;

                if (minBattery > drone.Battery)
                {
                    throw new BO.DroneNotEnoughBattery(droneId);
                }

                drone.Battery -= minBattery;

                drone.Location = station.Location;

                drone.Status = DO.DroneStatuses.Maintenance;

                station.ChargeSlots--;

                this._idalObj.AddDroneToCharge(droneId, station.Id);
            }

            else
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

            DO.Drone drone = this._idalObj.GetDroneById(droneId);

            if (drone.Status != DO.DroneStatuses.Maintenance)
            {
                throw new BO.DroneNotInMaintenance(droneId);
            }

            this._idalObj.DroneRelease(droneId, hours);
        }

        //getters:

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
    }
}