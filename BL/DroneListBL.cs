using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class DroneListBL
        {
            private IDAL.DO.Drone _drone;
            private int _parcelsDelivered;

            public DroneListBL(IDAL.DO.Drone drone)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton

                _drone = drone;
                
                foreach (var parcel in dalObj.GetParcelsList())
                    if ((parcel.DroneId == drone.Id) && (parcel.Status == IDAL.DO.ParcelStatuses.Delivered))
                        _parcelsDelivered++;
            }

            public double Id => _drone.Id;

            public string Model => _drone.Model;

            public IDAL.DO.WeightCategories Whigt => _drone.MaxWeight;

            public double Battery => _drone.Battery;

            public IDAL.DO.DroneStatuses Status => _drone.Status;

            public IDAL.DO.Location Location => _drone.Location;

            public int ParcelsDelivered => _parcelsDelivered;
        }
    }
}