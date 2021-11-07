using System;
using System.Collections.Generic;
using System.Text;

namespace IBL
{
    namespace BO
    {
        public class NonItems: Exception
        {
            public NonItems(string item) : base("ERROR: There is no " + item + ".\n")
            {
            }
        }

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

        public class ParcelAlreadyAssign : Exception
        {
            public ParcelAlreadyAssign(int droneId) : base("ERROR: Parcel is already assgin to drone. drone's id: " + droneId + ".\n")
            {
            }
        }
    }
}