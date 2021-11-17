using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
    namespace DO
    {
        public class DroneCharge
        {
            private int _droneId;
            private int _stationId;

            public DroneCharge(int droneId, int stationId)
            {
                SysLog.GetInstance().DroneToCharge(droneId, stationId);
                this._droneId = droneId;
                this._stationId = stationId;
            }

            public int DroneId
            {
                get => _droneId;
                set => _droneId = value;
            }

            public int StationId
            {
                get => _stationId;
                set => _stationId = value;
            }

            public override string ToString()
            {
                return string.Format("the droneID is: {0}\nthe stationID is: {1}\n"
                    , DroneId, StationId);
            }
        }
    }
}