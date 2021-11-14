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

            private IDAL.DO.Costumer _start;

            private IDAL.DO.Costumer _end;

            public TransferParcelBL(IDAL.DO.Parcel parcel)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton

                _parcel = parcel;

                _start = dalObj.GetCostumerById(parcel.SenderId);
                _end = dalObj.GetCostumerById(parcel.TargetId);


                _sender = new CostumerInParcel(_start);
                _receiver = new CostumerInParcel(_end);

                _isOnTheWay = (parcel.Status == IDAL.DO.ParcelStatuses.PickedUp);
            }

            public double Id => _parcel.Id;

            public bool IsOnTheWay => _isOnTheWay;

            public IDAL.DO.Priorities Priority => _parcel.Priority;

            public IDAL.DO.WeightCategories Weight => _parcel.Weight;

            public CostumerInParcel Sender => _sender;

            public CostumerInParcel Receiver => _receiver;

            public IDAL.DO.Location Start => _start.Location;
            public IDAL.DO.Location End => _end.Location;

            public double Distance => _start.Location.Distance(_end.Location);

            public override string ToString()
            {
                return $"Id: {Id}, Sender: {Sender}, Receiver: {Receiver}, Starting point: {Start}, Target point: {End} --> " + 
                       (IsOnTheWay ? "On the way.\n" : "Not collected yet.\n");
            }
        }
    }
}