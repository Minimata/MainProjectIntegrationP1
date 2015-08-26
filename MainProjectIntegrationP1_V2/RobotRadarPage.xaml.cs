using BluetoothZeuGroupeLib;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        KinectSensorChooser sensorChooser;
        BitmapImage bi = new BitmapImage(new Uri("robot.png", UriKind.Relative));
        String robotToPair;
        
        public RobotRadarPage(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.sensorChooser = parent.sensorChooser;
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged2;
            this.sensorChooserUi.KinectSensorChooser = sensorChooser;
            this.sensorChooser.Start();
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
            initBluetoothRadar();
        }
        private void SensorChooserOnKinectChanged2(object sender, KinectChangedEventArgs args)
        {

            bool error = false;
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                    args.OldSensor.ColorStream.Disable();
                }
                catch (InvalidOperationException inEX) { error = true; }
            }

            if (args.NewSensor != null)
            {

                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    //args.NewSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();
                    args.NewSensor.ColorStream.Enable();
                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                        args.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    }
                    catch (InvalidOperationException inEX)
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                        error = true;
                    }
                }
                catch (InvalidOperationException inEX) { error = true; }
            }
            if (!error) { kinectRegion.KinectSensor = args.NewSensor; }
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
            btnRefresh.Visibility = Visibility.Visible;
            lblScan.Visibility = Visibility.Hidden;
        }

        private void onDiscover(string name)
        {
            robotsNames.Add(name);
            System.Diagnostics.Debug.WriteLine("Robot " + name + " Découvert");
            KinectTileButton b = new KinectTileButton();
            b.Label = name;
            b.Click += btn_Click;
            b.Background = new ImageBrush(bi);
            this.scrollContent.Children.Add(b);
            
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            //scrollContent.Visibility = Visibility.Hidden;
            //pairingLbl.Visibility = Visibility.Visible;
            KinectTileButton b = sender as KinectTileButton;
            robotToPair = b.Label.ToString();
            statusLbl.Content = "Pairage en cours... veuillez patientez";
            parent.bluetooth.pairToRobot(robotsNames.IndexOf(robotToPair));
            parent.Content = new DrivingControlPage(parent);
            ////Lance l'écoute de flux en async
            //Thread t = new Thread(new ThreadStart(pairing));
            //t.Name = "Pairing Thread";
            //t.Start();
        }
        private void pairing()
        {
            parent.bluetooth.pairToRobot(robotsNames.IndexOf(robotToPair));
            this.Content = new DrivingControlPage(parent);
            Thread.CurrentThread.Abort();
        }

        private void KinectTileButton_Click(object sender, RoutedEventArgs e)
        {
            robotsNames.Clear();
            scrollContent.Children.Clear();
            btnRefresh.Visibility = Visibility.Hidden;
            lblScan.Visibility = Visibility.Visible;
            parent.bluetooth.scanRobots();

        }
    }
}
