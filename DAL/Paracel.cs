using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
    namespace DO
    {
        public class Parcel
        {
            private int _id;
            private int _senderId;
            private int _targetId;
            private WeightCategories _weight;
            private Priorities _priority;
            private DateTime _requested;
            private int _droneId;
            private DateTime _scheduled;
            private DateTime _pickedUp;
            private DateTime _delivered;


            public int Id
            {
                get => _id;
                set => _id = value;
            }

            public int SenderId
            {
                get => _senderId;
                set => _senderId = value;
            }

            public int TargetId
            {
                get => _targetId;
                set => _targetId = value;
            }

            public WeightCategories Weight
            {
                get => _weight;
                set => _weight = (WeightCategories) value;
            }

            public Priorities Priority
            {
                get => _priority;
                set => _priority = (Priorities) value;
            }

            public DateTime Requsted
            {
                get => _requested;
                set => _requested = value;
            }

            public int DroneId
            {
                get => _droneId;
                set => _droneId = value;
            }

            public DateTime Scheduled
            {
                get => _scheduled;
                set => _scheduled = value;
            }

            public DateTime PickedUp
            {
                get => _pickedUp;
                set => _pickedUp = value;
            }

            public DateTime Delivered
            {
                get => _delivered;
                set => _delivered = value;
            }

            public override string ToString()
            {
                return string.Format("the id is: {0}\nthe senderId is: {1}\nthe targetId is: {2}\n" +
                                     "the weight is: {3}\nthe priority is: {4}\nthe requsted is: {5}\n" +
                                     "the droneId is: {6}\nthe scheduled is: {7}\nthe pickedUp is: {8}\n" +
                                     "the pickedUp is: {9}\n"
                    , Id, SenderId, TargetId, Weight, Priority, Requsted, DroneId, Scheduled, PickedUp, Delivered);
            }

            public Parcel(int id, int senderId, int targetId, WeightCategories weight, Priorities priority,
                DateTime requested,
                int droneId, DateTime scheduled, DateTime pickedUp, DateTime delivered)
            {
                this._id = id;
                this._senderId = senderId;
                this._targetId = targetId;
                this._weight = weight;
                this._priority = priority;
                this._requested = requested;
                this._droneId = droneId;
                this._scheduled = scheduled;
                this._pickedUp = pickedUp;
                this._delivered = delivered;
            }
        }
    }
}