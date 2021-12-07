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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            menuButtons = new Button[] { DronesButton, ParcelsButton, CostumersButton, StationButton };
            SetMenuButtonsActive(false);
        }

        public void QuitOnClick(object o, EventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }

        public void DronesOnClick(object o, EventArgs e)
        {

        }

        public void ParcelsOnClick(object o, EventArgs e)
        {

        }

        public void CostumersOnClick(object o, EventArgs e)
        {

        }

        public void StationsOnClick(object o, EventArgs e)
        {

        }

        private void SetMenuButtonsActive(bool active)
        {
            for (int i = 0; i < menuButtons.Length; i++)
            {
                menuButtons[i].IsEnabled = active;
            }
        }

        private Button[] menuButtons;
    }
}
