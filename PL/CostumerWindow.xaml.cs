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

            else if (item is BO.StationBL)
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

        }

        private void NameChanged(object o, EventArgs e)
        {

        }

        private void PasswordChanged(object o, EventArgs e)
        {

        }

        private void IdChanged(object o, EventArgs e)
        {

        }

        private void PhoneChanged(object o, EventArgs e)
        {

        }

        private void ConfirmOnClick(object o, EventArgs e)
        {

        }

        private void UpdateOnClick(object o, EventArgs e)
        {

        }

        private void ParcelView(object o, EventArgs e)
        {

        }

        private void NewNameChanged(object o, EventArgs e)
        {

        }

        private void NewPhoneChanged(object o, EventArgs e)
        {

        }

        private void NewEmailChanged(object o, EventArgs e)
        {

        }

        private void NewPasswordChanged(object o, EventArgs e)
        {

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


    }
}
