using System;
using System.Collections.Generic;
using System.Text;


namespace DO
{
    public class DroneCharge
    {
        private int _droneId;
        private int _stationId;
        DateTime _startTime;

        public DroneCharge(int droneId, int stationId)
        {
            SysLog.SysLog.GetInstance().DroneToCharge(droneId, stationId);
            this._droneId = droneId;
            this._stationId = stationId;

            this._startTime = DateTime.Now;
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

        public DateTime StartTime
        {
            get => _startTime;
        }

        public override string ToString()
        {
            return string.Format("the droneID is: {0}\nthe stationID is: {1}\n"
                , DroneId, StationId);
        }
    }
}