using System;
using HerkulexManagerNs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerProjetEtudiantNS
{
    public class TaskArm : TaskBase
    {
        public enum TaskArmState
        {
            Waiting,
            Up,
            Down,
            Side
        }

        private TaskArmState state = TaskArmState.Waiting;

        private DateTime timestamp;

        public TaskArm(StrategyEurobot p) : base(p) { }

        public override void Init()
        {
            isFinished = false;
            state = TaskArmState.Waiting;
        }

        public void SetArmUp()
        {
            ResetSubState();
            isFinished = false;
            this.state = TaskArmState.Up;
        }

        public override void TaskStateMachine()
        {
            switch (state)
            {
                case TaskArmState.Waiting:
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
                case TaskArmState.Up:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetHerkulexPosition(ServoId.Drapeau, Positions.DrapeauManche);
                            timestamp = DateTime.Now;
                            break;
                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = TaskArmState.Waiting;
                            break;
                    }
                    break;
            }
        }
    }
}
