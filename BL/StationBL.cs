using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class StationBL
        {
            private IDAL.DO.Station _station;

            private List<DroneChargeBL> _dronesInStation;

            public StationBL(IDAL.DO.Station station)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton

                _dronesInStation = new List<DroneChargeBL>();

                _station = station;

                foreach (var droneCharge in dalObj.GetDroneChargeList())
                    if (droneCharge.StationId == station.Id)
                        _dronesInStation.Add(new DroneChargeBL(dalObj.GetDroneById(droneCharge.DroneId)));
            }

            public int Id => _station.Id;

            public string Name => _station.Name;

            public IDAL.DO.Location Location => _station.Location;

            public int FreeSlots => _station.ChargeSlots;

            public List<DroneChargeBL> DronesInStation => _dronesInStation;
        }
    }
}