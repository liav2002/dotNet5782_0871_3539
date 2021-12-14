using System;
using System.Collections.Generic;
using System.Text;

namespace DalApi
{
    public interface IDAL
    {
        //general methods
        public void AddStation(int id, string name, DO.Location location, int charge_solts);

        public void AddDrone(int id, string model, DO.WeightCategories maxWeight, double battery);

        public void AddCostumer(int id, string name, string phone, DO.Location location);

        public void AddParcel(int id, int senderId, int targetId, int weight, int priority, DateTime? requested,
            int droneId, DateTime? scheduled, DateTime? pickedUp, DateTime? delivered);

        public void AddDroneToCharge(int droneId, int stationId);
        public void DroneRelease(int droneId, double hours);
        public double[] PowerRequest();

        public bool IsDroneCharge(int droneId);
        //getters
        public DO.Parcel GetParcelById(int id);
        public DO.Costumer GetCostumerById(int id);
        public DO.Station GetStationById(int id);
        public DO.Drone GetDroneById(int id);
        public DO.Parcel GetParcelByDroneId(int droneId);
        public DO.DroneCharge GetDroneChargeByDroneId(int id);
        public IEnumerable<DO.DroneCharge> GetDroneChargeList(Func<DO.DroneCharge, bool> filter = null);
        public IEnumerable<DO.Station> GetStationsList(Func<DO.Station, bool> filter = null);
        public IEnumerable<DO.Costumer> GetCostumerList(Func<DO.Costumer, bool> filter = null);
        public IEnumerable<DO.Parcel> GetParcelsList(Func<DO.Parcel, bool> filter = null);
        public IEnumerable<DO.Drone> GetDroneList(Func<DO.Drone, bool> filter = null);
    }
}