using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SysLog
{
    public class SysLog
    {
        private bool _start;

        //Singleton design pattern
        private static SysLog _instance = null;
        public static SysLog GetInstance() => _instance ?? (_instance = new SysLog());

        private void Type(string str)
        {
            foreach (char ch in str)
            {
                Console.Write(ch.ToString());
                if (!_start)
                {
                    Thread.Sleep(30);
                }
            }

            if (_start)
            {
                Thread.Sleep(5);
            }

            Console.WriteLine();
        }

        private SysLog()
        {
            this._start = true;
        }

        public void SetStarttoFalse()
        {
            this._start = false;
        }

        public void CalculateNearStation(string obj)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: calculate near station to " + obj + "...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void InitDroneLocation(int droneId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: initalizing drone location (" + droneId + ")...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void InitDroneBattery(int droneId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: initalizing drone battery (" + droneId + ")...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void HandleAssignParcels()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type(
                "SYSTEM_LOG: find all the drones which was assign to a parcel, and change there status to Shipping.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void HandleAssignParcel(int parcelId, int droneId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: making assign between parcel (id: " + parcelId + ") and drone (id: " +
                 droneId + ")...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ChangeCostumerName(int costumerId, string name)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: change costumer (id: " + costumerId + ") name to " + name + "...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ChangeCostumerPhone(int costumerId, string phone)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: change costumer (id: " + costumerId + ") phone to " + phone + "...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ChangeStationName(int stationId, string name)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: change station (id: " + stationId + ") name to " + name + "...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ChangeStationChargeSlots(int stationId, int chargeSlots)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: change station (id: " + stationId + ") charge slots to " + chargeSlots +
                 "...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ChangeDroneStatus(int droneId, IDAL.DO.DroneStatuses status)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: change drone (id: " + droneId + ") status to " + status + "...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ChangeDroneModelName(int droneId, string name)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: change drone (id: " + droneId + ") model to " + name + "...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void AddStation(int stationId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: adding new station (id: " + stationId + ")...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void AddDrone(int droneId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: adding new drone (id: " + droneId + ")...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void AddCostumer(int costumerId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: adding new costumer (id: " + costumerId + ")...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void AddParcel(int parcelId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: adding new parcel (id: " + parcelId + ")...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void MoveParcelToWaitingList(int parcelId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: move parcel(id: " + parcelId + ") to waiting list...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void TryHandleWaitingParcels()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: try to make an assign to a waiting parcel...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ParcelCollection(int parcelId, int droneId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: parcel(id: " + parcelId + ") is pickedup by drone(id: " + droneId +
                 ")...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ParcelDelivered(int parcelId, int droneId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: parcel(id: " + parcelId + ") is delivered by drone(id: " + droneId +
                 ")...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DroneRelease(int droneId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: release drone(id: " + droneId + ")...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DroneToCharge(int droneId, int stationId)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("SYSTEM_LOG: send drone(id: " + droneId + ") to charging in station(id: " + stationId + ")...");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void SuccessMessage()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("Success.\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void FailedMessage(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Type("Failed.\n");
            Type(errorMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void EndMessage()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Type("Good bye !!\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}