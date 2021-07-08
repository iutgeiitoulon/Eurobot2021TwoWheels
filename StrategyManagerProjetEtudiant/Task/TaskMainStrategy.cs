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
            GetTrashCup,
            PutHarbor,
            GetPublicRackAlly,
            PutDownNorthBand,
            ActivateBeacon,
            
            TakeDownWindFlag,
            GetPrivateRack,
            PutDownSouthBand,
            StopMoving,
            EndMatch,
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
            parent.OnAddServo(ServoId.Drapeau, HerkulexDescription.JOG_MODE.positionControlJOG);
            Init();
        }
        public override void Init()
        {
            timestamp = DateTime.Now;

            parent.missionWindFlags.Init();
            parent.missionGetPrivateRack.Init();
            parent.missionGetRackPublicAlly.Init();
            parent.missionPutHarbor.Init();
            parent.missionGetNorthTrashCups.Init();
            parent.missionPutNorthBand.Init();
            parent.missionPutSouthBand.Init();
            parent.missionRaiseFlag.Init();
            parent.missionPutNorthBand.Init();
            parent.missionActivateBeacon.Init();
            parent.taskTurbine.Init();
            parent.taskArm.Init();

            parent.OnResetGhostPosition();
            parent.OnPololuSetUs(PololuActuators.ServoAscenseur, (ushort)GruePositions.Low);
            parent.taskArm.SetArmDown();
            parent.taskRackPrehension.SetRackPositionToVertical();
            parent.taskTurbine.TurnAllOff();
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

                if (state != GameState.EndMatch && state != GameState.Idle && state != GameState.WaitingForJack && DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 96000)
                {
                    ResetSubState();
                    state = GameState.EndMatch;
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
                                if (parent.localWorldMap.Team == TeamColor.Yellow)
                                {
                                    parent.OnSetActualLocation(new Location(RobotInitialX, RobotInitialY, RobotInitialTheta, 0, 0, 0));
                                    parent.OnSetWantedLocation(RobotInitialX, RobotInitialY);
                                }
                                else if (parent.localWorldMap.Team == TeamColor.Blue)
                                {
                                    parent.OnSetActualLocation(new Location(-RobotInitialX, RobotInitialY, RobotInitialTheta + Math.PI, 0, 0, 0));
                                    parent.OnSetWantedLocation(-RobotInitialX, RobotInitialY);
                                }
                                state = GameState.GetTrashCup;
                                //state = GameState.GetPrivateRack; /// TEMP
                                timestamp = DateTime.Now;
                                break;
                        }
                        break;
                    #endregion
                    #region GetTrashCup
                    case GameState.GetTrashCup:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("GetTrashCup");
                                //parent.OnCalibatrionAsked();
                                parent.OnDisableAvoidance();
                                parent.OnEnableDisableMotors(true);
                                parent.missionGetNorthTrashCups.Start();

                                break;

                            case SubTaskState.EnCours:
                                if (parent.missionGetNorthTrashCups.isFinished)
                                    ExitState();
                                break;

                            case SubTaskState.Exit:
                                state = GameState.PutHarbor;
                                break;
                        }
                        break;
                    #endregion
                    #region PutHarbor
                    case GameState.PutHarbor:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("PutHarbor");
                                //parent.OnCalibatrionAsked();
                                parent.OnEnableDisableMotors(true);
                                parent.missionPutHarbor.Start();

                                break;

                            case SubTaskState.EnCours:
                                if (parent.missionPutHarbor.isFinished)
                                    ExitState();
                                break;

                            case SubTaskState.Exit:
                                state = GameState.GetPublicRackAlly;
                                break;
                        }
                        break;
                    #endregion
                    #region GetPublicAllyRack
                    case GameState.GetPublicRackAlly:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("GetPublicAlly");
                                parent.OnCalibatrionAsked();
                                parent.OnEnableDisableMotors(true);
                                parent.missionGetRackPublicAlly.Start();
                                parent.OnEnableAvoidance();

                                break;

                            case SubTaskState.EnCours:
                                if (parent.missionGetRackPublicAlly.isFinished)
                                    ExitState();
                                break;

                            case SubTaskState.Exit:
                                state = GameState.PutDownNorthBand;
                                break;
                        }
                        break;
                    #endregion
                    #region Put North Band
                    case GameState.PutDownNorthBand:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("PutDownNorthBand");
                                //parent.OnCalibatrionAsked();
                                parent.OnEnableDisableMotors(true);
                                parent.missionPutNorthBand.Start();

                                break;

                            case SubTaskState.EnCours:
                                if (parent.missionPutNorthBand.isFinished)
                                    ExitState();
                                break;

                            case SubTaskState.Exit:
                                state = GameState.TakeDownWindFlag;
                                break;
                        }
                        break;
                    #endregion

                    #region ActivateBeacon
                    case GameState.ActivateBeacon:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("ActivateBeacon");
                                //parent.OnCalibatrionAsked();
                                parent.OnEnableDisableMotors(true);
                                parent.missionActivateBeacon.Start();

                                break;

                            case SubTaskState.EnCours:
                                if (parent.missionActivateBeacon.isFinished)
                                    ExitState();
                                break;

                            case SubTaskState.Exit:
                                state = GameState.TakeDownWindFlag;
                                break;
                        }
                        break;
                    #endregion

                    #region WindFlags
                    case GameState.TakeDownWindFlag:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("WindFlags");

                                /// TEMP:
                                //parent.OnCalibatrionAsked();
                                parent.OnEnableDisableMotors(true);
                                parent.missionWindFlags.Start();
                                break;

                            case SubTaskState.EnCours:
                                if (parent.missionWindFlags.isFinished)
                                    ExitState();
                                break;

                            case SubTaskState.Exit:
                                state = GameState.GetPrivateRack;
                                break;
                        }
                        break;
                    #endregion
                    #region GetPrivateRack
                    case GameState.GetPrivateRack:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("GetPrivateRack");
                                //parent.OnCalibatrionAsked();
                                parent.OnEnableDisableMotors(true);
                                parent.missionGetPrivateRack.Start();

                                break;

                            case SubTaskState.EnCours:
                                if (parent.missionGetPrivateRack.isFinished)
                                    ExitState();
                                break;

                            case SubTaskState.Exit:
                                state = GameState.PutDownSouthBand;
                                break;
                        }
                        break;
                    #endregion
                    #region Puts SimpleCups
                    case GameState.PutDownSouthBand:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("Put Simple Cups");
                                parent.missionPutSouthBand.Start();
                                break;

                            case SubTaskState.EnCours:
                                if (parent.missionPutSouthBand.isFinished)
                                    ExitState(); /// TEMP
                                break;

                            case SubTaskState.Exit:
                                state = GameState.StopMoving;
                                break;
                        }
                        break;
                    #endregion
                    #region StopMoving
                    case GameState.StopMoving:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("Stop Moving");
                                parent.OnEnableDisableMotors(false);
                                break;

                            case SubTaskState.EnCours:
                                //if (parent.missionSimplePutCup.isFinished)
                                //    ExitState(); /// TEMP
                                break;

                            case SubTaskState.Exit:
                                break;
                        }
                        break;
                    #endregion
                    #region EndMatch
                    case GameState.EndMatch:
                        switch (subState)
                        {
                            case SubTaskState.Entry:
                                Console.WriteLine("Match Ended !");
                                parent.OnEnableDisableMotors(false);
                                parent.missionRaiseFlag.Start();
                                break;
                            case SubTaskState.EnCours:
                                if (parent.missionRaiseFlag.isFinished)
                                    ExitState();
                                break;

                            case SubTaskState.Exit:
                                isFinished = true;
                                break;
                        }
                        break;
                        #endregion
                }
            }
        }
    }
}
