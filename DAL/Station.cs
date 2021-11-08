using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
    namespace DO
    {
        public class Station
        {
            private int _id;
            private string _name;
            private Point _location;
            private int _chargeSlots;

            public int Id
            {
                get => _id;
                set => _id = value;
            }

            public string Name
            {
                get => _name;
                set => _name = value;
            }

            public Point Location
            {
                get => _location;
                set => _location = value;
            }


            public int ChargeSlots
            {
                get => _chargeSlots;
                set => _chargeSlots = value;
            }

            public override string ToString()
            {
                return string.Format("the id is: {0}\nthe name is: {1}\nthe location is: {2}\n" +
                                     "\nthe number of available argument positions: {3}\n"
                    , Id, Name, Location, ChargeSlots);
            }

            public Station(int id, string name, Point location, int chargeSlots)
            {
                this._id = id;
                this._name = name;
                this._location = location;
                this._chargeSlots = chargeSlots;
            }
        }
    }
}