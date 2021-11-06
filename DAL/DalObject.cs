using System;
using System.Collections.Generic;
using System.Text;

namespace DalObject
{
    public class DalObject : IDAL.IDAL
    {
        public DalObject()
        {
            DataSource.Initialize();
        }

        /*
       	*Description: add new Station to stations.
       	*Parameters: station's id, station's name, station's longitube, station's latitude, charge's slots.
       	*Return: None.
        */
        public void AddStation(int id, string name, double longitube, double latitude, int charge_solts)
        {
            DataSource.stations.Add(new IDAL.DO.Station(id, name, longitube, latitude, charge_solts));
        }

        /*
   	    *Description: add new Drone to drones.
       	*Parameters: drone's details.	
       	*Return: None.
        */
        public void AddDrone(int id, string model, int maxWeight)
        {
            DataSource.drones.Add(new IDAL.DO.Drone(id, model, (IDAL.DO.WeightCategories) maxWeight));
        }

        /*
        *Description: add new Costumer to costumers.
        *Parameters: costumer's details.
        *Return: None.
        */
        public void AddCostumer(int id, string name, string phone, double longitube, double latitude)
        {
            DataSource.costumers.Add(new IDAL.DO.Costumer(id, name, phone, longitube, latitude));
        }

        /*
        *Description: add new Paracel to paracels.
        *Parameters: paracel's detatils.
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

        /*
        *Description: move parcel to waiting list.
        *Parameters: a paracel.
        *Return: None.
        */
        public void MoveParcelToWaitingList(IDAL.DO.Parcel parcel)
        {
            DataSource.waitingParcels.Enqueue(parcel); // if all the drones are not availible
            throw new IDAL.DO.NonAvilableDrones();
        }

        /*
        *Description: get next parcel from waiting list.
        *Parameters: None.
        *Return: parcel if the list is not empty, else null.
        */
        public IDAL.DO.Parcel GetNextParcel()
        {
            if (DataSource.waitingParcels.Count != 0)
            {
                return DataSource.waitingParcels.Dequeue();
            }

            else
            {
                return null; 
            }
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
        public void DroneRelease(int droneId)
        {
            IDAL.DO.DroneCharge dc;
            IDAL.DO.Drone drone;
            IDAL.DO.Station station;

            dc = GetDroneChargeByDroneId(droneId);
            drone = GetDroneById(droneId);
            station = GetStationById(dc.StationId);

            station.ChargeSolts++;
            DataSource.droneCharge.Remove(dc);
            drone.Status = IDAL.DO.DroneStatuses.Available;
            drone.Battery = 100;
        }

        public double[] PowerRequest() 
            //TODO: Implement this method
        {
            double[] arr = new double[] {0, 0, 0, 0, 0};
            return arr;
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

        public IDAL.DO.DroneCharge GetDroneChargeByDroneId(int id)
        {
            foreach (var dc in DataSource.droneCharge)
                if (dc.DroneId == id)
                {
                    return dc;
                }

            throw new IDAL.DO.ItemNotFound("Drone");
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

        public Queue<IDAL.DO.Parcel> GetWaitingParcels()
        {
            return DataSource.waitingParcels;
        }
    }
}