﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;


namespace BO
{
    public class ParcelBL
    {
        private DO.Parcel _parcel;
        private DroneParcelBL _drone;
        private CostumerInParcel _receiver;
        private CostumerInParcel _sender;

        public ParcelBL(DO.Parcel parcel)
        {
            DalApi.IDAL idalObj = DalFactory.GetDal(DO.DalTypes.DalObj); // Singleton

            _parcel = parcel;

            _receiver = new CostumerInParcel(idalObj.GetCostumerById(parcel.TargetId));

            _sender = new CostumerInParcel(idalObj.GetCostumerById(parcel.SenderId));

            _drone = (parcel.DroneId != 0) ? new DroneParcelBL(idalObj.GetDroneById(parcel.DroneId)) : null;
        }

        public double Id => _parcel.Id;

        public CostumerInParcel Receiver => _receiver;

        public CostumerInParcel Sender => _sender;

        public DO.WeightCategories Weight => _parcel.Weight;

        public DO.Priorities Priority => _parcel.Priority;

        public DroneParcelBL Drone => _drone;

        public DateTime? Delivered => _parcel.Delivered;

        public DateTime? Requested => _parcel.Requested;

        public DateTime? Scheduled => _parcel.Scheduled;

        public DateTime? PickedUp => _parcel.PickedUp;

        public override string ToString()
        {
            string droneId = (Drone != null) ? $"{Drone.Id}\n" : "None\n";
            return $"the id is: {Id}\nthe senderId is: {Sender.Id}\nthe targetId is: {Receiver.Id}\n" +
                   $"the weight is: {Weight}\nthe priority is: {Priority}\nthe requested time is: {Requested}\n" +
                   $"the droneId is: {droneId}\nthe scheduled time is: {Scheduled}\nthe pickedUp time is: {PickedUp}\n" +
                   $"the delivered time is: {Delivered}\n";
        }
    }
}