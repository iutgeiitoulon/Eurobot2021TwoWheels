using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;
using HerkulexManagerNs;

namespace StrategyManagerProjetEtudiantNS
{
    public class TaskRackPrehension : TaskBase
    {

        public enum TaskRackPrehensionState
        {
            Waiting,
            RackVertical,
            RackHorizontal
        }


        TaskRackPrehensionState state = TaskRackPrehensionState.Waiting;

        DateTime timestamp;


        
        public TaskRackPrehension(StrategyEurobot p) : base(p)
        {
           
        }

        public void SetState(TaskRackPrehensionState state)
        {
            ResetSubState();
            isFinished = false;
            this.state = state;
        }

        public void SetRackPositionToVertical()
        {
            SetState(TaskRackPrehensionState.RackVertical);
        }

        public void SetRackPositionToHorizontal()
        {
            SetState(TaskRackPrehensionState.RackHorizontal);
        }
        

        public override void Init()
        {
            ResetSubState();
            isFinished = false;
            state = TaskRackPrehensionState.Waiting;
        }

        public override void TaskStateMachine()
        {

            switch(state)
            {
                case TaskRackPrehensionState.Waiting:
                    switch (subState)
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

                #region RackVertical
                case TaskRackPrehensionState.RackVertical:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            //position initiale du rack
                            parent.OnSetHerkulexPosition(ServoId.Rack1, Positions.RackVertical);
                            parent.OnSetHerkulexPosition(ServoId.Rack2, Positions.RackVertical);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 2000)
                            {
                                ExitState();
                            }
                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = TaskRackPrehensionState.Waiting;
                            break;
                    }
                    break;
                #endregion
                #region TaskHorizontal
                case TaskRackPrehensionState.RackHorizontal:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            //position initiale du rack
                            parent.OnSetHerkulexPosition(ServoId.Rack1, Positions.RackHorizontal);
                            parent.OnSetHerkulexPosition(ServoId.Rack2, Positions.RackHorizontal);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 2000)
                            {
                                ExitState();
                            }
                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = TaskRackPrehensionState.Waiting;
                            break;
                    }
                    break;
                    #endregion
            }

        }
    }
}
