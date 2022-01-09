using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DalApi;
using System.Runtime.CompilerServices;

namespace Dal
{
    sealed class DalXml : DalApi.IDal
    {
        //singeltone design pattern

        internal static readonly Lazy<DalApi.IDal> _instance = new Lazy<DalApi.IDal>(() => new DalXml());

        public static DalApi.IDal GetInstance
        {
            get { return _instance.Value; }
        }

        //private variables
        readonly string dronePath;
        readonly string droneChargePath;
        readonly string stationPath;
        readonly string costumerPath;
        readonly string parcelPath;
        readonly string configPath;
        public static string localPath;

        //varialbe to save the currentLoggedUser
        internal static DO.Costumer loggedCostumer = null;

        private DalXml()
        {
            string str = Assembly.GetExecutingAssembly().Location;
            localPath = Path.GetDirectoryName(str).Split("PL")[0];
            localPath += @"xml";

            dronePath = localPath + @"\DroneXml.xml";
            droneChargePath = localPath + @"\DroneChargeXml.xml";
            stationPath = localPath + @"\StationXml.xml";
            costumerPath = localPath + @"\CostumerXml.xml";
            parcelPath = localPath + @"\ParcelXml.xml";
            configPath = localPath + @"\configXml.xml";
        }

        /*****************************
         * methods of interface IDAL * 
         *****************************/

        /*
       	*Description: add new Station to database xml.
       	*Parameters: station's id, station's name, station's location, charge's slots.
       	*Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(int id, string name, DO.Location location, int chargeSlots)
        {
            XElement stations = XmlTools.LoadListFromXMLElement(stationPath);
            DO.Station station = new DO.Station(id, name, location, chargeSlots);
            stations.Add(createStationElement(station));
            XmlTools.SaveListToXMLElement(stations, stationPath);
        }

        /*
   	    *Description: add new Drone to database xml.
       	*Parameters: drone's details.	
       	*Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(int id, string model, DO.WeightCategories maxWeight, double battery)
        {
            XElement drones = XmlTools.LoadListFromXMLElement(dronePath);
            DO.Drone drone = new DO.Drone(id, model, maxWeight, battery);
            drones.Add(createDroneElement(drone));
            XmlTools.SaveListToXMLElement(drones, dronePath);
        }

        /*
        *Description: add new Costumer to database xml.
        *Parameters: costumer's details.
        *Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCostumer(int id, string name, string phone, DO.Location location, string email, string password)
        {
            XElement costumers = XmlTools.LoadListFromXMLElement(costumerPath);
            DO.Costumer costumer = new DO.Costumer(id, name, phone, location, email, password);
            costumers.Add(createCostumerElement(costumer));
            XmlTools.SaveListToXMLElement(costumers, costumerPath);
        }

        /*
        *Description: add new Parcel to database xml.
        *Parameters: parcel's detatils.
        *Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddParcel(int id, int senderId, int targetId, int weight, int priority, DateTime? requested,
            int droneId, DateTime? scheduled, DateTime? pickedUp, DateTime? delivered)
        {
            try
            {
                GetCostumerById(senderId);
                GetCostumerById(targetId);
            }

            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            XElement parcels = XmlTools.LoadListFromXMLElement(parcelPath);

            DO.Parcel parcel = new DO.Parcel(id, senderId, targetId, (DO.WeightCategories) weight,
                (DO.Priorities) priority, requested,
                droneId, scheduled, pickedUp, delivered);

            parcels.Add(createParcelElement(parcel));

            XmlTools.SaveListToXMLElement(parcels, parcelPath);
        }

        /*
        *Description: Send drone to charge's station in database xml.
        *Parameters: a drone, a station
        *Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDroneToCharge(int droneId, int stationId)
        {
            try
            {
                XElement dronesCharge = XmlTools.LoadListFromXMLElement(droneChargePath);

                DO.DroneCharge dc = new DO.DroneCharge(droneId, stationId);
                dronesCharge.Add(createDroneChargeElement(dc));

                XmlTools.SaveListToXMLElement(dronesCharge, droneChargePath);
            }

            catch
            {
                throw new Exception("Failed to send drone to charge.");
            }
        }

        /*
        *Description: release drone from station (delete him from the table of <dronesInCharge> in database xml). 
        *Parameters: a drone.
        *Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DroneRelease(int droneId, double hours)
        {
            DO.DroneCharge dc;
            DO.Drone drone;
            DO.Station station;

            dc = GetDroneChargeByDroneId(droneId);
            drone = GetDroneById(droneId);
            station = GetStationById(dc.StationId);

            //remove drone from dronesCharge
            try
            {
                XElement dronesCharge = XmlTools.LoadListFromXMLElement(droneChargePath);
                (from droneInCharge in dronesCharge.Elements()
                    let id = int.Parse(droneInCharge.Element("DroneId").Value)
                    where id == dc.DroneId
                    select droneInCharge).FirstOrDefault().Remove();
                XmlTools.SaveListToXMLElement(dronesCharge, droneChargePath);
            }
            catch
            {
                throw new Exception("Failed to release drone.");
            }

            //handle the station
            station.ChargeSlots++;

            //change drone's status
            drone.Status = DO.DroneStatuses.Available;

            //change drone's battery
            double newBattery = drone.Battery + hours * DataSource.Config.chargeRatePH;

            if (newBattery > 100)
            {
                drone.Battery = 100;
            }

            else
            {
                drone.Battery = newBattery;
            }

            UpdateDrone(drone);
            UpdateStation(station);
        }

        /*
        *Description: The function pulls a drone from the appropriate table in the database and checks if it is charging. 
        *Parameters: drone's id.
        *Return: true - if the drone is chraging, false - else.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsDroneCharge(int droneId)
        {
            XElement dronesCharge = XmlTools.LoadListFromXMLElement(droneChargePath);

            DO.DroneCharge dc = new DO.DroneCharge();

            dc = (from drone in dronesCharge.Elements()
                let id = int.Parse(drone.Element("DroneId").Value)
                where droneId == id
                select new DO.DroneCharge()
                {
                    DroneId = int.Parse(drone.Element("DroneId").Value),
                    StationId = int.Parse(drone.Element("StationId").Value),
                    StartTime = DateTime.Parse(drone.Element("StartTime").Value)
                }).FirstOrDefault();

            if (dc.DroneId != 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        /*
        *Description: The function pulls a customer from the customer's table in the database and saves it as a variable of a loggedCostumer.
        *Parameters: costumer's id.
        *Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SignIn(int costumerId)
        {
            loggedCostumer = GetCostumerById(costumerId);
        }

        /*
        *Description: The function intialize the loggedCostumer variable to null.
        *Parameters: None.
        *Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SignOut()
        {
            loggedCostumer = null;
        }

        /*****************************
         * getters of interface IDAL * 
         *****************************/

