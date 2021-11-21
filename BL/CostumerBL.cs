using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL.DO;

namespace IBL
{
    namespace BO
    {
        public class CostumerBL
        {
            private IDAL.DO.Costumer _costumer;
            private List<ParcelAtCostumer> _parcelsSender;
            private List<ParcelAtCostumer> _parcelsReciever;

            public CostumerBL(IDAL.DO.Costumer costumer)
            {
                _parcelsSender = new List<ParcelAtCostumer>();

                _parcelsReciever = new List<ParcelAtCostumer>();

                _costumer = costumer;

                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton
                foreach (IDAL.DO.Parcel parcel in dalObj.GetParcelsList())
                {
                    if (parcel.SenderId == _costumer.Id)
                        _parcelsSender.Add(new ParcelAtCostumer(parcel, costumer));
                    if (parcel.TargetId == _costumer.Id)
                        _parcelsReciever.Add(new ParcelAtCostumer(parcel, costumer));
                }
            }

            public double Id => _costumer.Id;

            public string Name => _costumer.Name;

            public string Phone => _costumer.Phone;

            public Location Location => _costumer.Location;

            public List<ParcelAtCostumer> ParcelsSender => _parcelsSender;

            public List<ParcelAtCostumer> ParcelsReciever => _parcelsReciever;

            public override string ToString()
            {
                string strToPrint = string.Format("the id is: {0}\nthe name is: {1}\nthe phone is: {2}\n" +
                                                  "the location is: {3}\n", Id, Name, Phone, Location);

                strToPrint += "Shipped Parcels:\n";
                strToPrint += string.Join("\n", ParcelsSender);
                strToPrint += "\n";

                strToPrint += "Incoming Parcels:\n";
                strToPrint += string.Join("\n", ParcelsReciever);
                strToPrint += "\n";

                return strToPrint;
            }
        }
    }
}