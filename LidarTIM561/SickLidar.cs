using MadWizard.WinUSBNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Utilities;

namespace Lidar
{
    /// <summary>Interface du télémètre Laser de Sick.</summary>
    public class SickLidar : LidarDevice
    {
        #region Constructor & Properties

        /// <summary>Crée une instance de <see cref="SickLidar"/> et se connecte au numéro de série spécifié.</summary>
        public SickLidar(int serialNumber) => _serialNumber = serialNumber;

        /// <summary>Indique l'état de connexion du Lidar.</summary>
        public bool IsConnected { get; set; } = false;
        
        private readonly int _serialNumber = 0;
        private USBInterface _iface;
        private USBDevice _device;

        private Thread _connectionThread;
        private Thread _readThread;

        #endregion
        #region Data

        private void ProcessReceivedData(TiMDataMessage TimData)
        {
            lock (LidarPoints)
            {
                LidarPoints.Clear();

                double angleLidar = Toolbox.DegToRad(Location.Theta);
                for (int i = 0; i < TimData.AngleData.Count; i++)
                    if (TimData.AngleData[i] >= AngleMin && TimData.AngleData[i] <= AngleMax) // Filtre d'angle
                    {
                        double distance = TimData.DistanceData[i] / 1000.0;
                        double angle = Toolbox.DegToRad(TimData.AngleData[i]);
                        //On trouve les coordonnées du point brut en cartésien dans le ref du Lidar
                        double xRefRobot = Location.X + distance * Math.Cos(angle + angleLidar);
                        double yRefRobot = Location.Y + distance * Math.Sin(angle + angleLidar);

                        //On trouve les coordonnées du point en polaire dans le ref du robot
                        double distanceCentreRobot =
                            Math.Sqrt(Math.Pow(xRefRobot, 2) +
                                      Math.Pow(yRefRobot, 2));
                        double angleAxeRobot = Math.Atan2(yRefRobot, xRefRobot);

                        LidarPoint point = new LidarPoint(distanceCentreRobot, angleAxeRobot, TimData.RssiData[i]);

                        // Supression des points en dehors des limites hardware
                        if (distance >= 0.01 && distance <= 10.01) // Filtre de distance
                        {
                            LidarPoints.Add(point);
                        }
                    }
            }

            OnLidarPointsReady();
        }

        #endregion

        #region Methods

        /// <summary>Se connecte au Lidar. La connexion est essayée tant qu'il y a un échec.</summary>
        public override void Start()
        {
            _stopAsked = false;
            _connectionThread = new Thread(() =>
            {
                while (!IsConnected && !_stopAsked)
                {
                    // Class GUID : CC79D431 - E1F7 - 48c0 - B8F6 - EA3BF62A62BF 
                    // Device GUID : 40F8D7C6 - 6856 - 483d - AC31 - DC646CA2D89B - TIM3xx
                    var devices = USBDevice.GetDevices("40F8D7C6-6856-483d-AC31-DC646CA2D89B");
                    foreach (var deviceInfo in devices)
                    {
 
                        
                        // Recherche d'un TIM SickLidar précis utilisant son numéro de série
                        if (_serialNumber != 0 && deviceInfo.DevicePath.Contains(_serialNumber.ToString()))
                        {
                            try
                            {
                                USBDevice device = new USBDevice(deviceInfo);
                                OnLidarDeviceConnected(); 

                                _device = device;
                                _iface = device.Interfaces[0];

                                SendMessage("sMN SetAccessMode 03 F4724744");
                                IsConnected = true;

                                _readThread = new Thread(Read) {Name = $"ReadThread for Lidar {_serialNumber}" };
                                _readThread.Start();
                            }
                            catch (USBException ex)
                            {
                                Trace.WriteLine(ex);
                            }
                        }
                    }

                    Thread.Sleep(1000);
                }
            }) { Name = $"ConnectionThread for Lidar {_serialNumber}" };
            
            _connectionThread.Start();
        }


