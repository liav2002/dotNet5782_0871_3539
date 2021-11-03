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
       	*Description: add new Station to stations. check if station's list is not full before adding. if full print suitable message.
       	*Parameters: station's id, station's name, station's longitude, station's latitude, charge's slots.
       	*Return: true - if added successfully, false - else.
        */
        public void AddStation(int id, string name, double longitude, double latitude, int charge_solts)
        {
            foreach (var station in DataSource.stations)
            {
                if (station.Id == id) { throw new IDAL.DO.NonUniqueID("Staion's id"); }
            }

            if (id < 0) { throw new IDAL.DO.NegetiveValue("Station's id"); }

            if (charge_solts < 0) { throw new IDAL.DO.NegetiveValue("Charge's slots"); }

            DataSource.stations.Add(new IDAL.DO.Station(id, name, longitude, latitude, charge_solts));
        }

        /*
   	    *Description: add new Drone to drones. check if drone's list is not full before adding. if full print suitable message.
       	*Parameters: drone's details.	
       	*Return: true - if added successfully, false - else.
        */
        public void AddDrone(int id, string model, int maxWeight, int status = (int)IDAL.DO.DroneStatuses.Undefined, double battery = -1)
        {
            foreach (var drone in DataSource.drones)
            {
                if (drone.Id == id) { throw new IDAL.DO.NonUniqueID("Drone's id"); }
            }

            if (id < 0) { throw new IDAL.DO.NegetiveValue("Drone's id"); }

            if ((battery < 0 || battery > 100) && (battery != -1)) { throw new IDAL.DO.InvalidBattery(); }

            if (maxWeight < 0 || maxWeight > 2) { throw new IDAL.DO.WrongEnumValuesRange("maxWeight", "0", "2"); }

            if ((status < 0 || status > 2) && (status != (int)(IDAL.DO.DroneStatuses.Undefined))) { throw new IDAL.DO.WrongEnumValuesRange("status", "0", "2"); }

            DataSource.drones.Add(new IDAL.DO.Drone(id, model, (IDAL.DO.WeightCategories)maxWeight,
                (IDAL.DO.DroneStatuses)status, battery));
        }

        /*
        *Description: add new Costumer to costumers. check if costumer's list is not full before adding. if full print suitable message.
        *Parameters: costumer's details.
        *Return: true - if added successfully, false - else.
        */
        public void AddCostumer(int id, string name, string phone, double longitude, double latitude)
        {
            foreach (var costumer in DataSource.costumers)
            {
                if (costumer.Id == id) { throw new IDAL.DO.NonUniqueID("Costumer's id"); }
            }

            if (id < 0) { throw new IDAL.DO.NegetiveValue("Costumer's id"); }

            DataSource.costumers.Add(new IDAL.DO.Costumer(id, name, phone, longitude, latitude));
        }

        /*
        *Description: add new Paracel to paracels. check if paracel's list is not full before adding. if full print suitable message.
        *Parameters: paracel's detatils.
        *Return: true - if added successfully, false - else.
        */
        public void AddParcel(int id, int senderId, int targetId, int weight, int priority, DateTime requested,
            int droneId, DateTime scheduled, DateTime pickedUp, DateTime delivered)
        {
            foreach (var parcel in DataSource.parcels)
            {
                if (parcel.Id == id) { throw new IDAL.DO.NonUniqueID("Parcel's id"); }
            }

            if (targetId == senderId) { throw new IDAL.DO.SelfDelivery(); }

            if (0 > targetId || 0 > senderId) { throw new IDAL.DO.NegetiveValue("Id"); }

            if (id < 0) { throw new IDAL.DO.NegetiveValue("Parcel's id"); }

            try
            {
                _getCostumerById(senderId);
                _getCostumerById(targetId);
            }

            catch (Exception e) { throw new Exception(e.Message); }

            DataSource.parcels.Add(new IDAL.DO.Parcel(id, senderId, targetId, (IDAL.DO.WeightCategories)weight,
                (IDAL.DO.Priorities)priority, requested,
                droneId, scheduled, pickedUp, delivered));
        }

        /*
        *Description: find avaliable drone for deliverd a paracel. TODO: get paracel's id from the user, 
		*find avaliable drone and initalized paracel.DroneId = droneId.
        *Parameters: a paracel.
        *Return: true - if assign successfully, false - else.
        */
        public void AssignParcelToDrone(int id)
        {
            IDAL.DO.Parcel parcel;

            parcel = _getParcelById(id);

            foreach (var drone in DataSource.drones)
            {
                if (drone.Status == IDAL.DO.DroneStatuses.Available && // if the drone its avalible
                    (int)drone.MaxWeight >=
                    (int)parcel.Weight) // and the drone maxWeight is qual or bigger to the parcel weight
                {
                    parcel.Scheduled = DateTime.Now;
                    parcel.DroneId = drone.Id;
                    drone.Status = IDAL.DO.DroneStatuses.Shipping;
                    return;
                }
            }

            DataSource.waitingParcels.Enqueue(parcel); // if all the drones are not availible

            throw new IDAL.DO.NonAvilableDrones();
        }

        /*
        *Description: update PickedUp time to NOW.
        *Parameters: a paracel.
        *Return: true - if update successfully, false - else. In this part of the project this operation always will return true.
        */
        public void ParcelCollection(int id)
        {
            IDAL.DO.Parcel parcel;

            parcel = _getParcelById(id);

            parcel.PickedUp = DateTime.Now;
        }

        /*
        *Description: update delivered time to NOW.
        *Parameters: a paracel..
        *Return: true - if update successfully, false - else. In this part of the project this operation always will return true.
        */
        public void ParcelDelivered(int id)
        {
            IDAL.DO.Parcel parcel;
            IDAL.DO.Drone drone;

            parcel = _getParcelById(id);
            drone = _getDroneById(parcel.DroneId);

            drone.Status = IDAL.DO.DroneStatuses.Available;
            parcel.Delivered = DateTime.Now;

            // assign new parcel to the current drone
            if (DataSource.waitingParcels.Count != 0)
            {
                IDAL.DO.Parcel nextParcel = DataSource.waitingParcels.Dequeue();
                AssignParcelToDrone(nextParcel.Id);
            }
        }

        /*
        *Description: Send drone to charge's station. TODO: change drone's state, get a station from the user according to station list, 
        *Parameters: a drone, a station
        *Return: true - if send successfully, false - else.
        */
        public void SendDroneToCharge(int droneId, int stationId)
        {
            IDAL.DO.Drone drone;
            IDAL.DO.Station station;

            station = _getStationById(stationId);
            drone = _getDroneById(droneId);

            if (station.ChargeSolts <= 0) { throw new IDAL.DO.NegetiveValue("Charge's slots"); }

            station.ChargeSolts--;
            IDAL.DO.DroneCharge dc = new IDAL.DO.DroneCharge(drone.Id, station.Id);
            DataSource.droneCharge.Add(dc);
            drone.Status = IDAL.DO.DroneStatuses.Maintenance;
        }

        /*
        *Description: release drone from station. 
        *Parameters: a drone.
        *Return: true - if send successfully, false - else.
        */
        public void DroneRelease(int droneId)
        {
            IDAL.DO.DroneCharge dc;
            IDAL.DO.Drone drone;
            IDAL.DO.Station station;

            dc = _getDroneChargeByDroneId(droneId);
            drone = _getDroneById(droneId);
            station = _getStationById(dc.StationId);

            station.ChargeSolts++;
            DataSource.droneCharge.Remove(dc);
            drone.Status = IDAL.DO.DroneStatuses.Available;


            // assign new parcel to the current drone
            if (DataSource.waitingParcels.Count != 0)
            {
                IDAL.DO.Parcel nextParcel = DataSource.waitingParcels.Dequeue();
                AssignParcelToDrone(nextParcel.Id);
            }
        }

        public double[] PowerRequest() //TODO: Implement this method
        {
            double[] arr = new double[] { 0, 0, 0, 0, 0 };
            return arr;
        }

        public IDAL.DO.Parcel _getParcelById(int id)
        {
            foreach (var parcel in DataSource.parcels)
                if (parcel.Id == id) { return parcel; }
            throw new IDAL.DO.ItemNotFound("Parcel");
        }

        public IDAL.DO.Costumer _getCostumerById(int id)
        {
            foreach (var costumer in DataSource.costumers)
                if (costumer.Id == id) { return costumer; }
            throw new IDAL.DO.ItemNotFound("Costumer");
        }

        public IDAL.DO.Station _getStationById(int id)
        {
            foreach (var station in DataSource.stations)
                if (station.Id == id) { return station; }
            throw new IDAL.DO.ItemNotFound("Station");
        }

        public IDAL.DO.Drone _getDroneById(int id)
        {
            foreach (var drone in DataSource.drones)
                if (drone.Id == id) { return drone; }
            throw new IDAL.DO.ItemNotFound("Drone");
        }

        public IDAL.DO.DroneCharge _getDroneChargeByDroneId(int id)
        {
            foreach (var dc in DataSource.droneCharge)
                if (dc.DroneId == id) { return dc; }
            throw new IDAL.DO.ItemNotFound("Drone");
        }

        public IEnumerable<IDAL.DO.Station> _getStationList() { return DataSource.stations; }

        public IEnumerable<IDAL.DO.Costumer> _getCostumerList() { return DataSource.costumers; }

        public IEnumerable<IDAL.DO.Parcel> _getParceList() { return DataSource.parcels; }

        public IEnumerable<IDAL.DO.Drone> _getDroneList() { return DataSource.drones; }

        public Queue<IDAL.DO.Parcel> _getWaitingParcels() { return DataSource.waitingParcels; }
    }
}