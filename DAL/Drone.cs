using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
    namespace DO
    {
        public struct Drone
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

            public IDAL.DO.DroneStatuses Status
            {
                get => (IDAL.DO.DroneStatuses) _status;
                set => _status = (IDAL.DO.DroneStatuses) value;
            }

            public double Battery
            {
                get => _battery;
                set => _battery = value;
            }

            public override string ToString()
            {
                return string.Format("the id is: {0}\nthe model is: {1}\nthe maxWegiht is: {2}\n" +
                                     "the status is: {3}\nthe battery is: {4}\n"
                    , Id, Model, MaxWeight, Status, Battery);
            }

            public Drone(int id, string model, WeightCategories maxWeight, DroneStatuses status, double battery)
            {
                this._id = id;
                this._model = model;
                this._maxWeight = maxWeight;
                this._status = status;
                this._battery = battery;
            }
        }
    }
}