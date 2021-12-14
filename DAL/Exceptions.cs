using System;
using System.Collections.Generic;
using System.Text;


namespace DO
{
    public class ItemNotFound : Exception
    {
        public ItemNotFound(string item) : base("ERROR: " + item + " not found.\n")
        {
        }
    }

    public class NonAvilableDrones : Exception
    {
        public NonAvilableDrones() : base(
            "Logistic Problem: There is no any avilable drone.\nmoving parcel to waiting list...\n")
        {
        }
    }
}