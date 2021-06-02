using Constants;
using EventArgsLibrary;
using System;
using System.Text;
using System.Timers;
using Utilities;

namespace MessageProcessorNS
{
    public class MsgProcessor
    {
        GameMode competition;
        Timer tmrComptageMessage;
        int robotID;

        public MsgProcessor(int robotId, GameMode compet)
        {
            robotID = robotId;
            competition = compet;
            tmrComptageMessage = new Timer(1000);
            tmrComptageMessage.Elapsed += TmrComptageMessage_Elapsed;
            tmrComptageMessage.Start();
        }

        int nbMessageIMUReceived = 0;
        int nbMessageSpeedReceived = 0;
        private void TmrComptageMessage_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnMessageCounter(nbMessageIMUReceived, nbMessageSpeedReceived);
            nbMessageIMUReceived = 0;
            nbMessageSpeedReceived = 0;
        }

        //Input CallBack        
        public void ProcessRobotDecodedMessage(object sender, MessageByteArgs e)
        {
            ProcessDecodedMessage((Int16)e.MsgFunction, (Int16)e.MsgPayloadLength, e.MsgPayload);
        }

        //Processeur de message en provenance du robot...
        //Une fois processé, le message sera transformé en event sortant
        public void ProcessDecodedMessage(Int16 command, Int16 payloadLength, byte[] payload)
        {
            switch (command)
            {
                case (short)Commands.R2PC_WelcomeMessage:
                    OnWelcomeMessageFromRobot();
                    break;

                case (short)Commands.R2PC_SpeedPolarAndIndependantOdometry:
                    OnPolarAndIndependantOdometrySpeedFromRobot(payload);
                    break;

                case (short)Commands.R2PC_IMUData:
                    OnIMUDataFromRobot(payload);
                    break;

                case (short)Commands.R2PC_MotorCurrentsMonitoring:
                    OnMotorsCurrentsFromRobot(payload);
                    break;

                case (short)Commands.R2PC_SpeedAuxiliaryOdometry:
                    OnAuxiliaryOdometrySpeedFromRobot(payload);
                    break;

                case (short)Commands.R2PC_SpeedAuxiliaryMotorsConsignes:
                    OnAuxiliarySpeedConsigneDataFromRobot(payload);
                    break;

                case (short)Commands.R2PC_EncoderRawData:
                    OnEncoderRawDataFromRobot(payload);
                    break;

                case (short)Commands.R2PC_IOMonitoring:
                    OnIOValuesFromRobot(payload);
                    break;

                case (short)Commands.R2PC_PowerMonitoring:
                    OnPowerMonitoringValuesFromRobot(payload);
                    break;

                case (short)Commands.R2PC_SpeedPolarPidDebugErrorCorrectionConsigne:
                    OnPolarPidErrorCorrectionConsigneDataFromRobot(payload);
                    break;

                case (short)Commands.R2PC_SpeedIndependantPidDebugErrorCorrectionConsigne:
                    OnSpeedIndependantPidDebugDataFromRobot(payload);
                    break;

                case (short)Commands.R2PC_SpeedPolarPidDebugInternal:
                    OnSpeedPolarPidCorrectionDataFromRobot(payload);
                    break;

                case (short)Commands.R2PC_SpeedIndependantPidDebugInternal:
                    OnSpeedIndependantPidCorrectionDataFromRobot(payload);
                    break;

                case (short)Commands.R2PC_MotorsEnableDisableStatus:
                    OnEnableDisableMotorsACKFromRobot(payload);
                    break;

                case (short)Commands.R2PC_TirEnableDisableStatus:
                    OnEnableDisableTirACKFromRobot(payload);
                    break;

                case (short)Commands.R2PC_AsservissementModeStatus:
                    OnAsservissementModeStatusFromRobot(payload);
                    break;

                case (short)Commands.R2PC_SpeedPIDEnableDebugInternalStatus:
                    OnEnableAsservissementDebugDataACKFromRobot(payload);
                    break;

                case (short)Commands.R2PC_MotorCurrentMonitoringEnableStatus:
                    OnEnableMotorCurrentACKFromRobot(payload);
                    break;

                case (short)Commands.R2PC_EncoderRawMonitoringEnableStatus:
                    OnEnableEncoderRawDataACKFromRobot(payload);
                    break;

                case (short)Commands.R2PC_SpeedConsigneMonitoringEnableStatus:
                    OnEnableMotorSpeedConsigneDataACKFromRobot(payload);
                    break;

                case (short)Commands.R2PC_PowerMonitoringEnableStatus:
                    OnEnablePowerMonitoringDataACKFromRobot(payload);
                    break;

                case (short)Commands.R2PC_ErrorMessage:
                    OnErrorTextFromRobot(payload);
                    break;

                default: break;
            }
        }

