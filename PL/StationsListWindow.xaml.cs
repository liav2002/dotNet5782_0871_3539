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
    /// Interaction logic for StationsListWindow.xaml
    /// </summary>
    public partial class StationsListWindow : Window
    {
        private BlApi.IBL iBL;

        IEnumerable<BO.StationListBL> stations = new List<BO.StationListBL>();

        public StationsListWindow()
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.BackToMain(); };
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();

            this.stations = this.iBL.GetStationsList();

            StationsListView.ItemsSource = this.stations;
        }

        private void InputChanged(object o, EventArgs e)
        {
            string slotsStr = RequiredSlotsInput.Text;
            int inputSlots = 0;
            errorMessage.Text = "";

            if(slotsStr == "")
            {
                this.stations = this.iBL.GetStationsList();
                StationsListView.ItemsSource = this.stations;
            }
            
            else if (!int.TryParse(slotsStr, out inputSlots))
            {
                errorMessage.Text = "Charge slots must be integer.";
            }

            else
            {
                this.stations = this.iBL.GetStationsList(station => station.ChargeSlots >= inputSlots);
                StationsListView.ItemsSource = this.stations;
            }
        }

        private void RequiredSlotsClearButtonOnClick(object o, EventArgs e)
        {
            errorMessage.Text = "";
            RequiredSlotsInput.Text = "";
        }

        private void StationView(object o, EventArgs e)
        {
            errorMessage.Text = "Window has not been developed yet.";
            //TODO: Implemeted StationViewWindow
            //App.ShowWindow<StationViewWindow>();
        }

        private void AddStationButtonOnClick(object o, EventArgs e)
        {
            App.ShowWindow<StationWindow>();
        }

        private void RemoveStationButtonOnClick(object o, EventArgs e)
        {
            errorMessage.Text = "Window has not been developed yet.";
            //TODO: Implemeted StationViewWindow (Add constructor)
            //App.ShowWindow<StationViewWindow>();
        }

        private void AvliableChargeSlotsChecked(object o, EventArgs e)
        {
            int chargeSlots = 0;

            if (AvaliableSlotsOnly.IsChecked == true)
            {
                chargeSlots = 1;

                if (RequiredSlotsInput.Text != "")
                {
                    int.TryParse(RequiredSlotsInput.Text, out chargeSlots);
                }
            }

            else
            {
                if (RequiredSlotsInput.Text != "")
                {
                    int.TryParse(RequiredSlotsInput.Text, out chargeSlots);
                }
            }

            this.stations = iBL.GetStationsList(station => station.ChargeSlots >= chargeSlots);
            StationsListView.ItemsSource = this.stations;
        }
    }
}
