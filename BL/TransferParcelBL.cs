using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class TransferParcelBL
        {
            private IDAL.DO.Parcel _parcel;

            private CostumerInParcel _sender;

            private CostumerInParcel _receiver;

            private bool _isOnTheWay;

            private IDAL.DO.Location _start;

            private IDAL.DO.Location _end;

            public TransferParcelBL(IDAL.DO.Parcel parcel)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton

                _parcel = parcel;

                _start = dalObj.GetCostumerById(parcel.SenderId).Location;
                _end = dalObj.GetCostumerById(parcel.TargetId).Location;

                _isOnTheWay = (parcel.Status == IDAL.DO.ParcelStatuses.PickedUp);
            }

            public double Id => _parcel.Id;

            public bool IsOnTheWay => _isOnTheWay;

            public IDAL.DO.Priorities Priority => _parcel.Priority;

            public IDAL.DO.WeightCategories Weight => _parcel.Weight;

            public CostumerInParcel Sender => _sender;

            public CostumerInParcel Receiver => _receiver;

            public IDAL.DO.Location Start => _start;

            public IDAL.DO.Location End => _end;

            public double Distance => _start.Distance(_end);
        }
    }
}