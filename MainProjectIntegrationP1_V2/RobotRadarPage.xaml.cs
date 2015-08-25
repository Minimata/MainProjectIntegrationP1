using BluetoothZeuGroupeLib;
using Microsoft.Kinect.Toolkit.Controls;
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

namespace MainProjectIntegrationP1
{
    /// <summary>
    /// Interaction logic for RobotRadarPage.xaml
    /// </summary>
    public partial class RobotRadarPage : Page
    {

        MainWindow parent;
        List<String> robotsNames = new List<String>();
        bool scanDone = false;

        public RobotRadarPage(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
            initBluetoothRadar();
        }

        private void initBluetoothRadar()
        {
            parent.bluetooth.onRobotDiscovered_Event += new BluetoothClientModule.onRobotDiscovered(onDiscover);
            parent.bluetooth.onRobotsDiscoverDone_Event += new BluetoothClientModule.onRobotsDiscoverDone(onDiscoverDone);
            parent.bluetooth.scanRobots();
        }

        private void onDiscoverDone(List<string> robotsNames)
        {
            scanDone = true;
        }

        private void onDiscover(string name)
        {
            robotsNames.Add(name);
            System.Diagnostics.Debug.WriteLine("Robot " + name + " Découvert");
            KinectTileButton b = new KinectTileButton();
            b.Label = name;
            b.Click += btn_Click;
            this.scrollContent.Children.Add(b);
            
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            scrollContent.Visibility = Visibility.Hidden;
            pairingLbl.Visibility = Visibility.Visible;
            KinectTileButton b = sender as KinectTileButton;
            parent.bluetooth.pairToRobot(robotsNames.IndexOf(b.ToString()));

        }

    }
}
