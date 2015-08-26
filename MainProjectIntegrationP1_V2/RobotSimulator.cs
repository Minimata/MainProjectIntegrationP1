using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MainProjectIntegrationP1
{
    class RobotSimulator
    {
        double x { get; set; }
        double y { get; set; }

        Rectangle shape;

        public double directionAngle { get; set; }
        public double speed { get; set; }

        public RobotSimulator()
        {
            shape = new Rectangle();
            shape.Stroke = new SolidColorBrush(Colors.Black);
            shape.Fill = new SolidColorBrush(Colors.Black);
            shape.Width = 10;
            shape.Height = 30;
            x = 300;
            y = 100;
            speed = 0;
            directionAngle = 0;
        }

        public void update()
        {
            //Calcul avec nombre complexes
            x += speed * Math.Cos(directionAngle);
            y += speed * Math.Sin(directionAngle);
        }

        public void draw(Canvas canvas)
        {
            Canvas.SetLeft(shape, x);
            Canvas.SetTop(shape, y);
            canvas.Children.Add(shape);
        }
    }
}
