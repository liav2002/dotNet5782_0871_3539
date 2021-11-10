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

            private int _takingSlots;

            public StationBL(IDAL.DO.Station station)
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton

                _station = station;

                foreach (var droneCharge in dalObj.GetDroneChargeList())
                    if (droneCharge.StationId == station.Id)
                        _takingSlots++;
            }

            public int Id => _station.Id;

            public string Name => _station.Name;

            public int FreeSlots => _station.ChargeSlots;

            public int TakingSlots => _takingSlots;
        }
    }
}