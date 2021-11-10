using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
    namespace DO
    {
        public class Parcel
        {
            public int Id { get; set; }

            public int SenderId { get; set; }

            public int TargetId { get; set; }

            public WeightCategories Weight { get; set; }

            public Priorities Priority { get; set; }

            public DateTime Requested { get; set; }

            public int DroneId { get; set; }

            public DateTime Scheduled { get; set; }

            public DateTime PickedUp { get; set; }

            public DateTime Delivered { get; set; }

            public ParcelStatuses Status { get; set; }

            public override string ToString()
            {
                return $"the id is: {Id}\nthe senderId is: {SenderId}\nthe targetId is: {TargetId}\n" +
                       $"the weight is: {Weight}\nthe priority is: {Priority}\nthe requested is: {Requested}\n" +
                       $"the droneId is: {DroneId}\nthe scheduled is: {Scheduled}\nthe pickedUp is: {PickedUp}\n" +
                       $"the pickedUp is: {Delivered}\n";
            }

            public Parcel(int id, int senderId, int targetId, WeightCategories weight, Priorities priority,
                DateTime requested,
                int droneId, DateTime scheduled, DateTime pickedUp, DateTime delivered)
            {
                this.Id = id;
                this.DroneId = droneId;
                this.SenderId = senderId;
                this.TargetId = targetId;
                this.Weight = weight;
                this.Priority = priority;
                this.Requested = requested; // new parcel is created.
                this.Scheduled = scheduled; // we make an assign between drone to parcel.
                this.PickedUp = pickedUp; // parcel is being picked up.
                this.Delivered = delivered; // costumer get the parcel.

                if (delivered != default(DateTime)) this.Status = ParcelStatuses.Delivered;
                else if (pickedUp != default(DateTime)) this.Status = ParcelStatuses.PickedUp;
                else if (scheduled != default(DateTime)) this.Status = ParcelStatuses.Assign;
                else if (requested != default(DateTime)) this.Status = ParcelStatuses.Created;
            }
        }
    }
}