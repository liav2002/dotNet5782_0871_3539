using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;
using DO;


namespace BO
{
    public class CostumerBL
    {
        private DO.Costumer _costumer;
        private List<ParcelAtCostumer> _parcelsSender;
        private List<ParcelAtCostumer> _parcelsReciever;
        private bool _isAvaliable;

        public CostumerBL(DO.Costumer costumer)
        {
            _parcelsSender = new List<ParcelAtCostumer>();

            _parcelsReciever = new List<ParcelAtCostumer>();

            _costumer = costumer;

            _isAvaliable = true;

            DalApi.IDal idalObj = DalFactory.GetDal();
            foreach (DO.Parcel parcel in idalObj.GetParcelsList())
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

        public DO.Location Location => _costumer.Location;

        public List<ParcelAtCostumer> ParcelsSender => _parcelsSender;

        public List<ParcelAtCostumer> ParcelsReciever => _parcelsReciever;

        public bool IsAvliable
        {
            get => _isAvaliable;
            set => _isAvaliable = value;
        }

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