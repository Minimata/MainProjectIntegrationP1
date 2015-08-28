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
    /// Interaction logic for SimulatorPage.xaml
    /// </summary>
    public partial class SimulatorPage : Page
    {
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

        public SimulatorPage(MainWindow parent)
        {
            InitializeComponent();

            this.parent = parent;

            // Init button
            double buttonWidth = this.parent.Width / 4;
            double buttonHeight = this.parent.Height / 4;
            buttonCanvas.Width = buttonWidth;
            buttonCanvas.Height = buttonHeight;

            Init();
            Subscribe();
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

        private void onColorFrameReadyEvent(object sender, EventArgs e, ImageSource bSource)
        {
            this.ImageVideo.Source = bSource;
        }

        private void onSkeletonEvent(object sender, EventArgs e, double[] data)
        {
            processor = new DataProcessing(data);

            bruteWheelRotation = processor.WheelRotation();
            bruteWheelSpeed = processor.WheelSpeed();
            //bruteAssyRotation = processor.AssyRotation();
            //bruteAssySpeed = processor.AssySpeed();

            //assyRotSmoother.updateValue(bruteAssyRotation);
            //assySpeedSmoother.updateValue(bruteAssySpeed);
            wheelSpeedSmoother.updateValue(bruteWheelSpeed);
            wheelRotSmoother.updateValue(bruteWheelRotation);

            double wheelRotValue = wheelRotSmoother.UpdateExponential();
            double wheelSpeedValue = wheelSpeedSmoother.UpdateExponential();
            //double assySpeedValue = assySpeedSmoother.UpdateExponential();
            //double assyRotValue = assyRotSmoother.UpdateExponential();

            robot.directionAngle += wheelRotValue;
            robot.speed = wheelSpeedValue;
            robot.update();
            MainCanvas.Children.Clear();
            robot.draw(MainCanvas);

            if (robot.x > this.Width)
            {
                robot.x = 0;
            }
            else if (robot.x < 0)
            {
                robot.x = this.Width;
            }

            else if (robot.y < 10)
            {
                robot.y = this.Height;
            }

            else if (robot.y > this.Height)
            {
                robot.y = 10;
            }
        }


    }
}
