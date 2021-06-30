using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace HerkulexManagerNS
{
    public class Servo
    {
        private ServoId ID;
        private HerkulexDescription.JOG_MODE Mode;
        private HerkulexDescription.LedColor LEDState;
        private UInt16 TargetAbsolutePosition;
        public bool IsNextOrderSynchronous = false;

        public byte _playtime;

        private byte _SET;

        public bool isDeplacementFinished
        {
            get
            {
                if(TargetAbsolutePosition - CalibratedPosition < 0.5)//In degree
                    return true;
                return false;
            }
            set
            { }
        }

        //values
        public UInt16 AbsolutePosition;
        public UInt16 CalibratedPosition;

        //flags
        public bool IsMoving;
        public bool IsInposition;
        public bool IsMotorOn;

        //error details
        public bool CheckSumError;
        public bool UnknownCommandError;
        public bool ExceedRegRangeError;
        public bool GarbageDetectedError;

        //errors
        public bool Exceed_input_voltage_limit;
        public bool Exceed_allowed_pot_limit;
        public bool Exceed_Temperature_limit;
        public bool Invalid_packet;
        public bool Overload_detected;
        public bool Driver_fault_detected;
        public bool EEP_REG_distorted;

        public Servo(ServoId pID, HerkulexDescription.JOG_MODE mode)
        {
            ID = pID;
            Mode = mode;
        }

        public byte GetSETByte()
        {
            _SET = 0;
            _SET |= (byte)((byte)Mode << 1);
            _SET |= (byte)((byte)LEDState << 2);
            return _SET;
        }

        public void SetAbsolutePosition(ushort absolutePosition)
        {
            TargetAbsolutePosition = absolutePosition;
        }

        public ushort GetTargetAbsolutePosition()
        {
            return TargetAbsolutePosition;
        }

        public byte GetPlaytime()
        {
            return _playtime;
        }

        public ServoId GetID()
        {
            return ID;
        }
    }
}
