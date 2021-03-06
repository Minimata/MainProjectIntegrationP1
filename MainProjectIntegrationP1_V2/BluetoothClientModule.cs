﻿using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace BluetoothZeuGroupeLib
{
    public class BluetoothClientModule
    {

        /*
         * Bluetooth vars & classes
         * */
        private List<BluetoothDeviceInfo>   robots{get;set;}
        // mac is mac address of local bluetooth device
        private BluetoothEndPoint           localEndpoint{get;set;}
        // client is used to manage connections
        private BluetoothClient             localClient { get; set; }
        // component is used to manage device discovery
        private BluetoothComponent          localComponent { get; set; }
        private BluetoothListener           Bluetoothlistener {get;set;}
        //Lecture du flux réseau
        private NetworkStream               Ns = null;
        private BluetoothDeviceInfo         pairedRobot { get; set; }
        private Stream stream;
        private StreamWriter sw;

        /*
         * Delegates & Events
         * */

        //Event pour message recus 
        public delegate void    onReceiveMessageDelegate(String name);
        public event            onReceiveMessageDelegate onReceiveMessage;

        //Event pour robot découvert
        public delegate void    onRobotDiscovered(String name);
        public event            onRobotDiscovered onRobotDiscovered_Event;

        //Event pour  découverte fini
        public delegate void    onRobotsDiscoverDone(List<String> robotsNames);
        public event            onRobotsDiscoverDone onRobotsDiscoverDone_Event;

        //Event pour connection terminer
        public delegate void    onConnectionEnded(String cause);
        public event            onConnectionEnded onConnectionEnded_Event;
        
        public  Boolean isPaired    {get; set;}
        private Boolean isScanDone  {get;set;}
        public  Boolean isConnected {get;set;}
        private Boolean isSlave     { get; set; }
        public  Boolean listen      = false;
        public  Boolean stop        = false;


        private int listenAttemps = 0;
        private int sendAttemps = 0;
        public System.Timers.Timer timer;
        private String macAddr;

        public BluetoothClientModule(String macAddress)
        {
            init(macAddress);
            macAddr = macAddress;
           
        }

        private void init(String macAddress)
        {
            isPaired = false;
            isScanDone = false;
            isConnected = false;
            isSlave = true;
            stop = false;
            robots = new List<BluetoothDeviceInfo>();


            //Bind de la carte bluetooth
            try
            {
                localEndpoint = new BluetoothEndPoint(BluetoothAddress.Parse(macAddress), BluetoothService.SerialPort);
                localClient = new BluetoothClient(localEndpoint);
                localComponent = new BluetoothComponent(localClient);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            localComponent.DiscoverDevicesProgress += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesProgress);
            localComponent.DiscoverDevicesComplete += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesComplete);

        }

        /// <summary>
        ///  Lance le scan des robots aux alenetours, cette fonction triggera les events "onRobotDiscovered" et "onRobotsDiscovered_Done"
        /// </summary>
        public void scanRobots()
        {
            isSlave = false;
            robots.Clear();
            localComponent.DiscoverDevicesAsync(255, true, true, true, true, null);
            
        }

        private void component_DiscoverDevicesProgress(object sender, DiscoverDevicesEventArgs e)
        {
            // log and save all found devices
            for (int i = 0; i < e.Devices.Length; i++)
            {
                robots.Add(e.Devices[i]);
                if (onRobotDiscovered_Event != null)
                    onRobotDiscovered_Event.Invoke(e.Devices[i].DeviceName);
            }
        }

        private void component_DiscoverDevicesComplete(object sender, DiscoverDevicesEventArgs e)
        {
            Console.WriteLine("done");
            isScanDone = true;
            List<String> targetList = robots.ConvertAll(x => x.DeviceName);
            if (onRobotsDiscoverDone_Event != null)
                onRobotsDiscoverDone_Event.Invoke(targetList);
        }


        public static List<BluetoothRadio> getAllBluetoothAdapters()
        {
            BluetoothRadio[] radios = BluetoothRadio.AllRadios;
            return radios.ToList();
        }



        /// <summary>
        ///  Pairage au robot.
        ///  @index index du robot dans la liste robots
        /// </summary>
        public void pairToRobot(int index)
        {
            try
            {
                isPaired = BluetoothSecurity.PairRequest(robots.ElementAt(index).DeviceAddress, "1234");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            

            if (isPaired)
            {
                Console.WriteLine("Paired !");
                pairedRobot = robots.ElementAt(index);
                // check if device is paired
                if (pairedRobot.Authenticated)
                {
                    localClient.SetPin("1234");

                    if (!localClient.Connected)
                        localClient.Connect(pairedRobot.DeviceAddress, BluetoothService.SerialPort);

                    if (localClient.Connected)
                    {
                        Console.WriteLine("Connected !");
                        isConnected = true;
                    }
                }
            }
            else
            {
                Console.WriteLine("NOT Paired!");
            }
        }


        /// <summary>
        ///  Envoie un message au robot pairé
        /// </summary>
        public void sendToPairedRobot(String msg)
        {

            Console.WriteLine("SEND " + msg);
            
            if (localClient != null && localClient.Connected && !stop)
            {
                   try
                   {
                        stream = localClient.GetStream();
                        stream.WriteTimeout = 5000;
                        if(sw == null)
                        {
                            sw = new StreamWriter(stream);
                            sw.AutoFlush = true;
                        }
                        sw.WriteLine(msg);
                        sendAttemps = 0;
                   }
                   catch(Exception e)
                   {
                       Console.WriteLine(e.ToString());
                        sendAttemps ++;
                       //Connexion coupé
                       if (onConnectionEnded_Event != null && sendAttemps >= 5)
                       {
                           onConnectionEnded_Event.Invoke("cut");
                       }
                   }
            }
        }

        public void Listen()
        {
            //Lance l'écoute de flux en async
            timer = new System.Timers.Timer();
            timer.Elapsed += doListen;
            timer.Start();
        }


        private void doListen(object sender, ElapsedEventArgs e)
        {
            if (isSlave && !listen)
            {
                Bluetoothlistener = new BluetoothListener(BluetoothService.SerialPort);
                Bluetoothlistener.Start();
                localClient = Bluetoothlistener.AcceptBluetoothClient();
                isPaired = true;
            }

            listen = true;

                if (localClient != null && localClient.Connected && !stop)
                {
                    try
                    {
                        byte[] data = new byte[6];

                        Ns = localClient.GetStream();
                        Ns.ReadTimeout = 5000;
                        Ns.Read(data, 0, data.Length);
                        listenAttemps = 0;
                        // event message
                        if (onReceiveMessage != null)
                        {
                            Console.WriteLine("RECEIVED " + data);
                            onReceiveMessage.Invoke(ASCIIEncoding.ASCII.GetString(data));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        listenAttemps++;
                        //Connexion coupé
                        if (onConnectionEnded_Event != null && listenAttemps >= 20)
                        {
                            onConnectionEnded_Event.Invoke("cut");
                        }
                    }
                }
                else if(stop)
                {
                    Ns.Dispose();
                    Ns.Close();
                    closeConnection();
                    init(macAddr);
                }
                else if (!localClient.Connected)
                {
                    Console.WriteLine("Connection Terminer ou jamais initié");
                }
        }

        public void reset()
        {
            if (listen)
                stop = true;
            else
                closeConnection();
        }

        public void closeConnection()
        {
            listen = false;
            stop = true;
            timer.Stop();
            try
            {
                //localClient.Dispose();
                localClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            
            //Connexion coupé
            if (onConnectionEnded_Event != null)
            {
                //onConnectionEnded_Event.Invoke("user");
            }
        }
    }
}
