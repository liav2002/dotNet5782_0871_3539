using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
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

            private double _chargeRate;

            public Drone(int id, string model, WeightCategories maxWeight, double battery)
            {
                this._id = id;
                this._model = model;
                this._maxWeight = maxWeight;
                this._chargeRate = DalObject.DataSource.Config.chargeRatePH;
                this._status = DroneStatuses.Available;
                this._battery = battery;
                this._location = new Location();
            }

            public double ChargeRate
            {
                get => _chargeRate;
                set => _chargeRate = value;
            }

            public int Id
            {
                get => _id;
                set => _id = value;
            }

            public string Model
            {
                get => _model;
                set => _model = value;
            }

            public WeightCategories MaxWeight
            {
                get => (WeightCategories) _maxWeight;
                set => _maxWeight = (WeightCategories) value;
            }

            public DO.DroneStatuses Status
            {
                get => (DO.DroneStatuses) _status;
                set => _status = (DO.DroneStatuses) value;
            }

            public double Battery
            {
                get => _battery;
                set => _battery = value;
            }

            public Location Location
            {
                get => _location;
                set => _location = value;
            }


            public override string ToString()
            {
                return string.Format("the id is: {0}\nthe model is: {1}\nthe maxWegiht is: {2}\n" +
                                     "the status is: {3}\nthe battery is: {4}\n"
                    , Id, Model, MaxWeight, Status, String.Format("{0:F3}", Battery));
            }
        }
    }
}