using System;
using System.Collections.Generic;
using System.Text;


namespace BlApi
{
    public interface IBL
    {
        public void AddStation(int id, string name, double longitude, double latitude, int charge_solts);
        public void RemoveStation(int stationId);
        public void RestoreStation(int stationId);
        public void AddDrone(int id, string model, int maxWeight, int stationId);
        public void RemoveDrone(int droneId);
        public void RestoreDrone(int droneId);
        public void AddCostumer(int id, string name, string phone, double longitude, double latitude, string email, string password);
        public void RemoveCostumer(int costumerId);
        public void RestoreCostumer(int costumerId);
        public void SetAsManager(int costumerId);
        public void NonManagement(int costumerId);
        public void AddParcel(int senderId, int targetId, int weight, int priority, int droneId);
        public void RemoveParcel(int parcelId);
        public void RestoreParcel(int parcelId);
        public void AssignParcelToDrone(int droneId);
        public void ParcelCollection(int droneId);
        public void ParcelDelivered(int droneId);
        public void SendDroneToCharge(int droneId, int stationId = -1);
        public void DroneCharging(int droneId);
        public void DroneRelease(int droneId);
        public void UpdateDroneName(int droneId, string name);
        public void UpdateStation(int stationId, string name, int chargeSlots);
        public void UpdateCostumer(int costumerId, string name, string phoneNumber, string email, string password);
        public void SignIn(string username, string password);
        public void SignOut();

        public void StartSimulator(BO.DroneBL drone);
        public void SetDroneStartTimeOfCharge(int droneId);
        public List<DO.Parcel> GetWaitingParcels();

        //getters
        public BO.CostumerBL GetLoggedUser();
        public BO.ParcelBL GetParcelById(int parcelId);
        public BO.CostumerBL GetCostumerById(int id);
        public BO.StationBL GetStationById(int id);
        public BO.DroneBL GetDroneById(int droneId);
        public BO.DroneChargeBL GetDroneChargeByDroneId(int id);
        public IEnumerable<BO.StationListBL> GetStationsList(Func<DO.Station, bool> filter = null);
        public IEnumerable<BO.CostumerListBL> GetCostumerList(Func<DO.Costumer, bool> filter = null);
        public IEnumerable<BO.ParcelListBL> GetParcelsList(Func<DO.Parcel, bool> filter = null);
        public IEnumerable<BO.DroneListBL> GetDroneList(Func<DO.Drone, bool> filter = null);
        public SysLog.SysLog Sys();
    }
}