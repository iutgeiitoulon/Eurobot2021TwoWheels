using System;
using System;
using EventArgsLibrary;
using Utilities;
using Constants;
using System.Linq;

namespace MessageGeneratorNS
{
    public class MsgGenerator
    {
        //Input events
        public void GenerateMessageSetSpeedConsigneToRobot(object sender, PolarSpeedArgs e)
        {
            byte[] payload = new byte[12];
            payload.SetValueRange(((float) e.Vx).GetBytes(), 0);
            payload.SetValueRange(((float) e.Vy).GetBytes(), 4);
            payload.SetValueRange(((float) e.Vtheta).GetBytes(), 8);
            //OnMessageToRobot((Int16)Commands.PC2R_SpeedPolarSetConsigne, 12, payload);
            OnSetSpeedConsigneToRobotReceived(e); //Pour affichage graphique supervision
        }

        public event EventHandler<PolarSpeedArgs> OnSetSpeedConsigneToRobotReceivedEvent;
        public virtual void OnSetSpeedConsigneToRobotReceived(PolarSpeedArgs args)
        {
            OnSetSpeedConsigneToRobotReceivedEvent?.Invoke(this, args);
        }

        public void GenerateMessageSetIOPollingFrequencyToRobot(object sender, ByteEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0]= e.Value;
            OnMessageToRobot((Int16)Commands.PC2R_IOPollingSetFrequency, 1, payload);
        }

        public void GenerateMessageSetSpeedConsigneToMotor(object sender, SpeedConsigneToMotorArgs e)
        {
            byte[] payload = new byte[5];
            payload.SetValueRange(((float)e.V).GetBytes(), 0);
            payload[4] = (byte)e.MotorNumber;
            OnMessageToRobot((Int16)Commands.PC2R_SpeedIndividualMotorSetConsigne, 5, payload);
        }
        public void GenerateMessageTir(object sender, TirEventArgs e)
        {
            byte[] payload = new byte[4];
            payload.SetValueRange(((float)e.Puissance).GetBytes(), 0);
            OnMessageToRobot((Int16)Commands.PC2R_TirCommand, 4, payload);
        }
        public void GenerateMessageMoveTirUp(object sender, EventArgs e)
        {
            OnMessageToRobot((Int16)Commands.PC2R_TirMoveUp, 0, null);
        }
        
        public void GenerateMessageMoveTirDown(object sender, EventArgs e)
        {
            OnMessageToRobot((Int16)Commands.PC2R_TirMoveDown, 0, null);
        }

