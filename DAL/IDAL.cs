using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
    interface IDAL
    {
        //general methods
        public void AddStation(int id, string name, double longitude, double latitude, int charge_solts);
        public void AddDrone(int id, string model, int maxWeight, int status = (int)DO.DroneStatuses.Undefined, double battery = -1);
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
        public DO.Parcel _getParcelById(int id);
        public DO.Costumer _getCostumerById(int id);
        public DO.Station _getStationById(int id);
        public DO.Drone _getDroneById(int id);
        public DO.DroneCharge _getDroneChargeByDroneId(int id);
        public IEnumerable<DO.Station> _getStationList();
        public IEnumerable<DO.Costumer> _getCostumerList();
        public IEnumerable<DO.Parcel> _getParceList();
        IEnumerable<DO.Drone> _getDroneList();
        public Queue<DO.Parcel> _getWaitingParcels();
    }
}
