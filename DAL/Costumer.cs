using System;

namespace IDAL
{
    namespace DO
    {
        public class Costumer
        {
            private int _id;
            private string _name;
            private string _phone;
            private double _longitube;
            private double _latitude;

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

            public string Phone
            {
                get => _phone;
                set => _phone = value;
            }

            public double Longitube
            {
                get => _longitube;
                set => _longitube = value;
            }

            public double Latitude
            {
                get => _latitude;
                set => _latitude = value;
            }

            public override string ToString()
            {
                return string.Format("the id is: {0}\nthe name is: {1}\nthe phone is: {2}\n" +
                                     "the longitube is: {3}\nthe latitude is: {4}\n"
                    , Id, Name, Phone, Longitube, Latitude);
            }

            public Costumer(int id, string name, string phone, double longitube, double latitude)
            {
                this._id = id;
                this._name = name;
                this._phone = phone;
                this._longitube = longitube;
                this._latitude = latitude;
            }
        }
    }
}