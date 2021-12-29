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
            StationsListView.DataContext = this.stations;

            RemoveStationButton.Visibility = Visibility.Collapsed;

            SetListViewForeground();
        }

        private void SetListViewForeground()
        {
            //TODO: Try do implement this function (Foreground List View).
            //The function set the foreground of unvaliable items as red.

            //foreach(ListViewItem item in StationsListView)
            //{
            //    BO.StationListBL stationList = (BO.StationListBL)(item.Content);
            //    BO.StationBL station = this.iBL.GetStationById(stationList.Id);

            //    if (station.IsAvaliable == false)
            //    {
            //        item.Foreground = Brushes.Red;
            //    }
            //}

            //for (int i = 0; i < StationsListView.Items.Count; ++i)
            //{
            //    var item = StationsListView.Items[i];
            //    BO.StationListBL stationList = (BO.StationListBL)item;
            //    BO.StationBL station = this.iBL.GetStationById(stationList.Id);

            //    if (station.IsAvaliable == false)
            //    {
            //        ListViewItem itemContainer = (ListViewItem)StationsListView.ItemContainerGenerator.ContainerFromItem(item);
            //        itemContainer.Foreground = Brushes.Red;
            //    }
            //}
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
            StationsListView.SelectedItem = null;
        }

        private void StationView(object o, EventArgs e)
        {
            StationWindow nextWindow = new StationWindow(StationsListView.SelectedItem);
            App.ShowWindow(nextWindow);
        }

        private void AddStationButtonOnClick(object o, EventArgs e)
        {
            App.ShowWindow<StationWindow>();
        }

        private void RemoveStationButtonOnClick(object o, EventArgs e)
        {
            try
            {
                if(StationsListView.SelectedItem != null)
                {
                    if(RemoveStationButton.Content.ToString() == "Remove")
                    {
                        this.iBL.RemoveStation(((BO.StationListBL)StationsListView.SelectedItem).Id);
                        StationsListView.ItemsSource = this.iBL.GetStationsList();
                        SetListViewForeground();
                    }

                    else if(RemoveStationButton.Content.ToString() == "Restore")
                    {
                        this.iBL.RestoreStation(((BO.StationListBL)StationsListView.SelectedItem).Id);
                        StationsListView.ItemsSource = this.iBL.GetStationsList();
                        SetListViewForeground();
                    }
                }

                else
                {
                    errorMessage.Text = "You need to select station first.";
                }
            }

            catch(Exception ex)
            {
                errorMessage.Text = ex.Message;
            }
        }

        private void StationSelected(object o, EventArgs e)
        {
            if (StationsListView.SelectedItem != null)
            {
                BO.StationBL station = this.iBL.GetStationById(((BO.StationListBL)StationsListView.SelectedItem).Id);

                RemoveStationButton.Visibility = Visibility.Visible;

                if (station.IsAvailable)
                {
                    RemoveStationButton.Content = "Remove";
                }

                else
                {
                    RemoveStationButton.Content = "Restore";
                }
            }

            else
            {
                RemoveStationButton.Visibility = Visibility.Collapsed;
            }
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
