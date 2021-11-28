using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class CostumerListBL
        {
            private IDAL.DO.Costumer _costumer;
            
            private int _sendDelivered;
            
            private int _sendNotDelivered;
            
            private int _recieved;
            
            private int _parcelsAwait;

            public CostumerListBL(IDAL.DO.Costumer costumer)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton
                _costumer = costumer;
                _sendDelivered = 0;
                _sendNotDelivered = 0;
                _recieved = 0;
                _parcelsAwait = 0;
                foreach (IDAL.DO.Parcel parcel in dalObj.GetParcelsList())
                {
                    if (parcel.SenderId == costumer.Id)
                    {
                        if (parcel.Delivered != null)
                            _sendDelivered++;
                        else
                            _sendNotDelivered++;
                    }

                    else if (parcel.TargetId == costumer.Id)
                    {
                        if (parcel.Delivered != null)
                            _recieved++;
                        else
                            _parcelsAwait++;
                    }
                }
            }

            public double Id => _costumer.Id;

            public string Name => _costumer.Name;

            public string Phone => _costumer.Phone;

            public int SendDelivered => _sendDelivered;

            public int SendNotDelivered => _sendNotDelivered;

            public int Recieve => _recieved;

            public int ParcelsAwait => _parcelsAwait;

            public override string ToString()
            {
                return $"Id: {Id}, Name: {Name}, Phone: {Phone}\n" +
                       $"Number of parcels send and delivered: {SendDelivered}.\n" +
                       $"Number of parcels sent but not yet delivered: {SendNotDelivered}.\n" +
                       $"Number of parcels he received: {Recieve}.\n" +
                       $"Number of parcels on the way to costumer: {ParcelsAwait}.\n";
            }
        }
    }
}