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
        public void AssignParcelToDrone(int droneId);
        public void ParcelCollection(int droneId);
        public void ParcelDelivered(int droneId);
        public void SendDroneToCharge(int droneId, int stationId = -1);
        public void DroneRelease(int droneId, double hours);
        public void UpdateDroneName(int droneId, string name);
        public void UpdateStation(int stationId, string name, int chargeSlots);
        public void UpdateCostumer(int costumerId, string name, string phoneNumber);
        public List<IDAL.DO.Parcel> GetWaitingParcels();

        //getters
        public BO.ParcelBL GetParcelById(int parcelId);
        public BO.CostumerBL GetCostumerById(int id);
        public BO.StationBL GetStationById(int id);
        public BO.DroneBL GetDroneById(int droneId);
        public BO.DroneChargeBL GetDroneChargeByDroneId(int id);
        public IEnumerable<BO.StationListBL> GetStationsList();
        public IEnumerable<BO.CostumerListBL> GetCostumerList();
        public IEnumerable<BO.ParcelListBL> GetParcelsList();
        public IEnumerable<BO.DroneListBL> GetDroneList();
        public SysLog.SysLog Sys();
    }
}