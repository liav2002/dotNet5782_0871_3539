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

        public DroneWindow()
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();
            AddDrone.Visibility = Visibility.Visible;
            UpdateDrone.Visibility = Visibility.Hidden;
            DroneWeight.ItemsSource = Enum.GetValues(typeof(DO.WeightCategories));
            DroneStation.ItemsSource = this.iBL.GetStationsList(station => true);
            this.drone = null;
            stationId = 0;
        }

        public DroneWindow(object item, int stationId = 0)
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();

            if (item is BO.DroneListBL)
            {
                this.drone = this.iBL.GetDroneById(((BO.DroneListBL)item).Id);
            }

            else if (item is BO.DroneChargeBL)
            {
                this.drone = this.iBL.GetDroneById(((BO.DroneChargeBL)item).Id);
                this.stationId = stationId;
            }

            AddDrone.Visibility = Visibility.Hidden;
            UpdateDrone.Visibility = Visibility.Visible;
            DroneLabel.Content = this.drone;

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
            App.ShowWindow<DronesListWindow>();
        }

        private void ReturnOnClick(object o, EventArgs e)
        {
            if(stationId == 0)
            {
                App.ShowWindow<DronesListWindow>();
            }

            else
            {
                StationWindow nextWindow = new StationWindow(this.iBL.GetStationById(stationId));
                App.ShowWindow(nextWindow);
            }
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

                catch(Exception ex)
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

            if(this.drone.IsAvaliable == false)
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

                FirstButton.Content = "Collect delivery";
                SecondButton.Content = "Deliver parcel";
            }

            else //Deliver parcel
            {
                try
                {
                    iBL.ParcelDelivered(drone.Id);
                    DroneLabel.Content = iBL.GetDroneById(drone.Id);
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
    }
}