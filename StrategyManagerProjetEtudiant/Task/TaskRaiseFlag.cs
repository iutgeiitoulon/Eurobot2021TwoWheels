using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerProjetEtudiantNS
{
    public class TaskRaiseFlag : TaskBase
    {
        public enum TaskRaiseFlagState
        {
            Waiting,
            RaiseFlag
        }

        private TaskRaiseFlagState state = TaskRaiseFlagState.Waiting;

        private DateTime timestamp;

        public TaskRaiseFlag(StrategyEurobot p) : base(p) { Init();  }

        public override void Init()
        {
            ResetSubState();
            state = TaskRaiseFlagState.Waiting;
            isFinished = false;
        }

        public void Start()
        {
            ResetSubState();
            state = TaskRaiseFlagState.RaiseFlag;
        }

        public override void TaskStateMachine()
        {
            switch (state)
            {
                case TaskRaiseFlagState.Waiting:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            break;
                        case SubTaskState.EnCours:
                            break;
                        case SubTaskState.Exit:
                            break;
                    }
                    break;

                case TaskRaiseFlagState.RaiseFlag:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskRackPrehension.SetRackPositionToVertical();
                            parent.taskArm.SetArmUp();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 3000 || parent.taskRackPrehension.isFinished)
                                ExitState();

                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            break;
                    }
                    break;
            }
        }
    }
}