        /*
        *Description: The function return the save loggedCostumer.
        *Parameters: None.
        *Return: curren logged user (type: DO.Costumer).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Costumer GetLoggedUser()
        {
            return loggedCostumer;
        }

        /*
        *Description: The function retrieves a parcel from the database.
        *Parameters: parcel's id.
        *Return: parcel (type: DO.Parcel).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Parcel GetParcelById(int id)
        {
            XElement parcels = XmlTools.LoadListFromXMLElement(parcelPath);

            DO.Parcel parcel = new DO.Parcel();

            parcel = (from currentParcel in parcels.Elements()
                let parcelId = int.Parse(currentParcel.Element("Id").Value)
                where id == parcelId
                select new DO.Parcel()
                {
                    Id = int.Parse(currentParcel.Element("Id").Value),
                    DroneId = int.Parse(currentParcel.Element("DroneId").Value),
                    SenderId = int.Parse(currentParcel.Element("SenderId").Value),
                    TargetId = int.Parse(currentParcel.Element("TargetId").Value),
                    Status = (DO.ParcelStatuses) Enum.Parse(typeof(DO.ParcelStatuses),
                        currentParcel.Element("Status").Value),
                    Weight = (DO.WeightCategories) Enum.Parse(typeof(DO.WeightCategories),
                        currentParcel.Element("Weight").Value),
                    Priority = (DO.Priorities) Enum.Parse(typeof(DO.Priorities),
                        currentParcel.Element("Priority").Value),
                    Requested = DateTime.Parse(currentParcel.Element("Requested").Value),
                    Scheduled = DateTime.Parse(currentParcel.Element("Scheduled").Value),
                    PickedUp = DateTime.Parse(currentParcel.Element("PickedUp").Value),
                    Delivered = DateTime.Parse(currentParcel.Element("Delivered").Value),
                    IsAvailable = bool.Parse(currentParcel.Element("IsAvailable").Value)
                }).FirstOrDefault();

            if (parcel.Id == 0)
            {
                throw new DO.ItemNotFound("parcel");
            }

            return parcel;
        }

        /*
        *Description: The function retrieves a costumer from the database.
        *Parameters: costumer's id.
        *Return: costumrt (type: DO.Costumer).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Costumer GetCostumerById(int id)
        {
            XElement costumers = XmlTools.LoadListFromXMLElement(costumerPath);

            DO.Costumer costumer = new DO.Costumer();

            costumer = (from currentCostumer in costumers.Elements()
                let costumerId = int.Parse(currentCostumer.Element("Id").Value)
                where id == costumerId
                select new DO.Costumer()
                {
                    Id = int.Parse(currentCostumer.Element("Id").Value),
                    Name = currentCostumer.Element("Name").Value,
                    Phone = currentCostumer.Element("Phone").Value,
                    Location = new DO.Location(double.Parse(currentCostumer.Element("Latitude").Value),
                        double.Parse(currentCostumer.Element("Longitude").Value)),
                    IsAvaliable = bool.Parse(currentCostumer.Element("IsAvailable").Value),
                    Email = currentCostumer.Element("Email").Value,
                    Password = currentCostumer.Element("Password").Value,
                    IsManger = bool.Parse(currentCostumer.Element("IsManager").Value)
                }).FirstOrDefault();

            if (costumer == null)
            {
                throw new DO.ItemNotFound("costumer");
            }

            return costumer;
        }

        /*
        *Description: The function retrieves a station from the database.
        *Parameters: station's id.
        *Return: station (type: DO.Station).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Station GetStationById(int id)
        {
            XElement stations = XmlTools.LoadListFromXMLElement(stationPath);

            DO.Station station = new DO.Station();

            station = (from currentStation in stations.Elements()
                let stationId = int.Parse(currentStation.Element("Id").Value)
                where id == stationId
                select new DO.Station()
                {
                    Id = int.Parse(currentStation.Element("Id").Value),
                    Name = currentStation.Element("Name").Value,
                    Location = new DO.Location(double.Parse(currentStation.Element("Latitude").Value),
                        double.Parse(currentStation.Element("Longitude").Value)),
                    ChargeSlots = int.Parse(currentStation.Element("ChargeSlots").Value),
                    IsAvailable = bool.Parse(currentStation.Element("IsAvailable").Value)
                }).FirstOrDefault();

            if (station.Id == 0)
            {
                throw new DO.ItemNotFound("station");
            }

            return station;
        }

        /*
        *Description: The function retrieves a drone from the database.
        *Parameters: drone's id.
        *Return: drone (type: DO.Drone).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Drone GetDroneById(int id)
        {
            XElement drones = XmlTools.LoadListFromXMLElement(dronePath);

            DO.Drone drone = new DO.Drone();

            drone = (from currentDrone in drones.Elements()
                let droneId = int.Parse(currentDrone.Element("Id").Value)
                where id == droneId
                select new DO.Drone()
                {
                    Id = int.Parse(currentDrone.Element("Id").Value),
                    Model = currentDrone.Element("Model").Value,
                    MaxWeight = (DO.WeightCategories) Enum.Parse(typeof(DO.WeightCategories),
                        currentDrone.Element("MaxWeight").Value),
                    Status = (DO.DroneStatuses) Enum.Parse(typeof(DO.DroneStatuses),
                        currentDrone.Element("Status").Value),
                    Battery = double.Parse(currentDrone.Element("Battery").Value),
                    Location = new DO.Location(double.Parse(currentDrone.Element("Latitude").Value),
                        double.Parse(currentDrone.Element("Longitude").Value)),
                    IsAvaliable = bool.Parse(currentDrone.Element("IsAvailable").Value)
                }).FirstOrDefault();

            if (drone.Id == 0)
            {
                throw new DO.ItemNotFound("drone");
            }

            return drone;
        }

        /*
        *Description: The function retrieves a parcel from the database (according to drone's id).
        *Parameters: drone's id.
        *Return: parcel (type: DO.Parcel).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Parcel GetParcelByDroneId(int droneId)
        {
            XElement parcels = XmlTools.LoadListFromXMLElement(parcelPath);

            DO.Parcel parcel = new DO.Parcel();

            parcel = (from currentParcel in parcels.Elements()
                let id = int.Parse(currentParcel.Element("DroneId").Value)
                where id == droneId
                select new DO.Parcel()
                {
                    Id = int.Parse(currentParcel.Element("Id").Value),
                    DroneId = int.Parse(currentParcel.Element("DroneId").Value),
                    SenderId = int.Parse(currentParcel.Element("SenderId").Value),
                    TargetId = int.Parse(currentParcel.Element("TargetId").Value),
                    Status = (DO.ParcelStatuses) Enum.Parse(typeof(DO.ParcelStatuses),
                        currentParcel.Element("Status").Value),
                    Weight = (DO.WeightCategories) Enum.Parse(typeof(DO.WeightCategories),
                        currentParcel.Element("Weight").Value),
                    Priority = (DO.Priorities) Enum.Parse(typeof(DO.Priorities),
                        currentParcel.Element("Priority").Value),
                    Requested = DateTime.Parse(currentParcel.Element("Requested").Value),
                    Scheduled = DateTime.Parse(currentParcel.Element("Scheduled").Value),
                    PickedUp = DateTime.Parse(currentParcel.Element("PickedUp").Value),
                    Delivered = DateTime.Parse(currentParcel.Element("Delivered").Value),
                    IsAvailable = bool.Parse(currentParcel.Element("IsAvailable").Value)
                }).FirstOrDefault();

            if (parcel.Id == 0)
            {
                throw new DO.ItemNotFound("parcel");
            }

            return parcel;
        }

        /*
        *Description: The function retrieves a drone in charge from the database (according to drone's id).
        *Parameters: drone's id.
        *Return: drone (type: DO.DroneCharge).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.DroneCharge GetDroneChargeByDroneId(int id)
        {
            XElement dronesCharge = XmlTools.LoadListFromXMLElement(droneChargePath);

            DO.DroneCharge dc = new DO.DroneCharge();

            dc = (from drone in dronesCharge.Elements()
                let droneId = int.Parse(drone.Element("DroneId").Value)
                where droneId == id
                select new DO.DroneCharge()
                {
                    DroneId = int.Parse(drone.Element("DroneId").Value),
                    StationId = int.Parse(drone.Element("StationId").Value),
                    StartTime = DateTime.Parse(drone.Element("StartTime").Value)
                }).FirstOrDefault();

            if (dc.DroneId == 0)
            {
                throw new DO.ItemNotFound("DronesCharge");
            }

            return dc;
        }

        /*
        *Description: The function retrieves from the database all the drones in charge 
        *             according to a certain filtering parameter and represents them as a IEnumurable-type list.
        *Parameters: filtering function.
        *Return: list of drones in charge (type: IEnumrable<DO.DroneCharge>).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DO.DroneCharge> GetDroneChargeList(Func<DO.DroneCharge, bool> filter = null)
        {
            try
            {
                XElement dronesCharge = XmlTools.LoadListFromXMLElement(droneChargePath);

                return from drone in dronesCharge.Elements()
                    let currentDrone = new DO.DroneCharge()
                    {
                        DroneId = int.Parse(drone.Element("DroneId").Value),
                        StationId = int.Parse(drone.Element("StationId").Value),
                        StartTime = DateTime.Parse(drone.Element("StartTime").Value)
                    }
                    where (filter == null) ? true : filter(currentDrone)
                    select currentDrone;
            }

            catch
            {
                throw new DO.ItemNotFound("DronesCharge list");
            }
        }

        /*
        *Description: The function retrieves from the database all the stations according to
        *             a certain filtering parameter and represents them as a IEnumurable-type list.
        *Parameters: filtering function.
        *Return: list of stations (type: IEnumrable<DO.Station>).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DO.Station> GetStationsList(Func<DO.Station, bool> filter = null)
        {
            try
            {
                XElement stations = XmlTools.LoadListFromXMLElement(stationPath);

                return from station in stations.Elements()
                    let currentStation = new DO.Station()
                    {
                        Id = int.Parse(station.Element("Id").Value),
                        Name = station.Element("Name").Value,
                        Location = new DO.Location(double.Parse(station.Element("Latitude").Value),
                            double.Parse(station.Element("Longitude").Value)),
                        ChargeSlots = int.Parse(station.Element("ChargeSlots").Value),
                        IsAvailable = bool.Parse(station.Element("IsAvailable").Value)
                    }
                    where (filter == null) ? true : filter(currentStation)
                    select currentStation;
            }

            catch
            {
                throw new DO.ItemNotFound("Station's list");
            }
        }

        /*
        *Description: The function retrieves from the database all the costumers according to
        *             a certain filtering parameter and represents them as a IEnumurable-type list.
        *Parameters: filtering function.
        *Return: list of costumers (type: IEnumrable<DO.Costumer>).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DO.Costumer> GetCostumerList(Func<DO.Costumer, bool> filter = null)
        {
            try
            {
                XElement costumers = XmlTools.LoadListFromXMLElement(costumerPath);

                return from costumer in costumers.Elements()
                    let currentCostumer = new DO.Costumer()
                    {
                        Id = int.Parse(costumer.Element("Id").Value),
                        Name = costumer.Element("Name").Value,
                        Phone = costumer.Element("Phone").Value,
                        Location = new DO.Location(double.Parse(costumer.Element("Latitude").Value),
                            double.Parse(costumer.Element("Longitude").Value)),
                        IsAvaliable = bool.Parse(costumer.Element("IsAvailable").Value),
                        Email = costumer.Element("Email").Value,
                        Password = costumer.Element("Password").Value,
                        IsManger = bool.Parse(costumer.Element("IsManager").Value)
                    }
                    where (filter == null) ? true : filter(currentCostumer)
                    select currentCostumer;
            }

            catch
            {
                throw new DO.ItemNotFound("Costumer's list");
            }
        }

        /*
        *Description: The function retrieves from the database all the parcels according to
        *             a certain filtering parameter and represents them as a IEnumurable-type list.
        *Parameters: filtering function.
        *Return: list of parcels (type: IEnumrable<DO.Parcel>).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DO.Parcel> GetParcelsList(Func<DO.Parcel, bool> filter = null)
        {
            try
            {
                XElement parcels = XmlTools.LoadListFromXMLElement(parcelPath);

                return from parcel in parcels.Elements()
                    let currentParcel = new DO.Parcel()
                    {
                        Id = int.Parse(parcel.Element("Id").Value),
                        DroneId = int.Parse(parcel.Element("DroneId").Value),
                        SenderId = int.Parse(parcel.Element("SenderId").Value),
                        TargetId = int.Parse(parcel.Element("TargetId").Value),
                        Status = (DO.ParcelStatuses) Enum.Parse(typeof(DO.ParcelStatuses),
                            parcel.Element("Status").Value),
                        Weight = (DO.WeightCategories) Enum.Parse(typeof(DO.WeightCategories),
                            parcel.Element("Weight").Value),
                        Priority = (DO.Priorities) Enum.Parse(typeof(DO.Priorities), parcel.Element("Priority").Value),
                        Requested = DateTime.Parse(parcel.Element("Requested").Value),
                        Scheduled = DateTime.Parse(parcel.Element("Scheduled").Value),
                        PickedUp = DateTime.Parse(parcel.Element("PickedUp").Value),
                        Delivered = DateTime.Parse(parcel.Element("Delivered").Value),
                        IsAvailable = bool.Parse(parcel.Element("IsAvailable").Value)
                    }
                    where (filter == null) ? true : filter(currentParcel)
                    select currentParcel;
            }

            catch
            {
                throw new DO.ItemNotFound("Parcel's list");
            }
        }

        /*
        *Description: The function retrieves from the database all the drones according to
        *             a certain filtering parameter and represents them as a IEnumurable-type list.
        *Parameters: filtering function.
        *Return: list of drones (type: IEnumrable<DO.Drone>).
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DO.Drone> GetDroneList(Func<DO.Drone, bool> filter = null)
        {
            try
            {
                XElement drones = XmlTools.LoadListFromXMLElement(dronePath);

                return from drone in drones.Elements()
                    let currentDrone = new DO.Drone()
                    {
                        Id = int.Parse(drone.Element("Id").Value),
                        Model = drone.Element("Model").Value,
                        MaxWeight = (DO.WeightCategories) Enum.Parse(typeof(DO.WeightCategories),
                            drone.Element("MaxWeight").Value),
                        Status = (DO.DroneStatuses) Enum.Parse(typeof(DO.DroneStatuses), drone.Element("Status").Value),
                        Battery = double.Parse(drone.Element("Battery").Value),
                        Location = new DO.Location(double.Parse(drone.Element("Latitude").Value),
                            double.Parse(drone.Element("Longitude").Value)),
                        IsAvaliable = bool.Parse(drone.Element("IsAvailable").Value)
                    }
                    where (filter == null) ? true : filter(currentDrone)
                    select currentDrone;
            }

            catch
            {
                throw new DO.ItemNotFound("Drone's list");
            }
        }

        public DO.DalTypes type()
        {
            return DO.DalTypes.DalXml;
        }

        /*****************************
         * private metohds for help  * 
         *****************************/