        private bool _stopAsked = false;
        /// <summary>Arrête le Lidar.</summary>
        public override void Stop()
        {
            _stopAsked = true;
            if (IsConnected)
            {
                _iface?.InPipe.Abort();
                _iface?.OutPipe.Abort();
                _device?.Dispose();
            }
        }

        #endregion
        #region Input/Output

        private void SendMessage(string message)
        {
            byte[] msg = new byte[message.Length + 2];
            int pos = 0;

            msg[pos++] = 0x02; //STX
            foreach (var c in message)
                msg[pos++] = Convert.ToByte(c);
            msg[pos++] = 0x03;

            // 3017    SEND        18 < 02 >< 73 >< 4d >< 49 >< 20 >< 30 >< 20 >< 33 >< 20 >< 46 >< 34 >< 37 >< 32 >< 34 >< 37 >< 34 >< 34 >< 03 >

            _iface.OutPipe.Write(msg);
        }

        private readonly StringBuilder _receivedMessage = new StringBuilder("");
        private void Read()
        {
            try
            {
                while (!_stopAsked)
                {
                    byte[] buffer = new byte[10];
                    int numberOfBytesReceived = 0;

                    try
                    {
                        // Obtient les nouvelles données reçues ainsi que leur nombre
                        numberOfBytesReceived = _iface.InPipe.Read(buffer, 0, buffer.Length);
                    }
                    catch (ObjectDisposedException) { }

                    if (numberOfBytesReceived == 0)
                    {
                        // Si pas de données, s'arrêter
                        throw new USBException("InPipe a renvoyé des données vides.");
                    }

                    for (int i = 0; i < numberOfBytesReceived; i++)
                    {
                        byte b = buffer[i];
                        switch (b)
                        {
                            case 2:
                                _receivedMessage.Clear();
                                break;
                            case 3:
                                // On a une trame complète, on la parse en fonction de son type
                                var argList = _receivedMessage.ToString().Split(' ');
                                if (argList.Length > 0)
                                {
                                    string msgType = argList[0];
                                    switch (msgType)
                                    {
                                        case "sRA": // data un seule fois
                                        case "sSN": // data en continu
                                            var timMsg = ParseTimDataMessage(argList);
                                            ProcessReceivedData(timMsg);
                                            break;
                                        case "sEA": // acknowledgement of request
                                            if (argList[1] != "LMDscandata")
                                                ParseAndContinueInitTimMessage(argList);
                                            break;
                                        case "sAN":
                                        case "sWA":
                                            ParseAndContinueInitTimMessage(argList);
                                            break;
                                    }
                                }

                                break;
                            default:
                                _receivedMessage.Append(Encoding.UTF8.GetString(new[] {b}));
                                break;
                        }
                    }
                }
            }
            catch (USBException ex)
            {
                IsConnected = false;
                Trace.WriteLine(ex.Message);

                if (!_stopAsked)
                {
                    Trace.WriteLine("Redémarrage du Lidar");
                    _device.Dispose();
                    Start();
                }
            }
        }
        
        private void ParseAndContinueInitTimMessage(string[] argList)
        {
            int pos = 1;
            if (argList[pos] == "SetAccessMode")
            {
                pos++;
                int value = int.Parse(argList[pos], System.Globalization.NumberStyles.HexNumber);
                if (value == 1)
                    SendMessage("sWN LMDscandatacfg 01 00 1 1 0 00 00 1 1 1 1 1");
            }
            else if (argList[pos] == "Run")
            {
                pos++;
                int value = int.Parse(argList[pos], System.Globalization.NumberStyles.HexNumber);
                if (value == 1)
                    SendMessage("sEN LMDscandata 1");
            }
            else if (argList[pos] == "LMDscandatacfg")
            {
                pos++;
                SendMessage("sMN Run");
            }
        }