        public void GenerateMessageEnablePowerMonitoring(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);
            OnMessageToRobot((Int16)Commands.PC2R_PowerMonitoringEnable, 1, payload);
        }
        public void GenerateMessageEnableIOPolling(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);
            OnMessageToRobot((Int16)Commands.PC2R_IOPollingEnable , 1, payload);
        }

        public void GenerateMessageEnableDisableMotors(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);
            OnMessageToRobot((Int16)Commands.PC2R_MotorsEnableDisable, 1, payload);
        }
        public void GenerateMessageResetSpeedPid(object sender, RobotIdEventArgs e)
        {
            OnMessageToRobot((Int16)Commands.PC2R_SpeedPIDReset, 0, null);
        }

        public void GenerateMessageForwardHerkulex(object sender, DataReceivedArgs e)
        {
            OnMessageToRobot((Int16)Commands.PC2R_HerkulexForward, (short)e.Data.Length, e.Data);
        }

        public void GenerateMessageEnableDisableTir(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);
            OnMessageToRobot((Int16)Commands.PC2R_TirEnableDisable, 1, payload);
        }

        public void GenerateMessageSetAsservissementMode(object sender, ByteEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.Value);
            OnMessageToRobot((Int16)Commands.PC2R_SetAsservissementMode, 1, payload);
        }

        public void GenerateMessageSpeedPIDEnableDebugInternal(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);
            OnMessageToRobot((Int16)Commands.PC2R_SpeedPIDEnableDebugInternal, 1, payload);
        }

        public void GenerateMessageSpeedPIDEnableDebugErrorCorrectionConsigne(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);
            OnMessageToRobot((Int16)Commands.PC2R_SpeedPIDEnableDebugErrorCorrectionConsigne, 1, payload);
        }

        public void GenerateMessageEnableEncoderRawData(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);
            OnMessageToRobot((Int16)Commands.PC2R_EncoderRawMonitoringEnable, 1, payload);
        }

        public void GenerateMessageOdometryPointToMeter(object sender, DoubleEventArgs e)
        {
            byte[] payload = new byte[4];
            payload.SetValueRange(((float)(e.Value)).GetBytes(), 0);
            OnMessageToRobot((Int16)Commands.PC2R_OdometryPointToMeter, 4, payload);
        }

        public void GenerateMessage4WheelsAngleSet(object sender, FourWheelsAngleArgs e)
        {
            byte[] payload = new byte[16];
            payload.SetValueRange(((float)(e.angleMotor1)).GetBytes(), 0);
            payload.SetValueRange(((float)(e.angleMotor2)).GetBytes(), 4);
            payload.SetValueRange(((float)(e.angleMotor3)).GetBytes(), 8);
            payload.SetValueRange(((float)(e.angleMotor4)).GetBytes(), 12);
            OnMessageToRobot((Int16)Commands.PC2R_4WheelsAngleSet, 16, payload);
        }

        public void GenerateMessage2WheelsAngleSet(object sender, TwoWheelsAngleArgs e)
        {
            byte[] payload = new byte[8];
            payload.SetValueRange(((float)(e.angleMotor1)).GetBytes(), 0);
            payload.SetValueRange(((float)(e.angleMotor2)).GetBytes(), 4);
            OnMessageToRobot((Int16)Commands.PC2R_2WheelsAngleSet, 8, payload);
        }

        public void GenerateMessage4WheelsToPolarMatrixSet(object sender, FourWheelsToPolarMatrixArgs e)
        {
            byte[] payload = new byte[48];
            payload.SetValueRange(((float)(e.mx1)).GetBytes(), 0);
            payload.SetValueRange(((float)(e.mx2)).GetBytes(), 4);
            payload.SetValueRange(((float)(e.mx3)).GetBytes(), 8);
            payload.SetValueRange(((float)(e.mx4)).GetBytes(), 12);
            payload.SetValueRange(((float)(e.my1)).GetBytes(), 16);
            payload.SetValueRange(((float)(e.my2)).GetBytes(), 20);
            payload.SetValueRange(((float)(e.my3)).GetBytes(), 24);
            payload.SetValueRange(((float)(e.my4)).GetBytes(), 28);
            payload.SetValueRange(((float)(e.mtheta1)).GetBytes(), 32);
            payload.SetValueRange(((float)(e.mtheta2)).GetBytes(), 36);
            payload.SetValueRange(((float)(e.mtheta3)).GetBytes(), 40);
            payload.SetValueRange(((float)(e.mtheta4)).GetBytes(), 44);
            OnMessageToRobot((Int16)Commands.PC2R_4WheelsToPolarMatrixSet, 48, payload);
        }

        public void GenerateMessage2WheelsToPolarMatrixSet(object sender, TwoWheelsToPolarMatrixArgs e)
        {
            byte[] payload = new byte[16];
            payload.SetValueRange(((float)(e.mx1)).GetBytes(), 0);
            payload.SetValueRange(((float)(e.mx2)).GetBytes(), 4);
            payload.SetValueRange(((float)(e.mtheta1)).GetBytes(), 8);
            payload.SetValueRange(((float)(e.mtheta2)).GetBytes(), 12);
            OnMessageToRobot((Int16)Commands.PC2R_2WheelsToPolarMatrixSet, 16, payload);
        }

        public void GenerateMessageEnableMotorCurrentData(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);
            OnMessageToRobot((Int16)Commands.PC2R_MotorCurrentMonitoringEnable, 1, payload);
        }

        public void GenerateMessageEnableMotorSpeedConsigne(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);
            OnMessageToRobot((Int16)Commands.PC2R_SpeedConsigneMonitoringEnable, 1, payload);
        }

        public void GenerateMessageSTOP(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);

            OnMessageToRobot((Int16)Commands.PC2R_EmergencyStop, 1, payload);
        }

        public void GenerateMessageSetupSpeedPolarPIDToRobot(object sender, PolarPIDSetupArgs e)
        {
            byte[] payload = new byte[72];
            payload.SetValueRange(((float)(e.P_x)).GetBytes(), 0);
            payload.SetValueRange(((float)(e.I_x)).GetBytes(), 4);
            payload.SetValueRange(((float)(e.D_x)).GetBytes(), 8);
            payload.SetValueRange(((float)(e.P_y)).GetBytes(), 12);
            payload.SetValueRange(((float)(e.I_y)).GetBytes(), 16);
            payload.SetValueRange(((float)(e.D_y)).GetBytes(), 20);
            payload.SetValueRange(((float)(e.P_theta)).GetBytes(), 24);
            payload.SetValueRange(((float)(e.I_theta)).GetBytes(), 28);
            payload.SetValueRange(((float)(e.D_theta)).GetBytes(), 32);
            payload.SetValueRange(((float)(e.P_x_Limit)).GetBytes(), 36);
            payload.SetValueRange(((float)(e.I_x_Limit)).GetBytes(), 40);
            payload.SetValueRange(((float)(e.D_x_Limit)).GetBytes(), 44);
            payload.SetValueRange(((float)(e.P_y_Limit)).GetBytes(), 48);
            payload.SetValueRange(((float)(e.I_y_Limit)).GetBytes(), 52);
            payload.SetValueRange(((float)(e.D_y_Limit)).GetBytes(), 56);
            payload.SetValueRange(((float)(e.P_theta_Limit)).GetBytes(), 60);
            payload.SetValueRange(((float)(e.I_theta_Limit)).GetBytes(), 64);
            payload.SetValueRange(((float)(e.D_theta_Limit)).GetBytes(), 68);
            OnMessageToRobot((Int16)Commands.PC2R_SpeedPolarPIDSetGains, 72, payload);
            OnMessageToDisplaySpeedPolarPidSetup(e);
        }

        public void GenerateMessageSetupSpeedIndependantPIDToRobot(object sender, IndependantPIDSetupArgs e)
        {
            byte[] payload = new byte[96];
            payload.SetValueRange(((float)(e.P_M1)).GetBytes(), 0);
            payload.SetValueRange(((float)(e.I_M1)).GetBytes(), 4);
            payload.SetValueRange(((float)(e.D_M1)).GetBytes(), 8);
            payload.SetValueRange(((float)(e.P_M2)).GetBytes(), 12);
            payload.SetValueRange(((float)(e.I_M2)).GetBytes(), 16);
            payload.SetValueRange(((float)(e.D_M2)).GetBytes(), 20);
            payload.SetValueRange(((float)(e.P_M3)).GetBytes(), 24);
            payload.SetValueRange(((float)(e.I_M3)).GetBytes(), 28);
            payload.SetValueRange(((float)(e.D_M3)).GetBytes(), 32);
            payload.SetValueRange(((float)(e.P_M4)).GetBytes(), 36);
            payload.SetValueRange(((float)(e.I_M4)).GetBytes(), 40);
            payload.SetValueRange(((float)(e.D_M4)).GetBytes(), 44);
            payload.SetValueRange(((float)(e.P_M1_Limit)).GetBytes(), 48);
            payload.SetValueRange(((float)(e.I_M1_Limit)).GetBytes(), 52);
            payload.SetValueRange(((float)(e.D_M1_Limit)).GetBytes(), 56);
            payload.SetValueRange(((float)(e.P_M2_Limit)).GetBytes(), 60);
            payload.SetValueRange(((float)(e.I_M2_Limit)).GetBytes(), 64);
            payload.SetValueRange(((float)(e.D_M2_Limit)).GetBytes(), 68);
            payload.SetValueRange(((float)(e.P_M3_Limit)).GetBytes(), 72);
            payload.SetValueRange(((float)(e.I_M3_Limit)).GetBytes(), 76);
            payload.SetValueRange(((float)(e.D_M3_Limit)).GetBytes(), 80);
            payload.SetValueRange(((float)(e.P_M4_Limit)).GetBytes(), 84);
            payload.SetValueRange(((float)(e.I_M4_Limit)).GetBytes(), 88);
            payload.SetValueRange(((float)(e.D_M4_Limit)).GetBytes(), 92);
            OnMessageToRobot((Int16)Commands.PC2R_SpeedIndependantPIDSetGains, 96, payload);
            OnMessageToDisplaySpeedIndependantPidSetup(e);
        }

        public void GenerateMessage2WheelsPolarSpeedPIDSetup(object sender, PolarPIDSetupArgs e)
        {
            byte[] payload = new byte[48];
            payload.SetValueRange(((float)(e.P_x)).GetBytes(), 0);
            payload.SetValueRange(((float)(e.I_x)).GetBytes(), 4);
            payload.SetValueRange(((float)(e.D_x)).GetBytes(), 8);
            payload.SetValueRange(((float)(e.P_theta)).GetBytes(), 12);
            payload.SetValueRange(((float)(e.I_theta)).GetBytes(), 16);
            payload.SetValueRange(((float)(e.D_theta)).GetBytes(), 20);
            payload.SetValueRange(((float)(e.P_x_Limit)).GetBytes(), 24);
            payload.SetValueRange(((float)(e.I_x_Limit)).GetBytes(), 28);
            payload.SetValueRange(((float)(e.D_x_Limit)).GetBytes(), 32);
            payload.SetValueRange(((float)(e.P_theta_Limit)).GetBytes(), 36);
            payload.SetValueRange(((float)(e.I_theta_Limit)).GetBytes(), 40);
            payload.SetValueRange(((float)(e.D_theta_Limit)).GetBytes(), 44);
            OnMessageToRobot((Int16)Commands.PC2R_2WheelsPolarSpeedPIDSetGains, 48, payload);
            OnMessageToDisplaySpeedPolarPidSetup(e);
        }

        public void GenerateMessage2WheelsIndependantSpeedPIDSetup(object sender, IndependantPIDSetupArgs e)
        {
            byte[] payload = new byte[48];
            payload.SetValueRange(((float)(e.P_M1)).GetBytes(), 0);
            payload.SetValueRange(((float)(e.I_M1)).GetBytes(), 4);
            payload.SetValueRange(((float)(e.D_M1)).GetBytes(), 8);
            payload.SetValueRange(((float)(e.P_M2)).GetBytes(), 12);
            payload.SetValueRange(((float)(e.I_M2)).GetBytes(), 16);
            payload.SetValueRange(((float)(e.D_M2)).GetBytes(), 20);
            payload.SetValueRange(((float)(e.P_M1_Limit)).GetBytes(), 24);
            payload.SetValueRange(((float)(e.I_M1_Limit)).GetBytes(), 28);
            payload.SetValueRange(((float)(e.D_M1_Limit)).GetBytes(), 32);
            payload.SetValueRange(((float)(e.P_M2_Limit)).GetBytes(), 36);
            payload.SetValueRange(((float)(e.I_M2_Limit)).GetBytes(), 40);
            payload.SetValueRange(((float)(e.D_M2_Limit)).GetBytes(), 44);
            OnMessageToRobot((Int16)Commands.PC2R_2WheelsIndependantSpeedPIDSetGains, 48, payload);
            OnMessageToDisplaySpeedIndependantPidSetup(e);
        }

        //public void GenerateTextMessage(object sender, EventArgsLibrary.SpeedConsigneArgs e)
        //{
        //    byte[] payload = new byte[12];
        //    payload.SetValueRange(e.Vx.GetBytes(), 0);
        //    payload.SetValueRange(e.Vy.GetBytes(), 4);
        //    payload.SetValueRange(e.Vtheta.GetBytes(), 8);
        //    OnMessageToRobot(Commands., 12, payload);
        //}

        //Output events
        public event EventHandler<MessageByteArgs> OnMessageToRobotGeneratedEvent;
        public virtual void OnMessageToRobot(Int16 msgFunction, Int16 msgPayloadLength, byte[] msgPayload)
        {
            OnMessageToRobotGeneratedEvent?.Invoke(this, new MessageByteArgs((ushort) msgFunction, (ushort) msgPayloadLength, msgPayload, 0x00));
        }

        public event EventHandler<PolarPIDSetupArgs> OnMessageToDisplaySpeedPolarPidSetupEvent;
        public virtual void OnMessageToDisplaySpeedPolarPidSetup(PolarPIDSetupArgs setup)
        {
            OnMessageToDisplaySpeedPolarPidSetupEvent?.Invoke(this, setup);
        }

        public event EventHandler<IndependantPIDSetupArgs> OnMessageToDisplaySpeedIndependantPidSetupEvent;
        public virtual void OnMessageToDisplaySpeedIndependantPidSetup(IndependantPIDSetupArgs setup)
        {
            OnMessageToDisplaySpeedIndependantPidSetupEvent?.Invoke(this, setup);
        }

        //public event EventHandler<MessageToRobotArgs> OnMessageToRobotGeneratedEvent;
        //public virtual void OnMessageToRobot(Int16 msgFunction, Int16 msgPayloadLength, byte[] msgPayload)
        //{
        //    OnMessageToRobotGeneratedEvent?.Invoke(this, new MessageToRobotArgs { MsgFunction = msgFunction, MsgPayloadLength = msgPayloadLength, MsgPayload = msgPayload });
        //}
    }
}
