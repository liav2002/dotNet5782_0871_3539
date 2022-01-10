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
using System.Threading;

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
        private bool _shouldStop;
        private int _secondsToSleep;
        private string _stringToLogger;
        private int _loggerCounter;

        public DroneWindow() //add drone
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();
            AddDrone.Visibility = Visibility.Visible;
            UpdateDrone.Visibility = Visibility.Hidden;
            DroneWeight.ItemsSource = Enum.GetValues(typeof(DO.WeightCategories));
            DroneStation.ItemsSource = this.iBL.GetStationsList(station => true);
            
            //unnecessary variables
            this.drone = null;
            stationId = 0;
            _shouldStop = true;
            _secondsToSleep = 0;
            _stringToLogger = "";
            _loggerCounter = 0;
            worker = null;
        }

        public DroneWindow(object item, int stationId = 0) //update drone (and alsop drone details)
        {
            InitializeComponent();
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

            PlayButton.Visibility = Visibility.Visible;
            StopButton.Visibility = Visibility.Collapsed;

            UpdateButton.Visibility = Visibility.Visible;
            FirstButton.Visibility = Visibility.Visible;
            SecondButton.Visibility = Visibility.Visible;

            _shouldStop = true;
            worker = null;
            _secondsToSleep = 3;
            _stringToLogger = "";
            _loggerCounter = 0;

            //drone detatils:
            updateDroeDetailsPL();

            if (!iBL.GetLoggedUser().IsManager)
            {
                UpdateButton.Visibility = Visibility.Hidden;
                PlayButton.Visibility = Visibility.Hidden;
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

            TimerToStopSimulator.Visibility = Visibility.Collapsed;
            SimulatorLogger.Visibility = Visibility.Collapsed;
            Scroller.Visibility = Visibility.Collapsed;
        }

        private void updateDroeDetailsPL()
        {
            //set drone's id
            DroneIDView.Text = "ID: " + drone.Id;

            //set drone's model
            DroneModelView.Text = "Model: " + drone.Model;

            //set drone's battery
            DroneBatteryView.Text = "Battery: ";
            if (drone.Battery <= 20)
            {
                DroneBatteryView.Inlines.Add(new Run(String.Format("{0:F3}", drone.Battery) + " %") { Foreground = Brushes.Red });
            }
            else if(drone.Battery > 20 && drone.Battery <= 50)
            {
                DroneBatteryView.Inlines.Add(new Run(String.Format("{0:F3}", drone.Battery) + " %") { Foreground = Brushes.Yellow });
            }
            else
            {
                DroneBatteryView.Inlines.Add(new Run(String.Format("{0:F3}", drone.Battery) + " %") { Foreground = Brushes.Green });
            }

            //set drone's weight
            DroneWeightView.Text = "Max Weight: ";
            if(drone.Weight == DO.WeightCategories.Heavy)
            {
                DroneWeightView.Inlines.Add(new Run(Enum.GetName(drone.Weight)) { Foreground = Brushes.Red });
            }
            else if(drone.Weight == DO.WeightCategories.Medium)
            {
                DroneWeightView.Inlines.Add(new Run(Enum.GetName(drone.Weight)) { Foreground = Brushes.Yellow });
            }
            else
            {
                DroneWeightView.Inlines.Add(new Run(Enum.GetName(drone.Weight)) { Foreground = Brushes.Green });
            }

            //set drone's location
            DroneLocationView.Text = $"Location: {String.Format("{0:F3}", drone.Location.Longitude)}° N, {String.Format("{0:F3}", drone.Location.Latitude)}° E";

            //set drone's status
            DroneStatusView.Text = "Status: ";
            if(drone.Status == DO.DroneStatuses.Shipping)
            {
                DroneStatusView.Inlines.Add(new Run(Enum.GetName(drone.Status)) { Foreground = Brushes.Red });
            }
            else if(drone.Status == DO.DroneStatuses.Maintenance)
            {
                DroneStatusView.Inlines.Add(new Run(Enum.GetName(drone.Status)) { Foreground = Brushes.Yellow });
            }
            else
            {
                DroneStatusView.Inlines.Add(new Run(Enum.GetName(drone.Status)) { Foreground = Brushes.Green });
            }

            //set drone's parcel
            if (drone.Parcel == null)
            {
                ParcelInDroneView.Text = "Parcel: None";
            }
            else
            {
                ParcelInDroneView.Text = "Parcel: " + drone.Parcel;
            }
        }

        private void ReturnOnClick(object o, EventArgs e)
        {
            if(UpdateDrone.Visibility == Visibility.Visible)
            {
                if(!_shouldStop)
                {
                    MessageBox.Show("You need to stop the simulator first.", "Alert");
                }

                else
                {
                    App.PrevWindow();
                }
            }

            else
            {
                App.PrevWindow();
            }
        }

        private void PlayOnClick(object o, EventArgs e)
        {
            try
            {
                //create worker
                _shouldStop = false;
                worker = new BackgroundWorker();
                worker.DoWork += Worker_DoWork;
                worker.ProgressChanged += Worker_ProgressChanged;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.RunWorkerAsync();

                //changed buttons vissibilities
                PlayButton.Visibility = Visibility.Collapsed;
                StopButton.Visibility = Visibility.Visible;

                UpdateButton.Visibility = Visibility.Collapsed;
                FirstButton.Visibility = Visibility.Collapsed;
                SecondButton.Visibility = Visibility.Collapsed;
                SimulatorLogger.Visibility = Visibility.Visible;
                Scroller.Visibility = Visibility.Visible;

                MessageBox.Show("Simulator has been starting", "System Message");
                SimulatorLogger.Inlines.Add(new Run("\nSYS_LOG: Simulator has been starting\n") { Foreground = Brushes.Green, FontWeight = FontWeights.Normal, FontSize = 12 });
                _loggerCounter++;
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
                SimulatorLogger.Inlines.Add(new Run("SYS_LOG: Request to stop the simulator was received\n") { Foreground = Brushes.Green, FontWeight = FontWeights.Normal, FontSize = 12 });
                _loggerCounter++;
                _shouldStop = true;
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

            this.drone = iBL.GetDroneById(drone.Id);
            updateDroeDetailsPL();
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
                this.drone =  iBL.GetDroneById(drone.Id);
                updateDroeDetailsPL();
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

                this.drone = iBL.GetDroneById(drone.Id);
                updateDroeDetailsPL();
            }

            else //Deliver parcel
            {
                try
                {
                    iBL.ParcelDelivered(drone.Id);
                    this.drone = iBL.GetDroneById(drone.Id);
                    updateDroeDetailsPL();

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
            try { Thread.Sleep(3000); } catch (Exception) { } // 3 sec sleep
           
            while(!_shouldStop)
            {
                try
                {
                    if (drone.Status != DO.DroneStatuses.Available)
                    {
                        _secondsToSleep = 3;
                    }

                    this.iBL.StartSimulator(drone);
                }

                catch(Exception ex)
                {
                    _stringToLogger = ex.Message;

                    if (ex.Message.Contains("ERROR"))
                    {
                        _secondsToSleep += 3;
                        worker.ReportProgress(1);
                    }

                    else 
                    {
                        worker.ReportProgress(2);
                    }
                }
                
                for(int i = 0; i < _secondsToSleep && !_shouldStop; ++i)
                {
                    try { Thread.Sleep(1000); } catch (Exception) { } // 1 sec sleep
                }
            }

            worker.ReportProgress(95);
            for(int p = 96; p <= 100; ++p) // 5 sec delay
            {
                Thread.Sleep(1000);
                worker.ReportProgress(p);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if(e.ProgressPercentage == 1)
            {
                SimulatorLogger.Inlines.Add(new Run(_stringToLogger) { Foreground = Brushes.Red, FontWeight = FontWeights.Normal, FontSize = 12 });
                _loggerCounter++;
            }
            
            else if(e.ProgressPercentage == 2)
            {
                this.drone = iBL.GetDroneById(drone.Id);
                updateDroeDetailsPL();
                SimulatorLogger.Inlines.Add(new Run(_stringToLogger) { Foreground = Brushes.Green, FontWeight = FontWeights.Normal, FontSize = 12 });
                _loggerCounter++;
            }
            
            else
            {
                TimerToStopSimulator.Visibility = Visibility.Visible;
                TimerToStopSimulator.Text = "Stop in: " + (100 - e.ProgressPercentage);
            }

            if(_loggerCounter > 10)
            {
                SimulatorLogger.Height += 10;
            }
        }

        private void Worker_RunWorkerCompleted(object o, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }

            TimerToStopSimulator.Visibility = Visibility.Collapsed;
            SimulatorLogger.Visibility = Visibility.Collapsed;
            Scroller.Visibility = Visibility.Collapsed;
            worker.CancelAsync();

            PlayButton.Visibility = Visibility.Visible;
            StopButton.Visibility = Visibility.Collapsed;

            UpdateButton.Visibility = Visibility.Visible;
            FirstButton.Visibility = Visibility.Visible;
            SecondButton.Visibility = Visibility.Visible;

            MessageBox.Show("Simulator has been stopped", "System Message");
        }
    }
}   