using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionPutSouthBand : TaskBase
    {
        DateTime timestamp;
        public enum MissionPutSouthBandState
        {
            Waiting,
            GotoSouthBand,
            LowerRack,
            StopTurbine,
            UpperRack,
            EscapeSouthBand
        }


        MissionPutSouthBandState state = MissionPutSouthBandState.Waiting;

        public MissionPutSouthBand(StrategyEurobot p) : base(p)
        {
            Init();
        
        }

        public void Start()
        {
            ResetSubState();
            state = MissionPutSouthBandState.GotoSouthBand;
        }

        public override void Init()
        {
            isFinished = false;
            state = MissionPutSouthBandState.Waiting;
        }

        //public void Abort()

        public override void TaskStateMachine()
        {
            switch (state)
            {

                case MissionPutSouthBandState.Waiting:
                    break;

                case MissionPutSouthBandState.GotoSouthBand:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(1.3, -0.17, false, - Math.PI / 2);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-1.3, -0.17, false, -Math.PI / 2);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished|| DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 10000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionPutSouthBandState.LowerRack;
                            break;
                    }
                    break;

                case MissionPutSouthBandState.LowerRack:
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
                            state = MissionPutSouthBandState.StopTurbine;
                            break;
                    }
                    break;

                case MissionPutSouthBandState.StopTurbine:
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
                            state = MissionPutSouthBandState.UpperRack;
                            break;
                    }
                    break;
                case MissionPutSouthBandState.UpperRack:
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
                            state = MissionPutSouthBandState.EscapeSouthBand;
                            break;
                    }
                    break;

                case MissionPutSouthBandState.EscapeSouthBand:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(1.3, -0.4, true, -Math.PI);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(- 1.3, -0.4, true, -Math.PI);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 10000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = MissionPutSouthBandState.Waiting;
                            break;
                    }
                    break;
            }
        }
    }
}
