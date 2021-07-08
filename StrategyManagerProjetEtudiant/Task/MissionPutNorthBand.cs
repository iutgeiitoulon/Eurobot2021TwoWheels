using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionPutNorthBand : TaskBase
    {
        DateTime timestamp;
        public enum MissionPutNorthBandState
        {
            Waiting,
            GotoNorthBand,
            LowerRack,
            StopTurbine,
            UpperRack,
            EscapeNorthBand,
            TempMovement,
        }


        MissionPutNorthBandState state = MissionPutNorthBandState.Waiting;

        public MissionPutNorthBand(StrategyEurobot p) : base(p)
        {
            Init();
        
        }

        public void Start()
        {
            ResetSubState();
            state = MissionPutNorthBandState.GotoNorthBand;
        }

        public override void Init()
        {
            isFinished = false;
            state = MissionPutNorthBandState.Waiting;
        }

        //public void Abort()

        public override void TaskStateMachine()
        {
            switch (state)
            {

                case MissionPutNorthBandState.Waiting:
                    break;

                case MissionPutNorthBandState.GotoNorthBand:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(1.25, 0.63, false, -Math.PI / 2);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-1.25, 0.63, false, -Math.PI / 2);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished|| DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 10000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionPutNorthBandState.LowerRack;
                            break;
                    }
                    break;

                case MissionPutNorthBandState.LowerRack:
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
                            state = MissionPutNorthBandState.StopTurbine;
                            break;
                    }
                    break;

                case MissionPutNorthBandState.StopTurbine:
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
                            state = MissionPutNorthBandState.UpperRack;
                            break;
                    }
                    break;
                case MissionPutNorthBandState.UpperRack:
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
                            state = MissionPutNorthBandState.EscapeNorthBand;
                            break;
                    }
                    break;

                case MissionPutNorthBandState.EscapeNorthBand:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnDisableAvoidance();
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(1.25, 0.8, true);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-1.25, 0.8, true);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 10000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            parent.OnEnableAvoidance();
                            state = MissionPutNorthBandState.TempMovement;
                            break;
                    }
                    break;
                case MissionPutNorthBandState.TempMovement:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnDisableAvoidance();
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(0.65, 0.7);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-0.65, 0.7);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 10000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            parent.OnEnableAvoidance();
                            isFinished = true;
                            state = MissionPutNorthBandState.Waiting;
                            break;
                    }
                    break;
                 
            }
        }
    }
}
