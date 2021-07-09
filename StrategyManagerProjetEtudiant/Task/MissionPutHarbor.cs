using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionPutHarbor : TaskBase
    {
        DateTime timestamp;
        public enum MissionPutHarborState
        {
            Waiting,
            GotoHarbor,
            LowerRack,
            StopTurbine,
            UpperRack,
            EscapeHarbor
        }


        MissionPutHarborState state = MissionPutHarborState.Waiting;

        public MissionPutHarbor(StrategyEurobot p) : base(p)
        {
            Init();
        
        }

        public void Start()
        {
            ResetSubState();
            state = MissionPutHarborState.GotoHarbor;
        }

        public override void Init()
        {
            isFinished = false;
            state = MissionPutHarborState.Waiting;
        }

        //public void Abort()

        public override void TaskStateMachine()
        {
            switch (state)
            {

                case MissionPutHarborState.Waiting:
                    break;

                case MissionPutHarborState.GotoHarbor:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(1.25, 0.2, false, 0);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-1.25, 0.2, false, Math.PI);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished|| DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 10000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionPutHarborState.LowerRack;
                            break;
                    }
                    break;

                case MissionPutHarborState.LowerRack:
                    switch(subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskRackPrehension.SetRackPositionToPutCups();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.taskRackPrehension.isFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionPutHarborState.StopTurbine;
                            break;
                    }
                    break;

                case MissionPutHarborState.StopTurbine:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskTurbine.SetAllStatesTo(TurbineState.Off);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.taskTurbine.isFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionPutHarborState.UpperRack;
                            break;
                    }
                    break;
                case MissionPutHarborState.UpperRack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskRackPrehension.SetRackPositionToVertical();
                            timestamp = DateTime.Now;
                            break;
                        case SubTaskState.EnCours:
                            if (parent.taskRackPrehension.isFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;
                        case SubTaskState.Exit:
                            state = MissionPutHarborState.EscapeHarbor;
                            break;
                    }
                    break;

                case MissionPutHarborState.EscapeHarbor:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(1.15, 0.2, true, 0);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-1.15, 0.2, true, Math.PI);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 10000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = MissionPutHarborState.Waiting;
                            break;
                    }
                    break;
            }
        }
    }
}
