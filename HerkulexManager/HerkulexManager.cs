using EventArgsLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace HerkulexManagerNs
{
    public class HerkulexManager
    {
        private int pollingTimeoutMs = 5000;

        #region classInst

        //private AutoResetEvent WaitingForAck = new AutoResetEvent(false);
        private ConcurrentDictionary<ServoId, Servo> Servos = new ConcurrentDictionary<ServoId, Servo>();
        private HerkulexDecoder decoder;

        //private System.Timers.Timer pollingTimer = new System.Timers.Timer(100);

        private ConcurrentQueue<byte[]> messageQueue = new ConcurrentQueue<byte[]>();

        Thread SendingThread;

        #endregion classInst

        public HerkulexManager()
        {
            decoder = new HerkulexDecoder();
            SendingThread = new Thread(SendingThreadProcessing);
            SendingThread.IsBackground = true;
            SendingThread.Start();
        }

        void SendingThreadProcessing()
        {
            byte[] message;
            while (true)
            {
                while (messageQueue.Count() > 0)
                {
                    if (messageQueue.TryDequeue(out message))
                    {
                        if (message != null)
                        {
                            //    if (serialPort.IsOpen)
                            //    {
                            //Ajouter l'event de forward au lieu de l'envoi direct
                           // OnHerkulexSendToSerial(message);
                            //serialPort.Write(message, 0, message.Length);
                            Thread.Sleep(50);
                            //}
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

        #region userMethods

        /// <summary>
        /// Sets the torque mode on the servo
        /// </summary>
        /// <param name="ID">Servo ID</param>
        /// <param name="mode">Torque mode</param>
        public void SetTorqueMode(ServoId ID, HerkulexDescription.TorqueControl mode)
        {
            if (Servos.ContainsKey(ID))
                _SetTorqueMode(ID, mode);
        }



        public void OnSetTorqueMode(object sender, TorqueModeArgs e)
        {
            SetTorqueMode(e.ID, e.Mode);
        }


        /// <summary>
        /// Adds a servo to the controller
        /// </summary>
        /// <param name="ID">Servo ID</param>
        /// <param name="mode">JOG mode</param>
        public void _AddServo(ServoId ID, HerkulexDescription.JOG_MODE mode)
        {
            Servo servo = new Servo(ID, mode);
            while (!Servos.TryAdd(ID, servo)) ; //ON tente l'ajout tant qu'il n'est pas validé
            //reply to all packets
            RAM_WRITE(ID, HerkulexDescription.RAM_ADDR.ACK_Policy, 1, 0x02); //reply to I_JOG / S_JOG
            RecoverErrors(ID);
            
        }

        
        public void AddServo(object sender, AddServoArgs e)
        {
            _AddServo(e.ID, e.Mode);
        }

        /// <summary>
        /// Recovers the servo from error state
        /// </summary>
        /// <param name="servo">Servo instance</param>
        public void RecoverErrors(ServoId servo)
        {
            ClearAllErrors(servo);
            SetTorqueMode(servo, HerkulexDescription.TorqueControl.TorqueOn);
        }

        #endregion userMethods


        #region outputEvents



        public void OnHerkulexPositionRequestEvent(object sender, HerkulexPositionsArgs e)
        {
            foreach (var positionCommand in e.servoPositions)
            {
                SetPosition((ServoId)positionCommand.Key, (UInt16)positionCommand.Value, 5); //TODO : fgaut pas déconner non plus !
            }
        }

        

        public void OnEnableDisableServosRequestEvent(object sender, BoolEventArgs e)
        {
            foreach (var servo in Servos)
            {
                if (e.value == false)
                    SetTorqueMode(servo.Key, HerkulexDescription.TorqueControl.TorqueFree);
                else
                    SetTorqueMode(servo.Key, HerkulexDescription.TorqueControl.TorqueOn);
            }
        }

        //On envoie la trame serie vers le manager de port série
        public event EventHandler<DataReceivedArgs> OnHerkulexSendToSerialEvent;
        public void OnHerkulexSendToSerial(byte[] data)
        {
            DataReceivedArgs arg = new DataReceivedArgs();
            arg.Data = data;
            OnHerkulexSendToSerialEvent?.Invoke(this, arg);
        }

        public event EventHandler<HerkulexServoInformationArgs> OnHerkulexServoInformationEvent;
        public event EventHandler<HerkulexErrorArgs> HerkulexErrorEvent;

        /// <summary>
        /// Sets the torque control mode of the specified servo I.e BreakOn / TorqueOn / TorqueFree
        /// </summary>
        /// <param name="pID">Servo ID</param>
        /// <param name="mode">torque mode (TorqueControl enum)</param>
        private void _SetTorqueMode(ServoId pID, HerkulexDescription.TorqueControl mode)
        {
            RAM_WRITE(pID, HerkulexDescription.RAM_ADDR.Torque_Control, 1, (ushort)mode);
        }

        /// <summary>
        /// Servo polled event
        /// </summary>
        /// <param name="servo"></param>
        public virtual void OnHerkulexServoInformation(Servo servo)
        {
            OnHerkulexServoInformationEvent?.Invoke(this, new HerkulexServoInformationArgs
            {
                Servo = servo
            });
        }

        /// <summary>
        /// Error occured event
        /// </summary>
        /// <param name="servo"></param>
        public virtual void OnHerkulexError(Servo servo)
        {
            //Ne doit être appelé que si il y a une erreur
            HerkulexErrorEvent?.Invoke(this, new HerkulexErrorArgs
            {
                Servo = servo
            });
        }

        #endregion outputEvents

        #region LowLevelMethods

        private void SetPosition(ServoId id, ushort targetPosition, byte playTime)
        {
            //On clear une éventuelle erreur
            RecoverErrors(id);
            if (Servos.ContainsKey(id))
            {
                Servos[id].SetAbsolutePosition(targetPosition);

                byte[] dataToSend = new byte[5];
                dataToSend[0] = (byte)(targetPosition >> 0);
                dataToSend[1] = (byte)(targetPosition >> 8);
                dataToSend[2] = Servos[id].GetSETByte();
                dataToSend[3] = (byte)id;

                dataToSend[4] = playTime;

                EncodeAndEnqueuePacket(id, (byte)HerkulexDescription.CommandSet.I_JOG, dataToSend);
            }
        }

        public void OnSetPosition(object sender, TargetPositionEventArgs e)
        {
            SetPosition(e.ID, e.TargetPosition, e.PlayTime);
        }

        /// <summary>
        /// Clears all of the servo error statuses
        /// </summary>
        /// <param name="pID">Servo ID</param>
        private void ClearAllErrors(ServoId pID)
        {
            RAM_WRITE(pID, HerkulexDescription.RAM_ADDR.Status_Error, 1, 0x00);
        }

        private void RAM_WRITE(ServoId pID, byte addr, byte length, UInt16 value)
        {
            if (length > 2)
                return;

            byte[] data = new byte[2 + length];
            data[0] = (byte)addr;
            data[1] = length;

            if (length >= 2)
            {
                data[2] = (byte)(value >> 0); //little endian, LSB first
                data[3] = (byte)(value >> 8);
            }
            else
                data[2] = (byte)(value);

            EncodeAndEnqueuePacket(pID, (byte)HerkulexDescription.CommandSet.RAM_WRITE, data);
        }

        private void RAM_WRITE(ServoId pID, HerkulexDescription.RAM_ADDR addr, byte length, UInt16 value)
        {
            RAM_WRITE(pID, (byte)addr, length, value);
        }

        private void RAM_READ(ServoId pID, byte startAddr, byte length)
        {
            byte[] data = { (byte)startAddr, length };
            EncodeAndEnqueuePacket(pID, (byte)HerkulexDescription.CommandSet.RAM_READ, data);
        }

        private void RAM_READ(ServoId pID, HerkulexDescription.RAM_ADDR startAddr, byte length)
        {
            RAM_READ(pID, (byte)startAddr, length);
        }
        int k = 0;

        public event EventHandler<DataReceivedArgs> SendHerkulexDataEvent;
        public virtual void SendHerkulexData(byte[] Packet)
        {
            SendHerkulexDataEvent?.Invoke(this, new DataReceivedArgs
            {
                Data = Packet
            });
        }
        private void EncodeAndEnqueuePacket(ServoId pID, byte CMD, byte[] dataToSend)
        {
            byte packetSize = (byte)(7 + dataToSend.Length);
            byte[] packet = new byte[packetSize];

            packet[0] = 0xFF;
            packet[1] = 0xFF;
            packet[2] = packetSize;
            packet[3] = (byte)pID;
            packet[4] = CMD;
            packet[5] = CommonMethods.CheckSum1(packet[2], packet[3], packet[4], dataToSend);
            packet[6] = CommonMethods.CheckSum2(packet[5]);

            for (int i = 0; i < dataToSend.Length; i++)
                packet[7 + i] = dataToSend[i];
            k++;

            SendHerkulexData(packet);

        }
        #endregion LowLevelMethods
    }
}
