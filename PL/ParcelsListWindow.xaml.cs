using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            SenderSelector.ItemsSource = iBL.GetCostumerList()
                .Select(costumer => ("Id: " + costumer.Id + " " + "Name: " + costumer.Name + ""));
            TargetSelector.ItemsSource = iBL.GetCostumerList()
                .Select(costumer => ("Id: " + costumer.Id + " " + "Name: " + costumer.Name + ""));
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
                    (SenderSelector.SelectedItem == null ||
                     parcel.SenderId == Convert.ToInt32(SenderSelector.SelectedItem.ToString().Split(' ')[1])) &&
                    (TargetSelector.SelectedItem == null ||
                     parcel.TargetId == Convert.ToInt32(TargetSelector.SelectedItem.ToString().Split(' ')[1]))
                );
            }

            ParcelsListView.ItemsSource = parcels;
        }

        private void StatusSelectorClearButtonOnClick(object o, EventArgs e)
        {
            errorMessage.Text = "";

            StatusSelector.SelectedItem = null;
        }

        private void SenderSelectorClearButtonOnClick(object o, EventArgs e)
        {
            errorMessage.Text = "";

            SenderSelector.SelectedItem = null;
        }

        private void TargetSelectorClearButtonOnClick(object o, EventArgs e)
        {
            errorMessage.Text = "";

            TargetSelector.SelectedItem = null;
        }

        private void RemoveParcelButtonOnClick(object o, EventArgs e)
        {
            errorMessage.Text = "";

            StatusSelector.SelectedItem = null;
            SenderSelector.SelectedItem = null;
            TargetSelector.SelectedItem = null;
            try
            {
                if (ParcelsListView.SelectedItem != null)
                {
                    iBL.RemoveParcel(((BO.ParcelListBL) ParcelsListView.SelectedItem).Id);
                    ParcelsListView.ItemsSource = iBL.GetParcelsList();
                    SetListViewForeground();
                }

                else
                {
                    errorMessage.Text = "You need to select parcel first.";
                }
            }

            catch (Exception ex)
            {
                errorMessage.Text = ex.Message;
            }
        }

        private void AddParcelButtonOnClick(object o, EventArgs e)
        {
            // ParcelWindow nextWindow = new ParcelWindow();
            errorMessage.Text = "";

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

        private void SetListViewForeground()
        {
            //TODO: Try do implement this function (Foreground List View).
            //The function set the foreground of unvaliable items as red.
            //
            // foreach (var VARIABLE in ParcelsListView.Items.)
            // {
            //     
            // }

        }
    }
}