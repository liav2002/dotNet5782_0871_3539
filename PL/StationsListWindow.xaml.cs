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
    /// Interaction logic for StationsListWindow.xaml
    /// </summary>
    public partial class StationsListWindow : Window
    {
        private BlApi.IBL iBL;

        IEnumerable<BO.StationListBL> drones = new List<BO.StationListBL>();

        public StationsListWindow()
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.BackToMain(); };
            this.Closing += App.Window_Closing;

            this.iBL = BlFactory.GetBl();

            
            //TODO: add delegates for buttons and implemented the code behind the gui. StationListWindow.xml

        }
    }
}
