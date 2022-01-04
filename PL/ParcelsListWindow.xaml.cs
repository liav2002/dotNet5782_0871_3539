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
            ReturnButton.Click += delegate { App.PrevWindow(); };
            Closing += App.Window_Closing;

            Initialized();
        }


        private bool Initialized()
        {
            iBL = BlFactory.GetBl();
            if(iBL.GetLoggedUser().IsManager)
            {
                parcels = iBL.GetParcelsList();

                SenderSelector.ItemsSource = iBL.GetCostumerList()
                    .Select(costumer => ("Id: " + costumer.Id + " " + "Name: " + costumer.Name + ""));
                TargetSelector.ItemsSource = iBL.GetCostumerList()
                    .Select(costumer => ("Id: " + costumer.Id + " " + "Name: " + costumer.Name + ""));
            }
            else
            {
                parcels = iBL.GetParcelsList(parcel => parcel.TargetId == iBL.GetLoggedUser().Id || parcel.SenderId == iBL.GetLoggedUser().Id);
                SenderSelector.Visibility = Visibility.Hidden;
                SenderLabel.Visibility = Visibility.Hidden;
                SenderSelectorClearButton.Visibility = Visibility.Hidden;
                TargetSelector.Visibility = Visibility.Hidden;
                TargetLabel.Visibility = Visibility.Hidden;
                TargetSelectorClearButton.Visibility = Visibility.Hidden;
            }

            ParcelsListView.ItemsSource = parcels;
            StatusSelector.ItemsSource = Enum.GetValues(typeof(DO.ParcelStatuses));

            RemoveParcelButton.Visibility = Visibility.Collapsed;
            return true;
        }

        private void SelectionChanged(object o, EventArgs e)
        {
            if (StatusSelector.SelectedItem == null && SenderSelector.SelectedItem == null &&
                TargetSelector.SelectedItem == null)
            {
                if(iBL.GetLoggedUser().IsManager)
                {
                    parcels = iBL.GetParcelsList();
                }

                else
                {
                    parcels = iBL.GetParcelsList(parcel => parcel.TargetId == iBL.GetLoggedUser().Id || parcel.SenderId == iBL.GetLoggedUser().Id);
                }
            }
            else
            {
                if (iBL.GetLoggedUser().IsManager)
                {
                    parcels = iBL.GetParcelsList(parcel =>
                    (StatusSelector.SelectedItem == null ||
                     parcel.Status == (DO.ParcelStatuses)StatusSelector.SelectedItem) &&
                    (SenderSelector.SelectedItem == null ||
                     parcel.SenderId == Convert.ToInt32(SenderSelector.SelectedItem.ToString().Split(' ')[1])) &&
                    (TargetSelector.SelectedItem == null ||
                     parcel.TargetId == Convert.ToInt32(TargetSelector.SelectedItem.ToString().Split(' ')[1])));
                }

                else
                {
                    parcels = iBL.GetParcelsList(parcel =>
                    (StatusSelector.SelectedItem == null ||
                     parcel.Status == (DO.ParcelStatuses)StatusSelector.SelectedItem) &&
                    (SenderSelector.SelectedItem == null ||
                     parcel.SenderId == Convert.ToInt32(SenderSelector.SelectedItem.ToString().Split(' ')[1])) &&
                    (TargetSelector.SelectedItem == null ||
                     parcel.TargetId == Convert.ToInt32(TargetSelector.SelectedItem.ToString().Split(' ')[1])) && 
                     (parcel.TargetId == iBL.GetLoggedUser().Id || parcel.SenderId == iBL.GetLoggedUser().Id));
                }
            }

            ParcelsListView.ItemsSource = parcels;
        }
        private void StatusSelectorClearButtonOnClick(object o, EventArgs e)
        {
            errorMessage.Text = "";

            StatusSelector.SelectedItem = null;
            
            ParcelsListView.SelectedItem = null;
        }

        private void SenderSelectorClearButtonOnClick(object o, EventArgs e)
        {
            errorMessage.Text = "";

            SenderSelector.SelectedItem = null;
            
            ParcelsListView.SelectedItem = null;

        }

        private void TargetSelectorClearButtonOnClick(object o, EventArgs e)
        {
            errorMessage.Text = "";

            TargetSelector.SelectedItem = null;
            
            ParcelsListView.SelectedItem = null;

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
                    if (RemoveParcelButton.Content.ToString() == "Remove")
                    {
                        this.iBL.RemoveParcel(((BO.ParcelListBL)ParcelsListView.SelectedItem).Id);
                        ParcelsListView.ItemsSource = this.iBL.GetParcelsList();
                        SetListViewForeground();
                    }

                    else if (RemoveParcelButton.Content.ToString() == "Restore")
                    {
                        this.iBL.RestoreParcel(((BO.ParcelListBL)ParcelsListView.SelectedItem).Id);
                        ParcelsListView.ItemsSource = this.iBL.GetParcelsList();
                        SetListViewForeground();
                    }
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
            ParcelWindow nextWindow = new ParcelWindow();
            errorMessage.Text = "";

            StatusSelector.SelectedItem = null;
            SenderSelector.SelectedItem = null;
            TargetSelector.SelectedItem = null;
            App.NextWindow(nextWindow, Initialized);
        }

        private void ParcelView(object o, EventArgs e)
        {
            if (ParcelsListView.SelectedItem != null)
            {
                ParcelWindow nextWindow = new ParcelWindow((ParcelsListView.SelectedItem as BO.ParcelListBL));
                errorMessage.Text = "";

                StatusSelector.SelectedItem = null;
                SenderSelector.SelectedItem = null;
                TargetSelector.SelectedItem = null;
                App.NextWindow(nextWindow, Initialized);
                this.iBL = BlFactory.GetBl();            }
        }

        private void SetListViewForeground()
        {
            //TODO: Try do implement this function (Foreground List View).
        }
        
        private void ParcelSelected(object o, EventArgs e)
        {
            if (ParcelsListView.SelectedItem != null)
            {
                BO.ParcelBL parcel = this.iBL.GetParcelById(((BO.ParcelListBL)ParcelsListView.SelectedItem).Id);

                RemoveParcelButton.Visibility = Visibility.Visible;

                if (parcel.IsAvailable)
                {
                    RemoveParcelButton.Content = "Remove";
                }

                else
                {
                    RemoveParcelButton.Content = "Restore";
                }
            }

            else
            {
                RemoveParcelButton.Visibility = Visibility.Collapsed;
            }

            errorMessage.Text = "";
        }
    }
}