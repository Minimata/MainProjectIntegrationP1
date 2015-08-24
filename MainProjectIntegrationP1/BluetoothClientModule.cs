﻿using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BluetoothZeuGroupeLib
{
    public class BluetoothClientModule
    {

        /*
         * Bluetooth vars & classes
         * */
        private List<BluetoothDeviceInfo> robots{get;set;}
        // mac is mac address of local bluetooth device
        private BluetoothEndPoint localEndpoint{get;set;}
        // client is used to manage connections
        private BluetoothClient localClient { get; set; }
        // component is used to manage device discovery
        private BluetoothComponent localComponent { get; set; }
        private BluetoothListener Bluetoothlistener {get;set;}
        //Lecture du flux réseau
        private NetworkStream Ns = null;
        private BluetoothDeviceInfo pairedRobot { get; set; }

        /*
         * Delegates & Events
         * */

        //Event pour message recus 
        public delegate void onReceiveMessageDelegate(String name);
        public event onReceiveMessageDelegate onReceiveMessage;

        //Event pour robot découvert
        public delegate void onRobotDiscovered(String name);
        public event onRobotDiscovered onRobotDiscovered_Event;

        //Event pour  découverte fini
        public delegate void onRobotsDiscoverDone(List<String> robotsNames);
        public event onRobotsDiscoverDone onRobotsDiscoverDone_Event;

        //Event pour connection terminer
        public delegate void onConnectionEnded(String cause);
        public event onConnectionEnded onConnectionEnded_Event;
        
        public Boolean isPaired {get; set;}
        private Boolean isScanDone {get;set;}
        public Boolean isConnected {get;set;}
        private String guid = "{00112233-4455-6677-8899-aabbccddeeff}";
        private Boolean isSlave { get; set; }

        public Boolean listen = false;

        public Boolean stop = false;

        public BluetoothClientModule(String macAddress)
        {

            isPaired = false;
            isScanDone = false;
            isConnected = false;
            isSlave = true;
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
        }

        /// <summary>
        ///  Lance le scan des robots aux alenetours, cette fonction triggera les events "onRobotDiscovered" et "onRobotsDiscovered_Done"
        /// </summary>
        public void scanRobots()
        {
            isSlave = false;
            localComponent.DiscoverDevicesAsync(255, true, true, true, true, null);
            localComponent.DiscoverDevicesProgress += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesProgress);
            localComponent.DiscoverDevicesComplete += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesComplete);
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
                isPaired = BluetoothSecurity.PairRequest(robots.ElementAt(index).DeviceAddress, "4843758");
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
                    localClient.SetPin("4843758");

                    if (!localClient.Connected)
                        localClient.Connect(pairedRobot.DeviceAddress, new Guid(guid));

                    if (localClient.Connected)
                    {
                        Console.WriteLine("Connected !");
                        isConnected = true;
                    }
                }
            }
            else
            {
                Console.WriteLine("NOT Paire!");
            }
        }


        /// <summary>
        ///  Envoie un message au robot pairé
        /// </summary>
        public void sendToPairedRobot(String msg)
        {
            Console.WriteLine(msg);
            if (localClient != null && localClient.Connected && !stop)
            {
                   Stream stream = localClient.GetStream();
                   stream.WriteTimeout = 10000;
                   try
                   {
                       StreamWriter sw = new StreamWriter(stream);
                       sw.WriteLine(msg);
                       sw.Flush();
                   }
                   catch(Exception e)
                   {
                       if(e is InvalidOperationException  || e is IOException)
                       //Connexion coupé
                       if (onConnectionEnded_Event != null)
                       {
                           onConnectionEnded_Event.Invoke("cut");
                       }
                   }
            }
        }

        public void Listen()
        {
            //Lance l'écoute de flux en async
            Thread t = new Thread(new ThreadStart(doListen));
            t.Name = "Listen Thread";
            t.Start();
        }

        private void doListen()
        {
            if(isSlave)
            {
                (Bluetoothlistener = new BluetoothListener(new Guid(guid))).Start();
                localClient = Bluetoothlistener.AcceptBluetoothClient();
                isPaired = true;
            }

            listen = true;

            while (listen)
            {
                try
                {
                    if (localClient != null && localClient.Connected)
                    {
                        //Eviter les boucles infinies !
                        byte[] data = new byte[128];

                        Ns = localClient.GetStream();
                        Ns.ReadTimeout = 10000;
                        Ns.Read(data, 0, data.Length);


                        // event message
                        if (onReceiveMessage != null)
                        {
                            onReceiveMessage.Invoke(System.Text.UTF8Encoding.ASCII.GetString(data));
                        }

                    }

                    else if(!localClient.Connected)
                    {
                        Console.WriteLine("Connection Terminer");
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    //Connexion coupé
                    if (onConnectionEnded_Event != null)
                    {
                        onConnectionEnded_Event.Invoke("cut");
                    }

                }
            }
        }

        public void stopListen()
        {
            listen = false;
            localClient.Close();
            stop = true;

            //Connexion coupé
            if (onConnectionEnded_Event != null)
            {
                onConnectionEnded_Event.Invoke("user");
            }
        }
    }
}