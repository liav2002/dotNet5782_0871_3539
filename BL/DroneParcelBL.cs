using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class DroneParcelBL
        {
            private IDAL.DO.Drone _drone;

            public DroneParcelBL(IDAL.DO.Drone drone)
            {
                _drone = drone;
            }

            public double Id => _drone.Id;

            public double Battery => _drone.Battery;

            public IDAL.DO.Location Location => _drone.Location;
        }
    }
}