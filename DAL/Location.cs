using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        public class Location
        {
            private double _latitude;
            private double _longitude;

            public Location(double latitude, double longitude)
            {
                this._latitude = latitude;
                this._longitude = longitude;
            }

            public Location() // Empty ctor: initial with random values
            {
                Random random = new Random();

                this._latitude = 0; // random.NextDouble() * 180 - 90; // get double value in range (-90, 90) TODO: show to eyal.
                this._longitude = 0; // random.NextDouble() * 360 - 180; // get double value in range (-180, 180)
            }


            public double Latitude
            {
                get => _latitude;
                set => _latitude = value;
            }

            public double Longitude
            {
                get => _longitude;
                set => _longitude = value;
            }

            /**********************************************************************************************
             * Details: this function calculate distance between to points with lattiude and longitude.   *
             * Parameters: 2 points values (lattidue and longitude)                                       *
             * Return: distatnce in km.                                                                   *
             **********************************************************************************************/
            private const double RADIUS = 6371.0088; // the average Radius of earth

            public double Distance(Location p) // the return value is in km
            {
                double dlon = _Radians(this._longitude - p._longitude);
                double dlat = _Radians(this._latitude - p._latitude);

                double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(_Radians(p._latitude)) *
                    Math.Cos(_Radians(this._latitude)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
                double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                return angle * RADIUS;
            }

            static private double _Radians(double x)
            {
                return x * Math.PI / 180;
            }

            public override string ToString()
            {
                return string.Format("({0}, {1})"
                    , String.Format("{0:F4}", Latitude), String.Format("{0:F4}", Longitude));
                // the format: (xx.xxxx, yy.yyyy)
            }
        }
    }
}