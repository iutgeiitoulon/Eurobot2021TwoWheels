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

        public PointD robotDestination = new PointD(0, 0);
        PlayingSide playingSide = PlayingSide.Left;     

        TaskDemoMove taskDemoMove;
        TaskDemoMessage taskDemoMessage;
        TaskDestination taskDestination;
        TaskUpdateCupWaypoints taskSetupWaypoints;
        TaskSetupFieldZone taskSetupDeadZone;

        int robotId, teamId;

        Timer configTimer;


        public StrategyEurobot(int robotId, int teamId, string multicastIpAddress) : base(robotId, teamId, multicastIpAddress)
        {
            taskDemoMove = new TaskDemoMove(this);
            taskDemoMessage = new TaskDemoMessage(this);
            taskDestination = new TaskDestination(this);
            taskSetupWaypoints = new TaskUpdateCupWaypoints(this);
            taskSetupDeadZone = new TaskSetupFieldZone(this);

            this.robotId = robotId;
            this.teamId = teamId;
            localWorldMap = new LocalWorldMap(robotId, teamId);

        }

        public override void InitStrategy()
        {
            configTimer = new System.Timers.Timer(1000);
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

            OnSetAsservissementMode((byte) AsservissementMode.Independant2Wheels);
        }


        /*********************************** Events reçus **********************************************/

        public override void OnGhostLocationReached(object sender, LocationArgs location)
        {
            
        }

        public override void OnRobotLocationReached(object sender, LocationArgs e)
        {
            if (taskDestination != null)
                taskDestination.TaskReached();
        }

        /*********************************** Events de sortie **********************************************/
    }
}
