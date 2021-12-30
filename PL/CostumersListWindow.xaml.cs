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
            this.Closing += App.PrevWindow;

            this.iBL = BlFactory.GetBl();

            this.costumers = this.iBL.GetCostumerList();

            CostumersListView.ItemsSource = this.costumers;

            BlockCostumerButton.Visibility = Visibility.Collapsed;

            SetListViewForeground();
        }

        private void SetListViewForeground()
        {
            //TODO: Try do implement this function (Foreground List View).
            //The function set the foreground of unvaliable items as red.

            //for (int i = 0; i < DronesListView.Items.Count; ++i)
            //{
            //    var item = DronesListView.ItemContainerGenerator.ContainerFromItem(i) as ListViewItem;

            //    if (((BO.DroneBL)item.Content).IsAvliable == false)
            //    {
            //        item.Foreground = Brushes.Red;
            //    }
            //}
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
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Id.ToString().Contains(id.ToString()) &&
                costumer.Name.Contains(name) && costumer.Phone.Contains(phone) );
            }

            //case 2: only id and name filtering
            else if (idStr != "" && name != "" && phone == "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Id.ToString().Contains(id.ToString()) && costumer.Name.Contains(name));
            }

            //case 3: only id and phone filtering
            else if (idStr != "" && name == "" && phone != "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Id.ToString().Contains(id.ToString()) && costumer.Phone.Contains(phone));
            }

            //case 3: only name and phone filtering
            else if (idStr == "" && name != "" && phone != "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Name.Contains(name) && costumer.Phone.Contains(phone));
            }

            //case 4: Only id filtering
            else if (idStr != "" && name == "" && phone == "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Id.ToString().Contains(id.ToString()));
            }

            //case 5: Only name filtering
            else if (idStr == "" && name != "" && phone == "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Name.Contains(name));
            }

            //case 6: Only phone filtering
            else if (idStr == "" && name == "" && phone != "")
            {
                this.costumers = this.iBL.GetCostumerList(costumer => costumer.Phone.Contains(phone));
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
            CostumersListView.SelectedItem = null;
        }

        private void AddCostumerButtonOnClick(object o, EventArgs e)
        {
            CostumerWindow nextWindow = new CostumerWindow(false);
            App.ShowWindow(nextWindow);
        }

        private void BlockCostumerButtonOnClick(object o, EventArgs e)
        {
            try
            {
                if (CostumersListView.SelectedItem != null)
                {
                    if(BlockCostumerButton.Content.ToString() == "Block")
                    {
                        this.iBL.RemoveCostumer(((BO.CostumerListBL)CostumersListView.SelectedItem).Id);
                        CostumersListView.ItemsSource = this.iBL.GetCostumerList();
                        SetListViewForeground();
                    }

                    else if(BlockCostumerButton.Content.ToString() == "Unblock")
                    {
                        this.iBL.RestoreCostumer(((BO.CostumerListBL)CostumersListView.SelectedItem).Id);
                        CostumersListView.ItemsSource = this.iBL.GetCostumerList();
                        SetListViewForeground();
                    }
                }

                else
                {
                    errorMessage.Text = "You need to select costumer first.";
                }
            }

            catch (Exception ex)
            {
                errorMessage.Text = ex.Message;
            }
        }

        private void CostumerSelected(object o, EventArgs e)
        {
            if(CostumersListView.SelectedItem != null)
            {
                BO.CostumerBL costumer = this.iBL.GetCostumerById(((BO.CostumerListBL)CostumersListView.SelectedItem).Id);

                BlockCostumerButton.Visibility = Visibility.Visible;

                if (costumer.IsAvaliable)
                {
                    BlockCostumerButton.Content = "Block";
                }

                else
                {
                    BlockCostumerButton.Content = "Unblock";
                }
            }

            else
            {
                BlockCostumerButton.Visibility = Visibility.Collapsed;
            }
        }

        private void CostumerView(object o, EventArgs e)
        {
            CostumerWindow nextWindow = new CostumerWindow(CostumersListView.SelectedItem);
            App.ShowWindow(nextWindow);
        }
    }
}
