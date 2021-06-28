using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerkulexManagerNS
{
    public static class HerkulexDescription
    {
        /// <summary>
        /// Holds the Servo configuration for S_JOG / I_JOG
        /// </summary>
        public struct JOG_TAG
        {
            public JOG_MODE mode;
            public byte ID;
            public byte playTime;
            public byte LED_GREEN;
            public byte LED_BLUE;
            public byte LED_RED;

            private byte _SET;
            public byte SET
            {
                get
                {
                    _SET = 0;
                    _SET |= (byte)((byte)mode << 1);
                    _SET |= (byte)(LED_GREEN << 2);
                    _SET |= (byte)(LED_BLUE << 3);
                    _SET |= (byte)(LED_RED << 4);
                    return _SET;
                }
            }

            private UInt16 _JOG;
            public UInt16 JOG
            {
                get => _JOG;
                set { _JOG = (ushort)(value & 0x7FFF); } //set bit 15 to 0
            }
        }

        /// <summary>
        /// Error Status
        /// </summary>
        public enum ErrorStatus
        {
            Exceed_input_voltage_limit = 1,
            Exceed_allowed_pot_limit = 2,
            Exceed_Temperature_limit = 4,
            Invalid_packet = 8,
            Overload_detected = 16,
            Driver_fault_detected = 32,
            EEP_REG_distorted = 64,
        }

        /// <summary>
        /// Error Status Detail
        /// </summary>
        public enum ErrorStatusDetail
        {
            Moving_flag = 1,
            Inposition_flag = 2,
            CheckSumError = 4,
            Unknown_Command = 8,
            Exceed_REG_RANGE = 16,
            Garbage_detected = 32,
            MOTOR_ON_flag = 64
        }

        /// <summary>
        /// Jog mode
        /// </summary>
        public enum JOG_MODE
        {
            positionControlJOG = 0,
            infiniteTurn = 1
        }

        /// <summary>
        ///all controller commands set
        /// </summary>
        public enum CommandSet
        {
            EEP_WRITE = 0x01,
            EEP_READ = 0x02,
            RAM_WRITE = 0x03,
            RAM_READ = 0x04,
            I_JOG = 0x05,
            S_JOG = 0x06,
            STAT = 0x07,
            ROLLBACK = 0x08,
            REBOOT = 0x09
        }

        /// <summary>
        /// all commands ACK set
        /// </summary>
        public enum CommandAckSet
        {
            ack_EEP_WRITE = 0x41,
            ack_EEP_READ = 0x42,
            ack_RAM_WRITE = 0x43,
            ack_RAM_READ = 0x44,
            ack_I_JOG = 0x45,
            ack_S_JOG = 0x46,
            ack_STAT = 0x47,
            ack_ROLLBACK = 0x48,
            ack_REBOOT = 0x49
        }

        /// <summary>
        /// all RAM addresses
        /// </summary>
        public enum RAM_ADDR
        {
            ID = 0,                                 // length: 1
            ACK_Policy = 1,                         // length: 1
            Alarm_LED_Policy = 2,                   // length: 1
            Torque_policy = 3,                      // length: 1
            Max_Temperature = 5,                    // length: 1
            Min_Voltage = 6,                        // length: 1
            Max_Voltage = 7,                        // length: 1
            Acceleration_Ratio = 8,                 // length: 1
            Max_Acceleration = 9,                   // length: 1
            Dead_Zone = 10,                         // length: 1
            Saturator_Offset = 11,                  // length: 1
            Saturator_Slope = 12,                   // length: 2
            PWM_Offset = 14,                        // length: 1
            Min_PWM = 15,                           // length: 1
            Max_PWM = 16,                           // length: 2
            Overload_PWM_Threshold = 18,            // length: 2
            Min_Position = 20,                      // length: 2
            Max_Position = 22,                      // length: 2
            Position_Kp = 24,                       // length: 2
            Position_Kd = 26,                       // length: 2
            Position_Ki = 28,                       // length: 2
            Pos_FreeFoward_1st_Gain = 30,           // length: 2
            Pos_FreeFoward_2nd_Gain = 32,           // length: 2
            LED_Blink_Period = 38,                  // length: 1
            ADC_Fault_Detect_Period = 39,           // length: 1
            Packet_Garbage_Detection_Period = 40,   // length: 1
            Stop_Detection_Period = 41,             // length: 1
            Overload_Detection_Period = 42,         // length: 1
            Stop_Threshold = 41,                    // length: 1
            Inposition_Margin = 44,                 // length: 1
            Calibration_Difference = 47,            // length: 1
            Status_Error = 48,                      // length: 1
            Status_Detail = 49,                     // length: 1
            Torque_Control = 52,                    // length: 1
            LED_Control = 53,                       // length: 1
            Voltage = 54,                           // length: 2
            Temperature = 55,                       // length: 2
            Current_Control_Mode = 56,              // length: 2
            Tick = 57,                              // length: 2
            Calibrated_Position = 58,               // length: 2
            Absolute_Position = 60,                 // length: 2
            Differential_Position = 62,             // length: 2
            PWM = 64,                               // length: 2
            Absolute_Goal_Position = 68,            // length: 2
            Absolute_Desired_Traject_Pos = 70,      // length: 2
            Desired_Velocity = 72                   // length: 1
        }

        /// <summary>
        /// all EEP addresses
        /// </summary>
        public enum EEP_ADDR
        {
            Model_No1 = 0,
            Model_NO2 = 1,
            Version1 = 2,
            Version2 = 3,
            BaudRate = 4,
            ID = 6,
            ACK_Policy = 7,
            Alarm_LED_Policy = 8,
            Torque_Policy = 9,
            Max_Temperature = 11,
            Min_Voltage = 12,
            Max_Voltage = 13,
            Acceleration_Ratio = 14,
            Max_Acceleration_Time = 15,
            Dead_Zone = 16,
            Saturator_Offset = 17,
            Saturator_Slope = 18,
            PWM_Offset = 20,
            Min_PWM = 21,
            Max_PWM = 22,
            Overload_PWM_Threshold = 24,
            Min_Position = 26,
            Max_Position = 28,
            Position_Kp = 30,
            Position_Kd = 32,
            Position_Ki = 34,
            Position_FreeForward_1st_Gain = 36,
            Position_FreeForward_2st_Gain = 38,
            LED_Blink_Period = 44,
            ADC_Fault_Check_Period = 45,
            Packet_Garbage_Check_Period = 46,
            Stop_Detection_Period = 47,
            Overload_Detection_Period = 48,
            Stop_Threshold = 49,
            Inposition_Margin = 50,
            Calibration_Difference = 53,
        }

        /// <summary>
        /// Torque control modes
        /// </summary>
        public enum TorqueControl
        {
            BreakOn = 0x40,
            TorqueOn = 0x60,
            TorqueFree = 0x00
        }

        /// <summary>
        /// possible led colors
        /// </summary>
        public enum LedColor
        {
            None = 0x00,
            Red = 0x04,
            Blue = 0x02,
            Green = 0x01,
            Yellow = 0x05,
            Cyan = 0x03,
            Megenta = 0x06,
            White = 0x07
        }
    }
}
