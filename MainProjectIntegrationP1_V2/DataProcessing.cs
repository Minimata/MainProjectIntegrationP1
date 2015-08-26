﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProjectIntegrationP1
{
    class DataProcessing
    {
        //Variable declaration
        private double LeftX, LeftY, RightX, RightY;
        double DeltaX, DeltaY, DistX, DistY;

        //Wheel Rotation
        const double maxWheelAngle = 90;
        const double coeffWheelRotation = 1;
        //Wheel Speed
        const double maxWheelSpeed = 100;
        const double coeffWheelSpeed = 100;
        const double FINALcoeffWheelSpeed = 1;
        //Assymetric Rotation
        const double maxAssyRotation = 180;
        const double coeffAssyRotation = 300;
        const double FINALcoeffAssyRotation = 1;
        const double neutralAssyRotation = 10;
        //Assymetric Speed
        const double maxAssySpeed = 100;
        const double coeffAssySpeed = 500;
        const double FINALcoeffAssySpeed = 1;
        const double neutralAssySpeed = 10;

        //Constructor
        public DataProcessing(double[] coordinates)
        {
            LeftX = coordinates[0];
            LeftY = coordinates[1];
            RightX = coordinates[2];
            RightY = coordinates[3];

            DeltaX = RightX - LeftX;
            DeltaY = RightY - LeftY;
            DistX = Math.Abs(DeltaX);
            DistY = Math.Abs(DeltaY);
        }

        public int ValueToPourcentage(string kind, double value)
        {
            Int16 retour = 0;
            double tmp = 0;

            switch (kind)
            {
                case "wheelRotation":
                    tmp = (value / maxWheelAngle) * 100;
                    break;

                case "wheelSpeed":
                    tmp = (value / maxWheelSpeed) * 100;
                    break;

                case "assyRotation":
                    tmp = (value / maxAssyRotation) * 100;
                    break;

                case "assySpeed":
                    tmp = (value / maxAssySpeed) * 100;
                    break;

                default:
                    tmp = 0;
                    break;
            }
            tmp = (tmp + 100) / 2;
            if (tmp > 99) tmp = 99;
            else if (tmp < 0) tmp = 0;
            retour = Convert.ToInt16(tmp);
            return retour;
        }

        public double WheelRotation()
        {
            double angle = 0;

            if (DeltaX > 0)
            {
                angle = Math.Atan(DeltaY / DeltaX);
                angle = (angle / (Math.PI * 2)) * 360; //Convert Radian to Degrees
            }
            else
            {
                //Limits the angle to +/- 90 degrees
                if (DeltaY >= 0) angle = maxWheelAngle;
                else angle = -maxWheelAngle;
            }
            angle *= coeffWheelRotation;

            return angle;
        }

        public double WheelSpeed()
        {
            double dist = Math.Sqrt(Math.Pow(DistX, 2) + Math.Pow(DistY, 2)); //Pythagore

            dist *= coeffWheelSpeed;
            if (dist > maxWheelSpeed) dist = maxWheelSpeed;
            dist *= FINALcoeffWheelSpeed;

            return dist;
        }

        public double AssySpeed()
        {
            double height = (RightY + LeftY) / 2;

            height *= coeffAssySpeed;
            if (height >= maxAssySpeed) height = maxAssySpeed;
            else
            {
                if (height <= -maxAssySpeed) height = -maxAssySpeed;
                else
                {
                    NeutralBand(height, neutralAssySpeed);
                }
            }
            height *= FINALcoeffAssySpeed;

            return height;
        }

        public double AssyRotation()
        {
            double rotation = RightX + LeftX;

            rotation *= coeffAssyRotation;
            if (rotation >= maxAssyRotation) rotation = maxAssyRotation;
            else
            {
                if (rotation <= -maxAssyRotation) rotation = -maxAssyRotation;
                else
                {
                    NeutralBand(rotation, neutralAssyRotation);
                }
            }
            rotation *= FINALcoeffAssyRotation;

            return rotation;
        }

        private double NeutralBand(double value, double bandWidth)
        {
            double height = value;
            double newHeight = 0;

            if (Math.Abs(height) <= bandWidth) height = 0;
            else
            {
                newHeight = Math.Abs(height) - neutralAssySpeed;
                if (height >= 0) height = newHeight;
                else height = -newHeight;
            }

            return height;
        }
    }
}
