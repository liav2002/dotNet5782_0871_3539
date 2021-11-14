using System;
using System.Collections.Generic;
using System.Text;

namespace IBL
{
    public interface IBL
    {
        public void AddStation(int id, string name, double longitude, double latitude, int charge_solts);
        public void AddDrone(int id, string model, int maxWeight, int stationId);
        public void AddCostumer(int id, string name, string phone, double longitude, double latitude);
        public void AddParcel(int senderId, int targetId, int weight, int priority, int droneId);
        public void AssignParcelToDrone(int parcelId);
        public void ParcelCollection(int parcelId);
        public void ParcelDelivered(int parcelId);
        public void SendDroneToCharge(int droneId, int stationId);
        public void DroneRelease(int droneId);
        public void UpdateDroneName(int droneId, string name);
        public void UpdateStation(int stationId, string name, int chargeSlots);
        public void UpdateCostumer(int costumerId, string name, string phoneNumber);

        //getters
        public BO.ParcelBL GetParcelById(int id);
        public BO.CostumerBL GetCostumerById(int id);
        public BO.StationBL GetStationById(int id);
        public BO.DroneBL GetDroneById(int id);
        public BO.DroneChargeBL GetDroneChargeByDroneId(int id);
        public IEnumerable<BO.StationListBL> GetStationsList();
        public IEnumerable<BO.CostumerListBL> GetCostumerList();
        public IEnumerable<BO.ParcelListBL> GetParcelsList();
        public IEnumerable<BO.DroneListBL> GetDroneList();
        public Queue<IDAL.DO.Parcel> GetWaitingParcels();
        public BO.SysLog Sys();
    }
}