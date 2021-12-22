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
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window
    {
        private BlApi.IBL iBL;
        private BO.StationListBL station;
 

        public StationWindow()
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

            AddStation.Visibility = Visibility.Visible;
            UpdateStation.Visibility = Visibility.Hidden;

            this.iBL = BlFactory.GetBl();
            this.station = null;
        }

        public StationWindow(object item)
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

            AddStation.Visibility = Visibility.Hidden;
            UpdateStation.Visibility = Visibility.Visible;


        }

        private void AddOnClick(object o, EventArgs e)
        {
            int id = 0;
            double longitude = 0;
            double lattitude = 0;
            int chargeSlots = 0;
            bool tryToAdd = true;

            //handle id
            if(StationID.Text == "")
            {
                IdError.Text = "Id is missing";
                tryToAdd = false;
            }

            else if(!int.TryParse(StationID.Text, out id))
            {
                IdError.Text = "Id must be integer.";
                tryToAdd = false;
            }

            //handle name
            if(StationID.Text == "")
            {
                NameError.Text = "Name is missing.";
                tryToAdd = false;
            }

            //handle longitude
            if(Longitude.Text == "")
            {
                LongitudeError.Text = "Longitude is missing.";
                tryToAdd = false;
            }

            else if(!double.TryParse(Longitude.Text, out longitude))
            {
                LongitudeError.Text = "Longitude must be number.";
                tryToAdd = false;
            }

            //handle lattiude
            if(Lattitude.Text == "")
            {
                LattitudeError.Text = "Lattitude is missing.";
                tryToAdd = false;
            }

            else if(!double.TryParse(Lattitude.Text, out lattitude))
            {
                LattitudeError.Text = "Lattitude must be number.";
                tryToAdd = false;
            }

            //handle charges slots
            if(ChargeSlots.Text == "")
            {
                ChargeSlotsError.Text = "Charge slots is missing.";
                tryToAdd = false;
            }

            else if(!int.TryParse(ChargeSlots.Text, out chargeSlots))
            {
                ChargeSlotsError.Text = "Charge slots must be integer.";
                tryToAdd = false;
            }

            try
            {
                if (tryToAdd)
                {
                    this.iBL.AddStation(id, StationID.Text, longitude, lattitude, chargeSlots);
                    MessageBox.Show("Station added successfully.", "SYSTEM");
                    App.ShowWindow<StationsListWindow>();
                }
            }

            catch(Exception ex)
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

        private void ReturnOnClick(object o, EventArgs e)
        {
            App.ShowWindow<StationsListWindow>();
        }
    }
}
