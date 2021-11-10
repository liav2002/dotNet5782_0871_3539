using System;
using IDAL.DO;

namespace ConsoleUI
{
    enum MenuOptionsE
    {
        Insert = '1',
        Update,
        Display,
        ListView,
        Exit
    }

    enum InsertOptionsE
    {
        BaseStation = '1',
        Drone,
        Costumer,
        Parcel,
        Back
    }

    enum UpdateOptionsE
    {
        AssignParcelToDrone = '1',
        ParcelCollection,
        ParcelDelivered,
        SendDroneToCharge,
        ReleaseDroneFromCharge,
        Back
    }

    enum DisplayOptionsE
    {
        BaseStation = '1',
        Drone,
        Costumer,
        Parcel,
        Back
    }

    enum ListViewOptionsE
    {
        BaseStations = '1',
        Drones,
        Costumers,
        Parcels,
        UnassignParcels,
        AvaliableBaseStations,
        Back
    }

    class Program
    {
        static void Main(string[] args)
        {
            IDAL.IDAL dalObj = DalObject.DalObject.GetInstance(); // Singleton
            MenuWindowHandle(dalObj);
        }

        static void MenuWindowHandle(IDAL.IDAL dalObj)
        {
            char ch = '0';
            MenuOptionsE op;

            while ((MenuOptionsE) ch != MenuOptionsE.Exit)
            {
                ch = PrintMenu();
                op = (MenuOptionsE) ch;

                switch (op)
                {
                    case MenuOptionsE.Insert:
                    {
                        InsertOptionsWindowHandle(dalObj);
                        break;
                    }

                    case MenuOptionsE.Update:
                    {
                        UpdateOptionsWindowHandle(dalObj);
                        break;
                    }

                    case MenuOptionsE.Display:
                    {
                        DisplayOptionsWindowHandle(dalObj);
                        break;
                    }

                    case MenuOptionsE.ListView:
                    {
                        ListViewOptionsWindowHandle(dalObj);
                        break;
                    }

                    case MenuOptionsE.Exit:
                    {
                        Console.WriteLine("Good bye !!\n");
                        break;
                    }

                    default:
                    {
                        Console.WriteLine("ERROR: Invalid Choice.\n");
                        pause();
                        break;
                    }
                }
            }
        }

