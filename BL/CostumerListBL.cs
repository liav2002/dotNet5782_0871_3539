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
            private int _sendArrived;
            private int _sendNotArrived;
            private int _recieved;
            private int _parcelsAwait;

            public CostumerListBL(IDAL.DO.Costumer costumer)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton
                _costumer = costumer;
                _sendArrived = 0;
                _sendNotArrived = 0;
                _recieved = 0;
                _parcelsAwait = 0;
                foreach (IDAL.DO.Parcel parcel in dalObj.GetParcelsList())
                {
                    if (parcel.SenderId == costumer.Id)
                    {
                        if (parcel.Delivered != default(DateTime))
                            _sendArrived++;
                        else
                            _sendNotArrived++;
                    }

                    if (parcel.TargetId == costumer.Id)
                    {
                        if (parcel.Delivered != default(DateTime))
                            _recieved++;
                        else
                            _parcelsAwait++;
                    }
                }
            }

            public double Id => _costumer.Id;

            public string Name => _costumer.Name;

            public string Phone => _costumer.Phone;

            public int SendArrived => _sendArrived;

            public int SendNotArrived => _sendNotArrived;

            public int Recieve => _recieved;

            public int ParcelsAwait => _parcelsAwait;
        }
    }
}