using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
    namespace DO
    {
        public class NonUniqueID : Exception
        {
            public NonUniqueID(string item) : base("ERROR: " + item + " must be unique.\n")
            {
            }
        }

        public class NegetiveValue : Exception
        {
            public NegetiveValue(string item) : base("ERROR: " + item + " must be positive.\n")
            {
            }
        }

        public class InvalidBattery : Exception
        {
            public InvalidBattery() : base("ERROR: Battery must be between 0-100.\n")
            {
            }
        }

        public class WrongEnumValuesRange : Exception
        {
            public WrongEnumValuesRange(string item, string start, string end) : base("ERROR: " + item +
                "(type: Enumorator) have only " + start + "-" + end + " values.\n")
            {
            }
        }

        public class SelfDelivery : Exception
        {
            public SelfDelivery() : base("ERROR: It's imposible to send parcel to yourself.\n")
            {
            }
        }

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
}