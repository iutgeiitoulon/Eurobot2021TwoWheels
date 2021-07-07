using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionRaiseFlag : TaskBase
    {
        public enum TaskRaiseFlagState
        {
            Waiting,
            RaiseFlag
        }

        private TaskRaiseFlagState state = TaskRaiseFlagState.Waiting;

        private DateTime timestamp;

        public MissionRaiseFlag(StrategyEurobot p) : base(p) { Init();  }

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
                    break;

                case TaskRaiseFlagState.RaiseFlag:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskRackPrehension.SetRackPositionToPrehensionRackUp();
                            parent.taskArm.SetArmUp();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000 || parent.taskRackPrehension.isFinished)
                                ExitState();

                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = TaskRaiseFlagState.Waiting;
                            break;
                    }
                    break;
            }
        }
    }
}
