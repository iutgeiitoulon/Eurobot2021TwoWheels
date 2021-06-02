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

        int robotId, teamId;

        Timer configTimer;


        public StrategyEurobot(int robotId, int teamId, string multicastIpAddress) : base(robotId, teamId, multicastIpAddress)
        {
            taskDemoMove = new TaskDemoMove(this);
            taskDemoMessage = new TaskDemoMessage(this);
            taskDestination = new TaskDestination(this);

            this.robotId = robotId;
            this.teamId = teamId;
            localWorldMap = new LocalWorldMap(robotId, teamId);

        }

        public override void InitStrategy()
        {
            configTimer = new System.Timers.Timer(1000);
            configTimer.Elapsed += ConfigTimer_Elapsed; ;
            configTimer.Start();

            /// Use only when the robot is disconnect
            //GhostTimer.Start();
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
            //On2WheelsIndependantSpeedPIDSetup(pM1: 5, iM1: 0, 0.0, pM2: 0, iM2: 0, 0, pM1Limit: 100, iM1Limit: 0, 0, pM2Limit: 0.0, iM2Limit: 0.0, 0);
            double KpIndependant = 5;
            double KiIndependant = 50;
            //On envoie périodiquement les réglages du PID de vitesse embarqué
            On2WheelsIndependantSpeedPIDSetup(pM1: KpIndependant, iM1: KiIndependant, 0.0, pM2: KpIndependant, iM2: KiIndependant, 0,
                pM1Limit: 4, iM1Limit: 4, 0, pM2Limit: 4.0, iM2Limit: 4.0, 0);

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
