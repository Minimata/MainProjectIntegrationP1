using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MainProjectIntegrationP1
{
    class RobotSimulator
    {
        public double x { get; set; }
        public double y { get; set; }

        RotateTransform rotation = new RotateTransform();
        Rectangle shape;

        public double directionAngle { get; set; }
        public double speed { get; set; }

        public RobotSimulator()
        {
            shape = new Rectangle();
            shape.Stroke = new SolidColorBrush(Colors.Black);
            shape.Fill = new SolidColorBrush(Colors.Black);
            shape.Width = 50;
            shape.Height = 10;
            x = 400;
            y = 200;
            speed = 1;
            directionAngle = 0;
            shape.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        public void update()
        {
            //Calcul avec nombre complexes
            x += speed * Math.Cos(directionAngle);
            y += speed * Math.Sin(directionAngle);
            rotation.Angle = directionAngle * 180.0 / Math.PI;
            shape.RenderTransform = rotation;
        }

        public void draw(Canvas canvas)
        {
            Canvas.SetTop(shape, y);
            Canvas.SetLeft(shape, x);
            canvas.Children.Add(shape);
        }
    }
}