        XElement createDroneChargeElement(DO.DroneCharge dc)
        {
            return new XElement("DroneCharge",
                new XElement("DroneId", dc.DroneId),
                new XElement("StationId", dc.StationId),
                new XElement("StartTime", dc.StartTime));
        }

        XElement createDroneElement(DO.Drone drone)
        {
            return new XElement("Drone",
                new XElement("Id", drone.Id),
                new XElement("Model", drone.Model),
                new XElement("MaxWeight", drone.MaxWeight),
                new XElement("Status", drone.Status),
                new XElement("Battery", drone.Battery),
                new XElement("Longitude", drone.Location.Longitude),
                new XElement("Latitude", drone.Location.Latitude),
                new XElement("IsAvailable", drone.IsAvaliable));
        }

        XElement createParcelElement(DO.Parcel parcel)
        {
            return new XElement("Parcel",
                new XElement("Id", parcel.Id),
                new XElement("DroneId", parcel.DroneId),
                new XElement("SenderId", parcel.SenderId),
                new XElement("TargetId", parcel.TargetId),
                new XElement("Status", parcel.Status),
                new XElement("Weight", parcel.Weight),
                new XElement("Priority", parcel.Priority),
                new XElement("Requested", parcel.Requested == null ? default(DateTime) : parcel.Requested),
                new XElement("Scheduled", parcel.Scheduled == null ? default(DateTime) : parcel.Scheduled),
                new XElement("PickedUp", parcel.PickedUp == null ? default(DateTime) : parcel.PickedUp),
                new XElement("Delivered", parcel.Delivered == null ? default(DateTime) : parcel.Delivered),
                new XElement("IsAvailable", parcel.IsAvailable));
        }

