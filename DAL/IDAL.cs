using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
    public interface IDAL
    {
        //general methods
        public void AddStation(int id, string name, DO.Location location, int charge_solts);

        public void AddDrone(int id, string model, DO.WeightCategories maxWeight, double battery);

        public void AddCostumer(int id, string name, string phone, DO.Location location);

        public void AddParcel(int id, int senderId, int targetId, int weight, int priority, DateTime requested,
            int droneId, DateTime scheduled, DateTime pickedUp, DateTime delivered);

        public void MoveParcelToWaitingList(DO.Parcel parcel);
        public DO.Parcel GetNextParcel();
        public void AddDroneToCharge(int droneId, int stationId);
        public void DroneRelease(int droneId);
        public double[] PowerRequest();

        //getters
        public DO.Parcel GetParcelById(int id);
        public DO.Costumer GetCostumerById(int id);
        public DO.Station GetStationById(int id);
        public DO.Drone GetDroneById(int id);
        public DO.DroneCharge GetDroneChargeByDroneId(int id);
        public IEnumerable<DO.DroneCharge> GetDroneChargeList();
        public IEnumerable<DO.Station> GetStationsList();
        public IEnumerable<DO.Costumer> GetCostumerList();
        public IEnumerable<DO.Parcel> GetParcelsList();
        public IEnumerable<DO.Drone> GetDroneList();
        public Queue<DO.Parcel> GetWaitingParcels();
    }
}