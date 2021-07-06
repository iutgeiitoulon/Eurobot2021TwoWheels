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
        public enum TaskState
        {
            waiting,
            GoTo

        }

        TaskState state = TaskState.waiting;

        public MissionWindFlags(StrategyEurobot p) : base(p)
        {

        }

        public override void Init()
        {
            
        }

        public override void TaskStateMachine()
        {
            switch(state)
            {
                case TaskState.waiting:
                    switch(subState)
                    {
                        case SubTaskState.Entry:

                            break;

                        case SubTaskState.EnCours:

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
