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
            GoTo

        }

        MissionWindFlagsState state = MissionWindFlagsState.Waiting;

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
                case MissionWindFlagsState.Waiting:
                    break;
            }
        }
    }
}