        public event EventHandler<EventArgs>                                        OnWelcomeMessageFromRobotGeneratedEvent;
        public event EventHandler<IMUDataEventArgs>                                 OnIMURawDataFromRobotGeneratedEvent;
        public event EventHandler<BoolEventArgs>                                    OnEnableDisableMotorsACKFromRobotGeneratedEvent;
        public event EventHandler<BoolEventArgs>                                    OnEnableDisableTirACKFromRobotGeneratedEvent;
        public event EventHandler<AsservissementModeEventArgs>                      OnAsservissementModeStatusFromRobotGeneratedEvent;
        public event EventHandler<BoolEventArgs>                                    OnEnableAsservissementDebugDataACKFromRobotEvent;
        public event EventHandler<BoolEventArgs>                                    OnEnableMotorCurrentACKFromRobotGeneratedEvent;
        public event EventHandler<BoolEventArgs>                                    OnEnableEncoderRawDataACKFromRobotGeneratedEvent;
        public event EventHandler<BoolEventArgs>                                    OnEnableMotorSpeedConsigneDataACKFromRobotGeneratedEvent;
        public event EventHandler<BoolEventArgs>                                    OnEnablePowerMonitoringDataACKFromRobotGeneratedEvent;
        public event EventHandler<StringEventArgs>                                  OnErrorTextFromRobotGeneratedEvent;
        public event EventHandler<PolarSpeedEventArgs>                              OnSpeedPolarOdometryFromRobotEvent;
        public event EventHandler<IndependantSpeedEventArgs>                        OnIndependantOdometrySpeedFromRobotEvent;
        public event EventHandler<MotorsCurrentsEventArgs>                          OnMotorsCurrentsFromRobotGeneratedEvent;
        public event EventHandler<AuxiliarySpeedArgs>                               OnAuxiliaryOdometrySpeedGeneratedEvent;
        public event EventHandler<EncodersRawDataEventArgs>                         OnEncoderRawDataFromRobotGeneratedEvent;
        public event EventHandler<IOValuesEventArgs>                                OnIOValuesFromRobotGeneratedEvent;
        public event EventHandler<PowerMonitoringValuesEventArgs>                   OnPowerMonitoringValuesFromRobotGeneratedEvent;
        public event EventHandler<AuxiliaryMotorsVitesseDataEventArgs>              OnAuxiliarySpeedConsigneDataFromRobotGeneratedEvent;
        public event EventHandler<PolarPidErrorCorrectionConsigneDataArgs>          OnSpeedPolarPidErrorCorrectionConsigneDataFromRobotGeneratedEvent;
        public event EventHandler<IndependantPidErrorCorrectionConsigneDataArgs>    OnSpeedIndependantPidErrorCorrectionConsigneDataFromRobotGeneratedEvent;
        public event EventHandler<PolarPidCorrectionArgs>                           OnSpeedPolarPidCorrectionDataFromRobotEvent;
        public event EventHandler<IndependantPidCorrectionArgs>                     OnSpeedIndependantPidCorrectionDataFromRobotEvent;
        public event EventHandler<MsgCounterArgs> OnMessageCounterEvent;

        public virtual void OnWelcomeMessageFromRobot()
        {
            OnWelcomeMessageFromRobotGeneratedEvent?.Invoke(this, new EventArgs());
        }

        //Output events

