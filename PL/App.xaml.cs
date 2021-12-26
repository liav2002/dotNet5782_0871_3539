using BO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void Application_Startup(object sender, StartupEventArgs e)
        {
            mainWindow = new MainWindow();
            ShowWindow(mainWindow);
        }

        public static void ShowWindow(Window nextWindow)
        {
            if (currentWindow != null)
            {
                nextWindow.Left = currentWindow.Left;
                nextWindow.Top = currentWindow.Top;
                currentWindow.Hide();
            }

            nextWindow.Show();

            currentWindow = nextWindow;
        }

        public static void ShowWindow<MyWindow>() where MyWindow : Window, new()
        {
            MyWindow nextWindow = new MyWindow();
            ShowWindow(nextWindow);
        }

        public static MainWindow GetMainWindow()
        {
            return mainWindow;
        }

        public static void Window_Closing(object o, EventArgs e)
        {
            mainWindow.Close();
            Application.Current.Shutdown();
        }

        public static void BackToMain()
        {
            mainWindow.errorMessage.Text = "";
            ShowWindow(mainWindow);
        }

        private static MainWindow mainWindow;
        private static Window currentWindow;
    }

    
}
