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
    /// Interaction logic for CostumersListWindow.xaml
    /// </summary>
    public partial class CostumersListWindow : Window
    {
        private BlApi.IBL iBL;

        IEnumerable<BO.CostumerListBL> costumers = new List<BO.CostumerListBL>();

        public CostumersListWindow()
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.BackToMain(); };
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();

            this.costumers = this.iBL.GetCostumerList();

            CostumersListView.ItemsSource = this.costumers;
        }

        private void InputChanged(object o, EventArgs e)
        {
            string idStr = CostumerIdInput.Text;
            int id = 0;
            string name = CostumerNameInput.Text;
            string phone = CostumerPhoneInput.Text;
            
            errorMessage.Text = "";

            if (idStr != "" && !int.TryParse(idStr, out id))
            {
                errorMessage.Text = "id must be integer.";
            }

            //case 1: all filters
            if(idStr != "" && name != "" && phone != "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Id == id &&
                costumer.Name == name && costumer.Phone == phone );
            }

            //case 2: only id and name filtering
            else if (idStr != "" && name != "" && phone == "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Id == id && costumer.Name == name);
            }

            //case 3: only id and phone filtering
            else if (idStr != "" && name == "" && phone != "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Id == id && costumer.Phone == phone);
            }

            //case 3: only name and phone filtering
            else if (idStr == "" && name != "" && phone != "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Name == name && costumer.Phone == phone);
            }

            //case 4: Only id filtering
            else if (idStr != "" && name == "" && phone == "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Id == id);
            }

            //case 5: Only name filtering
            else if (idStr == "" && name != "" && phone == "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Name == name);
            }

            //case 6: Only phone filtering
            else if (idStr == "" && name == "" && phone != "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Phone == phone);
            }

            //Case 7: all filters clear
            else
            {
                this.costumers = this.iBL.GetCostumerList();
            }

            CostumersListView.ItemsSource = this.costumers;
        }

        private void ClearButtonOnClick(object o, EventArgs e)
        {
            errorMessage.Text = "";
            CostumerIdInput.Text = "";
            CostumerNameInput.Text = "";
            CostumerPhoneInput.Text = "";
        }

        private void AddCostumerButtonOnClick(object o, EventArgs e)
        {
            CostumerWindow nextWindow = new CostumerWindow(false);
            App.ShowWindow(nextWindow);
        }

        private void RemoveCostumerButtonOnClick(object o, EventArgs e)
        {
            //TODO: Handle Remove of costumer
            errorMessage.Text = "CostumerWindow has not been developed yet.";
        }

        private void CostumerView(object o, EventArgs e)
        {
            CostumerWindow nextWindow = new CostumerWindow(CostumersListView.SelectedItem);
            App.ShowWindow(nextWindow);
        }
    }
}
