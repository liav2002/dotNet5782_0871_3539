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
    }
}