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
        public enum MissionRackPrivateState
        {
            Waiting,
            GotoRackSafety,
            AvanceVersRack,
            GetCups,
            UpperRack,
            GoingBack,

        }


        int timeout_PointA = 15000;
        int timeout_AvanceVersRack = 15000;
        MissionRackPrivateState state = MissionRackPrivateState.Waiting;

        public MissionGetRackPrivate(StrategyEurobot p) : base(p)
        {
            Init();
        
        }

        public void Start()
        {
            ResetSubState();
            state = MissionRackPrivateState.GotoRackSafety;
        }

        public override void Init()
        {
            isFinished = false;
            state = MissionRackPrivateState.Waiting;
        }

        public void Abort()
        {
            isFinished = true;
            state = MissionRackPrivateState.Waiting;
        }

        public override void TaskStateMachine()
        {
            switch (state)
            {

                case MissionRackPrivateState.Waiting:
                    break;

                case MissionRackPrivateState.GotoRackSafety:
                    switch(subState)
                    {
                        case SubTaskState.Entry:
                            timestamp = DateTime.Now;
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(1.265, -0.590, false, 0);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-1.265, -0.590, false, Math.PI);
                            parent.taskRackPrehension.SetRackPositionToVertical();
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished)
                                ExitState();
                            else if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= timeout_PointA)
                                Abort();
                            break;

                        case SubTaskState.Exit:
                            state = MissionRackPrivateState.AvanceVersRack;
                            break;
                    }
                    break;

                case MissionRackPrivateState.AvanceVersRack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(1.41, -0.590, false, 0);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-1.41, -0.590, false, Math.PI);
                            parent.taskRackPrehension.SetRackPositionToPrehensionRack();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= timeout_AvanceVersRack)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionRackPrivateState.GetCups;
                            break;
                    }
                    break;
                case MissionRackPrivateState.GetCups:
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
                            state = MissionRackPrivateState.UpperRack;
                            break;
                    }
                    break;
                case MissionRackPrivateState.UpperRack:
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
                            state = MissionRackPrivateState.GoingBack;
                            break;
                    }
                    break;

                case MissionRackPrivateState.GoingBack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(1.265, -0.590, true);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-1.265, -0.590, true);
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
                            state = MissionRackPrivateState.Waiting;
                            break;
                    }
                    break;

            }
        }
    }
}
