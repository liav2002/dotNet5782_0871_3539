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

namespace PL
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
            ReturnButton.Click += delegate { App.BackToMain(); };
            this.Closing += App.Window_Closing;
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            errorMessage.Text = "";
        }

        public void SignupOnClick(object o, EventArgs e)
        {
            try
            {
                //TODO: App.SignUp(inputUser.Text, inputPass.Text, inputEmail.Text, inputId.Text, inputPhone.Text);
                //this function add new costumer to the database users table. need to calculate location of user.
                App.BackToMain();
            }

            catch (Exception ex)
            {
                errorMessage.Text = ex.Message;
            }
        }
    }
}
