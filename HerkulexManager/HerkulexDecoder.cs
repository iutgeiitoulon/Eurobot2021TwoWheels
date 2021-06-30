using EventArgsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static HerkulexManagerNS.HerkulexEventArgs;

namespace HerkulexManagerNs
{
    public class HerkulexDecoder
    {
        private byte packetSize = 0;
        private byte pID = 0;
        private byte cmd = 0;
        private byte checkSum1 = 0;
        private byte checkSum2 = 0;
        private byte[] packetData;
        private byte packetDataByteIndex = 0;

        private enum ReceptionStates
        {
            Waiting,
            sof2,
            packetSize,
            pID,
            CMD,
            checkSum1,
            checkSum2,
            data,
        }

        ReceptionStates rcvState = ReceptionStates.Waiting;

        public void DecodePacket(object sender, DataReceivedArgs e)
        {
            foreach (byte b in e.Data)
            {
                switch (rcvState)
                {
                    case ReceptionStates.Waiting:
                        if (b == 0xFF)
                            rcvState = ReceptionStates.sof2;
                        break;

                    case ReceptionStates.sof2:
                        if (b == 0xFF)
                            rcvState = ReceptionStates.packetSize;
                        break;

                    case ReceptionStates.packetSize:
                        packetSize = b;
                        packetData = new byte[packetSize - 7]; //init to the data size only, -(status error, detail)
                        rcvState = ReceptionStates.pID;
                        break;

                    case ReceptionStates.pID:
                        pID = b;
                        rcvState = ReceptionStates.CMD;
                        break;

                    case ReceptionStates.CMD:
                        cmd = b;
                        rcvState = ReceptionStates.checkSum1;
                        break;

                    case ReceptionStates.checkSum1:
                        checkSum1 = b;
                        rcvState = ReceptionStates.checkSum2;
                        break;

                    case ReceptionStates.checkSum2:
                        checkSum2 = b;
                        rcvState = ReceptionStates.data;
                        break;

                    case ReceptionStates.data:
                        if (packetDataByteIndex < packetData.Length)
                        {
                            packetData[packetDataByteIndex] = b;
                            packetDataByteIndex++;
                        }

                        if (!(packetDataByteIndex < packetData.Length))
                        {
                            packetDataByteIndex = 0;

                            byte clcChksum1 = CommonMethods.CheckSum1(packetSize, (byte)pID, cmd, packetData);
                            byte clcChksum2 = CommonMethods.CheckSum2(clcChksum1);

                            if (checkSum1 == clcChksum1 && checkSum2 == clcChksum2)
                            {
                                byte statusError = packetData[packetData.Length - 2];
                                byte statusDetail = packetData[packetData.Length - 1];

                                //Console.WriteLine("Fin decodage Packet : " + DateTime.Now.Second + "." + DateTime.Now.Millisecond);
                                ProcessPacket(packetSize, (ServoId)pID, cmd, checkSum1, checkSum2, packetData, statusError, statusDetail);
                                OnAnyAckReveived((ServoId)pID, statusError, statusDetail);
                            }
                            rcvState = ReceptionStates.Waiting;
                        }
                        break;
                }
            }
        }

        byte[] _GetOnlyDataFromReadOperations(byte[] data)
        {
            if (data.Length <= 2)
                return data;

            byte[] _data = new byte[data.Length - 4];
            for (int i = 0; i < _data.Length; i++)
                _data[i] = data[i + 2];
            return _data;
        }


        public void ProcessPacket(byte packetSize, ServoId pID, byte cmd, byte checkSum1, byte checkSum2, byte[] packetData, byte statusError, byte statusDetail)
        {
            int dataLen = packetData.Length;
            byte[] readOperationData;

            switch (cmd)
            {
                case (byte)HerkulexDescription.CommandAckSet.ack_RAM_READ:
                    readOperationData = _GetOnlyDataFromReadOperations(packetData);
                    OnRamReadAck(pID, statusError, statusDetail, packetData[0], packetData[1], readOperationData);
                    break;
                case (byte)HerkulexDescription.CommandAckSet.ack_STAT:
                    OnStatAck(pID, statusError, statusDetail);
                    break;
                default:
                    break;
            }

        }


        #region eventGeneration

        public event EventHandler<Hklx_AnyAck_Args> OnAnyAckEvent;
        public event EventHandler<Hklx_RAM_READ_Ack_Args> OnRamReadAckEvent;
        public event EventHandler<Hklx_STAT_Ack_Args> OnStatAckEvent;

        public void OnAnyAckReveived(ServoId Pid, byte statusError, byte statusDetail)
        {
            OnAnyAckEvent?.Invoke(this, new Hklx_AnyAck_Args
            {
                PID = Pid,
                StatusErrors = CommonMethods.GetErrorStatusFromByte(statusError),
                StatusDetails = CommonMethods.GetErrorStatusDetailFromByte(statusDetail)
            });
        }

        public void OnRamReadAck(ServoId pID, byte statusError, byte statusDetail, byte address, byte length, byte[] data)
        {
            OnRamReadAckEvent?.Invoke(this, new Hklx_RAM_READ_Ack_Args
            {
                StatusErrors = CommonMethods.GetErrorStatusFromByte(statusError),
                StatusDetails = CommonMethods.GetErrorStatusDetailFromByte(statusDetail),
                Address = address,
                Length = length,
                ReceivedData = data,
                PID = pID
            });
        }

        public void OnStatAck(ServoId pID, byte statusError, byte statusDetail)
        {

            OnStatAckEvent?.Invoke(this, new Hklx_STAT_Ack_Args
            {
                StatusErrors = CommonMethods.GetErrorStatusFromByte(statusError),
                StatusDetails = CommonMethods.GetErrorStatusDetailFromByte(statusDetail),
                PID = pID
            });
        }

        #endregion eventGeneration
    }
}
