using System;
using System.Collections.Generic;
using System.Text;

namespace IDAL
{
    namespace DO
    {
        public enum DroneStatuses
        {
            Available,
            Maintenance,
            Shipping
        };
        
        public enum ParcelStatuses
        {
            Created,
            Assign,
            PickedUp,
            Delivered,
        };

        public enum WeightCategories
        {
            Light,
            Medium,
            Heavy
        };

        public enum Priorities
        {
            Regular,
            Fast,
            Emergency
        }
    }
}