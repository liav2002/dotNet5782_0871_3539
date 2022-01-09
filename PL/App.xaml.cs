using BO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.ComponentModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    internal class Pair
    {
        public Window win;
        public Action func;


        public Pair(Window w, Action f)
        {
            win = w;
            func = f;
        }
    }

    public partial class App : Application
    {
        public static bool IsDroneSimulate(int droneId)
        {
            return _process.Keys.ToList().Contains(droneId);
        }

        public static void StopWorker(BO.DroneBL drone)
        {
            BackgroundWorker worker = _process[drone.Id];
            worker.Dispose();
            _process.Remove(drone.Id);
        }

        public static void AddWorker(BO.DroneBL drone, BackgroundWorker worker)
        {
            _process.Add(drone.Id, worker);
        }

        public static BackgroundWorker GetWorker(BO.DroneBL drone)
        {
            return _process[drone.Id];
        }

        void Application_Startup(object sender, StartupEventArgs e)
        {
            windows = new Stack<Pair>();
            NextWindow(new MainWindow());
        }

        public static void NextWindow(Window nextWindow, Action onPop = null)
            // the onPop parameter: when the nextWindow is pop the onPop will called
        {
            if (windows.Count != 0)
            {
                Window currentWindow = windows.Peek().win;

                nextWindow.Left = currentWindow.Left;
                nextWindow.Top = currentWindow.Top;
                currentWindow.Hide();
            }

            windows.Push(new Pair(nextWindow, onPop));
            nextWindow.Show();
        }

        public static void PrevWindow()
        {
            Pair current = windows.Pop();
            Window prevWindow = windows.Peek().win;

            prevWindow.Left = current.win.Left;
            prevWindow.Top = current.win.Top;
            current.win.Hide();

            prevWindow.Show();
            if (current.func != null)
                current.func();
        }

        public static void Window_Closing(object o, EventArgs e)
        {
            while (windows.Count != 0)
            {
                windows.Pop().win.Close();
            }

            Application.Current.Shutdown();
        }

        private static Stack<Pair> windows;
        private static Dictionary<int, BackgroundWorker> _process = new Dictionary<int, BackgroundWorker>();
    }
}