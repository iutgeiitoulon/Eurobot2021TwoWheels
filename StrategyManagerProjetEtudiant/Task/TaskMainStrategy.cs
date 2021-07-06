using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;
using HerkulexManagerNs;
using Utilities;

namespace StrategyManagerProjetEtudiantNS
{
    public class TaskMainStrategy : TaskBase
    {

        public enum GameState
        {
            WaitingForJack,
            Idle,
            Turbine,
            MatchRunning,
            EndMatch,
            MatchStopped
        }


        double RobotInitialX = 1.287;
        double RobotInitialY = 0.200;
        double RobotInitialTheta = Math.PI;

        DateTime timestamp;
        GameState state = GameState.WaitingForJack;
        public bool Jack = false;

        public TaskMainStrategy(StrategyEurobot parent) : base(parent)
        {
            Init();
        }
        public override void Init()
        {
            timestamp = DateTime.Now;
            ResetSubState();
            parent.OnAddServo(ServoId.Rack1, HerkulexDescription.JOG_MODE.positionControlJOG);
            parent.OnAddServo(ServoId.Rack2, HerkulexDescription.JOG_MODE.positionControlJOG);
        }

        public void Reset()
        {
            timestamp = DateTime.Now;


            parent.OnEnableDisableMotors(false);
            parent.OnResetGhostPosition();
        }

        public override void TaskStateMachine()
        {
            if (parent.isIOReceived)
            {

                if (Jack && (state != GameState.Idle))
                {
                    ResetSubState();
                    state = GameState.Idle;
                }

                switch (state)
                {
                    #region WaitingForJack
                    case GameState.WaitingForJack:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("Waiting For Jack");
                                parent.OnEnableDisableMotors(false);
                                Reset();
                                break;

                            case SubTaskState.EnCours:
                                if (Jack)
                                {
                                    ExitState();
                                }
                                break;

                            case SubTaskState.Exit:
                                state = GameState.Idle;
                                break;

                        }
                        break;
                    #endregion
                    #region Idle
                    case GameState.Idle:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("Idle mode");
                                parent.OnEnableDisableMotors(false);
                                parent.taskRackPrehension.SetRackPositionToVertical();
                                parent.taskTurbine.TurnAllOff();
                                parent.OnPololuSetUs(PololuActuators.ServoAscenseur, (ushort)GruePositions.Low);
                                break;

                            case SubTaskState.EnCours:
                                if (!Jack)
                                {
                                    ExitState();
                                }
                                break;

                            case SubTaskState.Exit:
                                parent.OnEnableDisableMotors(true);
                                parent.OnSetActualLocation(new Location(RobotInitialX, RobotInitialY, RobotInitialTheta, 0, 0, 0));
                                parent.OnSetWantedLocation(RobotInitialX, RobotInitialY, RobotInitialTheta);
                                // parent.OnCalibatrionAsked();
                                state = GameState.MatchRunning;
                                timestamp = DateTime.Now;
                                break;
                        }
                        break;
                    #endregion
                    #region MatchRunnig
                    case GameState.MatchRunning:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("GOTO RACK PRIV JAUNE");
                                parent.taskPrivJaune.Start();

                                break;

                            case SubTaskState.EnCours:
                                if (parent.taskPrivJaune.isFinished)
                                    ExitState();
                                break;

                            case SubTaskState.Exit:
                                state = GameState.WaitingForJack;
                                break;
                        }
                        break;
                    #endregion

                    #region EndMatch
                    //case GameState.EndMatch:
                    //    switch (subState)
                    //    {
                    //        case SubTaskState.Entry:
                    //            Console.WriteLine("Match Ended !");
                    //            parent.taskRaiseFlag.Start();
                    //            break;
                    //        case SubTaskState.EnCours:
                    //            if (parent.taskRaiseFlag.isFinished)
                    //                ExitState();
                    //            break;

                    //        case SubTaskState.Exit:
                    //            isFinished = true;
                    //            break;
                    //    }
                    //    break;
                        #endregion

                        

                }
            }
        }
    }
}
