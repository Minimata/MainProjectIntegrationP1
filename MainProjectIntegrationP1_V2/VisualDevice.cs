using System;
using System.IO;
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
    class VisualDevice
    {
        private Image imageVideo;
        private double[] handPositions;
        private KinectSensor sensor;
        private byte[] colorImageData;

        public delegate void VideoFrameReadyEventHandler(object sender, EventArgs e, ImageSource bSource);
        public event VideoFrameReadyEventHandler onColorFrameReady;
        public delegate void SkeletonFrameReadyEventHandler(object sender, EventArgs e, double[] data);
        public event SkeletonFrameReadyEventHandler onSkeletonFrameReady;


        public void Load(KinectSensor sensor)
        {
            imageVideo = new Image();
            handPositions = new double[4];
            this.sensor = sensor;

            //foreach (var potentialSensor in KinectSensor.KinectSensors)
            //{
            //    if (potentialSensor.Status == KinectStatus.Connected)
            //    {
            //        this.sensor = potentialSensor;
            //        break;
            //    }
            //}

            if (null != this.sensor)
            {
                this.sensor.SkeletonStream.Enable();
                this.sensor.SkeletonStream.EnableTrackingInNearRange = true;
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                this.sensor.DepthStream.Enable();
                this.sensor.DepthStream.Range = DepthRange.Near;

                this.sensor.ColorStream.Enable();
                this.sensor.ColorFrameReady += this.VideoFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                Console.WriteLine("No sensor connected");
            }
        }

        public void Close()
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons.Length != 0)
            {
                foreach (Skeleton skel in skeletons)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        this.handPositions[0] = skel.Joints[JointType.HandLeft].Position.X;
                        this.handPositions[1] = skel.Joints[JointType.HandLeft].Position.Y;
                        this.handPositions[2] = skel.Joints[JointType.HandRight].Position.X;
                        this.handPositions[3] = skel.Joints[JointType.HandRight].Position.Y;
                    }
                }
                onSkeletonFrameReady.Invoke(this, e, this.handPositions);
            }
        }

        private void VideoFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (var colorImageFrame = e.OpenColorImageFrame())
            {
                if (colorImageFrame == null)
                {
                    return;
                }

                // Make a copy of the color frame for displaying.

                colorImageData = new byte[colorImageFrame.PixelDataLength];

                colorImageFrame.CopyPixelDataTo(this.colorImageData);
                //this.colorImageWritableBitmap.WritePixels(
                //    new Int32Rect(0, 0, colorImageFrame.Width, colorImageFrame.Height),
                //    this.colorImageData,
                //    colorImageFrame.Width,
                //    0);

                BitmapSource source = BitmapSource.Create(colorImageFrame.Width, colorImageFrame.Height, 96, 96,
                       PixelFormats.Bgr32, null, colorImageData, 640 * 4);
                imageVideo.Source = source;

                onColorFrameReady.Invoke(this, e, imageVideo.Source);
            }
        }
    }
}