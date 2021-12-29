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

        public ParcelWindow()
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();

            ParcelPriority.ItemsSource = Enum.GetValues(typeof(DO.Priorities));
            ParcelWeight.ItemsSource = Enum.GetValues(typeof(DO.WeightCategories));
            Target.ItemsSource = iBL.GetCostumerList(costumer => costumer.Id != iBL.GetLoggedUser().Id);
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
    }
}