        static void InsertOptionsWindowHandle(IDAL.IDAL dalObj)
        {
            char ch = '0';
            InsertOptionsE op;

            ch = InsertOptions();
            op = (InsertOptionsE) ch;

            switch (op)
            {
                case InsertOptionsE.BaseStation:
                {
                    int id = 0, chargeSlots = 0;
                    string name = "";
                    double longitude = 0, lattitude = 0;

                    Console.WriteLine("Base station's details:\n");

                    Console.WriteLine("Id: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Name: ");
                    name = Console.ReadLine();
                    Console.WriteLine("Longitude: ");
                    longitude = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Lattiude: ");
                    lattitude = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Charge's Slots: ");
                    chargeSlots = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        dalObj.AddStation(id, name, new IDAL.DO.Location(lattitude, longitude), chargeSlots);
                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case InsertOptionsE.Drone:
                {
                    int id = 0;
                    string model = "";
                    int maxWeight = 0;
                    int status = 0;
                    double battery = 0;

                    Console.WriteLine("Drone's details:\n");

                    Console.WriteLine("Id: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Model: ");
                    model = Console.ReadLine();
                    Console.WriteLine("Maximum weight to drag (0 - Light, 1 - Medium, 2 - Heavy): ");
                    maxWeight = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Status (0 - Avilable, 1 - Maintenance, 2 - Shipping): ");
                    status = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Battery: ");
                    battery = Convert.ToDouble(Console.ReadLine());

                    try
                    {
                        dalObj.AddDrone(id, model, (IDAL.DO.WeightCategories) maxWeight, battery);
                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case InsertOptionsE.Costumer:
                {
                    int id = 0;
                    string name = "";
                    string phone = "";
                    double longitude = 0;
                    double lattitude = 0;

                    Console.WriteLine("Costumer's details:\n");

                    Console.WriteLine("Id: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Name: ");
                    name = Console.ReadLine();
                    Console.WriteLine("Phone: ");
                    phone = Console.ReadLine();
                    Console.WriteLine("Longitude: ");
                    longitude = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Lattitude: ");
                    lattitude = Convert.ToDouble(Console.ReadLine());

                    try
                    {
                        dalObj.AddCostumer(id, name, phone, new IDAL.DO.Location(lattitude, longitude));
                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case InsertOptionsE.Parcel:
                {
                    int id = 0, senderId = 0, targetId = 0, weight = 0, priority = 0, droneId = 0;
                    DateTime requested = DateTime.Now,
                        scheduled = default(DateTime),
                        pickedUp = default(DateTime),
                        delivered = default(DateTime);

                    Console.WriteLine("Parcel's details:\n");

                    Console.WriteLine("Id: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Sender's id: ");
                    senderId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Target's id: ");
                    targetId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Weight (0 - Light, 1 - Medium, 2 - Heavy): ");
                    weight = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Priority (0 - Regular, 1 - Fast, 2 - Emergency): ");
                    priority = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        dalObj.AddParcel(id, senderId, targetId, weight, priority, requested, droneId, scheduled,
                            pickedUp, delivered);
                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case InsertOptionsE.Back:
                {
                    break;
                }

                default:
                {
                    Console.WriteLine("ERROR: Invalid Choice.\n");
                    pause();
                    break;
                }
            }
        }

        static void UpdateOptionsWindowHandle(IDAL.IDAL dalObj)
        {
            char ch = '0';
            UpdateOptionsE op;

            ch = UpdateOptions();
            op = (UpdateOptionsE) ch;

            switch (op)
            {
                case UpdateOptionsE.AssignParcelToDrone:
                {
                    int parcelId = 0;

                    Console.WriteLine("Enter parcel's id: ");
                    parcelId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        //dalObj.AssignParcelToDrone(parcelId);
                        throw new Exception(
                            "BL unit is missing. can't assign a drone to parcel. update version required.");
                        //Console.WriteLine("Success.\n");
                        //pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case UpdateOptionsE.ParcelCollection:
                {
                    int parcelId = 0;

                    Console.WriteLine("Enter parcel's id: ");
                    parcelId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        //dalObj.ParcelCollection(parcelId);
                        throw new Exception(
                            "BL unit is missing. can't assign a drone to parcel. update version required.");
                        //Console.WriteLine("Success.\n");
                        //pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case UpdateOptionsE.ParcelDelivered:
                {
                    int parcelId = 0;

                    Console.WriteLine("Enter parcel's id: ");
                    parcelId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        //dalObj.ParcelDelivered(parcelId);
                        throw new Exception(
                            "BL unit is missing. can't assign a drone to parcel. update version required.");
                        //Console.WriteLine("Success.\n");
                        //pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case UpdateOptionsE.SendDroneToCharge:
                {
                    int droneId = 0;
                    int stationId = 0;

                    Console.WriteLine("Enter drone's id: ");
                    droneId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter station's id: ");
                    stationId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        //dalObj.SendDroneToCharge(droneId, stationId);
                        throw new Exception(
                            "BL unit is missing. can't assign a drone to parcel. update version required.");
                        //Console.WriteLine("Success.\n");
                        //pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case UpdateOptionsE.ReleaseDroneFromCharge:
                {
                    int droneId = 0;

                    Console.WriteLine("Enter drone's id: ");
                    droneId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        dalObj.DroneRelease(droneId);
                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case UpdateOptionsE.Back:
                {
                    break;
                }

                default:
                {
                    Console.WriteLine("ERROR: Invalid Choice.\n");
                    pause();
                    break;
                }
            }
        }

        static void DisplayOptionsWindowHandle(IDAL.IDAL dalObj)
        {
            char ch = '0';
            DisplayOptionsE op;

            ch = DisplayOptions();
            op = (DisplayOptionsE) ch;

            switch (op)
            {
                case DisplayOptionsE.BaseStation:
                {
                    int stationId = 0;
                    IDAL.DO.Station station;

                    Console.Write("Enter station's Id: ");
                    stationId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        station = dalObj.GetStationById(stationId);

                        Console.WriteLine(station);
                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case DisplayOptionsE.Drone:
                {
                    int droneId = 0;
                    IDAL.DO.Drone drone;

                    Console.Write("Enter drone's Id: ");
                    droneId = Convert.ToInt32(Console.ReadLine());
                    ;

                    try
                    {
                        drone = dalObj.GetDroneById(droneId);

                        Console.WriteLine(drone);
                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case DisplayOptionsE.Costumer:
                {
                    int costumerId = 0;
                    IDAL.DO.Costumer costumer;

                    Console.Write("Enter costumer's Id: ");
                    costumerId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        costumer = dalObj.GetCostumerById(costumerId);

                        Console.WriteLine(costumer);
                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case DisplayOptionsE.Parcel:
                {
                    int parcelId = 0;
                    IDAL.DO.Parcel parcel;

                    Console.Write("Enter parcel's Id: ");
                    parcelId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        parcel = dalObj.GetParcelById(parcelId);

                        Console.WriteLine(parcel);
                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case DisplayOptionsE.Back:
                {
                    break;
                }

                default:
                {
                    Console.WriteLine("ERROR: Invalid Choice.\n");
                    pause();
                    break;
                }
            }
        }

        static void ListViewOptionsWindowHandle(IDAL.IDAL dalObj)
        {
            char ch = '0';
            ListViewOptionsE op;

            ch = ListViewOptions();
            op = (ListViewOptionsE) ch;

            switch (op)
            {
                case ListViewOptionsE.BaseStations:
                {
                    try
                    {
                        foreach (var station in dalObj.GetStationsList())
                        {
                            Console.WriteLine(station);
                        }

                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case ListViewOptionsE.Drones:
                {
                    try
                    {
                        foreach (var drone in dalObj.GetDroneList())
                        {
                            Console.WriteLine(drone);
                        }

                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case ListViewOptionsE.Costumers:
                {
                    try
                    {
                        foreach (var costumer in dalObj.GetCostumerList())
                        {
                            Console.WriteLine(costumer);
                        }

                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case ListViewOptionsE.Parcels:
                {
                    try
                    {
                        foreach (var parcel in dalObj.GetParcelsList())
                        {
                            Console.WriteLine(parcel);
                        }

                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case ListViewOptionsE.UnassignParcels:
                {
                    IDAL.DO.Parcel parcel;
                    int i = dalObj.GetWaitingParcels().Count;

                    if (i == 0)
                    {
                        Console.WriteLine("List is empty.\n");
                    }

                    try
                    {
                        while (i != 0)
                        {
                            parcel = dalObj.GetWaitingParcels().Dequeue();
                            Console.WriteLine(parcel); // print from top to bottom
                            dalObj.GetWaitingParcels().Enqueue(parcel);
                            i--;
                        }

                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case ListViewOptionsE.AvaliableBaseStations:
                {
                    try
                    {
                        foreach (var station in dalObj.GetStationsList())
                        {
                            if (station.ChargeSlots >= 1)
                            {
                                Console.WriteLine(station);
                            }
                        }

                        Console.WriteLine("Success.\n");
                        pause();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Failed.\n");
                        Console.WriteLine(e.Message);
                        pause();
                    }

                    break;
                }

                case ListViewOptionsE.Back:
                {
                    break;
                }

                default:
                {
                    Console.WriteLine("ERROR: Invalid Choice.\n");
                    pause();
                    break;
                }
            }
        }

        static char PrintMenu()
        {
            char userChoice = '0';

            Console.Clear();

            Console.WriteLine("Delivery Service Information System. (Demo-Version).");
            Console.WriteLine("By Liav Ariel and Eyal Seckbach.\n");

            Console.WriteLine("Menu-Window\n");

            Console.WriteLine("Your options:");
            Console.WriteLine("1. Insert options.");
            Console.WriteLine("2. Update options.");
            Console.WriteLine("3. Display options.");
            Console.WriteLine("4. List view options.");
            Console.WriteLine("5. Exit.");

            userChoice = Console.ReadLine()[0];

            return userChoice;
        }

        static char InsertOptions()
        {
            char userChoice = '0';

            Console.Clear();

            Console.WriteLine("Delivery Service Information System. (Demo-Version).");
            Console.WriteLine("By Liav Ariel and Eyal Seckbach.\n");

            Console.WriteLine("Insertion-Options-Window\n");

            Console.WriteLine("Your options:");
            Console.WriteLine("1. Add new base station.");
            Console.WriteLine("2. Add new drone.");
            Console.WriteLine("3. Add new costumer.");
            Console.WriteLine("4. Add new parcel.");
            Console.WriteLine("5. Back.");

            userChoice = Console.ReadLine()[0];

            return userChoice;
        }

        static char UpdateOptions()
        {
            char userChoice = '0';

            Console.Clear();

            Console.WriteLine("Delivery Service Information System. (Demo-Version).");
            Console.WriteLine("By Liav Ariel and Eyal Seckbach.\n");

            Console.WriteLine("Update-Options-Window\n");

            Console.WriteLine("Your options:");
            Console.WriteLine("1. Assign a parcel to a drone.");
            Console.WriteLine("2. Parcel collection by drone.");
            Console.WriteLine("3. Delivery parcel to Costumer.");
            Console.WriteLine("4. Sending a drone for charging at a base station.");
            Console.WriteLine("5. Release drone from charging at base station.");
            Console.WriteLine("6. Back.");

            userChoice = Console.ReadLine()[0];

            return userChoice;
        }

        static char DisplayOptions()
        {
            char userChoice = '0';

            Console.Clear();

            Console.WriteLine("Delivery Service Information System. (Demo-Version).");
            Console.WriteLine("By Liav Ariel and Eyal Seckbach.\n");

            Console.WriteLine("Display-Options-Window\n");

            Console.WriteLine("Your options:");
            Console.WriteLine("1. Base station view. (by unique id).");
            Console.WriteLine("2. Drone view. (by unique id).");
            Console.WriteLine("3. Costumer view. (by unique id).");
            Console.WriteLine("4. Parcel view. (by unique id).");
            Console.WriteLine("5. Back.");

            userChoice = Console.ReadLine()[0];

            return userChoice;
        }

        static char ListViewOptions()
        {
            char userChoice = '0';

            Console.Clear();

            Console.WriteLine("Delivery Service Information System. (Demo-Version).");
            Console.WriteLine("By Liav Ariel and Eyal Seckbach.\n");

            Console.WriteLine("List-View-Options-Window\n");

            Console.WriteLine("Your options:");
            Console.WriteLine("1. Displays a list of base stations.");
            Console.WriteLine("2. Display a list of drones.");
            Console.WriteLine("3. Display a list of costumers.");
            Console.WriteLine("4. Display a list of parcels.");
            Console.WriteLine("5. Displays a list of parcels that have not yet been assigned to a drone.");
            Console.WriteLine("6. Display of base stations with available charging stations.");
            Console.WriteLine("7. Back.");


            userChoice = Console.ReadLine()[0];

            return userChoice;
        }

        static void pause()
        {
            Console.WriteLine("Press any key to continue . . .\n");
            Console.ReadKey();
        }
    }
}