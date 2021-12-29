using System;
using System.Windows;
using BlApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneView.xaml
    /// </summary>
    public partial class ParcelWindow : Window
    {
        private BlApi.IBL iBL;
        private BO.ParcelBL _parcel;

        public ParcelWindow()
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();

            ParcelPriority.ItemsSource = Enum.GetValues(typeof(DO.Priorities));
            ParcelWeight.ItemsSource = Enum.GetValues(typeof(DO.WeightCategories));
            Target.ItemsSource = iBL.GetCostumerList(costumer => costumer.Id != iBL.GetLoggedUser().Id);

            ParcelDetails.Visibility = Visibility.Hidden;
            AddParcel.Visibility = Visibility.Visible;

            _parcel = null;
        }

        public ParcelWindow(object item)
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

            ParcelDetails.Visibility = Visibility.Visible;
            AddParcel.Visibility = Visibility.Hidden;

            this.iBL = BlFactory.GetBl();

            if (item is BO.ParcelListBL pl)
                _parcel = iBL.GetParcelById(pl.Id);
            else if (item is BO.ParcelBL p)
                _parcel = p;
            else
                throw new ArgumentException("Wrong Argument ParcelWindow");


            ParcelWeightView.Text = "Weight: " + Enum.GetName(_parcel.Weight);
            ParcelPriorityView.Text = "Priority: " + Enum.GetName(_parcel.Priority);
            ParcelStatusView.Text = "Status: " + Enum.GetName(_parcel.Status);
            
            
            
            if ((int) _parcel.Status >= 0)
            {
                Created.Visibility = Visibility.Visible;
                Created.Text = "Created: " + _parcel.Requested;
                if ((int) _parcel.Status >= 1)
                {
                    DroneDetail.Visibility = Visibility.Visible;

                    Assign.Visibility = Visibility.Visible;
                    Assign.Text = "Assign: " + _parcel.Scheduled;
                    if ((int) _parcel.Status >= 2)
                    {
                        PickedUp.Visibility = Visibility.Visible;
                        PickedUp.Text = "PickedUp: " + _parcel.PickedUp;
                        if ((int) _parcel.Status >= 3)
                        {
                            Delivered.Visibility = Visibility.Visible;
                            Delivered.Text = "Delivered: " + _parcel.Delivered;
                        }
                    }
                }
            } // end of if
        }


        private void AddOnClick(object o, EventArgs e)
        {
            if (ParcelPriority.SelectedItem == null || ParcelWeight.SelectedItem == null || Target.Text == "")
            {
                MessageBox.Show("There are missing values!", "WARNING");
                return;
            }

            DO.Priorities parcelPriority = (DO.Priorities) ParcelPriority.SelectedItem;
            DO.WeightCategories parcelWeight = (DO.WeightCategories) ParcelWeight.SelectedItem;
            int targetId = ((BO.CostumerListBL) Target.SelectedItem).Id;

            try
            {
                iBL.AddParcel(iBL.GetLoggedUser().Id, targetId, (int) parcelWeight, (int) parcelPriority, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR ");
                return;
            }

            MessageBox.Show("Parcel added successfully.", "SYSTEM");
            App.ShowWindow<ParcelsListWindow>();
        }

        private void ReturnOnClick(object o, EventArgs e)
        {
            App.ShowWindow<ParcelsListWindow>();
        }

        private void TargetOnClick(object sender, RoutedEventArgs e)
        {
            CostumerWindow nextWindow = new CostumerWindow(iBL.GetCostumerById(_parcel.Receiver.Id));
            App.ShowWindow(nextWindow);
        }

        private void SenderOnClick(object sender, RoutedEventArgs e)
        {
            CostumerWindow nextWindow = new CostumerWindow(iBL.GetCostumerById(_parcel.Sender.Id));
            App.ShowWindow(nextWindow);
        }

        private void DroneOnClick(object sender, RoutedEventArgs e)
        {
            DroneWindow nextWindow = new DroneWindow(iBL.GetDroneById(_parcel.Drone.Id));
            App.ShowWindow(nextWindow);
        }
    }
}