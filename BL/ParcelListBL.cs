using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;
using DO;

namespace BO
{
    public class ParcelListBL
    {
        private DO.Parcel _parcel;

        private string _senderName;

        private string _recieverName;


        public ParcelListBL(DO.Parcel parcel)
        {
            DalApi.IDal idalObj = DalApi.DalFactory.GetDal();

            _parcel = parcel;
            _senderName = idalObj.GetCostumerById(parcel.SenderId).Name;
            _recieverName = idalObj.GetCostumerById(parcel.TargetId).Name;
        }

        public double Id => _parcel.Id;

        public string ReceiverName => _recieverName;

        public string SenderName => _senderName;

        public DO.WeightCategories Weight => _parcel.Weight;

        public DO.Priorities Priority => _parcel.Priority;

        public DO.ParcelStatuses Status => _parcel.Status;

        public override string ToString()
        {
            return
                $"Id: {Id}, Receiver name: {ReceiverName}, Sender name: {SenderName}, Weight: {Weight}, Priority: {Priority}, Status: {Status}\n";
        }
    }
}