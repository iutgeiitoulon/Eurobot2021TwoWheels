using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionWindFlags : TaskBase
    {
        DateTime timestamp;
        public enum MissionWindFlagsState
        {
            Waiting,
            GoToWindFlag,
            DeployArm,
            PushWindFlag,
            FoldArm

        }

        MissionWindFlagsState state = MissionWindFlagsState.Waiting;

        public MissionWindFlags(StrategyEurobot p) : base(p)
        {
            Init();
        }

        public override void Init()
        {
            isFinished = false;
            state = MissionWindFlagsState.Waiting;
        }

        public void Start()
        {
            ResetSubState();
            state = MissionWindFlagsState.GoToWindFlag;
        }

        public override void TaskStateMachine()
        {
            switch(state)
            {
                case MissionWindFlagsState.Waiting:
                    break;

                case MissionWindFlagsState.GoToWindFlag:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetWantedLocation(1.25, -0.80, false, 0);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 10000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionWindFlagsState.DeployArm;
                            break;
                    }
                    break;
                case MissionWindFlagsState.DeployArm:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskRackPrehension.SetRackPositionToHorizontal();
                            parent.taskArm.SetArmPush();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionWindFlagsState.PushWindFlag;
                            break;
                    }
                    break;
                case MissionWindFlagsState.PushWindFlag:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetWantedLocation(0.9, -0.80, true);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 5000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionWindFlagsState.FoldArm;
                            break;
                    }
                    break;

                case MissionWindFlagsState.FoldArm:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskRackPrehension.SetRackPositionToVertical();
                            parent.taskArm.SetArmDown();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = MissionWindFlagsState.Waiting;
                            break;
                    }
                    break;

            }
        }
    }
}
