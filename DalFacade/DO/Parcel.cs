using System;
using System.Collections.Generic;
using System.Text;


namespace DO
{
    public class Parcel
    {
        private int _id;

        private int _droneId;

        private int _senderId;

        private int _targetId;

        private ParcelStatuses _status;

        private WeightCategories _weight;

        private Priorities _priority;

        private DateTime? _requested;

        private DateTime? _scheduled;

        private DateTime? _pickedUp;

        private DateTime? _delivered;

        private bool _isAvailable;

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

        public bool IsAvailable
        {
            get => this._isAvailable;
            set => this._isAvailable = value;
        }

        public WeightCategories Weight
        {
            get => _weight;
            set => _weight = value;
        }

        public Priorities Priority
        {
            get => _priority;
            set => _priority = value;
        }

        public DateTime? Requested
        {
            get => _requested;
            set => _requested = value;
        }

        public int DroneId
        {
            get => _droneId;
            set
            {
                _droneId = value;
                SysLog.SysLog.GetInstance().HandleAssignParcel(this.Id, value);
            }
        }

        public DateTime? Scheduled
        {
            get => _scheduled;
            set => _scheduled = value;
        }

        public DateTime? PickedUp
        {
            get => _pickedUp;
            set => _pickedUp = value;
        }

        public DateTime? Delivered
        {
            get => _delivered;
            set => _delivered = value;
        }

        public ParcelStatuses Status
        {
            get => _status;
            set
            {
                _status = value;
                if (_status == ParcelStatuses.Delivered)
                    SysLog.SysLog.GetInstance().ParcelDelivered(_id, _droneId);
                if (_status == ParcelStatuses.PickedUp)
                    SysLog.SysLog.GetInstance().ParcelCollection(_id, _droneId);
            }
        }

        public override string ToString()
        {
            return $"the id is: {Id}\nthe senderId is: {SenderId}\nthe targetId is: {TargetId}\n" +
                   $"the weight is: {Weight}\nthe priority is: {Priority}\nthe requested is: {Requested}\n" +
                   $"the droneId is: {DroneId}\nthe scheduled is: {Scheduled}\nthe pickedUp is: {PickedUp}\n" +
                   $"the pickedUp is: {Delivered}\n";
        }

        public Parcel(int id, int senderId, int targetId, WeightCategories weight, Priorities priority,
            DateTime? requested,
            int droneId, DateTime? scheduled, DateTime? pickedUp, DateTime? delivered)
        {
            SysLog.SysLog.GetInstance().AddParcel(id);
            this._id = id;
            this._droneId = droneId;
            this._senderId = senderId;
            this._targetId = targetId;
            this._weight = weight;
            this._priority = priority;
            this._requested = requested; // new parcel is created.
            this._scheduled = scheduled; // we make an assign between drone to parcel.
            this._pickedUp = pickedUp; // parcel is being picked up.
            this._delivered = delivered; // costumer get the parcel.
            this._isAvailable = true;

            if (delivered != null) this._status = ParcelStatuses.Delivered;
            else if (pickedUp != null) this._status = ParcelStatuses.PickedUp;
            else if (scheduled != null) this._status = ParcelStatuses.Assign;
            else if (requested != null) this._status = ParcelStatuses.Created;
        }

        public Parcel()
        {
            this._droneId = 0;
            this._id = 0;
            this._isAvailable = false;
            this._senderId = 0;
            this._targetId = 0;
            this._priority = default;
            this._weight = default;
            this._delivered = null;
            this._scheduled = null;
            this._pickedUp = null;
            this._requested = null;
            this._status = default;
        }
    }
}