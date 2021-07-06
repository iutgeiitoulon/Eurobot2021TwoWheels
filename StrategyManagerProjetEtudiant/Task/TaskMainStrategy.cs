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
            GetPrivateRack,
            EndMatch,
            ReturnToHarbor
        }


        double RobotInitialX = 1.287;
        double RobotInitialY = 0.200;
        double RobotInitialTheta = Math.PI;

        DateTime timestamp;
        GameState state = GameState.WaitingForJack;
        public bool Jack = false;

        public TaskMainStrategy(StrategyEurobot parent) : base(parent)
        {
            timestamp = DateTime.Now;
            ResetSubState();
            parent.OnAddServo(ServoId.Rack1, HerkulexDescription.JOG_MODE.positionControlJOG);
            parent.OnAddServo(ServoId.Rack2, HerkulexDescription.JOG_MODE.positionControlJOG);
            Init();
        }
        public override void Init()
        {
            timestamp = DateTime.Now;
            
            parent.OnResetGhostPosition();
            parent.OnPololuSetUs(PololuActuators.ServoAscenseur, (ushort)GruePositions.Low);
            parent.taskRackPrehension.SetRackPositionToVertical();
            parent.taskTurbine.TurnAllOff();

            parent.missionGetPrivateRack.Init();
            parent.missionRaiseFlag.Init();
            
        }

        public override void TaskStateMachine()
        {
            if (parent.isIOReceived)
            {

                if (Jack && !(state == GameState.Idle || state == GameState.WaitingForJack))
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
                                Init();
                                parent.OnEnableDisableMotors(false);
                                break;

                            case SubTaskState.EnCours:
                                if (Jack)
                                    ExitState();
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
                                Init();
                                parent.OnEnableDisableMotors(false);
                                break;

                            case SubTaskState.EnCours:
                                if (!Jack)
                                    ExitState();
                                break;

                            case SubTaskState.Exit:
                                parent.OnSetActualLocation(new Location(RobotInitialX, RobotInitialY, RobotInitialTheta, 0, 0, 0));
                                parent.OnSetWantedLocation(RobotInitialX, RobotInitialY);
                                state = GameState.GetPrivateRack;
                                timestamp = DateTime.Now;
                                break;
                        }
                        break;
                    #endregion
                    #region MatchRunnig
                    case GameState.GetPrivateRack:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("GOTO RACK PRIV JAUNE");
                                parent.OnCalibatrionAsked();
                                parent.OnEnableDisableMotors(true); 
                                parent.missionGetPrivateRack.Start();

                                break;

                            case SubTaskState.EnCours:
                                if (parent.missionGetPrivateRack.isFinished)
                                    ExitState();
                                break;

                            case SubTaskState.Exit:
                                state = GameState.ReturnToHarbor;
                                break;
                        }
                        break;
                    #endregion

                    #region 
                    case GameState.ReturnToHarbor:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("ReturnToHarbor");
                                PointD harbor = parent.localWorldMap.Fields.Where(x => x.Type == FieldType.StartZone).FirstOrDefault().Shape.Center;
                                parent.OnSetWantedLocation(harbor.X, harbor.Y, false, Math.PI);
                                break;

                            case SubTaskState.EnCours:
                                if (parent.isDeplacementFinished)
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
