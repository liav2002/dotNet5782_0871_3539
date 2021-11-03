using System;
using IDAL;
using System.Collections.Generic;
using DalObject;

namespace IBL
{
    namespace BO
    {
        class BL : IBL
        {
            private IDAL.IDAL dalObj;
            private IEnumerable<IDAL.DO.Drone> drones;
            private double[] charge;
            private List<double> chargeRate = new List<double>();

            public BL()
            {
                this.dalObj = new DalObject.DalObject();
                drones = dalObj._getDroneList();
                
                IEnumerable<IDAL.DO.Parcel> parcels = dalObj._getParceList();

                charge = dalObj.PowerRequest();

                foreach(var drone in drones)
                {
                }

                foreach (var parcel in parcels)
                {
                    if (!(parcel.DroneId == 0) && (parcel.Delivered == default(DateTime))) {
                        // for every parcel that not arrived but have a drone
                        var correctDrone = dalObj._getDroneById(parcel.DroneId);

                        correctDrone.Status = IDAL.DO.DroneStatuses.Shipping;
                        // change the correct drone status to shipping

                        correctDrone.
                    }
                }
            }
        }
    }
}