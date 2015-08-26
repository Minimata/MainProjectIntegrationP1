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

        public DrivingControlPage(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
            Init();
            Subscribe();
        }

        public void Init()
        {
                kinect = new VisualDevice();
                kinect.Load(parent.sensor);
                robot = new RobotSimulator();
        }

        public void Subscribe()
        {
            kinect.onColorFrameReady += new VisualDevice.VideoFrameReadyEventHandler(onColorFrameReadyEvent);
            kinect.onSkeletonFrameReady += new VisualDevice.SkeletonFrameReadyEventHandler(onSkeletonEvent);
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

            DataSmoother assyRotSmoother = new DataSmoother(bruteAssyRotation);
            DataSmoother assySpeedSmoother = new DataSmoother(bruteAssySpeed);
            DataSmoother wheelSpeedSmoother = new DataSmoother(bruteWheelSpeed);
            DataSmoother wheelRotSmoother = new DataSmoother(bruteWheelRotation);

            labelWheelSpeed.Content = "Wheel Speed Value : " + Math.Round(wheelSpeedSmoother.UpdateExponential());
            labelWheelRotation.Content = "Wheel Rotation Value : " + Math.Round(wheelRotSmoother.UpdateExponential());
            labelAssySpeed.Content = "Assymetric Speed Value : " + Math.Round(assySpeedSmoother.UpdateExponential());
            labelAssyRotation.Content = "Assymetric Rotation Value : " + Math.Round(assyRotSmoother.UpdateExponential());

            robot.directionAngle = bruteAssyRotation;
            robot.speed = bruteAssySpeed;
            robot.update();
        }

        private void onColorFrameReadyEvent(object sender, EventArgs e, ImageSource bSource)
        {
            this.ImageVideo.Source = bSource;
        }
    }
}
