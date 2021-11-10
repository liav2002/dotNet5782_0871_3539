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

            public DroneBL(IDAL.DO.Parcel parcel)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton

                _parcel = new TransferParcelBL(parcel);
                _drone = dalObj.GetDroneById(parcel.DroneId);
            }

            public double Id => _drone.Id;

            public string Model => _drone.Model;

            public IDAL.DO.WeightCategories Whigt => _drone.MaxWeight;

            public double Battery => _drone.Battery;

            public IDAL.DO.DroneStatuses Status => _drone.Status;

            public TransferParcelBL Parcel => _parcel;

            public IDAL.DO.Location Location => _drone.Location;
        }
    }
}