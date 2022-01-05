using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DalApi;
using System.Runtime.CompilerServices;

namespace Dal
{
    sealed class DalObject : DalApi.IDal
    {
        //singeltone design pattern

        internal static readonly Lazy<DalApi.IDal> _instance = new Lazy<DalApi.IDal>(() => new DalObject());

        public static DalApi.IDal GetInstance
        {
            get { return _instance.Value; }
        }

        private DalObject()
        {
            DataSource.Initialize();
        }

        /*
       	*Description: add new Station to stations.
       	*Parameters: station's id, station's name, station's location, charge's slots.
       	*Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(int id, string name, DO.Location location, int chargeSlots)
        {
            DataSource.stations.Add(new DO.Station(id, name, location, chargeSlots));
        }

        /*
   	    *Description: add new Drone to drones.
       	*Parameters: drone's details.	
       	*Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(int id, string model, DO.WeightCategories maxWeight, double battery)
        {
            DataSource.drones.Add(new DO.Drone(id, model, maxWeight, battery));
        }

        /*
        *Description: add new Costumer to costumers.
        *Parameters: costumer's details.
        *Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCostumer(int id, string name, string phone, DO.Location location, string email, string password)
        {
            DataSource.costumers.Add(new DO.Costumer(id, name, phone, location, email, password));
        }

        /*
        *Description: add new Parcel to parcels.
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

            DataSource.parcels.Add(new DO.Parcel(id, senderId, targetId, (DO.WeightCategories) weight,
                (DO.Priorities) priority, requested,
                droneId, scheduled, pickedUp, delivered));
        }

        /*
        *Description: Send drone to charge's station.
        *Parameters: a drone, a station
        *Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDroneToCharge(int droneId, int stationId)
        {
            DO.DroneCharge dc = new DO.DroneCharge(droneId, stationId);
            DataSource.droneCharge.Add(dc);
        }

        public void DroneCharging(int droneId, double hours)
        {
        }

        /*
        *Description: release drone from station. 
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

            //handle the station
            station.ChargeSlots++;
            DataSource.droneCharge.Remove(dc);

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
        }

        public double[] PowerRequest()
        {
            double[] arr = new double[]
            {
                DataSource.Config.avilablePPK, DataSource.Config.lightPPK,
                DataSource.Config.mediumPPK, DataSource.Config.heavyPPK, DataSource.Config.chargeRatePH
            };
            return arr;
        }

        /*
        *Description: Sign in to system. 
        *Parameters: costumer's id of logged user.
        *Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SignIn(int costumerId)
        {
            DataSource.loggetCostumer = GetCostumerById(costumerId);
        }

        /*
        *Description: Sign out from system. 
        *Parameters: None.
        *Return: None.
        */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SignOut()
        {
            DataSource.loggetCostumer = null;
        }

        //getters
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsDroneCharge(int droneId)
        {
            foreach (var droneCharge in DataSource.droneCharge)
            {
                if (droneCharge.DroneId == droneId) return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Costumer GetLoggedUser()
        {
            return DataSource.loggetCostumer;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Parcel GetParcelById(int id)
        {
            foreach (var parcel in DataSource.parcels)
                if (parcel.Id == id)
                {
                    return parcel;
                }

            throw new DO.ItemNotFound("Parcel");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Costumer GetCostumerById(int id)
        {
            foreach (var costumer in DataSource.costumers)
                if (costumer.Id == id)
                {
                    return costumer;
                }

            throw new DO.ItemNotFound("Costumer");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Station GetStationById(int id)
        {
            foreach (var station in DataSource.stations)
                if (station.Id == id)
                {
                    return station;
                }

            throw new DO.ItemNotFound("Station");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Drone GetDroneById(int id)
        {
            foreach (var drone in DataSource.drones)
                if (drone.Id == id)
                {
                    return drone;
                }

            throw new DO.ItemNotFound("Drone");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.Parcel GetParcelByDroneId(int droneId)
        {
            foreach (var parcel in DataSource.parcels)
            {
                if (parcel.DroneId == droneId && parcel.Delivered == null)
                    return parcel;
            }

            throw new DO.ItemNotFound("Parcel");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DO.DroneCharge GetDroneChargeByDroneId(int id)
        {
            foreach (var dc in DataSource.droneCharge)
                if (dc.DroneId == id)
                {
                    return dc;
                }

            throw new DO.ItemNotFound("Drone");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DO.DroneCharge> GetDroneChargeList(Func<DO.DroneCharge, bool> filter = null)
        {
            return filter == null ? DataSource.droneCharge : DataSource.droneCharge.Where(filter);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DO.Station> GetStationsList(Func<DO.Station, bool> filter = null)
        {
            return filter == null ? DataSource.stations : DataSource.stations.Where(filter);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DO.Costumer> GetCostumerList(Func<DO.Costumer, bool> filter = null)
        {
            return filter == null ? DataSource.costumers : DataSource.costumers.Where(filter);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DO.Parcel> GetParcelsList(Func<DO.Parcel, bool> filter = null)
        {
            return filter == null ? DataSource.parcels : DataSource.parcels.Where(filter);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DO.Drone> GetDroneList(Func<DO.Drone, bool> filter = null)
        {
            return filter == null ? DataSource.drones : DataSource.drones.Where(filter);
        }

        public DO.DalTypes type()
        {
            return DO.DalTypes.DalObj;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDrone(DO.Drone drone)
        {
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDroneCharge(DO.DroneCharge dc)
        {
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCostumer(DO.Costumer costumer)
        {
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStation(DO.Station station)
        {
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateParcel(DO.Parcel parcel)
        {
        }
    }
}