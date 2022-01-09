using BO;
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
using System.ComponentModel;
using System.Diagnostics;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneView.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        private BlApi.IBL iBL;
        private BO.DroneBL drone;
        private int stationId;

        private BackgroundWorker worker;

        public DroneWindow() //add drone
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.PrevWindow(); };
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();
            AddDrone.Visibility = Visibility.Visible;
            UpdateDrone.Visibility = Visibility.Hidden;
            DroneWeight.ItemsSource = Enum.GetValues(typeof(DO.WeightCategories));
            DroneStation.ItemsSource = this.iBL.GetStationsList(station => true);
            this.drone = null;
            stationId = 0;
        }

        public DroneWindow(object item, int stationId = 0) //update drone (and alsop drone details)
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.PrevWindow(); };
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();

            if (item is BO.DroneListBL)
            {
                this.drone = this.iBL.GetDroneById(((BO.DroneListBL) item).Id);
            }

            else if (item is BO.DroneChargeBL)
            {
                this.drone = this.iBL.GetDroneById(((BO.DroneChargeBL) item).Id);
                this.stationId = stationId;
            }

            else if (item is BO.DroneBL dbl)
            {
                this.drone = dbl;
                this.stationId = stationId;
            }

            AddDrone.Visibility = Visibility.Hidden;
            UpdateDrone.Visibility = Visibility.Visible;

            if(App.IsDroneSimulate(drone.Id))
            {
                PlayButton.Visibility = Visibility.Collapsed;
                StopButton.Visibility = Visibility.Visible;

                UpdateButton.Visibility = Visibility.Collapsed;
                FirstButton.Visibility = Visibility.Collapsed;
                SecondButton.Visibility = Visibility.Collapsed;

                worker = App.GetWorker(drone);
            }

            else
            {
                PlayButton.Visibility = Visibility.Visible;
                StopButton.Visibility = Visibility.Collapsed;

                UpdateButton.Visibility = Visibility.Visible;
                FirstButton.Visibility = Visibility.Visible;
                SecondButton.Visibility = Visibility.Visible;

                worker = null;
            }

            DroneLabel.Content = this.drone;

            if (!iBL.GetLoggedUser().IsManager)
            {
                UpdateButton.Visibility = Visibility.Hidden;
            }

            if (drone.Status == DO.DroneStatuses.Available)
            {
                if (iBL.GetLoggedUser().IsManager)
                {
                    FirstButton.Content = "Send to charge";
                    SecondButton.Content = "Send to delivery";
                }

                else
                {
                    FirstButton.Visibility = Visibility.Hidden;
                    SecondButton.Visibility = Visibility.Hidden;
                }
            }

            else if (drone.Status == DO.DroneStatuses.Maintenance)
            {
                if (iBL.GetLoggedUser().IsManager)
                {
                    FirstButton.Content = "Release from charge";
                    SecondButton.Visibility = Visibility.Hidden;
                }

                else
                {
                    FirstButton.Visibility = Visibility.Hidden;
                    SecondButton.Visibility = Visibility.Hidden;
                }
            }

            else
            {
                FirstButton.Content = "Collect delivery";
                SecondButton.Content = "Deliver parcel";
            }
        }

        private void PlayOnClick(object o, EventArgs e)
        {
            try
            {
                //create worker
                worker = new BackgroundWorker();
                worker.DoWork += Worker_DoWork;
                worker.ProgressChanged += Worker_ProgressChanged;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;

                //save worker and changed buttons vissibilities
                App.AddWorker(drone, worker);
                PlayButton.Visibility = Visibility.Collapsed;
                StopButton.Visibility = Visibility.Visible;

                UpdateButton.Visibility = Visibility.Collapsed;
                FirstButton.Visibility = Visibility.Collapsed;
                SecondButton.Visibility = Visibility.Collapsed;
                MessageBox.Show("Simulator has been starting", "System Message");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StopOnClick(object o, EventArgs e)
        {
            try
            {
                App.StopWorker(drone);
                PlayButton.Visibility = Visibility.Visible;
                StopButton.Visibility = Visibility.Collapsed;

                UpdateButton.Visibility = Visibility.Visible;
                FirstButton.Visibility = Visibility.Visible;
                SecondButton.Visibility = Visibility.Visible;

                MessageBox.Show("Simulator has been stopped", "System Message");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

            MessageBox.Show("Drone added successfully.", "SYSTEM");
            App.PrevWindow();
        }

        private void FirstOnClick(object o, EventArgs e)
        {
            if ((string) FirstButton.Content == "Send to charge")
            {
                try
                {
                    iBL.SendDroneToCharge(drone.Id);
                    FirstButton.Content = "Release from charge";
                    SecondButton.Visibility = Visibility.Hidden;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                }
            }

            else if ((string) FirstButton.Content == "Release from charge")
            {
                try
                {
                    iBL.DroneRelease(drone.Id);
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
                    FirstButton.Visibility = Visibility.Hidden;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                    return;
                }

                MessageBox.Show("The parcel has been collected", "SYSTEM");
            }

            DroneLabel.Content = iBL.GetDroneById(drone.Id);
        }

        private void UpdateOnClick(object sender, EventArgs e)
        {
            string droneModel = drone.Model;

            if (this.drone.IsAvaliable == false)
            {
                MessageBox.Show("ERROR: drone is not avliable.", "ERROR");
            }

            else
            {
                droneModel = Microsoft.VisualBasic.Interaction.InputBox("Enter value: ", "Drone's model", drone.Model);

                iBL.UpdateDroneName(drone.Id, droneModel);
                DroneLabel.Content = iBL.GetDroneById(drone.Id);
            }
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

                FirstButton.Visibility = Visibility.Visible;
                SecondButton.Visibility = Visibility.Visible;

                FirstButton.Content = "Collect delivery";
                SecondButton.Content = "Deliver parcel";

                DroneLabel.Content = iBL.GetDroneById(drone.Id);
            }

            else //Deliver parcel
            {
                try
                {
                    iBL.ParcelDelivered(drone.Id);
                    DroneLabel.Content = iBL.GetDroneById(drone.Id);
                    MessageBox.Show("Parcel delivered successfuly.", "SYSTEM");

                    if (iBL.GetLoggedUser().IsManager)
                    {
                        FirstButton.Visibility = Visibility.Visible;
                        SecondButton.Visibility = Visibility.Visible;
                    }

                    else
                    {
                        App.PrevWindow();
                    }

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

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void Worker_RunWorkerCompleted(object o, RunWorkerCompletedEventArgs e)
        {

        }
    }
}   