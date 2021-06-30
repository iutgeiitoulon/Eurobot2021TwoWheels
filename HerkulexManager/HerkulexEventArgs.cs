using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerkulexManagerNs
{
        #region LowLevelEventArgs
        /// <summary>
        /// Herkulex : packetDecoded args
        /// </summary>
        public class HklxPacketDecodedArgs : EventArgs
        {
            public byte PacketSize { get; set; }
            public ServoId PID { get; set; }
            public byte CMD { get; set; }
            public byte CheckSum1 { get; set; }
            public byte CheckSum2 { get; set; }
            public byte[] PacketData { get; set; }
            public byte StatusError;
            public byte StatusDetail;
        }

        /// <summary>
        /// Herkulex : Checksum error occured at reception
        /// </summary>
        public class HklxCheckSumErrorOccured : EventArgs
        {
            public byte CheckSum1 { get; set; }
            public byte CheckSum2 { get; set; }
            public ServoId PID;
        }

        /// <summary>
        /// Herkulex : EEPWRITE ack
        /// </summary>
        public class Hklx_EEP_WRITE_Ack_Args : EventArgs
        {
            public List<HerkulexDescription.ErrorStatus> StatusErrors;
            public List<HerkulexDescription.ErrorStatusDetail> StatusDetails;
            public ServoId PID;
        }

        /// <summary>
        /// Herkulex : EEPREAD ack
        /// </summary>
        public class Hklx_EEP_READ_Ack_Args : EventArgs
        {
            public byte[] ReceivedData;
            public byte Address;
            public byte Length;
            public List<HerkulexDescription.ErrorStatus> StatusErrors;
            public List<HerkulexDescription.ErrorStatusDetail> StatusDetails;
            public ServoId PID;
        }

        /// <summary>
        /// Heckulex : RAMWRITE ack
        /// </summary>
        public class Hklx_RAM_WRITE_Ack_Args : EventArgs
        {
            public List<HerkulexDescription.ErrorStatus> StatusErrors;
            public List<HerkulexDescription.ErrorStatusDetail> StatusDetails;
            public ServoId PID;
        }

        /// <summary>
        /// Herkulex : RAMREAD ack
        /// </summary>
        public class Hklx_RAM_READ_Ack_Args : EventArgs
        {
            public byte[] ReceivedData;
            public byte Address;
            public byte Length;
            public List<HerkulexDescription.ErrorStatus> StatusErrors;
            public List<HerkulexDescription.ErrorStatusDetail> StatusDetails;
            public ServoId PID;
        }

        /// <summary>
        /// Herkulex : I_JOG ack
        /// </summary>
        public class Hklx_I_JOG_Ack_Args : EventArgs
        {
            public List<HerkulexDescription.ErrorStatus> StatusErrors;
            public List<HerkulexDescription.ErrorStatusDetail> StatusDetails;
            public ServoId PID;
        }

        /// <summary>
        /// Herkulex : S_JOG ack
        /// </summary>
        public class Hklx_S_JOG_Ack_Args : EventArgs
        {
            public List<HerkulexDescription.ErrorStatus> StatusErrors;
            public List<HerkulexDescription.ErrorStatusDetail> StatusDetails;
            public ServoId PID;
        }

        /// <summary>
        /// Herkulex : STAT ack
        /// </summary>
        public class Hklx_STAT_Ack_Args : EventArgs
        {
            public List<HerkulexDescription.ErrorStatus> StatusErrors;
            public List<HerkulexDescription.ErrorStatusDetail> StatusDetails;
            public ServoId PID;
        }

        /// <summary>
        /// Herkulex : ROLLBACK ack
        /// </summary>
        public class Hklx_ROLLBACK_Ack_Args : EventArgs
        {
            public List<HerkulexDescription.ErrorStatus> StatusErrors;
            public List<HerkulexDescription.ErrorStatusDetail> StatusDetails;
            public ServoId PID;
        }

        /// <summary>
        /// Hekulex : REBOOT ack
        /// </summary>
        public class Hklx_REBOOT_Ack_Args : EventArgs
        {
            public List<HerkulexDescription.ErrorStatus> StatusErrors;
            public List<HerkulexDescription.ErrorStatusDetail> StatusDetails;
            public ServoId PID;
        }

        public class Hklx_AnyAck_Args : EventArgs
        {
            public List<HerkulexDescription.ErrorStatus> StatusErrors;
            public List<HerkulexDescription.ErrorStatusDetail> StatusDetails;
            public ServoId PID;
        }

        #endregion LowLevelEventArgs

        #region OutputEventArgs

        //ServoId id, ushort targetPosition, byte playTime
        public class TargetPositionEventArgs : EventArgs
        {
            public ServoId ID { get; set; }
            public ushort TargetPosition { get; set; }
            public byte PlayTime { get; set; }
        }

        public class AddServoArgs : EventArgs
        {
            public ServoId ID { get; set; }
            public HerkulexDescription.JOG_MODE Mode { get; set; }
        }

        public class TorqueModeArgs : EventArgs
        {
            public HerkulexDescription.TorqueControl Mode { get; set; }
            public ServoId ID { get; set; }
        }

        public class HerkulexServoInformationArgs : EventArgs
        {
            public Servo Servo;
        }
        
        public class HerkulexPositionsArgs : EventArgs
        {
            public Dictionary<ServoId, int> servoPositions;

        }

        public class HerkulexErrorArgs : EventArgs
        {
            public Servo Servo;
        }

        #endregion OutputEventArgs
}
