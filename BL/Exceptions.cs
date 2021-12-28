using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class NonItems : Exception
    {
        public NonItems(string item) : base("ERROR: There is no " + item + ".\n")
        {
        }
    }

    public class NonUniqueID : Exception
    {
        public NonUniqueID(string item) : base("ERROR: " + item + " must be unique.\n")
        {
        }
    }

    public class NoAvailable : Exception
    {
        public NoAvailable(string item) : base("ERROR: " + item + " is not available.\n")
        {
        }
    }
    
    public class RemoveError : Exception
    {
        public RemoveError(string item) : base("ERROR: " + item + " cannot be deleted.\n")
        {
        }
    }

    public class NotNewValue : Exception
    {
        public NotNewValue(string property, string item) : base(
            "ERROR: " + property + " is already " + item + ".\n")
        {
        }
    }

    public class NegetiveValue : Exception
    {
        public NegetiveValue(string item) : base("ERROR: " + item + " must be positive.\n")
        {
        }
    }

    public class WrongEnumValuesRange : Exception
    {
        public WrongEnumValuesRange(string item, string start, string end) : base("ERROR: " + item +
            "(type: Enumorator) have only " + start + "-" + end + " values.\n")
        {
        }
    }

    public class SelfDelivery : Exception
    {
        public SelfDelivery() : base("ERROR: It's imposible to send parcel to yourself.\n")
        {
        }
    }

    public class NoAssignParcels : Exception
    {
        public NoAssignParcels(int droneId) : base("ERROR: No parcels been assign to drone(" + droneId + ").\n")
        {
        }
    }

    public class ParcelAlreadyAssign : Exception
    {
        public ParcelAlreadyAssign(int droneId) : base("ERROR: Parcel is already assgin to drone. drone's id: " +
                                                       droneId + ".\n")
        {
        }
    }

    public class InvalidInput : Exception
    {
        public InvalidInput() : base("ERROR: Invalid input.\n")
        {
        }
    }

    public class NotAvilableStation : Exception
    {
        public NotAvilableStation() : base("ERROR: There is no any avilable station for the drone.\n")
        {
        }
    }

    public class DroneNotAvliable : Exception
    {
        public DroneNotAvliable(int droneId) : base("ERROR: drone (id: " + droneId +
                                                    ") is not in 'Avliable' status.\n")
        {
        }
    }

    public class DroneNotShipping : Exception
    {
        public DroneNotShipping(int droneId) : base("ERROR: drone (id: " + droneId +
                                                    ") is not in 'Shipping' status.\n")
        {
        }
    }

    public class DroneNotInMaintenance : Exception
    {
        public DroneNotInMaintenance(int droneId) : base("ERROR: drone (id: " + droneId +
                                                         ") is not in 'Maintenance' status.\n")
        {
        }
    }

    public class DroneNotEnoughBattery : Exception
    {
        public DroneNotEnoughBattery(int droneId) : base("ERROR: drone (id: " + droneId +
                                                         ") not have enough battery.\n")
        {
        }
    }

    public class NoParcelsForAssign : Exception
    {
        public NoParcelsForAssign() : base("ERROR: There is no any parcel in waiting list.\n")
        {
        }
    }

    public class ParcelNotInAssignStatus : Exception
    {
        public ParcelNotInAssignStatus(int parcelId) : base("ERROR: parcel (id: " + parcelId +
                                                            ") not in assigned status.\n")
        {
        }
    }

    public class ParcelNotPickedUp : Exception
    {
        public ParcelNotPickedUp(int parcelId) : base("ERROR: parcel (id: " + parcelId +
                                                      ") not in not picked up.\n")
        {
        }
    }

    public class ParcelIsAlreadyDelivered : Exception
    {
        public ParcelIsAlreadyDelivered(int parcelId) : base("ERROR: parcel (id: " + parcelId +
                                                             ") delivered arleady.\n")
        {
        }
    }

    public class ParcelIsAlreadyPickedUp : Exception
    {
        public ParcelIsAlreadyPickedUp(int parcelId) : base("ERROR: parcel (id: " + parcelId +
                                                            ") picked up already.\n")
        {
        }
    }

    public class NoSuitableParcelForDrone : Exception
    {
        public NoSuitableParcelForDrone(int droneId) : base("ERROR: there is no suitable parcel for drone(id: " +
                                                            droneId + ").\n")
        {
        }
    }

    public class CantRemoveUsedSlots : Exception 
    {
        public CantRemoveUsedSlots() : base("ERROR: you are trying to remove used slots.")
        {
        }
    }

    public class UsernameNotFound : Exception
    {
        public UsernameNotFound(string username) : base("ERROR: The username (" + username + ") cannot be found.\n")
        {
        }
    }

    public class FailedSignIn : Exception
    {
        public FailedSignIn() : base("ERROR: username or password are incorrect.\n")
        {
        }
    }

    public class UserBlocked : Exception
    {
        public UserBlocked() : base("Sorry, you're blocked by managment.\n")
        {
        }
    }
}