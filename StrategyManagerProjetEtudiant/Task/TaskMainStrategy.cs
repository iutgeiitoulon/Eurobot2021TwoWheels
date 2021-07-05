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
            parent.OnSetActualLocation(new Location(1.200, 0.200, Math.PI, 0, 0, 0));
        }

        public void Reset()
        {
            timestamp = DateTime.Now;
            parent.taskArm.Init();
            parent.taskRackPrehension.Init();
            parent.taskRaiseFlag.Init();
            //parent.taskTurbine.Init(); // TODO : make reinit possible

            parent.OnEnableDisableMotors(false);
            parent.OnResetGhostPosition();
        }

        public override void TaskStateMachine()
        {
            if (parent.isIOReceived)
            {
                if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 96999 && state != GameState.Idle && state != GameState.WaitingForJack && state != GameState.EndMatch)
                {
                    ResetSubState();
                    state = GameState.EndMatch;
                }

                //checking jack state
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
                                Reset();
                                parent.taskRackPrehension.SetRackPositionToVertical();
                                parent.taskTurbine.TurnAllOff();


                                break;

                            case SubTaskState.EnCours:
                                if (!Jack)
                                {
                                    ExitState();
                                }
                                break;

                            case SubTaskState.Exit:
                                parent.OnResetGhostPosition();
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
                                Console.WriteLine("Match en cours");
                                Console.WriteLine("timestamp = " + DateTime.Now.Subtract(timestamp).TotalSeconds);
                                parent.taskRackPrehension.SetRackPositionToHorizontal();
                                //parent.OnCalibatrionAsked();
                                //parent.OnEnableDisableMotors(true);

                                break;

                            case SubTaskState.EnCours:
                                if (parent.taskRackPrehension.isFinished)
                                    ExitState();
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
                                parent.taskRaiseFlag.Start();
                                break;
                            case SubTaskState.EnCours:
                                if (parent.taskRaiseFlag.isFinished)
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
