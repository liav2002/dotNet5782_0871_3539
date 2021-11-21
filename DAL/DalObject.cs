using System;
using System.Collections.Generic;
using System.Text;

namespace DalObject
{
    public sealed class DalObject : IDAL.IDAL
    {
        private static DalObject _instance = null;

        public static IDAL.IDAL GetInstance()
        {
            return _instance ?? (_instance = new DalObject());
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
        public void AddStation(int id, string name, IDAL.DO.Location location, int charge_solts)
        {
            DataSource.stations.Add(new IDAL.DO.Station(id, name, location, charge_solts));
        }

        /*
   	    *Description: add new Drone to drones.
       	*Parameters: drone's details.	
       	*Return: None.
        */
        public void AddDrone(int id, string model, IDAL.DO.WeightCategories maxWeight, double battery)
        {
            DataSource.drones.Add(new IDAL.DO.Drone(id, model, maxWeight, battery));
        }

        /*
        *Description: add new Costumer to costumers.
        *Parameters: costumer's details.
        *Return: None.
        */
        public void AddCostumer(int id, string name, string phone, IDAL.DO.Location location)
        {
            DataSource.costumers.Add(new IDAL.DO.Costumer(id, name, phone, location));
        }

        /*
        *Description: add new Parcel to parcels.
        *Parameters: parcel's detatils.
        *Return: None.
        */
        public void AddParcel(int id, int senderId, int targetId, int weight, int priority, DateTime requested,
            int droneId, DateTime scheduled, DateTime pickedUp, DateTime delivered)
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

            DataSource.parcels.Add(new IDAL.DO.Parcel(id, senderId, targetId, (IDAL.DO.WeightCategories) weight,
                (IDAL.DO.Priorities) priority, requested,
                droneId, scheduled, pickedUp, delivered));
        }

        public void RemoveParcelFromWaitingList(IDAL.DO.Parcel parcel)
        {
            //TODO: Implemented this function
        }

        /*
        *Description: Send drone to charge's station.
        *Parameters: a drone, a station
        *Return: None.
        */
        public void AddDroneToCharge(int droneId, int stationId)
        {
            IDAL.DO.DroneCharge dc = new IDAL.DO.DroneCharge(droneId, stationId);
            DataSource.droneCharge.Add(dc);
        }

        /*
        *Description: release drone from station. 
        *Parameters: a drone.
        *Return: None.
        */
        public void DroneRelease(int droneId, double hours)
        {
            IDAL.DO.DroneCharge dc;
            IDAL.DO.Drone drone;
            IDAL.DO.Station station;

            dc = GetDroneChargeByDroneId(droneId);
            drone = GetDroneById(droneId);
            station = GetStationById(dc.StationId);

            station.ChargeSlots++;
            DataSource.droneCharge.Remove(dc);
            drone.Status = IDAL.DO.DroneStatuses.Available;
            drone.Battery += hours * DataSource.Config.chargeRatePH;

            if (drone.Battery > 100)
            {
                drone.Battery = 100;
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

        public IDAL.DO.Parcel GetParcelById(int id)
        {
            foreach (var parcel in DataSource.parcels)
                if (parcel.Id == id)
                {
                    return parcel;
                }

            throw new IDAL.DO.ItemNotFound("Parcel");
        }

        public IDAL.DO.Costumer GetCostumerById(int id)
        {
            foreach (var costumer in DataSource.costumers)
                if (costumer.Id == id)
                {
                    return costumer;
                }

            throw new IDAL.DO.ItemNotFound("Costumer");
        }

        public IDAL.DO.Station GetStationById(int id)
        {
            foreach (var station in DataSource.stations)
                if (station.Id == id)
                {
                    return station;
                }

            throw new IDAL.DO.ItemNotFound("Station");
        }

        public IDAL.DO.Drone GetDroneById(int id)
        {
            foreach (var drone in DataSource.drones)
                if (drone.Id == id)
                {
                    return drone;
                }

            throw new IDAL.DO.ItemNotFound("Drone");
        }

        public IDAL.DO.Parcel GetParcelByDroneId(int droneId)
        {
            foreach (var parcel in DataSource.parcels)
            {
                if (parcel.DroneId == droneId)
                    return parcel;
            }

            throw new IDAL.DO.ItemNotFound("Parcel");
        }

        public IDAL.DO.DroneCharge GetDroneChargeByDroneId(int id)
        {
            foreach (var dc in DataSource.droneCharge)
                if (dc.DroneId == id)
                {
                    return dc;
                }

            throw new IDAL.DO.ItemNotFound("Drone");
        }

        public IEnumerable<IDAL.DO.DroneCharge> GetDroneChargeList()
        {
            return DataSource.droneCharge;
        }

        public IEnumerable<IDAL.DO.Station> GetStationsList()
        {
            return DataSource.stations;
        }

        public IEnumerable<IDAL.DO.Costumer> GetCostumerList()
        {
            return DataSource.costumers;
        }

        public IEnumerable<IDAL.DO.Parcel> GetParcelsList()
        {
            return DataSource.parcels;
        }

        public IEnumerable<IDAL.DO.Drone> GetDroneList()
        {
            return DataSource.drones;
        }
    }
}