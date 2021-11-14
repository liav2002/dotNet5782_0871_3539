using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class DroneChargeBL
        {
            private IDAL.DO.Drone _drone;

            public DroneChargeBL(IDAL.DO.Drone drone)
            {
                _drone = drone;
            }

            public double Id => _drone.Id;

            public double Battery => _drone.Battery;

            public override string ToString()
            {
                return $"drone's id: {Id}, battery: {Battery}.";
            }
        }
    }
}