using System;
using System.Collections.Generic;
using IDAL.DO;

namespace DalObject
{
    public class DataSource
    {
        //Singelton Design pattern
        private static DataSource instance = null;

        public static DataSource GetInstance()
        {
            if (instance == null)
                instance = new DataSource();
            return instance;
        }

        private DataSource()
        {
        }

        internal static List<IDAL.DO.Drone> drones = new List<IDAL.DO.Drone>();
        internal static List<IDAL.DO.Station> stations = new List<IDAL.DO.Station>();
        internal static List<IDAL.DO.Costumer> costumers = new List<IDAL.DO.Costumer>();
        internal static List<IDAL.DO.Parcel> parcels = new List<IDAL.DO.Parcel>();
        internal static List<IDAL.DO.DroneCharge> droneCharge = new List<IDAL.DO.DroneCharge>();

        internal static Random rand = new Random();


        public class Config
        {
            //TODO: Find out what the power consumption is on all cases and initail accordingly.
            public static double avilablePPK = 0.0005; // PPK - Power consumption Per Kilometer. for avilble drone.
            public static double lightPPK = 0.0015; // for drone who carried a light weight.
            public static double mediumPPK = 0.002; // for drone who carried a medium weight.
            public static double heavyPPK = 0.0025; // for drone who carried a heavy weight.

            //TODO: Find out what the charge rate is, and initailze accordingly.
            public static double chargeRatePH = 66.66; // drone charging rate per hour.
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
                drones.Add(
                    new IDAL.DO.Drone(rand.Next(1000, 10001), "Model-X", (IDAL.DO.WeightCategories) rand.Next(3),
                        rand.NextDouble() * 20 + 20));
            }
        }

        public static void InitializeStation()
        {
            stations.Add(new IDAL.DO.Station(1010, "Netanya - College",
                new IDAL.DO.Location(32.30747945219766, 34.87919798038194),
                rand.Next(10)));
            stations.Add(new IDAL.DO.Station(1020, "The Temple Mount",
                new IDAL.DO.Location(31.65266801604753, 35.2281960943494),
                rand.Next(10) + 1)); //I don't want stations without a free charge slots.
        }

        public static void InitializeCostumer()
        {
            for (int i = 0; i < 10; ++i)
            {
                costumers.Add(new IDAL.DO.Costumer(2000 + (i + 1) * 10, "Costumer " + (i + 1).ToString(),
                    "05" + rand.Next(5).ToString() + "-" + rand.Next(999).ToString() + "-" +
                    rand.Next(9999).ToString(), new IDAL.DO.Location(30.234196842399772, 48.74692937085842)));
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

                parcels.Add(new IDAL.DO.Parcel(rand.Next(50, 100), costumers[sender].Id, costumers[target].Id,
                    (IDAL.DO.WeightCategories) rand.Next(3),
                    (IDAL.DO.Priorities) rand.Next(3), DateTime.Now, drones[rand.Next(5)].Id, null,
                    null, null));
            }
        }

        public static int ParcelsLength()
        {
            return parcels.Count;
        }
    }
}