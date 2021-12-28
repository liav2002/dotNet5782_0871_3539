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
using BlApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BlApi.IBL iBL;

        public MainWindow()
        {
            InitializeComponent();

            this.iBL = BlFactory.GetBl();

            SignoutState.Visibility = Visibility.Visible;
            SigninAsManagerState.Visibility = Visibility.Hidden;
            SigninAsCostumerState.Visibility = Visibility.Hidden;

            helloUserLabel.Content = "hello {[name]}";
            helloUserLabel.Visibility = Visibility.Collapsed;
        }

        public MainWindow(bool isManager)
        {
            InitializeComponent();

            this.iBL = BlFactory.GetBl();

            SignoutState.Visibility = Visibility.Hidden;

            if (isManager)
            {
                SigninAsManagerState.Visibility = Visibility.Visible;
                SigninAsCostumerState.Visibility = Visibility.Hidden;
            }

            else
            {
                SigninAsManagerState.Visibility = Visibility.Hidden;
                SigninAsCostumerState.Visibility = Visibility.Visible;
            }

            helloUserLabel.Content = helloUserLabel.Content.ToString().Replace("{[name]}", this.iBL.GetLoggedUser().Name);
            helloUserLabel.Visibility = Visibility.Visible;
        }

        public void SignInOnClick(object o, EventArgs e)
        {
            if(inputUser.Text == "" || inputPass.Password == "")
            {
                errorMessage.Text = "Please enter username and password.";
                return;
            }

            try
            {
                this.iBL.SignIn(inputUser.Text, inputPass.Password);

                helloUserLabel.Content = helloUserLabel.Content.ToString().Replace("{[name]}", this.iBL.GetLoggedUser().Name);
                helloUserLabel.Visibility = Visibility.Visible;

                SignoutState.Visibility = Visibility.Hidden;

                if (this.iBL.GetLoggedUser().IsManager)
                {
                    SigninAsManagerState.Visibility = Visibility.Visible;
                    SigninAsCostumerState.Visibility = Visibility.Hidden;
                }

                else
                {
                    SigninAsManagerState.Visibility = Visibility.Hidden;
                    SigninAsCostumerState.Visibility = Visibility.Visible;
                }
            }

            catch(Exception ex)
            {
                errorMessage.Text = ex.Message;
            }
        }

        public void SignUpOnClick(object o, EventArgs e)
        {
            CostumerWindow nextWindow = new CostumerWindow(true);
            App.ShowWindow(nextWindow);
        }

        public void SignOutOnClick(object o, EventArgs e)
        {
            //Resets login details
            inputUser.Text = "";
            inputPass.Password = "";
            errorMessage.Text = "";

            this.iBL.SignOut();

            helloUserLabel.Content = "hello {[name]}";
            helloUserLabel.Visibility = Visibility.Collapsed;

            SignoutState.Visibility = Visibility.Visible;
            SigninAsManagerState.Visibility = Visibility.Hidden;
            SigninAsCostumerState.Visibility = Visibility.Hidden;
        }

        public void InputChange(object o, EventArgs e)
        {
            errorMessage.Text = "";
        }

        public void QuitOnClick(object o, EventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }

        public void DronesOnClick(object o, EventArgs e)
        {
            App.ShowWindow<DronesListWindow>();
        }

        public void ParcelsOnClick(object o, EventArgs e)
        {
            App.ShowWindow<ParcelsListWindow>();
        }

        public void CostumersOnClick(object o, EventArgs e)
        {
            App.ShowWindow<CostumersListWindow>();

        }

        public void StationsOnClick(object o, EventArgs e)
        {
            App.ShowWindow<StationsListWindow>();
        }

        public void MyParcelsOnClick(object o, EventArgs e)
        {

        }

        public void MyDetailsOnClick(object o, EventArgs e)
        {
            CostumerWindow nextWindow = new CostumerWindow(this.iBL.GetLoggedUser());
            App.ShowWindow(nextWindow);
        }
    }
}
