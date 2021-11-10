using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class ParcelListBL
        {
            private IDAL.DO.Parcel _parcel;
            
            private string _senderName;
            
            private string _recieverName;
            
            
            public ParcelListBL(IDAL.DO.Parcel parcel)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton
                _parcel = parcel;
                _senderName = dalObj.GetCostumerById(parcel.SenderId).Name;
                _recieverName = dalObj.GetCostumerById(parcel.TargetId).Name;


            }
            public double Id => _parcel.Id;

            public string ReceiverName => _recieverName;
            
            public string SenderName => _senderName;
            
            public IDAL.DO.WeightCategories Weight => _parcel.Weight;
            
            public IDAL.DO.ParcelStatuses Priority => _parcel.Status;
            
    
        }
    }
}