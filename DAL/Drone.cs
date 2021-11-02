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

            public override string ToString()
            {
                return string.Format("the id is: {0}\nthe model is: {1}\nthe maxWegiht is: {2}\n" +
                                     "the status is: {3}\nthe battery is: {4}\n"
                    , Id, Model, MaxWeight, Status, Battery);
            }

            public Drone(int id, string model, WeightCategories maxWeight, DroneStatuses status = DO.DroneStatuses.Undefined, double battery = -1) 
                // the default for status and battery is set to be uninitialized.
                // to avoid runing problems I initialzed it with unreal values.
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