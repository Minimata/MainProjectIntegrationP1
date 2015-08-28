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
using BluetoothZeuGroupeLib;

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
   

        bool started = false;
        int speed;
        int rotation;

        public DrivingControlPage(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;            
            Init();
            Subscribe();
            //parent.bluetooth.Listen();
            //parent.bluetooth.onReceiveMessage += new BluetoothClientModule.onReceiveMessageDelegate(onMessage);
            dispatcherTimer = new DispatcherTimer();
            armTimer();
        }

        private void onMessage(string name)
        {
            //lblConsole.Text += "The robot says : "+name+"\n";
        }

        public void armTimer()
        {
            dispatcherTimer.Tick += new EventHandler(TimerTick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            parent.bluetooth.sendToPairedRobot("88"+speed+""+rotation);
            //parent.bluetooth.sendToPairedRobot("880000");
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
            lblConsole.Text += "Kinect operationnal \n";
        }

        public void Subscribe()
        {
            kinect.onColorFrameReady += new VisualDevice.VideoFrameReadyEventHandler(onColorFrameReadyEvent);
            kinect.onSkeletonFrameReady += new VisualDevice.SkeletonFrameReadyEventHandler(onSkeletonEvent);
        }


        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            started = !started;
            if(started == true)
            {
                parent.bluetooth.sendToPairedRobot("77");
                startBtn.Content = "STOP";
                lblConsole.Text += "77 code sended\n";
                dispatcherTimer.Start();
            }
            else if (started == false)
            {
                parent.bluetooth.sendToPairedRobot("99");
                startBtn.Content = "START";
                lblConsole.Text += "99 code sended\n";
                dispatcherTimer.Stop();
            }
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
            updateCompassWidget(wheelRotValue);
            updatePowerBar(speed,wheelSpeedValue);
            lblConsole.Text += "88" + speed+""+rotation+"\n";
            scroolConsole.ScrollToEnd();
            //La méthode ValueToPourcentage retourn un Int16 et prend en paramères une string et un double.
            //Exemples d'utilisation de la méthode de transformation des valeurs pour le format de la trame.
        }

        public void updateCompassWidget(double rot)
        {
            RotateTransform rotation = new RotateTransform();
            Rectangle shape;
            Line lineV = new Line();
            Line lineH = new Line();
            shape = new Rectangle();
            shape.Stroke = new SolidColorBrush(Colors.Black);
            shape.Fill = new SolidColorBrush(Colors.Black);
            shape.Width = 250;
            shape.Height = 5;
            shape.RenderTransformOrigin = new Point(0.5, 0.5);
            rotation.Angle = rot * -1;
            shape.RenderTransform = rotation;
            
            lineV.Stroke = Brushes.LightSteelBlue;

            lineV.X1 = compassCanvas.ActualWidth / 2;
            lineV.X2 = compassCanvas.ActualWidth / 2;
            lineV.Y1 = 0;
            lineV.Y2 = compassCanvas.ActualHeight;

            lineV.StrokeThickness = 2;
            lineH.Stroke = Brushes.LightSteelBlue;

            lineH.X1 = 0;
            lineH.X2 = compassCanvas.ActualWidth;
            lineH.Y1 = compassCanvas.ActualHeight / 2;
            lineH.Y2 = compassCanvas.ActualHeight / 2;

            lineH.StrokeThickness = 2;
            compassCanvas.Children.Clear();
            compassCanvas.Children.Add(lineH);
            compassCanvas.Children.Add(lineV);
            compassCanvas.Children.Add(shape);

            Canvas.SetLeft(shape, compassCanvas.ActualWidth / 2 - (250/2));
            Canvas.SetTop(shape, compassCanvas.ActualHeight / 2);
        }

        private void updatePowerBar(double rawSpeedValuePourcentage,double rawSpeedValue)
        {
            if (rawSpeedValue < 10 )
            {
                enginePowerBar.Foreground = new SolidColorBrush(Colors.Yellow);
                rawSpeedValuePourcentage = 100;

            }
            else if (rawSpeedValue > 10 && rawSpeedValue < 50)
            {
                enginePowerBar.Foreground = new SolidColorBrush(Colors.Green);
                rawSpeedValuePourcentage = 100;
            }
            else
                enginePowerBar.Foreground = new SolidColorBrush(Colors.Blue);

            enginePowerBar.Value = 2 * (rawSpeedValuePourcentage - 50);
        }

        private void onColorFrameReadyEvent(object sender, EventArgs e, ImageSource bSource)
        {

            this.ImageVideo.Source = bSource;
        }
    }
}
