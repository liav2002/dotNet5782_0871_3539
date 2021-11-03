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





    }
}
