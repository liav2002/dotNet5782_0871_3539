using BO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;

namespace PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void Application_Startup(object sender, StartupEventArgs e)
        {
            windows = new Stack<Window>();
            NextWindow(new MainWindow());
        }

        public static void NextWindow(Window nextWindow)
        {
            if(windows.Count != 0)
            {
                Window currentWindow = windows.Peek();

                nextWindow.Left = currentWindow.Left;
                nextWindow.Top = currentWindow.Top;
                currentWindow.Hide();
            }

            windows.Push(nextWindow);
            nextWindow.Show();
        }

        public static void PrevWindow()
        {
            Window currentWindow = windows.Pop();
            Window prevWindow = windows.Peek();

            prevWindow.Left = currentWindow.Left;
            prevWindow.Top = currentWindow.Top;
            currentWindow.Hide();

            prevWindow.Show();
        }

        public static void Window_Closing(object o, EventArgs e)
        {
            while(windows.Count != 0)
            {
                windows.Pop().Close();
            }

            Application.Current.Shutdown();
        }

        private static Stack<Window> windows;
    }

    
}
