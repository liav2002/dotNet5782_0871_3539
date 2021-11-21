using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class DroneBL
        {
            private IDAL.DO.Drone _drone;
            private TransferParcelBL _parcel;

            public DroneBL(int droneId, int parcelId)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton

                _parcel = (parcelId != 0) ? new TransferParcelBL(dalObj.GetParcelById(parcelId)) : null;
                _drone = dalObj.GetDroneById(droneId);
            }

            public double Id => _drone.Id;

            public string Model => _drone.Model;

            public IDAL.DO.WeightCategories Weight => _drone.MaxWeight;

            public double Battery => _drone.Battery;

            public IDAL.DO.DroneStatuses Status => _drone.Status;

            public TransferParcelBL Parcel => _parcel;

            public IDAL.DO.Location Location => _drone.Location;

            public override string ToString()
            {
                return string.Format("the id is: {0}\nthe model is: {1}\nthe maxWegiht is: {2}\n" +
                                     "the status is: {3}\nthe battery is: {4}\nthe transfer parcel is: " +
                                     (Parcel == null ? "None" : Parcel) + "\n" +
                                     "the location is: {5}\n"
                    , Id, Model, Weight, Status, String.Format("{0:F3}", Battery), Location);
            }
        }
    }
}