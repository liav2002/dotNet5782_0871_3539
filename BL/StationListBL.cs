using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;


namespace BO
{
    public class StationListBL
    {
        private DO.Station _station;

        private int _takingSlots;

        public StationListBL(DO.Station station)
        {
            DalApi.IDal idalObj = DalFactory.GetDal();

            _station = station;

            foreach (var droneCharge in idalObj.GetDroneChargeList())
                if (droneCharge.StationId == station.Id)
                    _takingSlots++;
        }

        public int Id => _station.Id;

        public string Name => _station.Name;

        public int FreeSlots => _station.ChargeSlots;

        public int TakingSlots => _takingSlots;

        public bool IsAvailable => _station.IsAvailable;

        public override string ToString()
        {
            return
                $"Id: {Id}, Name: {Name}, Number of free slots: {FreeSlots}, Number of occupied slots: {TakingSlots}, Location: {this._station.Location}.\n";
        }
    }
}