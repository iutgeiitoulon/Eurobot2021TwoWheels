using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{
    public class TaskPrivJaune : TaskBase
    {
        DateTime timestamp;
        public enum TaskState
        {
            waiting,
            PointA,
            AvanceVersRack,


        }


        int timeout_PointA = 10000;
        int timeout_AvanceVersRack = 10000;
        TaskState state = TaskState.waiting;

        public TaskPrivJaune(StrategyEurobot p) : base(p)
        {
            Init();
        
        }

        public void Start()
        {
            ResetSubState();
            state = TaskState.PointA;
        }

        public override void Init()
        {

        }

        public override void TaskStateMachine()
        {
            switch (state)
            {

                case TaskState.waiting:
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

                case TaskState.PointA:
                    switch(subState)
                    {
                        case SubTaskState.Entry:
                            timestamp = DateTime.Now;
                            parent.OnSetWantedLocation(1.265, -0.600, 0, true);
                            parent.taskRackPrehension.SetRackPositionToVertical();
                            
                            break;

                        case SubTaskState.EnCours:
                            if(parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= timeout_PointA)
                            {
                                ExitState();
                            }
                            break;

                        case SubTaskState.Exit:
                            state = TaskState.AvanceVersRack;
                            break;
                    }
                    break;

                case TaskState.AvanceVersRack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetWantedLocation(1.440, -0.600, 0);
                            parent.OnPololuSetUs(PololuActuators.ServoAscenseur, (ushort)GruePositions.PreshensionRack);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= timeout_AvanceVersRack)
                            {
                                ExitState();
                            }
                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = TaskState.waiting;
                            break;
                    }
                    break;
            }
        }
    }
}
