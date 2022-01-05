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
    /// Interaction logic for DronesList.xaml
    /// </summary>
    public partial class DronesListWindow : Window
    {
        private BlApi.IBL iBL;

        IEnumerable<BO.DroneListBL> drones = new List<BO.DroneListBL>();

        public DronesListWindow()
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.PrevWindow(); };
            this.Closing += App.Window_Closing;

            Initialized();
        }

        private void Initialized()
        {
            this.iBL = BlFactory.GetBl();

            this.drones = this.iBL.GetDroneList();
            DronesListView.ItemsSource = this.drones;
            SetListViewForeground();

            StatusSelector.ItemsSource = Enum.GetValues(typeof(DO.DroneStatuses));
            WeightSelector.ItemsSource = Enum.GetValues(typeof(DO.WeightCategories));

            RemoveDroneButton.Visibility = Visibility.Collapsed;
        }

        private void SetListViewForeground()
        {
            //TODO: Try do implement this function (Foreground List View).
        }

        private void SelectionChanged(object o, EventArgs e)
        {
            if (StatusSelector.SelectedItem == null && WeightSelector.SelectedItem == null)
            {
                this.drones = iBL.GetDroneList();
            }
            else if (StatusSelector.SelectedItem != null && WeightSelector.SelectedItem != null)
            {
                this.drones = iBL.GetDroneList(drone =>
                    drone.Status == (DO.DroneStatuses) StatusSelector.SelectedItem &&
                    drone.MaxWeight == (DO.WeightCategories) WeightSelector.SelectedItem);
            }
            else if (StatusSelector.SelectedItem != null && WeightSelector.SelectedItem == null)
            {
                this.drones = iBL.GetDroneList(drone => drone.Status == (DO.DroneStatuses) StatusSelector.SelectedItem);
            }
            else
            {
                this.drones = iBL.GetDroneList(drone =>
                    drone.MaxWeight == (DO.WeightCategories) WeightSelector.SelectedItem);
            }

            DronesListView.ItemsSource = this.drones;
        }

        private void StatusSelectorClearButtonOnClick(object o, EventArgs e)
        {
            StatusSelector.SelectedItem = null;
            DronesListView.SelectedItem = null;
        }

        private void WeightSelectorClearButtonOnClick(object o, EventArgs e)
        {
            WeightSelector.SelectedItem = null;
            DronesListView.SelectedItem = null;
        }

        private void AddDroneButtonOnClick(object o, EventArgs e)
        {
            DroneWindow nextWindow = new DroneWindow();
            StatusSelector.SelectedItem = null;
            WeightSelector.SelectedItem = null;
            App.NextWindow(nextWindow, Initialized);
        }

        private void RemoveDroneButtonOnClick(object o, EventArgs e)
        {
            try
            {
                if (DronesListView.SelectedItem != null)
                {
                    if (RemoveDroneButton.Content.ToString() == "Remove")
                    {
                        this.iBL.RemoveDrone(((BO.DroneListBL) DronesListView.SelectedItem).Id);
                        DronesListView.ItemsSource = this.iBL.GetDroneList();
                        SetListViewForeground();
                    }

                    else if (RemoveDroneButton.Content.ToString() == "Restore")
                    {
                        this.iBL.RestoreDrone(((BO.DroneListBL) DronesListView.SelectedItem).Id);
                        DronesListView.ItemsSource = this.iBL.GetDroneList();
                        SetListViewForeground();
                    }
                }

                else
                {
                    errorMessage.Text = "You need to select drone first.";
                }
            }

            catch (Exception ex)
            {
                errorMessage.Text = ex.Message;
            }
        }

        private void DroneView(object o, EventArgs e)
        {
            DroneWindow nextWindow = new DroneWindow(DronesListView.SelectedItem);
            StatusSelector.SelectedItem = null;
            WeightSelector.SelectedItem = null;
            App.NextWindow(nextWindow, Initialized);
        }

        private void DroneSelected(object o, EventArgs e)
        {
            if (DronesListView.SelectedItem != null)
            {
                BO.DroneBL drone = this.iBL.GetDroneById(((BO.DroneListBL) DronesListView.SelectedItem).Id);

                RemoveDroneButton.Visibility = Visibility.Visible;

                if (drone.IsAvaliable)
                {
                    RemoveDroneButton.Content = "Remove";
                }

                else
                {
                    RemoveDroneButton.Content = "Restore";
                }
            }

            else
            {
                RemoveDroneButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}