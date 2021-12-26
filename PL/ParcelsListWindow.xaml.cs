using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BlApi;

namespace PL
{
    public partial class ParcelsListWindow : Window
    {
        private BlApi.IBL iBL;

        IEnumerable<BO.ParcelListBL> parcels;

        public ParcelsListWindow()
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.BackToMain(); };
            Closing += App.Window_Closing;

            iBL = BlFactory.GetBl();

            parcels = iBL.GetParcelsList();
            ParcelsListView.ItemsSource = parcels;
            StatusSelector.ItemsSource = Enum.GetValues(typeof(DO.ParcelStatuses));
            SenderSelector.ItemsSource = iBL.GetCostumerList().Select(costumer => costumer.Id);
            TargetSelector.ItemsSource = iBL.GetCostumerList().Select(costumer => costumer.Id);
        }

        private void SelectionChanged(object o, EventArgs e)
        {
            if (StatusSelector.SelectedItem == null && SenderSelector.SelectedItem == null &&
                TargetSelector.SelectedItem == null)
            {
                parcels = iBL.GetParcelsList();
            }
            else

            {
                parcels = iBL.GetParcelsList(parcel =>
                    (StatusSelector.SelectedItem == null ||
                     parcel.Status == (DO.ParcelStatuses) StatusSelector.SelectedItem) &&
                    (SenderSelector.SelectedItem == null || parcel.SenderId == Convert.ToInt32(SenderSelector.SelectedItem)) &&
                    (TargetSelector.SelectedItem == null || parcel.TargetId == Convert.ToInt32(TargetSelector.SelectedItem))
                );
            }

            ParcelsListView.ItemsSource = parcels;
        }

        private void StatusSelectorClearButtonOnClick(object o, EventArgs e)
        {
            StatusSelector.SelectedItem = null;
        }

        private void SenderSelectorClearButtonOnClick(object o, EventArgs e)
        {
            SenderSelector.SelectedItem = null;
        }

        private void TargetSelectorClearButtonOnClick(object o, EventArgs e)
        {
            TargetSelector.SelectedItem = null;
        }

        private void AddParcelButtonOnClick(object o, EventArgs e)
        {
            // ParcelWindow nextWindow = new ParcelWindow();
            StatusSelector.SelectedItem = null;
            SenderSelector.SelectedItem = null;
            TargetSelector.SelectedItem = null;
            // App.ShowWindow(nextWindow);
        }

        private void ParcelView(object o, EventArgs e)
        {
            // ParcelWindow nextWindow = new ParcelWindow(ParcelsListView.SelectedItem);
            StatusSelector.SelectedItem = null;
            SenderSelector.SelectedItem = null;
            // App.ShowWindow(nextWindow);
        }
    }
}