        public virtual void OnIMUDataFromRobot(byte[] payload)
        {
            float accelX = 0, accelY = 0, accelZ = 0, gyroX = 0, gyroY = 0, gyroZ = 0;
            uint timeStamp = 0;
            switch (competition)
            {
                case GameMode.RoboCup:
                    nbMessageIMUReceived++;
                    timeStamp = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24);
                    accelX = BitConverter.ToSingle(payload, 04);
                    accelY = BitConverter.ToSingle(payload, 08);
                    accelZ = BitConverter.ToSingle(payload, 12);
                    gyroX = BitConverter.ToSingle(payload, 16);
                    gyroY = BitConverter.ToSingle(payload, 20);
                    gyroZ = BitConverter.ToSingle(payload, 24);
                    break;

                case GameMode.Eurobot: //La carte de mesure est placée verticalement
                    nbMessageIMUReceived++;
                    timeStamp = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24);
                    accelY = -BitConverter.ToSingle(payload, 4);
                    accelZ = BitConverter.ToSingle(payload, 8);
                    accelX = -BitConverter.ToSingle(payload, 12);
                    gyroY = -BitConverter.ToSingle(payload, 16);
                    gyroZ = BitConverter.ToSingle(payload, 20);
                    gyroX = -BitConverter.ToSingle(payload, 24);
                    break;
            }


            OnIMURawDataFromRobotGeneratedEvent?.Invoke(this, new IMUDataEventArgs
            {
                EmbeddedTimeStampInMs = timeStamp,
                accelX = accelX,
                accelY = accelY,
                accelZ = accelZ,
                gyroX = gyroX,
                gyroY = gyroY,
                gyroZ = gyroZ
            });
        }


        public virtual void OnEnableDisableMotorsACKFromRobot(byte[] payload)
        {
            OnEnableDisableMotorsACKFromRobotGeneratedEvent?.Invoke(this, new BoolEventArgs { value = Convert.ToBoolean(payload[0]) });
        }


        public virtual void OnEnableDisableTirACKFromRobot(byte[] payload)
        {
            OnEnableDisableTirACKFromRobotGeneratedEvent?.Invoke(this, new BoolEventArgs { value = Convert.ToBoolean(payload[0]) });
        }


        public virtual void OnAsservissementModeStatusFromRobot(byte[] payload)
        {
            OnAsservissementModeStatusFromRobotGeneratedEvent?.Invoke(this, new AsservissementModeEventArgs { mode = (AsservissementMode) payload[0] });
        }


        public virtual void OnEnableAsservissementDebugDataACKFromRobot(byte[] payload)
        {
            OnEnableAsservissementDebugDataACKFromRobotEvent?.Invoke(this, new BoolEventArgs { value = Convert.ToBoolean(payload[0]) });
        }


        public virtual void OnEnableMotorCurrentACKFromRobot(byte[] payload)
        {
            OnEnableMotorCurrentACKFromRobotGeneratedEvent?.Invoke(this, new BoolEventArgs { value = Convert.ToBoolean(payload[0]) });
        }


        public virtual void OnEnableEncoderRawDataACKFromRobot(byte[] payload)
        {
            OnEnableEncoderRawDataACKFromRobotGeneratedEvent?.Invoke(this, new BoolEventArgs { value = Convert.ToBoolean(payload[0]) });
        }


        public virtual void OnEnableMotorSpeedConsigneDataACKFromRobot(byte[] payload)
        {
            OnEnableMotorSpeedConsigneDataACKFromRobotGeneratedEvent?.Invoke(this, new BoolEventArgs { value = Convert.ToBoolean(payload[0]) });
        }


        public virtual void OnEnablePowerMonitoringDataACKFromRobot(byte[] payload)
        {
            OnEnablePowerMonitoringDataACKFromRobotGeneratedEvent?.Invoke(this, new BoolEventArgs { value = Convert.ToBoolean(payload[0]) });
        }

        public virtual void OnErrorTextFromRobot(byte[] payload)
        {
            OnErrorTextFromRobotGeneratedEvent?.Invoke(this, new StringEventArgs { value = Encoding.UTF8.GetString(payload) });
        }


        public virtual void OnPolarAndIndependantOdometrySpeedFromRobot(byte[] payload)
        {
            nbMessageSpeedReceived++;
            uint timeStamp = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24);
            float vX = BitConverter.ToSingle(payload, 4);
            float vY = BitConverter.ToSingle(payload, 8);
            float vTheta = BitConverter.ToSingle(payload, 12);
            float vM1 = BitConverter.ToSingle(payload, 16);
            float vM2 = BitConverter.ToSingle(payload, 20);
            float vM3 = BitConverter.ToSingle(payload, 24);
            float vM4 = BitConverter.ToSingle(payload, 28);
            OnSpeedPolarOdometryFromRobotEvent?.Invoke(this, new PolarSpeedEventArgs
            {
                RobotId = robotID,
                timeStampMs = timeStamp,
                Vx = (float)vX,
                Vy = (float)vY,
                Vtheta = (float)vTheta
            });
            OnIndependantOdometrySpeedFromRobotEvent?.Invoke(this, new IndependantSpeedEventArgs
            {
                timeStampMs = timeStamp,
                VitesseMoteur1 = (float)vM1,
                VitesseMoteur2 = (float)vM2,
                VitesseMoteur3 = (float)vM3,
                VitesseMoteur4 = (float)vM4
            });
        }





        public virtual void OnMotorsCurrentsFromRobot(byte[] payload)
        {
            OnMotorsCurrentsFromRobotGeneratedEvent?.Invoke(this, new MotorsCurrentsEventArgs
            {
                timeStampMS = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24),
                motor1 = BitConverter.ToSingle(payload, 4 * 1),
                motor2 = BitConverter.ToSingle(payload, 4 * 2),
                motor3 = BitConverter.ToSingle(payload, 4 * 3),
                motor4 = BitConverter.ToSingle(payload, 4 * 4),
                motor5 = BitConverter.ToSingle(payload, 4 * 5),
                motor6 = BitConverter.ToSingle(payload, 4 * 6),
                motor7 = BitConverter.ToSingle(payload, 4 * 7)
            });
        }


        public virtual void OnAuxiliaryOdometrySpeedFromRobot(byte[] payload)
        {
            OnAuxiliaryOdometrySpeedGeneratedEvent?.Invoke(this, new AuxiliarySpeedEventArgs
            {
                timeStampMs = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24),
                VitesseMoteur5 = BitConverter.ToSingle(payload, 4 * 1),
                VitesseMoteur6 = BitConverter.ToSingle(payload, 4 * 2),
                VitesseMoteur7 = BitConverter.ToSingle(payload, 4 * 3)
        });
        }



        public virtual void OnEncoderRawDataFromRobot(byte[] payload)
        {
            OnEncoderRawDataFromRobotGeneratedEvent?.Invoke(this, new EncodersRawDataEventArgs
            {
                timeStampMS = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24),
                motor1 = (int)(payload[7] | payload[6] << 8 | payload[5] << 16 | payload[4] << 24),
                motor2 = (int)(payload[11] | payload[10] << 8 | payload[9] << 16 | payload[8] << 24),
                motor3 = (int)(payload[15] | payload[14] << 8 | payload[13] << 16 | payload[12] << 24),
                motor4 = (int)(payload[19] | payload[18] << 8 | payload[17] << 16 | payload[16] << 24),
                motor5 = (int)(payload[23] | payload[22] << 8 | payload[21] << 16 | payload[20] << 24),
                motor6 = (int)(payload[27] | payload[26] << 8 | payload[25] << 16 | payload[24] << 24),
                motor7 = (int)(payload[31] | payload[30] << 8 | payload[29] << 16 | payload[28] << 24)
            });
        }


        public virtual void OnIOValuesFromRobot(byte[] payload)
        {
            OnIOValuesFromRobotGeneratedEvent?.Invoke(this, new IOValuesEventArgs
            {
                timeStampMS = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24),
                ioValues = payload[4]
        });
        }


        public virtual void OnPowerMonitoringValuesFromRobot(byte[] payload)
        {
            OnPowerMonitoringValuesFromRobotGeneratedEvent?.Invoke(this, new PowerMonitoringValuesEventArgs
            {
                timeStampMS = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24),
                battCMDVoltage = BitConverter.ToSingle(payload, 4 * 1),
                battCMDCurrent = BitConverter.ToSingle(payload, 4 * 2),
                battPWRVoltage = BitConverter.ToSingle(payload, 4 * 3),
                battPWRCurrent = BitConverter.ToSingle(payload, 4 * 4)
            });
        }


        public virtual void OnAuxiliarySpeedConsigneDataFromRobot(byte[] payload)
        {
            OnAuxiliarySpeedConsigneDataFromRobotGeneratedEvent?.Invoke(this, new AuxiliaryMotorsVitesseDataEventArgs
            {
                timeStampMS = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24),
                vitesseMotor5 = BitConverter.ToSingle(payload, 4 * 1),
                vitesseMotor6 = BitConverter.ToSingle(payload, 4 * 2),
                vitesseMotor7 = BitConverter.ToSingle(payload, 4 * 3)
            });
        }


        public virtual void OnPolarPidErrorCorrectionConsigneDataFromRobot(byte[] payload)
        {
            OnSpeedPolarPidErrorCorrectionConsigneDataFromRobotGeneratedEvent?.Invoke(this, new PolarPidErrorCorrectionConsigneDataArgs
            {
                timeStampMS = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24),
                xErreur                 = BitConverter.ToSingle(payload, 4 * 1),
                yErreur                 = BitConverter.ToSingle(payload, 4 * 2),
                thetaErreur             = BitConverter.ToSingle(payload, 4 * 3),
                xCorrection             = BitConverter.ToSingle(payload, 4 * 4),
                yCorrection             = BitConverter.ToSingle(payload, 4 * 5),
                thetaCorrection         = BitConverter.ToSingle(payload, 4 * 6),
                xConsigneFromRobot      = BitConverter.ToSingle(payload, 4 * 7),
                yConsigneFromRobot      = BitConverter.ToSingle(payload, 4 * 8),
                thetaConsigneFromRobot  = BitConverter.ToSingle(payload, 4 * 9)
            });
        }


        public virtual void OnSpeedIndependantPidDebugDataFromRobot(byte[] payload)
        {
            OnSpeedIndependantPidErrorCorrectionConsigneDataFromRobotGeneratedEvent?.Invoke(this, new IndependantPidErrorCorrectionConsigneDataArgs
            {
                timeStampMS = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24),
                M1Erreur            = BitConverter.ToSingle(payload, 4 * 01),
                M2Erreur            = BitConverter.ToSingle(payload, 4 * 02),
                M3Erreur            = BitConverter.ToSingle(payload, 4 * 03),
                M4Erreur            = BitConverter.ToSingle(payload, 4 * 04),
                M1Correction        = BitConverter.ToSingle(payload, 4 * 05),
                M2Correction        = BitConverter.ToSingle(payload, 4 * 06),
                M3Correction        = BitConverter.ToSingle(payload, 4 * 07),
                M4Correction        = BitConverter.ToSingle(payload, 4 * 08),
                M1ConsigneFromRobot = BitConverter.ToSingle(payload, 4 * 09),
                M2ConsigneFromRobot = BitConverter.ToSingle(payload, 4 * 10),
                M3ConsigneFromRobot = BitConverter.ToSingle(payload, 4 * 11),
                M4ConsigneFromRobot = BitConverter.ToSingle(payload, 4 * 12)
            });
        }




        public virtual void OnSpeedPolarPidCorrectionDataFromRobot(byte[] payload)
        {
            uint timeStamp = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24);
            OnSpeedPolarPidCorrectionDataFromRobotEvent?.Invoke(this, new PolarPidCorrectionArgs
            {
                CorrPx      = BitConverter.ToSingle(payload, 4 * 1),
                CorrIx      = BitConverter.ToSingle(payload, 4 * 2),
                CorrDx      = BitConverter.ToSingle(payload, 4 * 3),
                CorrPy      = BitConverter.ToSingle(payload, 4 * 4),
                CorrIy      = BitConverter.ToSingle(payload, 4 * 5),
                CorrDy      = BitConverter.ToSingle(payload, 4 * 6),
                CorrPTheta  = BitConverter.ToSingle(payload, 4 * 7),
                CorrITheta  = BitConverter.ToSingle(payload, 4 * 8),
                CorrDTheta  = BitConverter.ToSingle(payload, 4 * 9)
            });
        }


        public virtual void OnSpeedIndependantPidCorrectionDataFromRobot(byte[] payload)
        {
            uint timeStamp = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24);
            OnSpeedIndependantPidCorrectionDataFromRobotEvent?.Invoke(this, new IndependantPidCorrectionArgs
            {
                CorrPM1 = BitConverter.ToSingle(payload, 4 * 01),
                CorrIM1 = BitConverter.ToSingle(payload, 4 * 02),
                CorrDM1 = BitConverter.ToSingle(payload, 4 * 03),
                CorrPM2 = BitConverter.ToSingle(payload, 4 * 04),
                CorrIM2 = BitConverter.ToSingle(payload, 4 * 05),
                CorrDM2 = BitConverter.ToSingle(payload, 4 * 06),
                CorrPM3 = BitConverter.ToSingle(payload, 4 * 07),
                CorrIM3 = BitConverter.ToSingle(payload, 4 * 08),
                CorrDM3 = BitConverter.ToSingle(payload, 4 * 09),
                CorrPM4 = BitConverter.ToSingle(payload, 4 * 10),
                CorrIM4 = BitConverter.ToSingle(payload, 4 * 11),
                CorrDM4 = BitConverter.ToSingle(payload, 4 * 12)
            });
        }


        public virtual void OnMessageCounter(int nbMessageFromImu, int nbMessageFromOdometry)
        {
            OnMessageCounterEvent?.Invoke(this, new MsgCounterArgs { nbMessageIMU = nbMessageFromImu, nbMessageOdometry = nbMessageFromOdometry });
        }

    }
}
