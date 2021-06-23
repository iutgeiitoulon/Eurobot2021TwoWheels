using EventArgsLibrary;
using MadWizard.WinUSBNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace USBDriverNS
{
    public class USBDriver
    {
        private const String DeviceInterfaceGuid = "{58D07210-27C1-11DD-BD0B-0800200C9a66}";
        private const int VendorID = 0x04D8;
        private const int ProductID = 0x0053;

        USBDevice device;
        USBInterface usbInterfaceMicrochip;

        byte[] rcvBuffer = new byte[256];

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        private ManagementEventWatcher _deviceArrivedWatcher;
        private ManagementEventWatcher _deviceRemovedWatcher;


        public USBDriver()
        {
            AddDeviceArrivedHandler();
            AddDeviceRemovedHandler();
            UpdateUsbConnection();

            Timer timerReconnectionIn = new Timer(100);
            timerReconnectionIn.Elapsed += TimerReconnectionIn_Elapsed;
            timerReconnectionIn.Start();

            Timer timerUsbReceptionWatchdog = new Timer(1000);
            timerUsbReceptionWatchdog.Elapsed += TimerUsbReceptionWatchdog_Elapsed;
            timerUsbReceptionWatchdog.Start();
        }

        int nbUsbPacketReceived = 0;
        private void TimerUsbReceptionWatchdog_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("Nombre de paquets USB reçus durant la dernière seconde : " + nbUsbPacketReceived);
            if (nbUsbPacketReceived == 0)
                UpdateUsbConnection();
            nbUsbPacketReceived = 0;
        }

        bool isListening = false;
        private void UpdateUsbConnection()
        {
            try
            {
                //On peut avoir un device déjà en usage et donc inaccessible
                device = USBDevice.GetSingleDevice(DeviceInterfaceGuid);
            }
            catch
            {

            }
            if (device != null)
            {
                /// Si on a trouvé le device
                var usbList = device.Interfaces.Where(usbIf => usbIf.BaseClass == USBBaseClass.VendorSpecific).ToList();
                if (usbList.Count > 0)
                {
                    /// Si il existe une interface de type VendorId
                    usbInterfaceMicrochip = usbList[0].InPipe.Interface;
                    if (usbInterfaceMicrochip != null)
                    {
                        try
                        {
                            //usbInterfaceMicrochip.InPipe.Abort();
                            usbInterfaceMicrochip.InPipe.BeginRead(rcvBuffer, 0, 256, new AsyncCallback(ReceiveUsbDataCallback), null);
                            isListening = true;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("USB problem : " + e.ToString());
                        }
                    }
                }
            }
        }        

        public void ReceiveUsbDataCallback(IAsyncResult result)
        {
            try
            {
                var nbBytesTransfered = usbInterfaceMicrochip.InPipe.EndRead(result);
                //Console.WriteLine(rcvBuffer);

                byte[] usbReceivedBuffer = new byte[nbBytesTransfered];
                Buffer.BlockCopy(rcvBuffer, 0, usbReceivedBuffer, 0, nbBytesTransfered);

                nbUsbPacketReceived++;

                OnUSBDataReceived(usbReceivedBuffer);

                usbInterfaceMicrochip.InPipe.BeginRead(rcvBuffer, 0, 256, new AsyncCallback(ReceiveUsbDataCallback), null);
            }
            catch(Exception e)
            {
                /// On a une erreur, potentiellement due au débranchement de l'USB, du coup on n'écoute plus
                Console.WriteLine("Exception USB ReceiveUsbDataCallback :\n" + e.ToString());
                isListening = false;
            }
        }

        private void TimerReconnectionIn_Elapsed(object sender, ElapsedEventArgs e)
        {
            /// On réarme l'USB quoiqu'il arrive si on est pas listening
            if(!isListening)
            {
                UpdateUsbConnection();
                //Console.WriteLine("Rénitialisation de la connexion USB sur erreur de réception");
            }
        }




        public event EventHandler<DataReceivedArgs> OnUSBDataReceivedEvent;
        public virtual void OnUSBDataReceived(byte[] data)
        {
            var handler = OnUSBDataReceivedEvent;
            if (handler != null)
            {
                handler(this, new DataReceivedArgs { Data = data });
            }
        }

        public void SendUSBMessage(object sender, EventArgsLibrary.MessageEncodedArgs e)
        {
            if (usbInterfaceMicrochip != null)
            {
                try
                {
                    usbInterfaceMicrochip.OutPipe.Write(e.Msg);
                }
                catch
                {
                    /// On a une une déconnexion pas encore notifiée
                }
            }
        }

        ///  <summary>
        ///  Called on removal of any device.
        ///  Calls a routine that searches to see if the desired device is still present.
        ///  </summary>
        /// 
        private void DeviceRemoved(object sender, EventArgs e)
        {
            //try
            {
                Console.WriteLine("A USB device has been removed");
                UpdateUsbConnection();
            }
            //catch (Exception ex)
            //{
            //    //DisplayException(Name, ex);
            //    throw;
            //}
        }

        ///  <summary>
        ///  Add a handler to detect removal of devices.
        ///  </summary>

        private void AddDeviceRemovedHandler()
        {
            const Int32 pollingIntervalMilliseconds = 3000;
            var scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;

            try
            {
                var q = new WqlEventQuery();
                q.EventClassName = "__InstanceDeletionEvent";
                q.WithinInterval = new TimeSpan(0, 0, 0, 0, pollingIntervalMilliseconds);
                q.Condition = @"TargetInstance ISA 'Win32_USBControllerdevice'";
                _deviceRemovedWatcher = new ManagementEventWatcher(scope, q);
                _deviceRemovedWatcher.EventArrived += DeviceRemoved;
                _deviceRemovedWatcher.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (_deviceRemovedWatcher != null)
                    _deviceRemovedWatcher.Stop();
            }
        }

        private void DeviceAdded(object sender, EventArrivedEventArgs e)
        {
            //try
            {
                Console.WriteLine("A USB device has been inserted");
                UpdateUsbConnection();

                //FindMyDevice();
                //_deviceDetected = FindDeviceUsingWmi();
            }
            //catch (Exception ex)
            //{
            //    //DisplayException(Name, ex);
            //    throw;
            //}
        }

        private void AddDeviceArrivedHandler()
        {
            const Int32 pollingIntervalMilliseconds = 3000;
            var scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;

            try
            {
                var q = new WqlEventQuery();
                q.EventClassName = "__InstanceCreationEvent";
                q.WithinInterval = new TimeSpan(0, 0, 0, 0, pollingIntervalMilliseconds);
                q.Condition = @"TargetInstance ISA 'Win32_USBControllerdevice'";
                _deviceArrivedWatcher = new ManagementEventWatcher(scope, q);
                _deviceArrivedWatcher.EventArrived += DeviceAdded;

                _deviceArrivedWatcher.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (_deviceArrivedWatcher != null)
                    _deviceArrivedWatcher.Stop();
            }
        }
    }
}
