﻿using System;

namespace ConsoleUI_BL
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
            Console.WriteLine("Building BL Unit...\n");
            IBL.IBL iBL = new IBL.BO.BL();
            pause();
            MenuWindowHandle(iBL);
        }

        static void MenuWindowHandle(IBL.IBL iBL)
        {
            char ch = '0';
            MenuOptionsE op;

            while ((MenuOptionsE) ch != MenuOptionsE.Exit)
            {
                op = PrintMenu();

                switch (op)
                {
                    case MenuOptionsE.Insert:
                    {
                        InsertOptionsWindowHandle(iBL);
                        break;
                    }

                    case MenuOptionsE.Update:
                    {
                        UpdateOptionsWindowHandle(iBL);
                        break;
                    }

                    case MenuOptionsE.Display:
                    {
                        DisplayOptionsWindowHandle(iBL);
                        break;
                    }

                    case MenuOptionsE.ListView:
                    {
                        ListViewOptionsWindowHandle(iBL);
                        break;
                    }

                    case MenuOptionsE.Exit:
                    {
                        Console.WriteLine("Good bye !!\n");
                        return;
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

        static void InsertOptionsWindowHandle(IBL.IBL iBL)
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
                    double longitude = 0, latitude = 0;

                    Console.WriteLine("Base station's details:\n");

                    Console.WriteLine("Id: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Name: ");
                    name = Console.ReadLine();
                    Console.WriteLine("lattiude: ");
                    latitude = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("longitude: ");
                    longitude = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Charge's Slots: ");
                    chargeSlots = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        iBL.AddStation(id, name, longitude, latitude, chargeSlots);
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
                    int stationId = 0;

                    Console.WriteLine("Drone's details:\n");

                    Console.WriteLine("Id: ");
                    id = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Model: ");
                    model = Console.ReadLine();
                    Console.WriteLine("Maximum weight to drag (0 - Light, 1 - Medium, 2 - Heavy): ");
                    maxWeight = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Station id to charge: ");
                    stationId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        iBL.AddDrone(id, model, maxWeight, stationId);
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
                    Console.WriteLine("longitude: ");
                    longitude = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Lattitude: ");
                    lattitude = Convert.ToDouble(Console.ReadLine());

                    try
                    {
                        iBL.AddCostumer(id, name, phone, longitude, lattitude);
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
                    int senderId = 0, targetId = 0, weight = 0, priority = 0, droneId = 0;

                    Console.WriteLine("Parcel's details:\n");

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
                        iBL.AddParcel(senderId, targetId, weight, priority, droneId);
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

        static void UpdateOptionsWindowHandle(IBL.IBL iBL)
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
                        iBL.AssignParcelToDrone(parcelId);
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

                case UpdateOptionsE.ParcelCollection:
                {
                    int parcelId = 0;

                    Console.WriteLine("Enter parcel's id: ");
                    parcelId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        iBL.ParcelCollection(parcelId);
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

                case UpdateOptionsE.ParcelDelivered:
                {
                    int parcelId = 0;

                    Console.WriteLine("Enter parcel's id: ");
                    parcelId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        iBL.ParcelDelivered(parcelId);
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
                        iBL.SendDroneToCharge(droneId, stationId);
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

                case UpdateOptionsE.ReleaseDroneFromCharge:
                {
                    int droneId = 0;

                    Console.WriteLine("Enter drone's id: ");
                    droneId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        iBL.DroneRelease(droneId);
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

        static void DisplayOptionsWindowHandle(IBL.IBL iBL)
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
                        station = iBL.GetStationById(stationId);

                        Console.WriteLine(station);
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

                case DisplayOptionsE.Drone:
                {
                    int droneId = 0;
                    IDAL.DO.Drone drone;

                    Console.Write(" Enter drone's Id: ");
                    droneId = Convert.ToInt32(Console.ReadLine());
                    ;

                    try
                    {
                        drone = iBL.GetDroneById(droneId);

                        Console.WriteLine(drone);
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

                case DisplayOptionsE.Costumer:
                {
                    int costumerId = 0;
                    IDAL.DO.Costumer costumer;

                    Console.Write("Enter costumer's Id: ");
                    costumerId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        costumer = iBL.GetCostumerById(costumerId);

                        Console.WriteLine(costumer);
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

                case DisplayOptionsE.Parcel:
                {
                    int parcelId = 0;
                    IDAL.DO.Parcel parcel;

                    Console.Write("Enter parcel's Id: ");
                    parcelId = Convert.ToInt32(Console.ReadLine());

                    try
                    {
                        parcel = iBL.GetParcelById(parcelId);

                        Console.WriteLine(parcel);
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

        static void ListViewOptionsWindowHandle(IBL.IBL iBL)
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
                        foreach (var station in iBL.GetStationsList())
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
                        foreach (var drone in iBL.GetDroneList())
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
                        foreach (var costumer in iBL.GetCostumerList())
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
                        foreach (var parcel in iBL.GetParcelsList())
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
                    int i = iBL.GetWaitingParcels().Count;

                    if (i == 0)
                    {
                        Console.WriteLine("List is empty.\n");
                    }

                    try
                    {
                        while (i != 0)
                        {
                            parcel = iBL.GetWaitingParcels().Dequeue();
                            Console.WriteLine(parcel); // print from top to bottom
                            iBL.GetWaitingParcels().Enqueue(parcel);
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
                        foreach (var station in iBL.GetStationsList())
                        {
                            if (station.ChargeSolts >= 1)
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

        static MenuOptionsE PrintMenu()
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

            return (MenuOptionsE) userChoice;
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