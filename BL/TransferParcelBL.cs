using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;


namespace BO
{
    public class TransferParcelBL
    {
        private DO.Parcel _parcel;

        private CostumerInParcel _sender;

        private CostumerInParcel _receiver;

        private bool _isOnTheWay;
        private bool _isDelivered;

        private DO.Costumer _start;

        private DO.Costumer _end;

        public TransferParcelBL(DO.Parcel parcel)
        {
            DalApi.IDal idalObj = DalFactory.GetDal();

            _parcel = parcel;

            _start = idalObj.GetCostumerById(parcel.SenderId);
            _end = idalObj.GetCostumerById(parcel.TargetId);


            _sender = new CostumerInParcel(_start);
            _receiver = new CostumerInParcel(_end);

            _isOnTheWay = (parcel.Status == DO.ParcelStatuses.PickedUp);
            _isDelivered = (parcel.Status == DO.ParcelStatuses.Delivered);
        }

        public double Id => _parcel.Id;

        public bool IsOnTheWay => _isOnTheWay;
        public bool IsDelivered => _isDelivered;

        public DO.Priorities Priority => _parcel.Priority;

        public DO.WeightCategories Weight => _parcel.Weight;

        public CostumerInParcel Sender => _sender;

        public CostumerInParcel Receiver => _receiver;

        public DO.Location Start => _start.Location;
        public DO.Location End => _end.Location;

        public double Distance => _start.Location.Distance(_end.Location);

        public override string ToString()
        {
            return
                $"Id: {Id}, Sender: {Sender}, \n\t\tReceiver: {Receiver}, \n\t\tStarting point: {Start}, \n\t\tTarget point: {End} \n\t\t--> " +
                (IsOnTheWay ? "On the way.\n" : "Not collected yet.\n");
        }
    }
}