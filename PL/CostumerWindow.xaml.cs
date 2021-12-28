using System;
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
using System.Device.Location;
using BlApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for CostumerWindow.xaml
    /// </summary>
    public partial class CostumerWindow : Window //TODO: Implemented handlers of CostumerWindow
    {
        private BlApi.IBL iBL;
        private BO.CostumerBL costumer;
        bool returnMain;

        public CostumerWindow(bool returnMain)
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

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
            Longitude.Text = this.iBL.GetCurrentLongitude().ToString();
            Latitude.Text = this.iBL.GetCurrentLatitude().ToString();
        }

        public CostumerWindow(object item)
        {
            InitializeComponent();
            this.Closing += App.Window_Closing;

            AddCostumer.Visibility = Visibility.Hidden;
            CostumerDetails.Visibility = Visibility.Visible;
            UpdateCostumer.Visibility = Visibility.Hidden;

            this.iBL = BlFactory.GetBl();

            BO.CostumerListBL costumerList = null;
            if (item is BO.CostumerListBL)
            {
                costumerList = (BO.CostumerListBL)item;
                this.costumer = this.iBL.GetCostumerById(costumerList.Id);
            }

            else if (item is BO.CostumerBL)
            {
                this.costumer = (BO.CostumerBL)item;
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
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();
            this.costumer = costumer;

            AddCostumer.Visibility = Visibility.Hidden;
            CostumerDetails.Visibility = Visibility.Hidden;
            UpdateCostumer.Visibility = Visibility.Visible;

            NewCostumerName.Text = name;
            NewPhone.Text = phone;
            NewEmail.Text = email;
            NewPassword.Text = password;
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
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");

            if (CostumerEmail.Text == "")
            {
                EmailError.Text = "Email is missing.";
                addCostumer = false;
            }

            else if(!regex.IsMatch(CostumerEmail.Text))
            {
                EmailError.Text = "Email format must be username@domain.tld";
                addCostumer = false;
            }

            //check username
            if(CostumerName.Text == "")
            {
                NameError.Text = "Name is missing.";
                addCostumer = false;
            }

            //check password
            if(Password.Text == "")
            {
                PasswordError.Text = "Password is missing.";
                addCostumer = false;
            }


            //check id
            if(Id.Text == "")
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
            string phonePrefix = Phone.Text.Substring(0, 3);

            if (Phone.Text == "")
            {
                PhoneError.Text = "Phone number is missing";
                addCostumer = false;
            }

            else if(Phone.Text.Length != 10)
            {
                PhoneError.Text = "Phone number is illegal.";
                addCostumer = false;
            }

            else if (phonePrefix != "050" && phonePrefix != "052" && phonePrefix != "053" &&
                phonePrefix != "054" && phonePrefix != "055" && phonePrefix != "058")
            {
                PhoneError.Text = "An unfamiliar cellular company.";
                addCostumer = false;
            }

            foreach (var letter in Phone.Text)
            {
                if(!(letter <= '9' && letter >= '0'))
                {
                    PhoneError.Text = "Phone number is illegal.";
                    addCostumer = false;
                }
            }

            //check longitude
            if(Longitude.Text == "")
            {
                LongitudeError.Text = "Longitude is missing.";
                addCostumer = false;
            }

            else if(!double.TryParse(Longitude.Text, out longitude))
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
                if(addCostumer)
                {
                    this.iBL.AddCostumer(id, CostumerName.Text, Phone.Text, longitude, latitude, CostumerEmail.Text, Password.Text);
                    MessageBox.Show("Costumer added successfully.", "SYSTEM");
                    App.ShowWindow<CostumersListWindow>();
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
            CostumerWindow nextWindow = new CostumerWindow(CostumerName.Text, Phone.Text, CostumerEmail.Text, Password.Text, costumer);
            App.ShowWindow(nextWindow);
        }

        private void ParcelView(object o, EventArgs e)
        {

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

        }

        private void ReturnOnClick(object o, EventArgs e)
        {
            if(returnMain)
            {
                App.BackToMain();
            }

            else if (UpdateCostumer.Visibility == Visibility.Visible)
            {
                CostumerWindow nextWindow = new CostumerWindow(this.costumer);
                App.ShowWindow(nextWindow);
            }

            else
            {
                App.ShowWindow<CostumersListWindow>();
            }
        }

        private bool checkPhonePrefix(string phone)
        {
            return true;
        }


    }
}
