using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class DroneListBL
        {
            private IDAL.DO.Drone _drone;
            private int _parcelId;

            public DroneListBL(IDAL.DO.Drone drone)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton

                _drone = drone;

                _parcelId = -1;

                foreach (var parcel in dalObj.GetParcelsList())
                    if (parcel.DroneId == drone.Id) { _parcelId = parcel.Id; }
            }

            public int Id => _drone.Id;

            public string Model => _drone.Model;

            public IDAL.DO.WeightCategories Weight => _drone.MaxWeight;

            public double Battery => _drone.Battery;

            public IDAL.DO.DroneStatuses Status => _drone.Status;

            public IDAL.DO.Location Location => _drone.Location;

            public int ParcelId => _parcelId;

            public override string ToString()
            {
                string strToPrint = $"Id: {Id}, Model: {Model}.\n" +
                                    $"Weight: {Weight}, Battery: {Battery}, Status: {Status}, Location: {Location}\n";

                if(ParcelId != -1)
                {
                    strToPrint += $"Parcel's id: {ParcelId}.\n";
                }

                else
                {
                    strToPrint += "The drone doesn't carry any parcel.\n";
                }

                return strToPrint;
            }
        }
    }
}