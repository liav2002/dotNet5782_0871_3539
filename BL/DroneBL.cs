using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;

namespace BO
{
    public class DroneBL
    {
        private DO.Drone _drone;
        private TransferParcelBL _parcel;
        public DroneBL(int droneId, int parcelId)
        {
            DalApi.IDal idalObj = DalFactory.GetDal();

            _parcel = (parcelId != 0) ? new TransferParcelBL(idalObj.GetParcelById(parcelId)) : null;
            if (_parcel != null && _parcel.IsDelivered)
            {
                _parcel = null;
            }

            _drone = idalObj.GetDroneById(droneId);
        }

        public void SetAsUnAvailable()
        {
            this._drone.IsAvaliable = false;
        }

        public int Id => _drone.Id;

        public string Model => _drone.Model;

        public DO.WeightCategories Weight => _drone.MaxWeight;

        public double Battery => _drone.Battery;

        public DO.DroneStatuses Status => _drone.Status;

        public TransferParcelBL Parcel => _parcel;

        public DO.Location Location => _drone.Location;

        public bool IsAvliable => _drone.IsAvaliable;

        public override string ToString()
        {
            return string.Format("the id is: {0}\nthe model is: {1}\nthe maxWegiht is: {2}\n" +
                                 "the status is: {3}\nthe battery is: {4}\n\nthe transfer parcel is: " +
                                 (Parcel == null ? "None" : Parcel) + "\n" +
                                 "the location is: {5}\n"
                , Id, Model, Weight, Status, String.Format("{0:F3}", Battery), Location);
        }
    }
}