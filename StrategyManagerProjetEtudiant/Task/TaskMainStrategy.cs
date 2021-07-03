using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerProjetEtudiantNS.Task
{
    class TaskMainStrategy : TaskBase
    {

        public enum GameState
        {
            WaitingForJack,
            Idle,
            Turbine
        }

        GameState state = GameState.WaitingForJack;

        public TaskMainStrategy(StrategyEurobot parent) : base(parent)
        {

        }
        public override void Init()
        {

        }

        public override void TaskStateMachine()
        {
            switch (state)
            {
                case GameState.WaitingForJack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnEnableDisableMotors(false);
                            parent.taskTurbine.TurnAllOff();
                            break;

                        case SubTaskState.EnCours:
                            if(parent.JackState)
                            {
                                ExitState();
                            }
                            break;

                        case SubTaskState.Exit:
                            state = GameState.Idle;
                            break;

                    }
                    break;
                case GameState.Idle:
                    //insert SubMachine
                    break;
            }
        }
    }
}