        XElement createStationElement(DO.Station station)
        {
            return new XElement("Station",
                new XElement("Id", station.Id),
                new XElement("Name", station.Name),
                new XElement("Longitude", station.Location.Longitude),
                new XElement("Latitude", station.Location.Latitude),
                new XElement("ChargeSlots", station.ChargeSlots),
                new XElement("IsAvailable", station.IsAvailable));
        }

        XElement createCostumerElement(DO.Costumer costumer)
        {
            return new XElement("Costumer",
                new XElement("Id", costumer.Id),
                new XElement("Name", costumer.Name),
                new XElement("Phone", costumer.Phone),
                new XElement("Longitude", costumer.Location.Longitude),
                new XElement("Latitude", costumer.Location.Latitude),
                new XElement("IsAvailable", costumer.IsAvaliable),
                new XElement("Email", costumer.Email),
                new XElement("Password", costumer.Password),
                new XElement("IsManager", costumer.IsManger));
        }

        public void UpdateDrone(DO.Drone drone)
        {
            XElement root = XmlTools.LoadListFromXMLElement(dronePath);
            IEnumerable<XElement> selectedDrone = from obj in root.Descendants("Id")
                where obj.Value != null
                      && obj.Value == drone.Id.ToString()
                select obj;

            if (selectedDrone.Count() == 0)
                return;

            XElement xDrone = selectedDrone.First();
            XElement newDrone = new XElement("Drone",
                new XElement("Id", drone.Id),
                new XElement("Model", drone.Model),
                new XElement("MaxWeight", drone.MaxWeight),
                new XElement("Status", drone.Status),
                new XElement("Battery", drone.Battery),
                new XElement("Longitude", drone.Location.Longitude),
                new XElement("Latitude", drone.Location.Latitude),
                new XElement("IsAvailable", drone.IsAvaliable)
            );

            root.Elements()
                .Where(x => (string) x.Attribute("Id") == (string) xDrone.Parent.Attribute("Id"))
                .FirstOrDefault()
                .AddAfterSelf(newDrone);

            xDrone.Parent.Remove();

            root.Save(dronePath);
        }


