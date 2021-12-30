﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CostumerWindow.xaml
    /// </summary>
    public partial class CostumerWindow : Window
    {
        private BlApi.IBL iBL;
        private BO.CostumerBL costumer;
        bool returnMain;

        public CostumerWindow(bool returnMain)
        {
            InitializeComponent();
            this.Closing += App.PrevWindow;

            AddCostumer.Visibility = Visibility.Visible;
            CostumerDetails.Visibility = Visibility.Hidden;
            UpdateCostumer.Visibility = Visibility.Hidden;

            this.iBL = BlFactory.GetBl();
            this.costumer = null;
            this.returnMain = returnMain;

            //Example Costumer to add

            CostumerEmail.Text = "name@domain.tld";
            CostumerName.Text = "User2022";
            Password.Text = "Aa123456";
            Id.Text = "123456789";
            Phone.Text = "0521111111";
            Longitude.Text = "31.765";
            Latitude.Text = "35.191";
        }

        public CostumerWindow(object item)
        {
            InitializeComponent();
            this.Closing += App.PrevWindow;

            AddCostumer.Visibility = Visibility.Hidden;
            CostumerDetails.Visibility = Visibility.Visible;
            UpdateCostumer.Visibility = Visibility.Hidden;

            this.iBL = BlFactory.GetBl();

            BO.CostumerListBL costumerList = null;
            if (item is BO.CostumerListBL)
            {
                costumerList = (BO.CostumerListBL) item;
                this.costumer = this.iBL.GetCostumerById(costumerList.Id);
            }

            else if (item is BO.CostumerBL)
            {
                this.costumer = (BO.CostumerBL) item;
            }

            //initalized text blocks
            CostumerIDView.Text = "Id: " + this.costumer.Id;
            CostumerNameView.Text = "Name: " + this.costumer.Name;
            CostumerPhoneView.Text = "Phone: " + this.costumer.Phone;
            CostumerLocationView.Text = "Location: " + this.costumer.Location;

            //initialized costumer's shipped parcels ListView
            ShippedParcelsView.ItemsSource = this.costumer.ParcelsSender;

            //intialized costumer's incoming parcels ListView
            IncomingParcelsView.ItemsSource = this.costumer.ParcelsReciever;
        }

        CostumerWindow(string name, string phone, string email, string password, BO.CostumerBL costumer)
        {
            InitializeComponent();
            this.Closing += App.PrevWindow;

            this.iBL = BlFactory.GetBl();
            this.costumer = costumer;

            AddCostumer.Visibility = Visibility.Hidden;
            CostumerDetails.Visibility = Visibility.Hidden;
            UpdateCostumer.Visibility = Visibility.Visible;

            NewCostumerName.Text = name;
            NewPhone.Text = phone;
            NewEmail.Text = email;
            NewPassword.Password = password;
        }

        private void EmailChanged(object o, EventArgs e)
        {
            EmailError.Text = "";
            AddCostumerError.Text = "";
        }

        private void NameChanged(object o, EventArgs e)
        {
            NameError.Text = "";
            AddCostumerError.Text = "";
        }

        private void PasswordChanged(object o, EventArgs e)
        {
            PasswordError.Text = "";
            AddCostumerError.Text = "";
        }

        private void IdChanged(object o, EventArgs e)
        {
            IdError.Text = "";
            AddCostumerError.Text = "";
        }

        private void PhoneChanged(object o, EventArgs e)
        {
            PhoneError.Text = "";
            AddCostumerError.Text = "";
        }

        private void LongitudeChanged(object o, EventArgs e)
        {
            LongitudeError.Text = "";
            AddCostumerError.Text = "";
        }

        private void LatitudeChanged(object o, EventArgs e)
        {
            LatitudeError.Text = "";
            AddCostumerError.Text = "";
        }

        private void ConfirmOnClick(object o, EventArgs e)
        {
            int id = 0;
            bool addCostumer = true;
            double longitude = 0;
            double latitude = 0;

            //check email

            if (CostumerEmail.Text == "")
            {
                EmailError.Text = "Email is missing.";
                addCostumer = false;
            }

            else if (!IsEmailValid(CostumerEmail.Text))
            {
                EmailError.Text = "Email format must be username@domain.tld";
                addCostumer = false;
            }

            //check username
            if (CostumerName.Text == "")
            {
                NameError.Text = "Name is missing.";
                addCostumer = false;
            }

            //check password
            if (Password.Text == "")
            {
                PasswordError.Text = "Password is missing.";
                addCostumer = false;
            }


            //check id
            if (Id.Text == "")
            {
                IdError.Text = "Id is missing.";
                addCostumer = false;
            }

            else if (!int.TryParse(Id.Text, out id))
            {
                IdError.Text = "Id must be ingeter.";
                addCostumer = false;
            }

            //check phone number

            if (Phone.Text == "")
            {
                PhoneError.Text = "Phone number is missing";
                addCostumer = false;
            }

            else if (!IsPhonePrefixValid(Phone.Text))
            {
                PhoneError.Text = "An unfamiliar cellular company.";
                addCostumer = false;
            }

            else if (!IsPhoneValid(Phone.Text))
            {
                PhoneError.Text = "Phone number is illegal.";
                addCostumer = false;
            }

            //check longitude
            if (Longitude.Text == "")
            {
                LongitudeError.Text = "Longitude is missing.";
                addCostumer = false;
            }

            else if (!double.TryParse(Longitude.Text, out longitude))
            {
                LongitudeError.Text = "Longitude must be number";
                addCostumer = false;
            }

            //check latitude
            if (Latitude.Text == "")
            {
                LatitudeError.Text = "Latitude is missing.";
                addCostumer = false;
            }

            else if (!double.TryParse(Latitude.Text, out latitude))
            {
                LongitudeError.Text = "Latitude must be number";
                addCostumer = false;
            }

            try
            {
                if (addCostumer)
                {
                    this.iBL.AddCostumer(id, CostumerName.Text, Phone.Text, longitude, latitude, CostumerEmail.Text,
                        Password.Text);
                    MessageBox.Show("Costumer added successfully.", "SYSTEM");

                    if (this.iBL.GetLoggedUser() == null)
                    {
                        App.BackToMain();
                    }

                    else
                    {
                        App.NextWindow(CostumersListWindow);
                    }
                }

                else
                {
                    AddCostumerError.Text = "Fix details.";
                }
            }

            catch (Exception ex)
            {
                AddCostumerError.Text = ex.Message;
            }
        }

        private void UpdateOnClick(object o, EventArgs e)
        {
            if (costumer.IsAvaliable)
            {
                CostumerWindow nextWindow = new CostumerWindow(costumer.Name, costumer.Phone, costumer.Email,
                    costumer.Password, costumer);
                App.NextWindow(nextWindow);
            }

            else
            {
                MessageBox.Show("ERROR: costumer is not available.", "ERROR");
            }
        }

        private void ParcelView(object o, EventArgs e)
        {
            //TODO: Implement ParcelDetails Window
        }

        private void NewPhoneChanged(object o, EventArgs e)
        {
            NewPhoneError.Text = "";
            ConfirmError.Text = "";
        }

        private void NewEmailChanged(object o, EventArgs e)
        {
            NewEmailError.Text = "";
            ConfirmError.Text = "";
        }

        private void NewPasswordChanged(object o, EventArgs e)
        {
            NewPasswordError.Text = "";
            ConfirmError.Text = "";
        }

        private void ConfirmUpdateOnClick(object o, EventArgs e)
        {
            bool update = true;

            //check new phone
            if (!IsPhonePrefixValid(NewPhone.Text))
            {
                NewPhoneError.Text = "An unfamiliar cellular company.";
                update = false;
            }

            else if (!IsPhoneValid(NewPhone.Text))
            {
                NewPhoneError.Text = "Phone number is illegal.";
                update = false;
            }

            //check new email

            if (NewEmail.Text != "" && !IsEmailValid(NewEmail.Text))
            {
                NewEmailError.Text = "Email format must be username@domain.tld";
                update = false;
            }

            try
            {
                if (update)
                {
                    this.iBL.UpdateCostumer(costumer.Id, NewCostumerName.Text, NewPhone.Text, NewEmail.Text,
                        NewPassword.Password);
                    CostumerNameView.Text = NewCostumerName.Text;
                    CostumerPhoneView.Text = NewPhone.Text;
                }
            }

            catch (Exception ex)
            {
                ConfirmError.Text = ex.Message;
                return;
            }

            if (update)
            {
                CostumerWindow nextWindow = new CostumerWindow(costumer);
                App.NextWindow(nextWindow);
            }
        }

        private void ReturnOnClick(object o, EventArgs e)
        {
            App.PrevWindow();

            // if (returnMain)
            // {
            //     App.BackToMain();
            // }
            //
            // else if (UpdateCostumer.Visibility == Visibility.Visible)
            // {
            //     CostumerWindow nextWindow = new CostumerWindow(this.costumer);
            //     App.ShowWindow(nextWindow);
            // }
            //
            // else
            // {
            //     if (this.iBL.GetLoggedUser().IsManager)
            //     {
            //         App.ShowWindow<CostumersListWindow>();
            //     }
            //     else
            //     {
            //         App.BackToMain();
            //     }
            // }
        }

        private bool IsPhonePrefixValid(string phone)
        {
            string phonePrefix = phone.Substring(0, 3);

            return (phonePrefix == "050" || phonePrefix == "052" || phonePrefix == "053" ||
                    phonePrefix == "054" || phonePrefix == "055" || phonePrefix == "058");
        }

        private bool IsPhoneValid(string phone)
        {
            if (phone.Length != 10)
            {
                return false;
            }

            foreach (var letter in phone)
            {
                if (!(letter <= '9' && letter >= '0'))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsEmailValid(string email)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");

            return regex.IsMatch(email);
        }
    }
}