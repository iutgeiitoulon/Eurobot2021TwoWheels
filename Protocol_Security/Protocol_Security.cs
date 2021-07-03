using System;
using Constants;

namespace Protocol
{
    public static class Protocol_Security
    {
        public static short CheckFunctionLenght(ushort msgFunction)
        {
            switch (msgFunction)
            {
                // -2               : UNKNOW
                // -1               : UNLIMITED 
                // [0:MAX_LENGHT]   : FIXED
                case (ushort)Commands.R2PC_WelcomeMessage:
                    return 0;
                case (ushort)Commands.R2PC_ErrorMessage:
                    return -1;
                case (ushort)Commands.R2PC_IMUData:
                    return 28;
                case (ushort)Commands.R2PC_IOMonitoring:
                    return 5;
                case (ushort)Commands.R2PC_PowerMonitoring:
                    return 20;
                case (ushort)Commands.R2PC_EncoderRawData:
                    return 36;
                case (ushort)Commands.R2PC_SpeedPolarAndIndependantOdometry:
                    return 32;
                case (ushort)Commands.R2PC_SpeedAuxiliaryOdometry:
                    return 16;
                case (ushort)Commands.R2PC_SpeedPolarPidDebugErrorCorrectionConsigne:
                    return 40;
                case (ushort)Commands.R2PC_SpeedIndependantPidDebugErrorCorrectionConsigne:
                    return 28;
                case (ushort)Commands.R2PC_SpeedPolarPidDebugInternal:
                    return 40;
                case (ushort)Commands.R2PC_SpeedIndependantPidDebugInternal:
                    return 52;
                case (ushort)Commands.R2PC_SpeedAuxiliaryMotorsConsignes:
                    return 36;
                case (ushort)Commands.R2PC_MotorCurrentsMonitoring:
                    return 36;
                case (ushort)Commands.R2PC_2WheelsSpeedPolarPidCommandErrorCorrectionConsigne:
                    return 28;
                case (ushort)Commands.R2PC_2WheelsSpeedIndependantPidCommandErrorCorrectionConsigne:
                    return 28;
                case (ushort)Commands.R2PC_2WheelsSpeedPolarPidCorrections:
                    return 28;
                case (ushort)Commands.R2PC_2WheelsSpeedIndependantPidCorrections:
                    return 28;
                case (ushort)Commands.R2PC_IOPollingEnableStatus:
                    return 1;
                case (ushort)Commands.R2PC_PowerMonitoringEnableStatus:
                    return 1;
                case (ushort)Commands.R2PC_EncoderRawMonitoringEnableStatus:
                    return 1;
                case (ushort)Commands.R2PC_AsservissementModeStatus:
                    return 1;
                case (ushort)Commands.R2PC_SpeedPIDEnableDebugErrorCorrectionConsigneStatus:
                    return 1;
                case (ushort)Commands.R2PC_SpeedPIDEnableDebugInternalStatus:
                    return 1;
                case (ushort)Commands.R2PC_SpeedConsigneMonitoringEnableStatus:
                    return 1;
                case (ushort)Commands.R2PC_MotorsEnableDisableStatus:
                    return 1;
                case (ushort)Commands.R2PC_MotorCurrentMonitoringEnableStatus:
                    return 1;
                case (ushort)Commands.R2PC_TirEnableDisableStatus:
                    return 1;
                case (ushort)Commands.PC2R_EmergencyStop:
                    return -1;
                case (ushort)Commands.PC2R_IOPollingEnable:
                    return 1;
                case (ushort)Commands.PC2R_IOPollingSetFrequency:
                    return 1;
                case (ushort)Commands.PC2R_PowerMonitoringEnable:
                    return 1;
                case (ushort)Commands.PC2R_EncoderRawMonitoringEnable:
                    return 1;
                case (ushort)Commands.PC2R_OdometryPointToMeter:
                    return 4;
                case (ushort)Commands.PC2R_4WheelsAngleSet:
                    return 16;
                case (ushort)Commands.PC2R_4WheelsToPolarMatrixSet:
                    return 48;
                case (ushort)Commands.PC2R_2WheelsAngleSet:
                    return 8;
                case (ushort)Commands.PC2R_2WheelsToPolarMatrixSet:
                    return 16;
                case (ushort)Commands.PC2R_SetAsservissementMode:
                    return 1;
                case (ushort)Commands.PC2R_SpeedPIDEnableDebugErrorCorrectionConsigne:
                    return 1;
                case (ushort)Commands.PC2R_SpeedPIDEnableDebugInternal:
                    return 1;
                case (ushort)Commands.PC2R_SpeedConsigneMonitoringEnable:
                    return 1;
                case (ushort)Commands.PC2R_SpeedPolarPIDSetGains:
                    return 72;
                case (ushort)Commands.PC2R_SpeedIndependantPIDSetGains:
                    return 96;
                case (ushort)Commands.PC2R_SpeedPolarSetConsigne:
                    return 12;
                case (ushort)Commands.PC2R_2WheelsPolarSpeedPIDSetGains:
                    return 48;
                case (ushort)Commands.PC2R_2WheelsIndependantSpeedPIDSetGains:
                    return 48;
                case (ushort)Commands.PC2R_SpeedIndividualMotorSetConsigne:
                    return 5;
                case (ushort)Commands.PC2R_SpeedPIDReset:
                    return 0;
                case (ushort)Commands.PC2R_MotorsEnableDisable:
                    return 1;
                case (ushort)Commands.PC2R_TirEnableDisable:
                    return 1;
                case (ushort)Commands.PC2R_TirCommand:
                    return 14;
                case (ushort)Commands.PC2R_TirMoveUp:
                    return 0;
                case (ushort)Commands.PC2R_TirMoveDown:
                    return 0;
                case (ushort)Commands.PC2R_HerkulexForward:
                    return -1;
                case (ushort)Commands.PC2R_PololuServoSetPosition:
                    return -1;
                default:
                    return -2;
            }
        }
    }
}
