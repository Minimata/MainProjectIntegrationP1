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
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Interaction;
using BluetoothZeuGroupeLib;
using InTheHand.Net.Bluetooth;

namespace MainProjectIntegrationP1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public KinectSensor sensor;
        public KinectSensorChooser sensorChooser;
        public BluetoothClientModule bluetooth;
        public String playerName = "ZPlayer";

        public MainWindow()
        {
            InitializeComponent();
            initBluetooth();
            initKinectInteraction();

            Closed += new EventHandler(onWindowClosed);
        }

        private void onWindowClosed(object sender, EventArgs e)
        {
            bluetooth.sendToPairedRobot("99");
            bluetooth.closeConnection();
        }

        public void initKinectInteraction()
        {
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();
        }

        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs e)
        {
            sensor = e.NewSensor;
            sensor.Start();
            sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            sensor.SkeletonStream.Enable();
            sensor.DepthStream.Range = DepthRange.Near;
            sensor.SkeletonStream.EnableTrackingInNearRange = true;
            sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            kinectRegion.KinectSensor = sensor;

            //Lance la page principale...
            startProgram();
            
        }

        private void startProgram()
        {
            this.Content = new MainPage(this);
            //this.Content = new DrivingControlPage(this);
        }

        public void initBluetooth()
        {
            List<BluetoothRadio> radios = BluetoothClientModule.getAllBluetoothAdapters();
            bluetooth = new BluetoothClientModule(radios.First().LocalAddress.ToString());
            //bluetooth.onConnectionEnded_Event += new BluetoothClientModule.onConnectionEnded(onBluetoothConnectionEnd);
        }

        /// <summary>
        /// Callback lorsque la connexion est terminé ou interrompu
        /// </summary>
        /// <param name="cause">cause de la fermeture</param>
        //public void onBluetoothConnectionEnd(string cause)
        //{
        //    System.Environment.Exit(0);
        //}
    }
}
