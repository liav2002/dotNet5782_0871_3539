using IBL.BO;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for DronesList.xaml
    /// </summary>
    public partial class DronesList : Window
    {
        private IBL.IBL iBL;

        bool isReturnButtonUsed = false;

        IEnumerable<DroneListBL> drones = new List<DroneListBL>();

        public DronesList()
        {
            InitializeComponent();
            this.iBL = BL.GetInstance();
            this.drones = this.iBL.GetDroneList();
            DronesListView.ItemsSource = this.drones;
            StatusSelector.ItemsSource = Enum.GetValues(typeof(IDAL.DO.DroneStatuses));
            WeightSelector.ItemsSource = Enum.GetValues(typeof(IDAL.DO.WeightCategories));
            AddDroneButton.IsEnabled = false; //TODO: Implented Drone Window
        }

        private void SelectionChanged(object o, EventArgs e)
        {
            if(StatusSelector.SelectedItem == null && WeightSelector.SelectedItem == null)
            {
                this.drones = iBL.GetDroneList();
            }
            else if(StatusSelector.SelectedItem != null && WeightSelector.SelectedItem != null)
            {
                this.drones = iBL.GetDroneList(drone => drone.Status == (IDAL.DO.DroneStatuses)StatusSelector.SelectedItem && 
                                           drone.MaxWeight == (IDAL.DO.WeightCategories)WeightSelector.SelectedItem);
            }
            else if(StatusSelector.SelectedItem != null && WeightSelector.SelectedItem == null)
            {
                this.drones = iBL.GetDroneList(drone => drone.Status == (IDAL.DO.DroneStatuses)StatusSelector.SelectedItem);
            }
            else
            {
                this.drones = iBL.GetDroneList(drone => drone.MaxWeight == (IDAL.DO.WeightCategories)WeightSelector.SelectedItem);
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

        private void ReturnButtonOnClick(object o, EventArgs e)
        {
            isReturnButtonUsed = true;
            App.ShowWindow<MainWindow>();
        }
        private void AddDroneButtonOnClick(object o, EventArgs e)
        {
            StatusSelector.SelectedItem = null;
            WeightSelector.SelectedItem = null;
            //TODO: Show Drone Window
        }

        private void DroneView(object o, EventArgs e)
        {
            StatusSelector.SelectedItem = null;
            WeightSelector.SelectedItem = null;
            //TODO: Show Drone Window
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
