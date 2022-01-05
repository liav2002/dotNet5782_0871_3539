using System;
using System.Collections.Generic;
using System.Text;

namespace DalApi
{
    public interface IDal
    {
        //general methods
        public void AddStation(int id, string name, DO.Location location, int chargeSlots);
        public void AddDrone(int id, string model, DO.WeightCategories maxWeight, double battery);
        public void AddCostumer(int id, string name, string phone, DO.Location location, string email, string password);

        public void AddParcel(int id, int senderId, int targetId, int weight, int priority, DateTime? requested,
            int droneId, DateTime? scheduled, DateTime? pickedUp, DateTime? delivered);

        public void AddDroneToCharge(int droneId, int stationId);
        public void DroneRelease(int droneId, double hours);
        public bool IsDroneCharge(int droneId);
        public void SignIn(int costumerId);
        public void SignOut();

        //getters
        public DO.Costumer GetLoggedUser();
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

        // updates:

        public void UpdateDrone(DO.Drone drone);
        public void UpdateCostumer(DO.Costumer costumer);
        public void UpdateStation(DO.Station station);
        public void UpdateParcel(DO.Parcel parcel);
    }
}