using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BO
{
    public class CostumerInParcel
    {
        private DO.Costumer _costumer;

        public CostumerInParcel(DO.Costumer costumer)
        {
            _costumer = costumer;
        }

        public int Id => _costumer.Id;

        public string Name => _costumer.Name;

        public override string ToString()
        {
            return $"{Name}(id: {Id})";
        }
    }
}