﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BlApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window
    {
        private BlApi.IBL iBL;
        private BO.StationBL station;
        private object genericStation;


        public StationWindow() // add station
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.PrevWindow(); };
            this.Closing += App.Window_Closing;

            AddStation.Visibility = Visibility.Visible;
            StationDetails.Visibility = Visibility.Hidden;
            UpdateStation.Visibility = Visibility.Hidden;

            this.iBL = BlFactory.GetBl();
            this.station = null;
        }

        public StationWindow(object item) // station details
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.PrevWindow(); };
            this.Closing += App.Window_Closing;

            AddStation.Visibility = Visibility.Hidden;
            StationDetails.Visibility = Visibility.Visible;
            UpdateStation.Visibility = Visibility.Hidden;
            genericStation = item;
            InitializeView();
        }

        private void InitializeView()
        {
            this.iBL = BlFactory.GetBl();

            BO.StationListBL stationList = null;
            if (genericStation is BO.StationListBL)
            {
                stationList = (BO.StationListBL) genericStation;
                this.station = this.iBL.GetStationById(stationList.Id);
            }

            else if (genericStation is BO.StationBL)
            {
                this.station = (BO.StationBL) genericStation;
            }

            //initalized text blocks
            StationIDView.Text = "ID: " + this.station.Id;
            StationNameView.Text = "Name: " + this.station.Name;
            StationSlotsView.Text = "Number of Free Slots: " + this.station.FreeSlots;
            StationLocationView.Text = "Location: " + this.station.Location;

            //initialized drones ListView
            DronesInStationView.ItemsSource = this.station.DronesInStation;
            SetListViewForeground();

        }

        StationWindow(string name, string FreeSlots, BO.StationBL station) // update station
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.PrevWindow(); };
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();
            this.station = station;

            AddStation.Visibility = Visibility.Hidden;
            StationDetails.Visibility = Visibility.Hidden;
            UpdateStation.Visibility = Visibility.Visible;

            NewStationName.Text = name;
            NewChargeSlots.Text = FreeSlots;
        }

        private void SetListViewForeground()
        {
            //TODO: Try do implement this function (Foreground List View).
            //The function set the foreground of unvaliable items as red.

            //for (int i = 0; i < DronesInStationView.Items.Count; ++i)
            //{
            //    var item = DronesInStationView.ItemContainerGenerator.ContainerFromItem(i) as ListViewItem;

            //    if (((BO.DroneBL)item.Content).IsAvliable == false)
            //    {
            //        item.Foreground = Brushes.Red;
            //    }
            //}
        }

        private void AddOnClick(object o, EventArgs e)
        {
            int id = 0;
            double longitude = 0;
            double lattitude = 0;
            int chargeSlots = 0;
            bool tryToAdd = true;

            //handle id
            if (StationID.Text == "")
            {
                IdError.Text = "Id is missing";
                tryToAdd = false;
            }

            else if (!int.TryParse(StationID.Text, out id))
            {
                IdError.Text = "Id must be integer.";
                tryToAdd = false;
            }

            //handle name
            if (StationName.Text == "")
            {
                NameError.Text = "Name is missing.";
                tryToAdd = false;
            }

            //handle longitude
            if (Longitude.Text == "")
            {
                LongitudeError.Text = "Longitude is missing.";
                tryToAdd = false;
            }

            else if (!double.TryParse(Longitude.Text, out longitude))
            {
                LongitudeError.Text = "Longitude must be number.";
                tryToAdd = false;
            }

            //handle lattiude
            if (Lattitude.Text == "")
            {
                LattitudeError.Text = "Lattitude is missing.";
                tryToAdd = false;
            }

            else if (!double.TryParse(Lattitude.Text, out lattitude))
            {
                LattitudeError.Text = "Lattitude must be number.";
                tryToAdd = false;
            }

            //handle charges slots
            if (ChargeSlots.Text == "")
            {
                ChargeSlotsError.Text = "Charge slots is missing.";
                tryToAdd = false;
            }

            else if (!int.TryParse(ChargeSlots.Text, out chargeSlots))
            {
                ChargeSlotsError.Text = "Charge slots must be integer.";
                tryToAdd = false;
            }

            try
            {
                if (tryToAdd)
                {
                    this.iBL.AddStation(id, StationName.Text, longitude, lattitude, chargeSlots);
                    MessageBox.Show("Station added successfully.", "SYSTEM");
                    App.PrevWindow();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void IdChanged(object o, EventArgs e)
        {
            IdError.Text = "";
        }

        private void NameChanged(object o, EventArgs e)
        {
            NameError.Text = "";
        }

        private void LattitudeChanged(object o, EventArgs e)
        {
            LattitudeError.Text = "";
        }

        private void LongitudeChanged(object o, EventArgs e)
        {
            LongitudeError.Text = "";
        }

        private void ChargeSlotsChanged(object o, EventArgs e)
        {
            ChargeSlotsError.Text = "";
        }

        private void NewNameChanged(object o, EventArgs e)
        {
            ConfirmError.Text = "";
        }

        private void NewChargeSlotsChanged(object o, EventArgs e)
        {
            NewChargeSlotsError.Text = "";
            ConfirmError.Text = "";
        }

        private void DroneView(object o, EventArgs e)
        {
            if (DronesInStationView.SelectedItem != null)
            {
                DroneWindow nextWindow = new DroneWindow(DronesInStationView.SelectedItem, station.Id);
                App.NextWindow(nextWindow);
            }
        }

        private void UpdateOnClick(object o, EventArgs e)
        {
            if (station.IsAvailable == false)
            {
                MessageBox.Show("ERROR: station is not avaliable.", "ERROR");
            }

            else
            {
                StationWindow nextWindow = new StationWindow(station.Name, station.FreeSlots.ToString(), station);
                App.NextWindow(nextWindow, InitializeView);
            }
        }

        private void ConfirmOnClick(object o, EventArgs e)
        {
            int freeSlots = 0;

            if (!int.TryParse(NewChargeSlots.Text, out freeSlots))
            {
                NewChargeSlotsError.Text = "Free slots must be ingeter.";
                return;
            }

            try
            {
                iBL.UpdateStation(station.Id, NewStationName.Text, freeSlots);
                StationNameView.Text = "Name: " + station.Name;
                StationSlotsView.Text = "Number of Free Slots: " + station.FreeSlots;
            }

            catch (Exception ex)
            {
                ConfirmError.Text = ex.Message;
                return;
            }

            App.PrevWindow();
        }
    }
}