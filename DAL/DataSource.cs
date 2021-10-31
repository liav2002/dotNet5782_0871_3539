using System;
using System.Collections.Generic;

namespace DalObject
{
    public class DataSource
    {
        internal static List<IDAL.DO.Drone> drones = new List<IDAL.DO.Drone>();
        internal static List<IDAL.DO.Station> stations = new List<IDAL.DO.Station>();
        internal static List<IDAL.DO.Costumer> costumers = new List<IDAL.DO.Costumer>();
        internal static List<IDAL.DO.Parcel> parcels = new List<IDAL.DO.Parcel>();
        internal static List<IDAL.DO.DroneCharge> droneCharge = new List<IDAL.DO.DroneCharge>();

        internal static Queue<IDAL.DO.Parcel> waitingParcels =
            new Queue<IDAL.DO.Parcel>();


        internal static Random rand = new Random();

        internal class Config
        {
            internal static Random rand = new Random();
        }

        internal static void Initialize()
        {
            InitializeDrone();
            InitializeStation();
            InitializeCostumer();
            InitializeParcel();
        }

        public static void InitializeDrone()
        {
            for (int i = 0; i < 5; i++)
            {
                drones.Add(new IDAL.DO.Drone(rand.Next(1000, 10001), "Model-X", (IDAL.DO.WeightCategories)rand.Next(3), 
                    (IDAL.DO.DroneStatuses)rand.Next(3), rand.Next(101)));
            }
        }

        public static void InitializeStation()
        {
            stations.Add(new IDAL.DO.Station(1010, "Netanya - College", 32.30747945219766, 34.87919798038194, rand.Next(10)));
            stations.Add(new IDAL.DO.Station(1020, "The Temple Mount", 31.65266801604753, 35.2281960943494, rand.Next(10)));
        }

        public static void InitializeCostumer()
        {
            for (int i = 0; i < 10; ++i)
            {
                costumers.Add(new IDAL.DO.Costumer(2000 + (i + 1) * 10, "Costumer " + (i + 1).ToString(), 
                    "05" + rand.Next(5).ToString() + "-" + rand.Next(999).ToString() + "-" +
                            rand.Next(9999).ToString(), 30.234196842399772, 48.74692937085842));
            }
        }

        public static void InitializeParcel()
        {
            int target = 0, sender = 0;

            for (int i = 0; i < 10; ++i)
            {
                //generate random sender and random target (they must be different)
                do
                {
                    target = rand.Next(10);
                    sender = rand.Next(10);
                } while (target == sender);

                parcels.Add(new IDAL.DO.Parcel(rand.Next(50, 100), costumers[sender].Id, costumers[target].Id, (IDAL.DO.WeightCategories)rand.Next(3),
                    (IDAL.DO.Priorities)rand.Next(3), DateTime.Now, drones[rand.Next(5)].Id, DateTime.Now, DateTime.Now, DateTime.Now));
            }
        }
    }
}