using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DalObject
{
    internal sealed class DalObject : DalApi.IDAL
    {
        internal static readonly Lazy<DalApi.IDAL> _instance = new Lazy<DalApi.IDAL>(() => new DalObject());

        internal static DalApi.IDAL GetInstance
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
        public void AddStation(int id, string name, DO.Location location, int charge_solts)
        {
            DataSource.stations.Add(new DO.Station(id, name, location, charge_solts));
        }

        /*
   	    *Description: add new Drone to drones.
       	*Parameters: drone's details.	
       	*Return: None.
        */
        public void AddDrone(int id, string model, DO.WeightCategories maxWeight, double battery)
        {
            DataSource.drones.Add(new DO.Drone(id, model, maxWeight, battery));
        }

        /*
        *Description: add new Costumer to costumers.
        *Parameters: costumer's details.
        *Return: None.
        */
        public void AddCostumer(int id, string name, string phone, DO.Location location)
        {
            DataSource.costumers.Add(new DO.Costumer(id, name, phone, location));
        }

        /*
        *Description: add new Parcel to parcels.
        *Parameters: parcel's detatils.
        *Return: None.
        */
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
        public void AddDroneToCharge(int droneId, int stationId)
        {
            DO.DroneCharge dc = new DO.DroneCharge(droneId, stationId);
            DataSource.droneCharge.Add(dc);
        }

        /*
        *Description: release drone from station. 
        *Parameters: a drone.
        *Return: None.
        */
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

        public bool IsDroneCharge(int droneId)
        {
            foreach (var droneCharge in DataSource.droneCharge)
            {
                if (droneCharge.DroneId == droneId) return true;
            }

            return false;
        }

        public DO.Parcel GetParcelById(int id)
        {
            foreach (var parcel in DataSource.parcels)
                if (parcel.Id == id)
                {
                    return parcel;
                }

            throw new DO.ItemNotFound("Parcel");
        }

        public DO.Costumer GetCostumerById(int id)
        {
            foreach (var costumer in DataSource.costumers)
                if (costumer.Id == id)
                {
                    return costumer;
                }

            throw new DO.ItemNotFound("Costumer");
        }

        public DO.Station GetStationById(int id)
        {
            foreach (var station in DataSource.stations)
                if (station.Id == id)
                {
                    return station;
                }

            throw new DO.ItemNotFound("Station");
        }

        public DO.Drone GetDroneById(int id)
        {
            foreach (var drone in DataSource.drones)
                if (drone.Id == id)
                {
                    return drone;
                }

            throw new DO.ItemNotFound("Drone");
        }

        public DO.Parcel GetParcelByDroneId(int droneId)
        {
            foreach (var parcel in DataSource.parcels)
            {
                if (parcel.DroneId == droneId)
                    return parcel;
            }

            throw new DO.ItemNotFound("Parcel");
        }

        public DO.DroneCharge GetDroneChargeByDroneId(int id)
        {
            foreach (var dc in DataSource.droneCharge)
                if (dc.DroneId == id)
                {
                    return dc;
                }

            throw new DO.ItemNotFound("Drone");
        }

        public IEnumerable<DO.DroneCharge> GetDroneChargeList(Func<DO.DroneCharge, bool> filter = null)
        {
            return filter == null ? DataSource.droneCharge : DataSource.droneCharge.Where(filter);
        }

        public IEnumerable<DO.Station> GetStationsList(Func<DO.Station, bool> filter = null)
        {
            return filter == null ? DataSource.stations : DataSource.stations.Where(filter);
        }

        public IEnumerable<DO.Costumer> GetCostumerList(Func<DO.Costumer, bool> filter = null)
        {
            return filter == null ? DataSource.costumers : DataSource.costumers.Where(filter);
        }

        public IEnumerable<DO.Parcel> GetParcelsList(Func<DO.Parcel, bool> filter = null)
        {
            return filter == null ? DataSource.parcels : DataSource.parcels.Where(filter);
        }


        public IEnumerable<DO.Drone> GetDroneList(Func<DO.Drone, bool> filter = null)
        {
            return filter == null ? DataSource.drones : DataSource.drones.Where(filter);
        }
    }
}