        public void UpdateCostumer(DO.Costumer costumer)
        {
            XElement root = XmlTools.LoadListFromXMLElement(costumerPath);

            var selectedCostumer = from obj in root.Descendants("Id")
                where obj.Value != null
                      && obj.Value == costumer.Id.ToString()
                select obj;

            if (selectedCostumer.Count() == 0)
                return;

            XElement xCostumer = selectedCostumer.First();
            XElement newCostumer = new XElement("Costumer",
                new XElement("Id", costumer.Id),
                new XElement("Name", costumer.Name),
                new XElement("Phone", costumer.Phone),
                new XElement("Longitude", costumer.Location.Longitude),
                new XElement("Latitude", costumer.Location.Latitude),
                new XElement("IsAvailable", costumer.IsAvaliable),
                new XElement("Email", costumer.Email),
                new XElement("Password", costumer.Password),
                new XElement("IsManager", costumer.IsManger)
            );

            root.Elements()
                .Where(x => (string) x.Attribute("Id") == (string) xCostumer.Parent.Attribute("Id"))
                .FirstOrDefault()
                .AddAfterSelf(newCostumer);

            xCostumer.Parent.Remove();

            root.Save(costumerPath);
        }

        public void UpdateStation(DO.Station station)
        {
            XElement root = XmlTools.LoadListFromXMLElement(stationPath);

            var selectedStation = from obj in root.Descendants("Id")
                where obj.Value != null
                      && obj.Value == station.Id.ToString()
                select obj;

            if (selectedStation.Count() == 0)
                return;

            XElement xStation = selectedStation.First();
            XElement newStation = new XElement("Station",
                new XElement("Id", station.Id),
                new XElement("Name", station.Name),
                new XElement("Longitude", station.Location.Longitude),
                new XElement("Latitude", station.Location.Latitude),
                new XElement("ChargeSlots", station.ChargeSlots),
                new XElement("IsAvailable", station.IsAvailable)
            );

            root.Elements()
                .Where(x => (string) x.Attribute("Id") == (string) xStation.Parent.Attribute("Id"))
                .FirstOrDefault()
                .AddAfterSelf(newStation);

            xStation.Parent.Remove();
            root.Save(stationPath);
        }

