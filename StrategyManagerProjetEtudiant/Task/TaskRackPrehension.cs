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
            RackHorizontal,
            RackPrehension,
            RackPrehensionUp,
            RackPutCups,
        }


        TaskRackPrehensionState state = TaskRackPrehensionState.Waiting;

        DateTime timestamp;


        
        public TaskRackPrehension(StrategyEurobot p) : base(p)
        {
            Init();  
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

        public void SetRackPositionToPrehensionRack()
        {
             SetState(TaskRackPrehensionState.RackPrehension);
        }

        public void SetRackPositionToPrehensionRackUp()
        {
            SetState(TaskRackPrehensionState.RackPrehensionUp);
        }

        public void SetRackPositionToPutCups()
        {
            SetState(TaskRackPrehensionState.RackPutCups);
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
                #region RackHorizontal
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
                #region RackPrehension
                case TaskRackPrehensionState.RackPrehension:
                    switch(subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetHerkulexPosition(ServoId.Rack1, Positions.RackPrehension);
                            parent.OnSetHerkulexPosition(ServoId.Rack2, Positions.RackPrehension);
                            parent.OnPololuSetUs(PololuActuators.ServoAscenseur, (ushort)GruePositions.PreshensionRack);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = TaskRackPrehensionState.Waiting;
                            break;
                    }
                    break;
                #endregion
                #region RackUpper
                case TaskRackPrehensionState.RackPrehensionUp:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetHerkulexPosition(ServoId.Rack1, Positions.RackVertical);
                            parent.OnSetHerkulexPosition(ServoId.Rack2, Positions.RackVertical);
                            parent.OnPololuSetUs(PololuActuators.ServoAscenseur, (ushort)GruePositions.EndMatch);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
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

                #region Rack Put Cups
                case TaskRackPrehensionState.RackPutCups:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetHerkulexPosition(ServoId.Rack1, Positions.RackHorizontal);
                            parent.OnSetHerkulexPosition(ServoId.Rack2, Positions.RackHorizontal);
                            parent.OnPololuSetUs(PololuActuators.ServoAscenseur, (ushort)GruePositions.Low);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
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
