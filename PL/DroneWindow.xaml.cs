﻿using BO;
using System;
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
    /// Interaction logic for DroneView.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        private BlApi.IBL iBL;
        private BO.DroneListBL drone;
        ListView listViewDrones;
        bool isReturnButtonUsed = false;

        public DroneWindow(object listViewDrones)
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();
            AddDrone.Visibility = Visibility.Visible;
            UpdateDrone.Visibility = Visibility.Hidden;
            DroneWeight.ItemsSource = Enum.GetValues(typeof(DO.WeightCategories));
            DroneStation.ItemsSource = this.iBL.GetStationsList(station => true);
            this.listViewDrones = (ListView) listViewDrones;
            this.drone = null;
        }

        public DroneWindow(object item, object listViewDrones)
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();
            this.drone = (BO.DroneListBL) item;
            AddDrone.Visibility = Visibility.Hidden;
            UpdateDrone.Visibility = Visibility.Visible;
            DroneLabel.Content = this.iBL.GetDroneById(this.drone.Id);

            if (drone.Status == DO.DroneStatuses.Available)
            {
                FirstButton.Content = "Send to charge";
                SecondButton.Content = "Send to delivery";
            }

            else if (drone.Status == DO.DroneStatuses.Maintenance)
            {
                FirstButton.Content = "Release from charge";
                SecondButton.Visibility = Visibility.Hidden;
            }

            else
            {
                FirstButton.Content = "Collect delivery";
                SecondButton.Content = "Deliver parcel";
            }

            this.listViewDrones = (ListView) listViewDrones;
        }

        private void AddOnClick(object o, EventArgs e)
        {
            if (DroneWeight.SelectedItem == null || DroneStation.SelectedItem == null || DroneID.Text == "" ||
                DroneModel.Text == "")
            {
                MessageBox.Show("There are missing values !!", "WARNING");
                return;
            }

            string droneId_Str = DroneID.Text;
            int droneId;

            if (!int.TryParse(droneId_Str, out droneId))
            {
                MessageBox.Show("Drone's id must be integer !!", "ERROR");
                return;
            }

            string droneModel = DroneModel.Text;
            DO.WeightCategories droneMaxWeight = (DO.WeightCategories) DroneWeight.SelectedItem;
            BO.StationListBL station = (BO.StationListBL) DroneStation.SelectedItem;

            try
            {
                iBL.AddDrone(droneId, droneModel, (int) droneMaxWeight, station.Id);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
                return;
            }

            MessageBox.Show("Drone added successful.", "SYSTEM");
            App.ShowWindow<DronesListWindow>();
        }

        private void ReturnOnClick(object o, EventArgs e)
        {
            isReturnButtonUsed = true;
            App.ShowWindow<DronesListWindow>();
        }

        private void FirstOnClick(object o, EventArgs e)
        {
            if ((string) FirstButton.Content == "Send to charge")
            {
                iBL.SendDroneToCharge(drone.Id); //TODO: Add time variable to DroneCharge in DAL
                FirstButton.Content = "Release from charge";
                SecondButton.Visibility = Visibility.Hidden;
            }

            else if ((string) FirstButton.Content == "Release from charge")
            {
                string timeStr = "";
                int time = 0;

                timeStr = Microsoft.VisualBasic.Interaction.InputBox("Enter value: ", "Charge time", "0");

                if (!int.TryParse(timeStr, out time))
                {
                    MessageBox.Show("Charge time must be integer !!", "ERROR");
                    return;
                }

                try
                {
                    iBL.DroneRelease(drone.Id, time); //TODO: After add time variable to DroneChare, we dont neet time parameter.
                                                      //The time can be easliy calculate by current time - started time (make sure it's in hours).
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                    return;
                }

                FirstButton.Content = "Send to charge";
                SecondButton.Content = "Send to delivery";
                SecondButton.Visibility = Visibility.Visible;
            }

            else //Collect delivery
            {
                try
                {
                    iBL.ParcelCollection(drone.Id);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                    return;
                }

                MessageBox.Show("The parcel has been collected", "SYSTEM");
            }

            DroneLabel.Content = iBL.GetDroneById(drone.Id);
            listViewDrones.ItemsSource = iBL.GetDroneList(drone => true);
        }

        private void UpdateOnClick(object sender, EventArgs e)
        {
            string droneModel = drone.Model;

            droneModel = Microsoft.VisualBasic.Interaction.InputBox("Enter value: ", "Drone's model", drone.Model);

            if (droneModel == "")
            {
                MessageBox.Show("Missing value", "WARNING");
                return;
            }

            iBL.UpdateDroneName(drone.Id, droneModel);
            DroneLabel.Content = iBL.GetDroneById(drone.Id);
            listViewDrones.ItemsSource = iBL.GetDroneList(drone => true);
        }

        private void SecondOnClick(object o, EventArgs e)
        {
            if ((string) SecondButton.Content == "Send to delivery")
            {
                try
                {
                    iBL.AssignParcelToDrone(drone.Id);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                    return;
                }

                FirstButton.Content = "Collect delivery";
                SecondButton.Content = "Deliver parcel";
            }

            else //Deliver parcel
            {
                try
                {
                    iBL.ParcelDelivered(drone.Id);
                    DroneLabel.Content = iBL.GetDroneById(drone.Id);
                    listViewDrones.ItemsSource = iBL.GetDroneList(drone => true);
                    FirstButton.Content = "Send to charge";
                    SecondButton.Content = "Send to delivery";
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                    return;
                }
            }
        }

        private void Window_Closing(object o, System.ComponentModel.CancelEventArgs e)
        {
            if (!isReturnButtonUsed)
            {
                e.Cancel = true;
            }
        }
    }
}