        private TiMDataMessage ParseTimDataMessage(string[] argList)
        {
            TiMDataMessage tms = new TiMDataMessage();

            int pos = 1;
            if (argList[pos++] != "LMDscandata")
                return tms;

            tms.VersionNumber = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.DeviceNumber = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.SerialNumber = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.DeviceStatus1 = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.DeviceStatus2 = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.TelegramCounter = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.ScanCounter = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.TimeSinceStartUp = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.TimeOfTransmission = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.InputStatus1 = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.InputStatus2 = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.OutputStatus1 = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.OutputStatus2 = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.ReservedByteA = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.ScanningFrequency = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.MeasurementFrequency = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.NumberOfEncoders = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.NumberOf16BitsChannels = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.MeasuredDataContents = argList[pos++];
            tms.ScalingFactor = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.ScalingOffset = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.StartingAngle = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.AngularStepWidth = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.NumberOfData = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);

            tms.DistanceData = new List<int>();
            tms.AngleData = new List<double>();
            tms.RssiData = new List<int>();
            tms.RssiDataPercent = new List<double>();

            double angleOffset = -900000; // Pour placer le zéro d'angle en face du LIDAR
            for (int i = 0; i < tms.NumberOfData; i++)
            {
                double angle = (tms.StartingAngle + angleOffset + i * tms.AngularStepWidth) / 10000.0;
                if (IsUpsideDown) // Inverse les angles si le Lidar est retourné
                    angle *= -1;

                tms.AngleData.Add(angle);
                tms.DistanceData.Add(int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber));
            }
            tms.RssiDataType = argList[pos++];
            tms.ScalingFactor = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.ScalingOffset = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.OutputStatus1 = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.OutputStatus2 = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.NumberOfDataRssi = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);

            for (int i = 0; i < tms.NumberOfDataRssi; i++)
            {
                tms.RssiData.Add(int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber));
                double calculatePercentRssi = tms.RssiData[i] / 65536.0 * 100.0;//(((byte)(tms.RssiData[i])) / 255.0) * 100;
                tms.RssiDataPercent.Add(calculatePercentRssi);
            }

            //double maxvalue = tms.RssiData.Max();
            int outputOfPosition = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber); //RSSI missing
            int deviceName = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber); ; //RSSI missing : not really clear in the datasheet
            tms.Comment = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);

            if (tms.Comment == 1)
            {
                pos++;//int CommentLenght = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
                pos++;//int CommentTransmit = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            }

            pos++;//tms.TimeInformation = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);
            tms.EventInformation = int.Parse(argList[pos++], System.Globalization.NumberStyles.HexNumber);

            return tms;
        }

        #endregion
    }

    /// <summary>
    /// Message transmis par le Lidar.
    /// </summary>
    public class TiMDataMessage
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public int VersionNumber;
        public int DeviceNumber;
        public int SerialNumber;
        public int DeviceStatus1;
        public int DeviceStatus2;
        public int TelegramCounter;
        public int ScanCounter;
        public int TimeSinceStartUp; //in us
        public int TimeOfTransmission; //in us
        public int InputStatus1;
        public int InputStatus2;
        public int OutputStatus1;
        public int OutputStatus2;
        public int ReservedByteA;
        public int ScanningFrequency;
        public int MeasurementFrequency;
        public int NumberOfEncoders;
        public int NumberOf16BitsChannels;
        public string MeasuredDataContents;
        public int ScalingFactor;
        public int ScalingOffset;
        public int StartingAngle; //in 1/10000 degree
        public int AngularStepWidth; // in 1/10000 degree
        public int NumberOfData;
        public int NumberOfDataRssi;
        public List<double> AngleData;
        public List<int> DistanceData;
        public List<int> RssiData;
        public List<double> RssiDataPercent;
        public string RssiDataType;
        public int NumberOf8BitsChannels;
        public int Position;
        public int Name;
        public int Comment;
        public int TimeInformation;
        public int EventInformation;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
