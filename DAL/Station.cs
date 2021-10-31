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
            private double _longitude;
            private double _latitude;
            private int _chargeSolts;

            public int Id { get => _id; set => _id = value; }
            public string Name { get => _name; set => _name = value; }
            public double Longitude { get => _longitude; set => _longitude = value; }
            public double Latitude { get => _latitude; set => _latitude = value; }
            public int ChargeSolts { get => _chargeSolts; set => _chargeSolts = value; }
            public override string ToString()
            {
                return string.Format("the id is: {0}\nthe name is: {1}\nthe longitude is: {2}\n" +
                    "the latitude is: {3}\nthe number of available argument positions: {4}\n"
                    , Id, Name, Longitude, Latitude, ChargeSolts);
            }
			public Station(int id, string name, double longitude, double latitude, int charge_solts){
				this._id = id;
				this._name = name;
				this._longitude = longitude;
				this._latitude = latitude;	
				this._chargeSolts = charge_solts;
			}
        }
    }

}