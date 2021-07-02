using Constants;
using EventArgsLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Utilities;
using WorldMap;


namespace StrategyManagerProjetEtudiantNS
{
    public class StrategyEurobot : StrategyGenerique
    {
        Stopwatch sw = new Stopwatch();
        int robotId, teamId;
        
        Timer configTimer;

        public TaskStrategy taskStrategy;
        public TaskCalibrate taskCalibrate;
        public TaskWindFlag taskWindFlag;
        public TaskReturnHarbor taskReturnHarbor;
        public TaskActivateBeacon taskActivateBeacon;
        public TaskHerkulexTest taskHerkulexTest;


        public StrategyEurobot(int robotId, int teamId, string multicastIpAddress) : base(robotId, teamId, multicastIpAddress)
        {
            this.robotId = robotId;
            this.teamId = teamId;
            localWorldMap = new LocalWorldMap(robotId, teamId);

            
        }

        public override void InitStrategy()
        {
            /*taskHerkulexTest = new TaskHerkulexTest(this);
            taskWindFlag = new TaskWindFlag(this);
            taskReturnHarbor = new TaskReturnHarbor(this);
            taskActivateBeacon = new TaskActivateBeacon(this);
            taskCalibrate = new TaskCalibrate(this);
            taskStrategy = new TaskStrategy(this);*/

            configTimer = new Timer(1000);
            configTimer.Elapsed += ConfigTimer_Elapsed;
            configTimer.Start();          
        }

        private void ConfigTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            /// Obtenus directement à partir du script Matlab
            OnOdometryPointToMeter(ConstVar.EUROBOT_ODOMETRY_POINT_TO_METER);

            On2WheelsAngleSetup(
                -ConstVar.EUROBOT_WHEELS_ANGLE,
                ConstVar.EUROBOT_WHEELS_ANGLE
            );

            On2WheelsToPolarMatrixSetup(
                ConstVar.EUROBOT_MATRIX_X_COEFF,
                -ConstVar.EUROBOT_MATRIX_X_COEFF,
                ConstVar.EUROBOT_MATRIX_THETA_COEFF,
                ConstVar.EUROBOT_MATRIX_THETA_COEFF
            );

            

            //On envoie périodiquement les réglages du PID de vitesse embarqué
            On2WheelsIndependantSpeedPIDSetup( 
                ConstVar.PID_SPEED_MOTOR1_KP, ConstVar.PID_SPEED_MOTOR1_KI, ConstVar.PID_SPEED_MOTOR1_KD,
                ConstVar.PID_SPEED_MOTOR2_KP, ConstVar.PID_SPEED_MOTOR2_KI, ConstVar.PID_SPEED_MOTOR2_KD,
                ConstVar.PID_SPEED_MOTOR1_KP_MAX, ConstVar.PID_SPEED_MOTOR1_KI_MAX, ConstVar.PID_SPEED_MOTOR1_KD_MAX,
                ConstVar.PID_SPEED_MOTOR2_KP_MAX, ConstVar.PID_SPEED_MOTOR2_KI_MAX, ConstVar.PID_SPEED_MOTOR2_KD_MAX
                );

            OnSetAsservissementMode((byte)AsservissementMode.Independant2Wheels);
            //OnEnableDisableIndependant2WheelsPIDGainDebug(true);
            //OnEnableDisableMotorCurrentData(true);

        }
        private void IOValues(bool jack, bool team)
        {
            taskStrategy.Jack = jack;
            OnSetupTeamColor(team ? TeamColor.Yellow : TeamColor.Blue);
        }

        /*********************************** Events reçus **********************************************/
        public override void OnIOValuesReceived(object sender, IOValuesEventArgs e)
        {
            byte configStatus = Convert.ToByte(e.ioValues);
            bool jack = (configStatus & 1) != 0;
            bool config1 = (configStatus & 2) != 0;
            bool config2 = (configStatus & 4) != 0;
            bool config3 = (configStatus & 8) != 0;
            bool config4 = (configStatus & 16) != 0;

            IOValues(jack, config1);
        }

        /*********************************** Events de sortie **********************************************/
    }
}
