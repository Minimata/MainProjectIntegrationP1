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

namespace MainProjectIntegrationP1
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        MainWindow parent;

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

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            parent.Close();
        }
    }
}
