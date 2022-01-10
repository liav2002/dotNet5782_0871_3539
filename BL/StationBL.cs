using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;
using DO;

namespace BO
{
    public class StationBL
    {
        private DO.Station _station;

        private List<DroneChargeBL> _dronesInStation;

        public StationBL(DO.Station station)
        {
            DalApi.IDal idalObj = DalFactory.GetDal();

            _dronesInStation = new List<DroneChargeBL>();

            _station = station;

            foreach (var drone in idalObj.GetDroneList())
            {
                if (String.Format("{0:F3}", drone.Location) == String.Format("{0:F3}", this.Location))
                {
                    _dronesInStation.Add(new DroneChargeBL(drone, idalObj.IsDroneCharge(drone.Id)));
                }
            }
        }

        public void SetAvailability(bool availability)
        {
            this._station.IsAvailable = availability;
        }

        public int Id => _station.Id;

        public string Name => _station.Name;

        public DO.Location Location => _station.Location;

        public int FreeSlots => _station.ChargeSlots;

        public List<DroneChargeBL> DronesInStation => _dronesInStation;

        public bool IsAvailable => _station.IsAvailable;
        public override string ToString()
        {
            string strToPrint = string.Format("the id is: {0}\nthe name is: {1}\nthe location is: {2}\n" +
                                              "\nthe number of available argument positions: {3}\n" +
                                              "the drones in the station:\n"
                , Id, Name, Location, FreeSlots);

            strToPrint += string.Join("\n", DronesInStation);
            strToPrint += "\n";

            return strToPrint;
        }
    }
}