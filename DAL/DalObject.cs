﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DalObject
{
    public class DalObject
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
        public bool AddStation(int id, string name, double longitude, double latitude, int charge_solts)
        {
            IDAL.DO.Station station = new IDAL.DO.Station(id, name, longitude, latitude, charge_solts);

            if (DataSource.Config.indexStation >= DataSource.stations.Length) return false;

            for (int i = 0; i < DataSource.Config.indexStation; ++i)
            {
                if (DataSource.stations[i].Id == id)
                {
                    return false;
                }
            }

            if (id < 0 || charge_solts < 0)
            {
                return false;
            }

            DataSource.stations[DataSource.Config.indexStation++] = station;

            return true;
        }

        /*
   	    *Description: add new Drone to drones. check if drone's list is not full before adding. if full print suitable message.
       	*Parameters: drone's details.	
       	*Return: true - if added successfully, false - else.
        */
        public bool AddDrone(int id, string model, int maxWeight, int status, double battery)
        {
            IDAL.DO.Drone drone = new IDAL.DO.Drone(id, model, (IDAL.DO.WeightCategories)maxWeight,
                (IDAL.DO.DroneStatuses)status, battery);

            if (DataSource.Config.indexDrone >= DataSource.drones.Length) return false;

            for (int i = 0; i < DataSource.Config.indexDrone; ++i)
            {
                if (DataSource.drones[i].Id == id)
                {
                    return false;
                }
            }

            if (id < 0 || battery < 0 || battery > 100) return false;

            if (maxWeight < 0 || maxWeight > 2) return false;

            if (status < 0 || status > 2) return false;

            DataSource.drones[DataSource.Config.indexDrone++] = drone;
            return true;
        }

        /*
        *Description: add new Costumer to costumers. check if costumer's list is not full before adding. if full print suitable message.
        *Parameters: costumer's details.
        *Return: true - if added successfully, false - else.
        */
        public bool AddCostumer(int id, string name, string phone, double longitude, double latitude)
        {
            IDAL.DO.Costumer costumer = new IDAL.DO.Costumer(id, name, phone, longitude, latitude);

            if (DataSource.Config.indexCostumer >= DataSource.costumers.Length) return false;

            for (int i = 0; i < DataSource.Config.indexCostumer; ++i)
            {
                if (DataSource.costumers[i].Id == id)
                {
                    return false;
                }
            }

            if (id < 0) return false;

            DataSource.costumers[DataSource.Config.indexCostumer++] = costumer;

            return true;
        }

        /*
        *Description: add new Paracel to paracels. check if paracel's list is not full before adding. if full print suitable message.
        *Parameters: paracel's detatils.
        *Return: true - if added successfully, false - else.
        */
        public bool AddParcel(int id, int senderId, int targetId, int weight, int priority, DateTime requested,
            int droneId, DateTime scheduled, DateTime pickedUp, DateTime delivered)
        {
            IDAL.DO.Parcel parcel = new IDAL.DO.Parcel(id, senderId, targetId, (IDAL.DO.WeightCategories)weight,
                (IDAL.DO.Priorities)priority, requested,
                droneId, scheduled, pickedUp, delivered);

            if (DataSource.Config.indexParcel >= DataSource.parcels.Length) return false;
            if (targetId == senderId) return false;
            if (0 > targetId || 0 > senderId) return false;

            try
            {
                _getCostumerById(senderId);
                _getCostumerById(targetId);
            }
            catch (Exception e)
            {
                return false;
            }

            for (int i = 0; i < DataSource.Config.indexParcel; ++i)
            {
                if (DataSource.parcels[i].Id == id)
                {
                    return false;
                }
            }

            if (id < 0) return false;

            DataSource.parcels[DataSource.Config.indexParcel++] = parcel;

            return true;
        }

        /*
        *Description: find avaliable drone for deliverd a paracel. TODO: get paracel's id from the user, 
		*find avaliable drone and initalized paracel.DroneId = droneId.
        *Parameters: a paracel.
        *Return: true - if assign successfully, false - else.
        */
        public bool AssignParcelToDrone(int id)
        {
            IDAL.DO.Parcel parcel;
            try
            {
                parcel = _getParcelById(id);
            }
            catch (ArgumentException e)
            {
                return false;
            }

            for (int i = 0; i < DataSource.Config.indexDrone; i++)
            {
                if (DataSource.drones[i].Status == IDAL.DO.DroneStatuses.Available && // if the drone its avalible
                    (int)DataSource.drones[i].MaxWeight >=
                    (int)parcel.Weight) // and the drone maxWeight is qual or bigger to the parcel weight
                {
                    parcel.Scheduled = DateTime.Now;
                    parcel.DroneId = DataSource.drones[i].Id;
                    DataSource.drones[i].Status = IDAL.DO.DroneStatuses.Shipping;
                    return true;
                }
            }

            DataSource.waitingParcels.Enqueue(parcel); // if all the drones are not availible

            return false;
        }

        /*
        *Description: update PickedUp time to NOW.
        *Parameters: a paracel.
        *Return: true - if update successfully, false - else. In this part of the project this operation always will return true.
        */
        public bool ParcelCollection(int id)
        {
            IDAL.DO.Parcel parcel;
            try
            {
                parcel = _getParcelById(id);
            }
            catch (ArgumentException e)
            {
                return false;
            }

            parcel.PickedUp = DateTime.Now;
            return true;
        }

        /*
        *Description: update delivered time to NOW.
        *Parameters: a paracel..
        *Return: true - if update successfully, false - else. In this part of the project this operation always will return true.
        */
        public bool ParcelDelivered(int id)
        {
            IDAL.DO.Parcel parcel;
            IDAL.DO.Drone drone;
            try
            {
                parcel = _getParcelById(id);
                drone = _getDroneById(parcel.DroneId);
            }
            catch (ArgumentException e)
            {
                return false;
            }

            drone.Status = IDAL.DO.DroneStatuses.Available;
            parcel.Delivered = DateTime.Now;

            // assign new parcel to the current drone
            if (DataSource.waitingParcels.Count != 0)
            {
                IDAL.DO.Parcel nextParcel = DataSource.waitingParcels.Dequeue();
                AssignParcelToDrone(nextParcel.Id);
            }

            return true;
        }

        /*
        *Description: Send drone to charge's station. TODO: change drone's state, get a station from the user according to station list, 
        *Parameters: a drone, a station
        *Return: true - if send successfully, false - else.
        */
        public bool SendDroneToCharge(int droneId, int stationId)
        {
            IDAL.DO.Drone drone;
            IDAL.DO.Station station;
            try
            {
                station = _getStationById(stationId);
                drone = _getDroneById(droneId);
            }
            catch (ArgumentException e)
            {
                return false;
            }

            if (station.ChargeSolts <= 0) return false;

            station.ChargeSolts--;
            IDAL.DO.DroneCharge dc = new IDAL.DO.DroneCharge(drone.Id, station.Id);
            DataSource.droneCharge.Add(dc);
            drone.Status = IDAL.DO.DroneStatuses.Maintenance;

            return true;
        }

        /*
        *Description: release drone from station. 
        *Parameters: a drone.
        *Return: true - if send successfully, false - else.
        */
        public bool DroneRelease(int droneId)
        {
            IDAL.DO.DroneCharge dc;
            IDAL.DO.Drone drone;
            IDAL.DO.Station station;
            try
            {
                dc = _getDroneChargeByDroneId(droneId);
                drone = _getDroneById(droneId);
                station = _getStationById(dc.StationId);
            }
            catch (ArgumentException e)
            {
                return false;
            }

            station.ChargeSolts++;
            DataSource.droneCharge.Remove(dc);
            drone.Status = IDAL.DO.DroneStatuses.Available;


            // assign new parcel to the current drone
            if (DataSource.waitingParcels.Count != 0)
            {
                IDAL.DO.Parcel nextParcel = DataSource.waitingParcels.Dequeue();
                AssignParcelToDrone(nextParcel.Id);
            }

            return true;
        }


        public IDAL.DO.Parcel _getParcelById(int id)
        {
            for (int i = 0; i < DataSource.Config.indexParcel; i++)
                if (DataSource.parcels[i].Id == id)
                    return DataSource.parcels[i];
            throw new ArgumentException("Id not found");
        }

        public IDAL.DO.Costumer _getCostumerById(int id)
        {
            for (int i = 0; i < DataSource.Config.indexCostumer; i++)
                if (DataSource.costumers[i].Id == id)
                    return DataSource.costumers[i];
            throw new ArgumentException("Id not found");
        }

        public IDAL.DO.Station _getStationById(int id)
        {
            for (int i = 0; i < DataSource.Config.indexStation; i++)
                if (DataSource.stations[i].Id == id)
                    return DataSource.stations[i];
            throw new ArgumentException("Id not found");
        }

        public IDAL.DO.Drone _getDroneById(int id)
        {
            for (int i = 0; i < DataSource.Config.indexDrone; i++)
                if (DataSource.drones[i].Id == id)
                    return DataSource.drones[i];
            throw new ArgumentException("Id not found");
        }

        public IDAL.DO.DroneCharge _getDroneChargeByDroneId(int id)
        {
            foreach (IDAL.DO.DroneCharge dc in DataSource.droneCharge)
            {
                if (dc.DroneId == id)
                    return dc;
            }

            throw new ArgumentException("Id not found");
        }

        public IDAL.DO.Station _getStationByIndex(int i)
        {
            if (i < 0 || i >= DataSource.Config.indexStation) { throw new ArgumentException("Wrong index."); } 
            return DataSource.stations[i];
        }

        public IDAL.DO.Costumer _getCostumerByIndex(int i)
        {
            if (i < 0 || i >= DataSource.Config.indexCostumer) { throw new ArgumentException("Wrong index."); }
            return DataSource.costumers[i];
        }

        public IDAL.DO.Parcel _getParcelByIndex(int i)
        {
            if (i < 0 || i >= DataSource.Config.indexParcel) { throw new ArgumentException("Wrong index."); }
            return DataSource.parcels[i];
        }

        public IDAL.DO.Drone _getDroneByIndex(int i)
        {
            if (i < 0 || i >= DataSource.Config.indexDrone) { throw new ArgumentException("Wrong index."); }
            return DataSource.drones[i];
        }

        public int _getIndexStaion() { return DataSource.Config.indexStation; }

        public int _getIndexCostumer() { return DataSource.Config.indexCostumer; }

        public int _getIndexParcel() { return DataSource.Config.indexParcel; }

        public int _getIndexDrone() { return DataSource.Config.indexDrone; }

        public Queue<IDAL.DO.Parcel> _getWaitingParcels(){ return DataSource.waitingParcels; }
    }
}