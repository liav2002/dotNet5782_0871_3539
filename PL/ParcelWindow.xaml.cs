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
        private object genericParcel;

        public ParcelWindow() // add parcel
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.PrevWindow(); };
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();

            ParcelPriority.ItemsSource = Enum.GetValues(typeof(DO.Priorities));
            ParcelWeight.ItemsSource = Enum.GetValues(typeof(DO.WeightCategories));
            Target.ItemsSource = iBL.GetCostumerList(costumer => costumer.Id != iBL.GetLoggedUser().Id);

            ParcelDetails.Visibility = Visibility.Hidden;
            AddParcel.Visibility = Visibility.Visible;

            _parcel = null;
        }

        public ParcelWindow(object item) // parcel deatils
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.PrevWindow(); };
            this.Closing += App.Window_Closing;

            ParcelDetails.Visibility = Visibility.Visible;
            AddParcel.Visibility = Visibility.Hidden;


            genericParcel = item;
            InitializedUpdate();
        }
        private bool InitializedUpdate()
        {
            this.iBL = BlFactory.GetBl();

            if (genericParcel is BO.ParcelListBL)
            {
                BO.ParcelListBL parcelList = null;
                parcelList = (BO.ParcelListBL)genericParcel;
                this._parcel = this.iBL.GetParcelById(parcelList.Id);
            }

            else if (genericParcel is BO.ParcelBL)
            {
                this._parcel = (BO.ParcelBL)genericParcel;
            }

            else if(genericParcel is BO.ParcelAtCostumer)
            {
                BO.ParcelAtCostumer parcelAtCostumer = null;
                parcelAtCostumer = (BO.ParcelAtCostumer)genericParcel;
                this._parcel = this.iBL.GetParcelById(parcelAtCostumer.Id);
            }

            ParcelIdView.Text = "ID: " + _parcel.Id;
            ParcelWeightView.Text = "Weight: " + Enum.GetName(_parcel.Weight);
            ParcelPriorityView.Text = "Priority: " + Enum.GetName(_parcel.Priority);
            ParcelStatusView.Text = "Status: " + Enum.GetName(_parcel.Status);

            Created.Visibility = Visibility.Hidden;
            DroneDetail.Visibility = Visibility.Hidden;
            Assign.Visibility = Visibility.Hidden;
            PickedUp.Visibility = Visibility.Hidden;
            Delivered.Visibility = Visibility.Hidden;

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
                            DroneDetail.Visibility = Visibility.Hidden;
                        }
                    }
                }
            } // end of if
            return true;
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
            App.PrevWindow();
        }

        private void TargetOnClick(object sender, RoutedEventArgs e)
        {
            
            CostumerWindow nextWindow = new CostumerWindow(iBL.GetCostumerById(_parcel.Receiver.Id));
            App.NextWindow(nextWindow);
        }

        private void SenderOnClick(object sender, RoutedEventArgs e)
        {
            CostumerWindow nextWindow = new CostumerWindow(iBL.GetCostumerById(_parcel.Sender.Id));
            App.NextWindow(nextWindow);
        }

        private void DroneOnClick(object sender, RoutedEventArgs e)
        {
            DroneWindow nextWindow = new DroneWindow(iBL.GetDroneById(_parcel.Drone.Id));
            App.NextWindow(nextWindow);
        }
    }
}