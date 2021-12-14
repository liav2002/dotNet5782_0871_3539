using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class DroneParcelBL
    {
        private DO.Drone _drone;

        public DroneParcelBL(DO.Drone drone)
        {
            _drone = drone;
        }

        public double Id => _drone.Id;

        public double Battery => _drone.Battery;

        public DO.Location Location => _drone.Location;
    }
}