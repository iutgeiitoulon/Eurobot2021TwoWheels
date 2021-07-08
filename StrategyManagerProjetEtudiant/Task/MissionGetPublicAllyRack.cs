using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionGetRackPublicAlly : TaskBase
    {
        DateTime timestamp;
        public enum MissionGetRackPublicAllyState
        {
            Waiting,
            GotoRackSafety,
            AvanceVersRack,
            GetCups,
            UpperRack,
            GoingBack,

        }


        int timeout_PointA = 10000;
        int timeout_AvanceVersRack = 10000;
        MissionGetRackPublicAllyState state = MissionGetRackPublicAllyState.Waiting;

        public MissionGetRackPublicAlly(StrategyEurobot p) : base(p)
        {
            Init();

        }

        public void Start()
        {
            ResetSubState();
            state = MissionGetRackPublicAllyState.GotoRackSafety;
        }

        public override void Init()
        {
            isFinished = false;
            state = MissionGetRackPublicAllyState.Waiting;
        }

        public void Abort()
        {
            isFinished = true;
            state = MissionGetRackPublicAllyState.Waiting;
        }

        public override void TaskStateMachine()
        {
            switch (state)
            {

                case MissionGetRackPublicAllyState.Waiting:
                    break;

                case MissionGetRackPublicAllyState.GotoRackSafety:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            timestamp = DateTime.Now;
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(0.625, 0.75, false, Math.PI / 2);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-0.625, 0.75, false, Math.PI / 2);
                            parent.taskRackPrehension.SetRackPositionToVertical();
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished)
                                ExitState();
                            else if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= timeout_PointA)
                                Abort();
                            break;

                        case SubTaskState.Exit:
                            state = MissionGetRackPublicAllyState.AvanceVersRack;
                            break;
                    }
                    break;

                case MissionGetRackPublicAllyState.AvanceVersRack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(0.625, 0.90, false, Math.PI / 2);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-0.625, 0.90, false, Math.PI / 2) ;
                            parent.taskRackPrehension.SetRackPositionToPrehensionRack();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= timeout_AvanceVersRack)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionGetRackPublicAllyState.GetCups;
                            break;
                    }
                    break;
                case MissionGetRackPublicAllyState.GetCups:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskTurbine.SetAllStatesTo(TurbineState.PrehensionHigh);
                            timestamp = DateTime.Now;
                            break;
                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 2000)
                                ExitState();
                            break;
                        case SubTaskState.Exit:
                            state = MissionGetRackPublicAllyState.UpperRack;
                            break;
                    }
                    break;
                case MissionGetRackPublicAllyState.UpperRack:
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
                            state = MissionGetRackPublicAllyState.GoingBack;
                            break;
                    }
                    break;

                case MissionGetRackPublicAllyState.GoingBack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(0.625, 0.75, true);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-0.625, 0.75, true);
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
                            state = MissionGetRackPublicAllyState.Waiting;
                            break;
                    }
                    break;

            }
        }
    }
}
