using System;
using System.Collections.Generic;
using System.Text;


namespace DO
{
    public class Drone
    {
        public DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays((new Random()).Next(range));
        }

        private int _id;

        private string _model;

        private WeightCategories _maxWeight;

        private DroneStatuses _status;

        private double _battery;

        private Location _location;

        private bool _isAvaliable;

        
        public Drone(int id, string model, WeightCategories maxWeight, double battery)
        {
            SysLog.SysLog.GetInstance().AddDrone(id);
            this._id = id;
            this._model = model;
            this._maxWeight = maxWeight;
            this._status = DroneStatuses.Available;
            this._battery = battery;
            this._location = new Location();
            this._isAvaliable = true;
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Model
        {
            get => _model;
            set
            {
                _model = value;
                SysLog.SysLog.GetInstance().ChangeDroneModelName(_id, value);
            }
        }

        public WeightCategories MaxWeight
        {
            get => (WeightCategories) _maxWeight;
            set => _maxWeight = (WeightCategories) value;
        }

        public DroneStatuses Status
        {
            get => (DroneStatuses) _status;
            set
            {
                SysLog.SysLog.GetInstance().ChangeDroneStatus(_id, value);
                if (_status == DroneStatuses.Maintenance && value == DroneStatuses.Available)
                    // we release now the drone from charge
                    SysLog.SysLog.GetInstance().DroneRelease(_id);
                _status = (DroneStatuses) value;
            }
        }

        public double Battery
        {
            get => _battery;
            set
            {
                _battery = value;
                SysLog.SysLog.GetInstance().InitDroneBattery(this.Id);
            }
        }

        public Location Location
        {
            get => _location;
            set
            {
                _location = value;
                SysLog.SysLog.GetInstance().InitDroneLocation(this.Id);
            }
        }

        public bool IsAvaliable
        {
            get => this._isAvaliable;
            set => this._isAvaliable = value;
        }

        public override string ToString()
        {
            return string.Format("the id is: {0}\nthe model is: {1}\nthe maxWegiht is: {2}\n" +
                                 "the status is: {3}\nthe battery is: {4}\n"
                , Id, Model, MaxWeight, Status, String.Format("{0:F3}", Battery));
        }
    }
}