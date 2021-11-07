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

        //getters
        public IDAL.DO.Parcel GetParcelById(int id);
        public IDAL.DO.Costumer GetCostumerById(int id);
        public IDAL.DO.Station GetStationById(int id);
        public IDAL.DO.Drone GetDroneById(int id);
        public IDAL.DO.DroneCharge GetDroneChargeByDroneId(int id);
        public IEnumerable<IDAL.DO.Station> GetStationsList();
        public IEnumerable<IDAL.DO.Costumer> GetCostumerList();
        public IEnumerable<IDAL.DO.Parcel> GetParcelsList();
        public IEnumerable<IDAL.DO.Drone> GetDroneList();
        public Queue<IDAL.DO.Parcel> GetWaitingParcels();
    }
}