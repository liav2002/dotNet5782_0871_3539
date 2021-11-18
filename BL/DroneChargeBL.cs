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
            private bool _isCharge;

            public DroneChargeBL(IDAL.DO.Drone drone, bool isCharge = true)
            {
                _drone = drone;
                _isCharge = isCharge;
            }

            public double Id => _drone.Id;

            public double Battery => _drone.Battery;

            public bool IsCharge => this._isCharge;

            public override string ToString()
            {
                return $"drone's id: {Id}, battery: {Battery} --> " + (this._isCharge ? "The drone is charging.\n" : "The drone is not charging.\n");
            }
        }
    }
}