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

        public static void ShowMainWindow()
        {
            ShowWindow(mainWindow);
        }

        public static void CloseMainWindow()
        {
            mainWindow.Close();
        }

        public static MainWindow GetMainWindow()
        {
            return mainWindow;
        }

        public static void Window_Closing(object o, EventArgs e)
        {
            if (closeAllWindows)
            {
                CloseMainWindow();
            }
            closeAllWindows = true;
        }

        public static void BackToMain()
        {
            closeAllWindows = false;
            currentWindow.Close();
            ShowWindow(mainWindow);
        }

        public static bool closeAllWindows;
        private static MainWindow mainWindow;
        private static Window currentWindow;
    }

    
}
