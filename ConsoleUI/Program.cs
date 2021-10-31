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
        Paracel
    }

    enum UpdateOptionsE
    {
        AssignParacelToDrone = '1',
        ParacelCollection,
        ParacelDelivered,
        SendDroneToCharge,
        ReleaseDroneFromCharge
    }

    enum DisplayOptionsE
    {
        BaseStation = '1',
        Drone,
        Costumer,
        Paracel
    }

    enum ListViewOptionsE
    {
        BaseStations = '1',
        Drones,
        Costumers,
        Paracels,
        UnassignParacels,
        AvaliableBaseStations
    }

    class Program
    {
        static void Main(string[] args)
        {
            DalObject.DalObject dalObj = new DalObject.DalObject();
            MenuWindowHandle(dalObj);
        }

        static void MenuWindowHandle(DalObject.DalObject dalObj)
        {
            char ch = '0';
            MenuOptionsE op;

            while ((MenuOptionsE)ch != MenuOptionsE.Exit)
            {
                ch = PrintMenu();
                op = (MenuOptionsE)ch;

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
                            break;
                        }
                }

            }
        }

        static void InsertOptionsWindowHandle(DalObject.DalObject dalObj)
        {
            char ch = '0';
            InsertOptionsE op;

            ch = InsertOptions();
            op = (InsertOptionsE)ch;

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

                        if (dalObj.AddStation(id, name, longitude, lattitude, chargeSlots))
                        {
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        else
                        {
                            Console.WriteLine("Failed.\n");
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

                        if (dalObj.AddDrone(id, model, maxWeight, status, battery))
                        {
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        else
                        {
                            Console.WriteLine("Failed.\n");
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

                        if (dalObj.AddCostumer(id, name, phone, longitude, lattitude))
                        {
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        else
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                case InsertOptionsE.Paracel:
                    {
                        int id = 0, senderId = 0, targetId = 0, weight = 0, priority = 0, droneId = 0;
                        DateTime requested = DateTime.Now, scheduled = default(DateTime), pickedUp = default(DateTime), delivered = default(DateTime); //TODO: make sure the default DateTime is ok.

                        Console.WriteLine("Paracel's details:\n");

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

                        if (dalObj.AddParcel(id, senderId, targetId, weight, priority, requested, droneId, scheduled, pickedUp, delivered))
                        {
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        else
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                default:
                    {
                        Console.WriteLine("ERROR: Invalid Choice.\n");
                        break;
                    }
            }
        }

        static void UpdateOptionsWindowHandle(DalObject.DalObject dalObj)
        {
            char ch = '0';
            UpdateOptionsE op;

            ch = UpdateOptions();
            op = (UpdateOptionsE)ch;

            switch (op)
            {
                case UpdateOptionsE.AssignParacelToDrone:
                    {
                        int paracelId = 0;

                        Console.WriteLine("Enter paracel's id: ");
                        paracelId = Convert.ToInt32(Console.ReadLine());

                        if (dalObj.AssignParcelToDrone(paracelId))
                        {
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        else
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                case UpdateOptionsE.ParacelCollection:
                    {
                        int paracelId = 0;

                        Console.WriteLine("Enter paracel's id: ");
                        paracelId = Convert.ToInt32(Console.ReadLine());

                        if (dalObj.ParcelCollection(paracelId))
                        {
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        else
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                case UpdateOptionsE.ParacelDelivered:
                    {
                        int paracelId = 0;

                        Console.WriteLine("Enter paracel's id: ");
                        paracelId = Convert.ToInt32(Console.ReadLine());

                        if (dalObj.ParcelDelivered(paracelId))
                        {
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        else
                        {
                            Console.WriteLine("Failed.\n");
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

                        if (dalObj.SendDroneToCharge(droneId, stationId))
                        {
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        else
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                case UpdateOptionsE.ReleaseDroneFromCharge:
                    {
                        int droneId = 0;

                        Console.WriteLine("Enter drone's id: ");
                        droneId = Convert.ToInt32(Console.ReadLine());

                        if (dalObj.DroneRelease(droneId))
                        {
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        else
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                default:
                    {
                        Console.WriteLine("ERROR: Invalid Choice.\n");
                        break;
                    }
            }
        }

        static void DisplayOptionsWindowHandle(DalObject.DalObject dalObj)
        {
            char ch = '0';
            DisplayOptionsE op;

            ch = DisplayOptions();
            op = (DisplayOptionsE)ch;

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
                            station = dalObj._getStationById(stationId);

                            Console.WriteLine(station);
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        catch (ArgumentException e)
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                case DisplayOptionsE.Drone:
                    {
                        int droneId = 0;
                        IDAL.DO.Drone drone;

                        Console.Write("Enter drone's Id: ");
                        droneId = Convert.ToInt32(Console.ReadLine()); ;

                        try
                        {
                            drone = dalObj._getDroneById(droneId);

                            Console.WriteLine(drone);
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        catch (ArgumentException e)
                        {
                            Console.WriteLine("Failed.\n");
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
                            costumer = dalObj._getCostumerById(costumerId);

                            Console.WriteLine(costumer);
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        catch (ArgumentException e)
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                case DisplayOptionsE.Paracel:
                    {
                        int paracelId = 0;
                        IDAL.DO.Parcel parcel;

                        Console.Write("Enter paracel's Id: ");
                        paracelId = Convert.ToInt32(Console.ReadLine());

                        try
                        {
                            parcel = dalObj._getParcelById(paracelId);

                            Console.WriteLine(parcel);
                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        catch (ArgumentException e)
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                default:
                    {
                        Console.WriteLine("ERROR: Invalid Choice.\n");
                        break;
                    }
            }
        }

        static void ListViewOptionsWindowHandle(DalObject.DalObject dalObj)
        {
            char ch = '0';
            ListViewOptionsE op;

            ch = ListViewOptions();
            op = (ListViewOptionsE)ch;

            switch (op)
            {
                case ListViewOptionsE.BaseStations:
                    {
                        try
                        {
                            foreach(var station in dalObj._getStationList())
                            {
                                Console.WriteLine(station);
                            }

                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                case ListViewOptionsE.Drones:
                    {
                        try
                        {
                            foreach(var drone in dalObj._getDroneList())
                            {
                                Console.WriteLine(drone);
                            }

                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                case ListViewOptionsE.Costumers:
                    {
                        try
                        {
                            foreach(var costumer in dalObj._getCostumerList())
                            {
                                Console.WriteLine(costumer);
                            }

                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                case ListViewOptionsE.Paracels:
                    {
                        try
                        {
                            foreach(var parcel in dalObj._getParceList())
                            {
                                Console.WriteLine(parcel);
                            }

                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                case ListViewOptionsE.UnassignParacels:
                    {
                        IDAL.DO.Parcel parcel;
                        int i = dalObj._getWaitingParcels().Count;

                        if(i == 0)
                        {
                            Console.WriteLine("List is empty.\n");
                        }

                        try
                        {
                            while(i != 0)
                            {
                                parcel = dalObj._getWaitingParcels().Dequeue();
                                Console.WriteLine(parcel); // print from top to bottom
                                dalObj._getWaitingParcels().Enqueue(parcel);
                                i--;
                            }

                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                case ListViewOptionsE.AvaliableBaseStations:
                    {
                        try
                        {
                            foreach(var station in dalObj._getStationList())
                            {
                                if (station.ChargeSolts >= 1) { Console.WriteLine(station); }
                            }

                            Console.WriteLine("Success.\n");
                            pause();
                        }

                        catch(Exception e)
                        {
                            Console.WriteLine("Failed.\n");
                            pause();
                        }

                        break;
                    }

                default:
                    {
                        Console.WriteLine("ERROR: Invalid Choice.\n");
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
            Console.WriteLine("4. Add new paracel.");

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
            Console.WriteLine("1. Assign a paracel to a drone.");
            Console.WriteLine("2. Paracel collection by drone.");
            Console.WriteLine("3. Delivery paracel to Costumer.");
            Console.WriteLine("4. Sending a drone for charging at a base station.");
            Console.WriteLine("5. Release drone from charging at base station.");

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
            Console.WriteLine("4. Paracel view. (by unique id).");

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
            Console.WriteLine("4. Display a list of paracels.");
            Console.WriteLine("5. Displays a list of paracels that have not yet been assigned to a drone.");
            Console.WriteLine("6. Display of base stations with available charging stations.");


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
