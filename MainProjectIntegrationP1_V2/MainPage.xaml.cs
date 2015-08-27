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

namespace MainProjectIntegrationP1
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        MainWindow parent;
        KinectSensorChooser sensorChooser;

        public MainPage(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
            //this.parent.Width = SystemParameters.WorkArea.Width;
            //this.parent.Height = SystemParameters.WorkArea.Height;
            this.parent.Width = SystemParameters.FullPrimaryScreenWidth;
            this.parent.Height = SystemParameters.FullPrimaryScreenHeight;
            


            //Variables
            double buttonWidth, buttonHeight, spaceWidth, spaceHeight1, spaceHeight2;

            //Set Variables
            buttonHeight = this.parent.Height / 4;
            buttonWidth = this.parent.Width / 4;
            spaceWidth = this.parent.Width / 16;
            spaceHeight1 = this.parent.Height / 4;
            spaceHeight2 = this.parent.Height / 8;

            //Set buttons width, heigth and position
            btnPseudo.Width = buttonWidth;
            btnPseudo.Height = buttonHeight;
            Canvas.SetLeft(btnPseudo, spaceWidth);
            Canvas.SetTop(btnPseudo, spaceHeight1);

            btnRobot.Width = buttonWidth;
            btnRobot.Height = buttonHeight;
            Canvas.SetLeft(btnRobot, 2 * spaceWidth + buttonWidth);
            Canvas.SetTop(btnRobot, spaceHeight1);

            btnTraining.Width = buttonWidth;
            btnTraining.Height = buttonHeight;
            Canvas.SetLeft(btnTraining, 3 * spaceWidth + 2 * buttonWidth);
            Canvas.SetTop(btnTraining, spaceHeight1);

            btnHighscore.Width = buttonWidth;
            btnHighscore.Height = buttonHeight;
            Canvas.SetLeft(btnHighscore, spaceWidth);
            Canvas.SetTop(btnHighscore, spaceHeight1 + buttonHeight + spaceHeight2);

            btnExit.Width = buttonWidth;
            btnExit.Height = buttonHeight;
            Canvas.SetLeft(btnExit, 2 * spaceWidth + buttonWidth);
            Canvas.SetTop(btnExit, spaceHeight1 + buttonHeight + spaceHeight2);

            //Set Textbloc Menu position
            Canvas.SetLeft(tbkMenu, this.parent.Width / 2 - tbkMenu.Width / 2);
            Canvas.SetTop(tbkMenu, spaceHeight2 / 2);

            //init kinect
            this.sensorChooser = parent.sensorChooser;
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged2;
            this.sensorChooserUi.KinectSensorChooser = sensorChooser;
            this.sensorChooser.Start();
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
        }

        private void btnPseudo_Click(object sender, RoutedEventArgs e)
        {
            UserNamePage pseudo = new UserNamePage(this.parent);
            parent.Content = pseudo;
        }

        private void btnRobot_Click(object sender, RoutedEventArgs e)
        {
            RobotRadarPage Robot = new RobotRadarPage(this.parent);
            parent.Content = Robot;
        }

        private void btnTraining_Click(object sender, RoutedEventArgs e)
        {
            SimulatorPage training = new SimulatorPage(this.parent);
            parent.Content = training;
        }

        private void btnHighscore_Click(object sender, RoutedEventArgs e)
        {
            //Scores highScore = new Scores(this.parent);
            //parent.Content = highScore;
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

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            parent.Close();
        }
    }
}
