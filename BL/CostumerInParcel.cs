using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class CostumerInParcel
        {
            private IDAL.DO.Costumer _costumer;

            public CostumerInParcel(IDAL.DO.Costumer costumer)
            {
                _costumer = costumer;
            }

            public int Id => _costumer.Id;
            
            public string Name => _costumer.Name;
        }
    }
}