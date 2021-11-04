using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
    public interface IDAL
    {
        //general methods
        public void AddStation(int id, string name, double longitude, double latitude, int charge_solts);
        public void AddDrone(int id, string model, int maxWeight, int status = (int)DO.DroneStatuses.Available, double battery = -1);
        public void AddCostumer(int id, string name, string phone, double longitude, double latitude);
        public void AddParcel(int id, int senderId, int targetId, int weight, int priority, DateTime requested,
            int droneId, DateTime scheduled, DateTime pickedUp, DateTime delivered);
        public void AssignParcelToDrone(int id);
        public void ParcelCollection(int id);
        public void ParcelDelivered(int id);
        public void SendDroneToCharge(int droneId, int stationId);
        public void DroneRelease(int droneId);
        public double[] PowerRequest();

        //getters
        public DO.Parcel GetParcelById(int id);
        public DO.Costumer GetCostumerById(int id);
        public DO.Station GetStationById(int id);
        public DO.Drone GetDroneById(int id);
        public DO.DroneCharge GetDroneChargeByDroneId(int id);
        public IEnumerable<DO.Station> GetStationsList();
        public IEnumerable<DO.Costumer> GetCostumerList();
        public IEnumerable<DO.Parcel> GetParcelsList();
        public IEnumerable<DO.Drone> GetDroneList();
        public Queue<DO.Parcel> GetWaitingParcels();
    }
}
