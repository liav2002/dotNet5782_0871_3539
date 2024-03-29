﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BO
{
    public class DroneChargeBL
    {
        private DO.Drone _drone;
        private bool _isCharge;

        public DroneChargeBL(DO.Drone drone, bool isCharge = true)
        {
            _drone = drone;
            _isCharge = isCharge;
        }

        public int Id => _drone.Id;

        public double Battery => _drone.Battery;

        public bool IsCharge => this._isCharge;

        public override string ToString()
        {
            return $"drone's id: {Id}, battery: {Battery} --> " +
                   (this._isCharge ? "The drone is charging.\n" : "The drone is not charging.\n");
        }
    }
}