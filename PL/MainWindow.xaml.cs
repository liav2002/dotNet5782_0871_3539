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
            menuButtons = new Button[] { DronesButton, ParcelsButton, CostumersButton, StationsButton };
            SetMenuButtonsActive(false);
            this.iBL = BlFactory.GetBl();
        }

        public void SignInOnClick(object o, EventArgs e)
        {
            try
            {
                //TODO: implemnted data-base, try to sign in
                //for example: App.SignIn(inputUser.Text, inputPass.Text);
                //the function get access to sql database and check if the name and passwords are suitable.
                //if they suitable: init currentUser in DataSource. else: throw suitable exception. 
                
                helloUserLabel.Content = helloUserLabel.Content.ToString().Replace("{[name]}", inputUser.Text);
                helloUserLabel.Visibility = Visibility.Visible;
                loginCanvas.Visibility = Visibility.Collapsed;
                signOutButton.Visibility = Visibility.Visible;
                SetMenuButtonsActive(true);

            }
            catch(Exception ex)
            {
                errorMessage.Text = ex.Message;
            }
        }

        public void SignUpOnClick(object o, EventArgs e)
        {
            App.ShowWindow<SignUpWindow>();
        }

        public void SignOutOnClick(object o, EventArgs e)
        {
            loginCanvas.Visibility = Visibility.Visible;

            helloUserLabel.Content = "Hello {[name]}";
            helloUserLabel.Visibility = Visibility.Collapsed;

            signOutButton.Visibility = Visibility.Collapsed;

            SetMenuButtonsActive(false);

            //Resets login details
            inputUser.Text = "";
            inputPass.Password = "";
            errorMessage.Text = "";

            //TODO: Implemted App.SignOut()
            //the function delete the currentUser variable in dal. make him null.
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
