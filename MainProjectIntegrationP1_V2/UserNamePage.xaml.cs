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
    /// Interaction logic for UserNamePage.xaml
    /// </summary>
    public partial class UserNamePage : Page
    {
        MainWindow parent;
        KinectSensorChooser sensorChooser;

        public UserNamePage(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;

            //Variables
            double buttonWidth = this.parent.Width / 14;
            double buttonHeight = this.parent.Height / 8;

            //Initializ buttons width
            btn0.Width = buttonWidth;
            btn1.Width = buttonWidth;
            btn2.Width = buttonWidth;
            btn3.Width = buttonWidth;
            btn4.Width = buttonWidth;
            btn5.Width = buttonWidth;
            btn6.Width = buttonWidth;
            btn7.Width = buttonWidth;
            btn8.Width = buttonWidth;
            btn9.Width = buttonWidth;
            btnA.Width = buttonWidth;
            btnB.Width = buttonWidth;
            btnC.Width = buttonWidth;
            btnD.Width = buttonWidth;
            btnE.Width = buttonWidth;
            btnF.Width = buttonWidth;
            btnG.Width = buttonWidth;
            btnH.Width = buttonWidth;
            btnI.Width = buttonWidth;
            btnJ.Width = buttonWidth;
            btnK.Width = buttonWidth;
            btnL.Width = buttonWidth;
            btnM.Width = buttonWidth;
            btnN.Width = buttonWidth;
            btnO.Width = buttonWidth;
            btnP.Width = buttonWidth;
            btnQ.Width = buttonWidth;
            btnR.Width = buttonWidth;
            btnS.Width = buttonWidth;
            btnT.Width = buttonWidth;
            btnU.Width = buttonWidth;
            btnV.Width = buttonWidth;
            btnW.Width = buttonWidth;
            btnX.Width = buttonWidth;
            btnY.Width = buttonWidth;
            btnZ.Width = buttonWidth;
            btnStart.Width = 2 * (buttonWidth);
            btnReturn.Width = 2 * (buttonWidth);

            //Initialize buttons height
            btnStart.Height = buttonHeight;
            btnReturn.Height = buttonHeight;
            btn0.Height = buttonHeight;
            btn1.Height = buttonHeight;
            btn2.Height = buttonHeight;
            btn3.Height = buttonHeight;
            btn4.Height = buttonHeight;
            btn5.Height = buttonHeight;
            btn6.Height = buttonHeight;
            btn7.Height = buttonHeight;
            btn8.Height = buttonHeight;
            btn9.Height = buttonHeight;
            btnA.Height = buttonHeight;
            btnB.Height = buttonHeight;
            btnC.Height = buttonHeight;
            btnD.Height = buttonHeight;
            btnE.Height = buttonHeight;
            btnF.Height = buttonHeight;
            btnG.Height = buttonHeight;
            btnH.Height = buttonHeight;
            btnI.Height = buttonHeight;
            btnJ.Height = buttonHeight;
            btnK.Height = buttonHeight;
            btnL.Height = buttonHeight;
            btnM.Height = buttonHeight;
            btnN.Height = buttonHeight;
            btnO.Height = buttonHeight;
            btnP.Height = buttonHeight;
            btnQ.Height = buttonHeight;
            btnR.Height = buttonHeight;
            btnS.Height = buttonHeight;
            btnT.Height = buttonHeight;
            btnU.Height = buttonHeight;
            btnV.Height = buttonHeight;
            btnW.Height = buttonHeight;
            btnX.Height = buttonHeight;
            btnY.Height = buttonHeight;
            btnZ.Height = buttonHeight;

            //Set Buttons and labels position x
            double spaceWidth = this.parent.Width / 14;
            double space = spaceWidth / 14;
            Canvas.SetLeft(lblName, this.parent.Width / 2 - lblName.Width / 2);
            Canvas.SetLeft(lblPseudo, (this.parent.Width / 2)-(lblPseudo.Width/2));
            Canvas.SetLeft(btnStart, (this.parent.Width / 2) - (2*buttonWidth));
            Canvas.SetLeft(btnReturn, (this.parent.Width / 2));
            Canvas.SetLeft(btnA, space);
            Canvas.SetLeft(btnB, space);
            Canvas.SetLeft(btnC, space);
            Canvas.SetLeft(btnD, space);
            Canvas.SetLeft(btnE, space);
            Canvas.SetLeft(btnF, space);
            Canvas.SetLeft(btnG, space);
            Canvas.SetLeft(btnH, 2 * space + buttonWidth);
            Canvas.SetLeft(btnI, 3 * space + 2 * buttonWidth);
            Canvas.SetLeft(btnJ, 4 * space + 3 * buttonWidth);
            Canvas.SetLeft(btnK, 5 * space + 4 * buttonWidth);
            Canvas.SetLeft(btnL, 6 * space + 5 * buttonWidth);
            Canvas.SetLeft(btnM, 7 * space + 6 * buttonWidth);
            Canvas.SetLeft(btnN, 8 * space + 7 * buttonWidth);
            Canvas.SetLeft(btnO, 9 * space + 8 * buttonWidth);
            Canvas.SetLeft(btnP, 10 * space + 9 * buttonWidth);
            Canvas.SetLeft(btnQ, 11 * space + 10 * buttonWidth);
            Canvas.SetLeft(btnR, 12 * space + 11 * buttonWidth);
            Canvas.SetLeft(btnS, 13 * space + 12 * buttonWidth);
            Canvas.SetLeft(btnT, 13 * space + 12 * buttonWidth);
            Canvas.SetLeft(btnU, 13 * space + 12 * buttonWidth);
            Canvas.SetLeft(btnV, 13 * space + 12 * buttonWidth);
            Canvas.SetLeft(btnW, 13 * space + 12 * buttonWidth);
            Canvas.SetLeft(btnX, 13 * space + 12 * buttonWidth);
            Canvas.SetLeft(btnY, 13 * space + 12 * buttonWidth);
            Canvas.SetLeft(btnZ, 12 * space + 11 * buttonWidth);
            Canvas.SetLeft(btn9, 11 * space + 10 * buttonWidth);
            Canvas.SetLeft(btn8, 10 * space + 9 * buttonWidth);
            Canvas.SetLeft(btn7, 9 * space + 8 * buttonWidth);
            Canvas.SetLeft(btn6, 8 * space + 7 * buttonWidth);
            Canvas.SetLeft(btn5, 7 * space + 6 * buttonWidth);
            Canvas.SetLeft(btn4, 6 * space + 5 * buttonWidth);
            Canvas.SetLeft(btn3, 5 * space + 4 * buttonWidth);
            Canvas.SetLeft(btn2, 4 * space + 3 * buttonWidth);
            Canvas.SetLeft(btn1, 3 * space + 2 * buttonWidth);
            Canvas.SetLeft(btn0, 2 * space + buttonWidth);

            //Set Buttons and labels position y
            double spaceheight = this.parent.Height / 8;
            double space2 = spaceheight / 10;
            Canvas.SetTop(lblName, this.parent.Height / 2);
            Canvas.SetTop(lblPseudo, this.parent.Height / 4);
            Canvas.SetTop(btnStart, 5 * this.parent.Height / 8);
            Canvas.SetTop(btnReturn, 5 * this.parent.Height / 8);
            Canvas.SetTop(btnA, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btnB, 6 * space2 + 5 * buttonHeight);
            Canvas.SetTop(btnC, 5 * space2 + 4 * buttonHeight);
            Canvas.SetTop(btnD, 4 * space2 + 3 * buttonHeight);
            Canvas.SetTop(btnE, 3 * space2 + 2 * buttonHeight);
            Canvas.SetTop(btnF, 2 * space2 + buttonHeight);
            Canvas.SetTop(btnG, space2);
            Canvas.SetTop(btnH, space2);
            Canvas.SetTop(btnI, space2);
            Canvas.SetTop(btnJ, space2);
            Canvas.SetTop(btnK, space2);
            Canvas.SetTop(btnL, space2);
            Canvas.SetTop(btnM, space2);
            Canvas.SetTop(btnN, space2);
            Canvas.SetTop(btnO, space2);
            Canvas.SetTop(btnP, space2);
            Canvas.SetTop(btnQ, space2);
            Canvas.SetTop(btnR, space2);
            Canvas.SetTop(btnS, space2);
            Canvas.SetTop(btnT, 2 * space2 + buttonHeight);
            Canvas.SetTop(btnU, 3 * space2 + 2 * buttonHeight);
            Canvas.SetTop(btnV, 4 * space2 + 3 * buttonHeight);
            Canvas.SetTop(btnW, 5 * space2 + 4 * buttonHeight);
            Canvas.SetTop(btnX, 6 * space2 + 5 * buttonHeight);
            Canvas.SetTop(btnY, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btnZ, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btn9, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btn8, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btn7, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btn6, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btn5, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btn4, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btn3, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btn2, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btn1, 7 * space2 + 6 * buttonHeight);
            Canvas.SetTop(btn0, 7 * space2 + 6 * buttonHeight);

            //init kinect
            this.sensorChooser = parent.sensorChooser;
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged2;
            this.sensorChooserUi.KinectSensorChooser = sensorChooser;
            this.sensorChooser.Start();
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
        }

        private void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Kinect.Toolkit.Controls.KinectTileButton btn = (Microsoft.Kinect.Toolkit.Controls.KinectTileButton)sender;
            lblName.Content += btn.Content.ToString();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            parent.playerName = lblName.Content.ToString();
            RobotRadarPage Robot = new RobotRadarPage(this.parent);
            parent.Content = Robot;
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

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            MainPage Menu = new MainPage(this.parent);
            parent.Content = Menu;
        }
    }
}
