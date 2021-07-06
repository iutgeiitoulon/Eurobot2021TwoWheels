using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionGetRackPrivate : TaskBase
    {
        DateTime timestamp;
        public enum MissionRackPrivate
        {
            Waiting,
            GotoRackSafety,
            AvanceVersRack,
            GetCups,
            UpperRack,
            GoingBack,

        }


        int timeout_PointA = 10000;
        int timeout_AvanceVersRack = 3000;
        MissionRackPrivate state = MissionRackPrivate.Waiting;

        public MissionGetRackPrivate(StrategyEurobot p) : base(p)
        {
            Init();
        
        }

        public void Start()
        {
            ResetSubState();
            state = MissionRackPrivate.GotoRackSafety;
        }

        public override void Init()
        {
            isFinished = false;
            state = MissionRackPrivate.Waiting;
        }

        public override void TaskStateMachine()
        {
            switch (state)
            {

                case MissionRackPrivate.Waiting:
                    break;

                case MissionRackPrivate.GotoRackSafety:
                    switch(subState)
                    {
                        case SubTaskState.Entry:
                            timestamp = DateTime.Now;
                            parent.OnSetWantedLocation(1.265, -0.600, false, 0);
                            parent.taskRackPrehension.SetRackPositionToVertical();
                            
                            break;

                        case SubTaskState.EnCours:
                            if(parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= timeout_PointA)
                            {
                                ExitState();
                            }
                            break;

                        case SubTaskState.Exit:
                            state = MissionRackPrivate.AvanceVersRack;
                            break;
                    }
                    break;

                case MissionRackPrivate.AvanceVersRack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetWantedLocation(1.38, -0.600, false, 0);
                            parent.taskRackPrehension.SetRackPositionToPrehensionRack();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= timeout_AvanceVersRack)
                            {
                                ExitState();
                            }
                            break;

                        case SubTaskState.Exit:
                            state = MissionRackPrivate.GetCups;
                            break;
                    }
                    break;
                case MissionRackPrivate.GetCups:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskTurbine.SetAllStatesTo(TurbineState.PrehensionHigh);
                            timestamp = DateTime.Now;
                            break;
                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;
                        case SubTaskState.Exit:
                            state = MissionRackPrivate.UpperRack;
                            break;
                    }
                    break;
                case MissionRackPrivate.UpperRack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskRackPrehension.SetRackPositionToPrehensionRackUp();
                            timestamp = DateTime.Now;
                            break;
                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;
                        case SubTaskState.Exit:
                            state = MissionRackPrivate.GoingBack;
                            break;
                    }
                    break;

                case MissionRackPrivate.GoingBack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetWantedLocation(1.265, -0.600, true);
                            
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
                            state = MissionRackPrivate.Waiting;
                            break;
                    }
                    break;

            }
        }
    }
}
