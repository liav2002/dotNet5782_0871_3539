using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class ParcelAtCostumer
        {
            private IDAL.DO.Parcel _parcel;
            private CostumerInParcel _csp;

            public ParcelAtCostumer(IDAL.DO.Parcel parcel, IDAL.DO.Costumer costumer)
                // need both parameters:
                // [parcel] - to know what parcel,
                // [costumer] - to know who is the other costumer
            {
                IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton
                int otherCostumerId;
                // if [costumer] is the sender so [otherCostumerId] reference to the receiver ext
                otherCostumerId = parcel.TargetId == costumer.Id
                    ? parcel.SenderId // if the [costumer] is the target 
                    : parcel.TargetId; // else
                _parcel = parcel;
                _csp = new CostumerInParcel(dalObj.GetCostumerById(otherCostumerId));
            }

            public double Id => _parcel.Id;

            public IDAL.DO.WeightCategories Weight => _parcel.Weight;

            public IDAL.DO.Priorities Priority => _parcel.Priority;

            public IDAL.DO.ParcelStatuses Status => _parcel.Status;

            public CostumerInParcel CostumerParcel => _csp;

            public override string ToString()
            {
                return $"id: {Id}, target's id: {this._parcel.TargetId}, weight: {Weight}, priority: {Priority}\n" +
                       $"drone's id: {this._parcel.DroneId}";
            }
        }
    }
}