        public void UpdateParcel(DO.Parcel parcel)
        {
            XElement root = XmlTools.LoadListFromXMLElement(parcelPath);

            var selectedParcel = from obj in root.Descendants("Id")
                where obj.Value != null
                      && obj.Value == parcel.Id.ToString()
                select obj;

            if (selectedParcel.Count() == 0)
                return;

            XElement xParcel = selectedParcel.First();

            XElement newParcel = new XElement("Parcel",
                new XElement("Id", parcel.Id),
                new XElement("DroneId", parcel.DroneId),
                new XElement("SenderId", parcel.SenderId),
                new XElement("TargetId", parcel.TargetId),
                new XElement("Status", parcel.Status),
                new XElement("Weight", parcel.Weight),
                new XElement("Priority", parcel.Priority),
                new XElement("Requested", parcel.Requested == null ? default(DateTime) : parcel.Requested),
                new XElement("Scheduled", parcel.Scheduled == null ? default(DateTime) : parcel.Scheduled),
                new XElement("PickedUp", parcel.PickedUp == null ? default(DateTime) : parcel.PickedUp),
                new XElement("Delivered", parcel.Delivered == null ? default(DateTime) : parcel.Delivered),
                new XElement("IsAvailable", parcel.IsAvailable)
            );

            root.Elements()
                .Where(x => (string) x.Attribute("Id") == (string) xParcel.Parent.Attribute("Id"))
                .FirstOrDefault()
                .AddAfterSelf(newParcel);

            xParcel.Parent.Remove();

            root.Save(parcelPath);
        }
    }
}