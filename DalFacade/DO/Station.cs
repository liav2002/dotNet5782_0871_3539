using System;
using System.Collections.Generic;
using System.Text;

namespace DO
{
    public class Station
    {
        private int _id;
        private string _name;
        private Location _location;
        private int _chargeSlots;
        private bool _isAvailable;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                SysLog.SysLog.GetInstance().ChangeStationName(_id, value);
            }
        }

        public Location Location
        {
            get => _location;
            set => _location = value;
        }


        public int ChargeSlots
        {
            get => _chargeSlots;
            set
            {
                _chargeSlots = value;
                SysLog.SysLog.GetInstance().ChangeStationChargeSlots(_id, value);
            }
        }

        public bool IsAvailable
        {
            get => this._isAvailable;
            set => this._isAvailable = value;
        }

        public override string ToString()
        {
            return string.Format("the id is: {0}\nthe name is: {1}\nthe location is: {2}\n" +
                                 "\nthe number of available argument positions: {3}\n"
                , Id, Name, Location, ChargeSlots);
        }

        public Station(int id, string name, Location location, int chargeSlots)
        {
            SysLog.SysLog.GetInstance().AddStation(id);
            this._id = id;
            this._name = name;
            this._location = location;
            this._chargeSlots = chargeSlots;
            this._isAvailable = true;
        }

        public Station()
        {
            this._id = 0;
            this._name = "";
            this._location = null;
            this._chargeSlots = 0;
            this._isAvailable = false;
        }
    }
}