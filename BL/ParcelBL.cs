using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class ParcelBL
        {
            private IDAL.DO.Parcel _parcel;
            private DroneParcelBL _drone;
            private CostumerInParcel _receiver;
            private CostumerInParcel _sender;

            public ParcelBL(IDAL.DO.Parcel parcel)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton

                _parcel = parcel;

                _receiver = new CostumerInParcel(dalObj.GetCostumerById(parcel.TargetId));

                _sender = new CostumerInParcel(dalObj.GetCostumerById(parcel.SenderId));

                _drone = new DroneParcelBL(dalObj.GetDroneById(parcel.DroneId));
            }

            public double Id => _parcel.Id;

            public CostumerInParcel Receiver => _receiver;

            public CostumerInParcel Sender => _sender;

            public IDAL.DO.WeightCategories Weight => _parcel.Weight;

            public IDAL.DO.Priorities Priority => _parcel.Priority;

            public DroneParcelBL Drone => _drone;

            public DateTime Delivered => _parcel.Delivered;

            public DateTime Requested => _parcel.Requested;

            public DateTime Scheduled => _parcel.Scheduled;

            public DateTime PickedUp => _parcel.PickedUp;
        }
    }
}