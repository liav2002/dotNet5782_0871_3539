using System;
using System.Collections.Generic;

namespace DalObject
{
    public class DataSource
    {
        internal static IDAL.DO.Drone[] drones = new IDAL.DO.Drone[10];
        internal static IDAL.DO.Station[] stations = new IDAL.DO.Station[5];
        internal static IDAL.DO.Costumer[] costumers = new IDAL.DO.Costumer[100];
        internal static IDAL.DO.Parcel[] parcels = new IDAL.DO.Parcel[1000];
        internal static List<IDAL.DO.DroneCharge> droneCharge = new List<IDAL.DO.DroneCharge>();

        internal static Queue<IDAL.DO.Parcel> waitingParcels =
            new Queue<IDAL.DO.Parcel>();


        internal static Random rand = new Random();

        internal class Config
        {
            internal static int indexDrone = 0;
            internal static int indexStation = 0;
            internal static int indexCostumer = 0;
            internal static int indexParcel = 0;
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
                drones[Config.indexDrone] = new IDAL.DO.Drone()
                {
                    Battery = rand.Next(101),
                    Id = rand.Next(1000, 10001),
                    Model = "Model-X",
                    Status = (IDAL.DO.DroneStatuses)rand.Next(3),
                    MaxWeight = (IDAL.DO.WeightCategories)rand.Next(3)
                };

                Config.indexDrone++;
            }
        }

        public static void InitializeStation()
        {
            stations[0] = new IDAL.DO.Station()
            {
                Id = 1010,
                Name = "Netanya - College",
                Latitude = 32.30747945219766,
                Longitude = 34.87919798038194,
                ChargeSolts = rand.Next(10),
            };

            stations[1] = new IDAL.DO.Station()
            {
                Id = 1020,
                Name = "The Temple Mount",
                Latitude = 31.65266801604753,
                Longitude = 35.2281960943494,
                ChargeSolts = rand.Next(10),
            };

            Config.indexStation = 2;
        }

        public static void InitializeCostumer()
        {
            for (int i = 0; i < 10; ++i)
            {
                costumers[Config.indexCostumer] = new IDAL.DO.Costumer()
                {
                    Id = 2000 + (i + 1) * 10,
                    Name = "Costumer " + (i + 1).ToString(),
                    Phone = "05" + rand.Next(5).ToString() + "-" + rand.Next(999).ToString() + "-" +
                            rand.Next(9999).ToString(),
                    Longitube = 30.234196842399772,
                    Latitude = 48.74692937085842,
                };

                Config.indexCostumer++;
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

                parcels[Config.indexParcel] = new IDAL.DO.Parcel()
                {
                    Id = rand.Next(50, 100),
                    SenderId = costumers[sender].Id,
                    TargetId = costumers[target].Id,
                    Weight = (IDAL.DO.WeightCategories)rand.Next(3),
                    Priority = (IDAL.DO.Priorities)rand.Next(3),
                    Requsted = DateTime.Now,
                    Scheduled = DateTime.Now,
                    PickedUp = DateTime.Now,
                    Delivered = DateTime.Now,
                    DroneId = drones[rand.Next(5)].Id,
                };
            }
        }
    }
}