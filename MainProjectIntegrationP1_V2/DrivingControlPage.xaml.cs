using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using BluetoothZeuGroupeLib;
using System.Timers;

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
        //DispatcherTimer dispatcherTimer = null;
        Timer timer;
        String drivingMode = "wheel";
   

        bool started = false;
        int speed;
        int rotation;

        RotateTransform rotationCompass = new RotateTransform();
        Rectangle shape;
        Line lineV = new Line();
        Line lineH = new Line();

        public DrivingControlPage(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;            
            Init();
            Subscribe();
            parent.bluetooth.Listen();
            parent.bluetooth.onReceiveMessage += new BluetoothClientModule.onReceiveMessageDelegate(onMessage);
            parent.bluetooth.onConnectionEnded_Event += new BluetoothClientModule.onConnectionEnded(onConnectionEnded);
            //dispatcherTimer = new DispatcherTimer();
            armTimer();

            shape = new Rectangle();
            shape.Stroke = new SolidColorBrush(Colors.Black);
            shape.Fill = new SolidColorBrush(Colors.Black);
            lblPlayerName.Content = parent.playerName;
        }

        private void onConnectionEnded(string cause)
        {
            if(cause.Equals("cut"))
            {
                MessageBox.Show("Connection coupé !");
                //Retenter une connexion...
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void onMessage(string name)
        {
            lblConsole.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (name.Substring(0, 2).Equals("88"))
                    lblConsole.Text += "robot responded everything is ok\n";
                else
                    lblConsole.Text += "robot says : " + name + "\n";
            }), (DispatcherPriority)10);
        }

        public void armTimer()
        {
            timer = new Timer(100);
            timer.Elapsed += sysTimerTick;
        }

        private void sysTimerTick(object sender, ElapsedEventArgs e)
        {
            parent.bluetooth.sendToPairedRobot("88" + speed + "" + rotation);
        }


        public void Init()
        {
            assyRotSmoother = new DataSmoother(0.62);
            assySpeedSmoother = new DataSmoother(0.62);
            wheelSpeedSmoother = new DataSmoother(0.75);
            wheelRotSmoother = new DataSmoother(0.5);

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
                timer.Start();
            }
            else if (started == false)
            {
                parent.bluetooth.sendToPairedRobot("99");
                startBtn.Content = "START";
                lblConsole.Text += "99 code sended\n";
                timer.Stop();
            }

            lblConsole.Text += "Button start / stop clicked \n";
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

            assyRotSmoother.updateValue(bruteAssyRotation);
            assySpeedSmoother.updateValue(bruteAssySpeed);
            wheelSpeedSmoother.updateValue(bruteWheelSpeed);
            wheelRotSmoother.updateValue(bruteWheelRotation);

            double wheelRotValue = wheelRotSmoother.UpdateExponential();
            double wheelSpeedValue = wheelSpeedSmoother.UpdateExponential();
            double assySpeedValue = assySpeedSmoother.UpdateExponential();
            double assyRotValue = assyRotSmoother.UpdateExponential();

            if(drivingDebugCheckBox.IsChecked.Value)
            {
                lblConsole.Text += "Wheel Speed Value : " + Math.Round(wheelSpeedValue) + "\n";
                lblConsole.Text += "Wheel Rotation Value : " + Math.Round(wheelRotValue) + "\n";
                lblConsole.Text += "Assymetric Speed Value : " + Math.Round(assySpeedValue) + "\n";
                lblConsole.Text += "Assymetric Rotation Value : " + Math.Round(assyRotValue) + "\n";
            }
            

            switch(drivingMode)
            {
                case "assy":
                    speed = processor.ValueToPourcentage("assySpeed", assySpeedValue);
                    rotation = processor.ValueToPourcentage("assyRotation", assyRotValue);
                    updateCompassWidget(assyRotValue);
                    updatePowerBar(speed, assySpeedValue);
                    break;
                case "wheel":
                    speed = processor.ValueToPourcentage("wheelSpeed", wheelSpeedValue);
                    rotation = processor.ValueToPourcentage("wheelRotation", wheelRotValue);
                    updateCompassWidget(wheelRotValue);
                    updatePowerBar(speed, wheelSpeedValue);
                    break;
            }
            

            scroolConsole.ScrollToEnd();
            lblConsole.Text = lblConsole.Text.ToString().Substring((int)lblConsole.Text.Length / 6);
            //La méthode ValueToPourcentage retourn un Int16 et prend en paramères une string et un double.
            //Exemples d'utilisation de la méthode de transformation des valeurs pour le format de la trame.
        }

        public void updateCompassWidget(double rot)
        {
            shape.Width = 250;
            shape.Height = 5;
            shape.RenderTransformOrigin = new Point(0.5, 0.5);
            rotationCompass.Angle = rot * -1;
            shape.RenderTransform = rotationCompass;   
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

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            drivingMode = "wheel";
        }

        private void radioButton2_Checked(object sender, RoutedEventArgs e)
        {
            drivingMode = "assy";
        }

        private void button_Click_deco(object sender, RoutedEventArgs e)
        {
            parent.bluetooth.sendToPairedRobot("99");
            lblConsole.Text += "99 code sended\n";
            timer.Stop();
            parent.bluetooth.reset();
            parent.Content = new MainPage(parent);
        }

        private void updatePowerBar(double rawSpeedValuePourcentage,double rawSpeedValue)
        {
            enginePowerBar.Value = 2 * (rawSpeedValuePourcentage - 50);

            if (rawSpeedValuePourcentage < 50)
                lblDir.Content = "BW <<";
            else if (rawSpeedValuePourcentage > 50)
                lblDir.Content = "FW >>";
            else
                lblDir.Content = "STOP ||";
        }

        private void onColorFrameReadyEvent(object sender, EventArgs e, ImageSource bSource)
        {
            this.ImageVideo.Source = bSource;
        }
    }
}
