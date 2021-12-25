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
            ReturnButton.Click += delegate { App.BackToMain(); };
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();

            this.drones = this.iBL.GetDroneList();
            DronesListView.ItemsSource = this.drones;
            StatusSelector.ItemsSource = Enum.GetValues(typeof(DO.DroneStatuses));
            WeightSelector.ItemsSource = Enum.GetValues(typeof(DO.WeightCategories));
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
        }

        private void WeightSelectorClearButtonOnClick(object o, EventArgs e)
        {
            WeightSelector.SelectedItem = null;
        }

        private void AddDroneButtonOnClick(object o, EventArgs e)
        {
            DroneWindow nextWindow = new DroneWindow();
            StatusSelector.SelectedItem = null;
            WeightSelector.SelectedItem = null;
            App.ShowWindow(nextWindow);
        }

        private void RemoveDroneButtonOnClick(object o, EventArgs e)
        {
            try 
            {
                if(DronesListView.SelectedItem != null)
                {
                    this.iBL.RemoveDrone(((BO.DroneListBL)DronesListView.SelectedItem).Id);
                    DronesListView.ItemsSource = this.iBL.GetDroneList();
                }

                else
                {
                    errorMessage.Text = "You need to select drone first.";
                }
            }

            catch(Exception ex)
            {
                errorMessage.Text = ex.Message;
            }
        }

        private void DroneView(object o, EventArgs e)
        {
            DroneWindow nextWindow = new DroneWindow(DronesListView.SelectedItem);
            StatusSelector.SelectedItem = null;
            WeightSelector.SelectedItem = null;
            App.ShowWindow(nextWindow);
        }
    }
}