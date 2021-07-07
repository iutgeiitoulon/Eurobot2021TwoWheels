using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionSimplePutCup : TaskBase
    {
        DateTime timestamp;
        public enum MissionSimplePutCupState
        {
            Waiting,
            LowerRack,
            StopTurbine,
            UpperRack,
        }


        MissionSimplePutCupState state = MissionSimplePutCupState.Waiting;

        public MissionSimplePutCup(StrategyEurobot p) : base(p)
        {
            Init();
        
        }

        public void Start()
        {
            ResetSubState();
            state = MissionSimplePutCupState.LowerRack;
        }

        public override void Init()
        {
            isFinished = false;
            state = MissionSimplePutCupState.Waiting;
        }

        public override void TaskStateMachine()
        {
            switch (state)
            {

                case MissionSimplePutCupState.Waiting:
                    break;

                case MissionSimplePutCupState.LowerRack:
                    switch(subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskRackPrehension.SetRackPositionToPutCups();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.taskRackPrehension.isFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionSimplePutCupState.StopTurbine;
                            break;
                    }
                    break;

                case MissionSimplePutCupState.StopTurbine:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskTurbine.SetAllStatesTo(TurbineState.Off);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.taskTurbine.isFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionSimplePutCupState.UpperRack;
                            break;
                    }
                    break;
                case MissionSimplePutCupState.UpperRack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskRackPrehension.SetRackPositionToVertical();
                            timestamp = DateTime.Now;
                            break;
                        case SubTaskState.EnCours:
                            if (parent.taskRackPrehension.isFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;
                        case SubTaskState.Exit:
                            isFinished = true;
                            state = MissionSimplePutCupState.Waiting;
                            break;
                    }
                    break;

            }
        }
    }
}
