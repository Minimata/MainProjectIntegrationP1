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
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;
using BluetoothRemoteControl;
using System.Windows.Threading;

namespace MainProjectIntegrationP1
{
    /// <summary>
    /// Interaction logic for DrivingControlPage.xaml
    /// </summary>
    public partial class DrivingControlPage : Page
    {
        //Variables declaration
        VisualDevice kinect;
        double bruteWheelSpeed, bruteWheelRotation;
        double bruteAssySpeed, bruteAssyRotation;
        DataProcessing processor;
        RobotSimulator robot;
        MainWindow parent;

        DataSmoother assyRotSmoother;
        DataSmoother assySpeedSmoother;
        DataSmoother wheelSpeedSmoother;
        DataSmoother wheelRotSmoother;
        DispatcherTimer dispatcherTimer = null;


        int speed;
        int rotation;

        public DrivingControlPage(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;

            if (!parent.bluetooth.isConnected)
                parent.Content = new RobotRadarPage(parent);

            //TrameSender frame = new TrameSender("770000", parent.bluetooth);
            
            Init();
            Subscribe();
            parent.bluetooth.sendToPairedRobot("77");
            parent.bluetooth.Listen();
            dispatcherTimer = new DispatcherTimer();
            armTimer();

        }

        public void armTimer()
        {
            dispatcherTimer.Tick += new EventHandler(TimerTick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dispatcherTimer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            parent.bluetooth.sendToPairedRobot("88"+speed+""+rotation);
        }

        public void Init()
        {
            assyRotSmoother = new DataSmoother();
            assySpeedSmoother = new DataSmoother();
            wheelSpeedSmoother = new DataSmoother();
            wheelRotSmoother = new DataSmoother();

            kinect = new VisualDevice();
            kinect.Load(parent.sensor);
            robot = new RobotSimulator();
        }

        public void Subscribe()
        {
            kinect.onColorFrameReady += new VisualDevice.VideoFrameReadyEventHandler(onColorFrameReadyEvent);
            kinect.onSkeletonFrameReady += new VisualDevice.SkeletonFrameReadyEventHandler(onSkeletonEvent);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           
            Console.WriteLine("ok");
        }

        //Methods where the magic happens.
        //Every action depending on Kinect's available frame and data is to be written here.


        private void onSkeletonEvent(object sender, EventArgs e, double[] data)
        {
            processor = new DataProcessing(data);

            bruteWheelRotation = processor.WheelRotation();
            bruteWheelSpeed = processor.WheelSpeed();
            bruteAssyRotation = processor.AssyRotation();
            bruteAssySpeed = processor.AssySpeed();

            labelLeftX.Content = "Left Hand Position in X-axis :" + Math.Round(data[0], 2).ToString();
            labelLeftY.Content = "Left Hand Position in Y-axis :" + Math.Round(data[1], 2).ToString();
            labelRightX.Content = "Right Hand Position in X-axis :" + Math.Round(data[2], 2).ToString();
            labelRightY.Content = "Right Hand Position in Y-axis :" + Math.Round(data[3], 2).ToString();

            assyRotSmoother.updateValue(bruteAssyRotation);
            assySpeedSmoother.updateValue(bruteAssySpeed);
            wheelSpeedSmoother.updateValue(bruteWheelSpeed);
            wheelRotSmoother.updateValue(bruteWheelRotation);

            double wheelRotValue = wheelRotSmoother.UpdateExponential();
            double wheelSpeedValue = wheelSpeedSmoother.UpdateExponential();
            double assySpeedValue = assySpeedSmoother.UpdateExponential();
            double assyRotValue = assyRotSmoother.UpdateExponential();

            labelWheelSpeed.Content = "Wheel Speed Value : " + Math.Round(wheelSpeedValue);
            labelWheelRotation.Content = "Wheel Rotation Value : " + Math.Round(wheelRotValue);
            labelAssySpeed.Content = "Assymetric Speed Value : " + Math.Round(assySpeedValue);
            labelAssyRotation.Content = "Assymetric Rotation Value : " + Math.Round(assyRotValue);

            speed = processor.ValueToPourcentage("wheelSpeed", wheelSpeedValue);
            rotation = processor.ValueToPourcentage("wheelRotation", wheelRotValue);
            //La méthode ValueToPourcentage retourn un Int16 et prend en paramères une string et un double.
            //Exemples d'utilisation de la méthode de transformation des valeurs pour le format de la trame.

           
            
            //TrameSender frame = new TrameSender("885050",parent.bluetooth);
            //parent.bluetooth.sendToPairedRobot("880000");

            //processor.ValueToPourcentage("assyRotation", assyRotValue);
            //processor.ValueToPourcentage("assySpeed", assySpeedValue);

            //robot.directionAngle = bruteAssyRotation;
            //robot.speed = bruteAssySpeed;
            //robot.update();
        }

        private void onColorFrameReadyEvent(object sender, EventArgs e, ImageSource bSource)
        {
            this.ImageVideo.Source = bSource;
        }
    }
}
