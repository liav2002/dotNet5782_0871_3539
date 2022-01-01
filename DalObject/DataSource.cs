using System;
using System.Collections.Generic;

namespace Dal
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

        internal static List<DO.Drone> drones = new List<DO.Drone>();
        internal static List<DO.Station> stations = new List<DO.Station>();
        internal static List<DO.Costumer> costumers = new List<DO.Costumer>();
        internal static List<DO.Parcel> parcels = new List<DO.Parcel>();
        internal static List<DO.DroneCharge> droneCharge = new List<DO.DroneCharge>();
        internal static DO.Costumer loggetCostumer = null;

        internal static Random rand = new Random();


        public class Config
        {
            public static double avilablePPK = 0.0005; // PPK - Power consumption Per Kilometer. for avilble drone.
            public static double lightPPK = 0.0015; // for drone who carried a light weight.
            public static double mediumPPK = 0.002; // for drone who carried a medium weight.
            public static double heavyPPK = 0.0025; // for drone who carried a heavy weight.

            public static double chargeRatePH = 66.66; // drone charging rate per hour.
        }

        internal static void Initialize()
        {
            InitializeDrone();
            InitializeStation();
            InitializeCostumer();
            InitializeParcel();

            // set the managers
            DO.Costumer manager1 = new DO.Costumer(212830871, "liav", "0524295007",
                new DO.Location(31.778860, 35.203070), "halkadar@g.jct.ac.il", "Aa123456");
            manager1.IsManger = true;
            costumers.Add(manager1);

            DO.Costumer manager2 = new DO.Costumer(324863539, "eyal", "0584312212",
                new DO.Location(31.778860, 35.203070), "eyal@g.jct.ac.il", "Aa123456");
            manager2.IsManger = true;
            costumers.Add(manager2);

            DO.Costumer manager3 = new DO.Costumer(012345678, "test", "0587654321",
                new DO.Location(31.778860, 35.203070), "test@g.jct.ac.il", "Aa123456");
            manager2.IsManger = true;
            costumers.Add(manager2);
        }

        public static void InitializeDrone()
        {
            for (int i = 0; i < 5; i++)
            {
                drones.Add(
                    new DO.Drone(rand.Next(1000, 10001), "Model-X", (DO.WeightCategories) rand.Next(3),
                        rand.NextDouble() * 20 + 20));
            }
        }

        public static void InitializeStation()
        {
            stations.Add(new DO.Station(1010, "Netanya - College",
                new DO.Location(32.30747945219766, 34.87919798038194),
                rand.Next(10)));
            stations.Add(new DO.Station(1020, "The Temple Mount",
                new DO.Location(31.65266801604753, 35.2281960943494),
                rand.Next(10) + 1)); //I don't want stations without a free charge slots.
        }

        public static void InitializeCostumer()
        {
            for (int i = 0; i < 10; ++i)
            {
                costumers.Add(new DO.Costumer(2000 + (i + 1) * 10, "Costumer " + (i + 1).ToString(),
                    "05" + rand.Next(5).ToString() + rand.Next(9999999).ToString(),
                    new DO.Location(30.234196842399772, 48.74692937085842), "Costumer" +
                                                                            (i + 1).ToString() + "@g.jct.ac.il",
                    "Aa123456"));
            }
        }

        public static void InitializeParcel()
        {
            int target = 0, sender = 0;

            bool[] isFreeToShip = new bool[drones.Count];
            for (int i = 0; i < drones.Count; ++i)
            {
                isFreeToShip[i] = true;
            }

            for (int i = 0; i < 10; ++i)
            {
                //generate random sender and random target (they must be different)
                do
                {
                    target = rand.Next(10);
                    sender = rand.Next(10);
                } while (target == sender);

                int droneIndex = rand.Next(5);
                int droneId = 0;

                if (isFreeToShip[droneIndex])
                {
                    droneId = drones[droneIndex].Id;
                    isFreeToShip[droneIndex] = false;
                }

                parcels.Add(new DO.Parcel(rand.Next(50, 100), costumers[sender].Id, costumers[target].Id,
                    (DO.WeightCategories) rand.Next(3),
                    (DO.Priorities) rand.Next(3), DateTime.Now, droneId, null,
                    null, null));
            }
        }

        public static int ParcelsLength()
        {
            return parcels.Count;
        }
    }
}