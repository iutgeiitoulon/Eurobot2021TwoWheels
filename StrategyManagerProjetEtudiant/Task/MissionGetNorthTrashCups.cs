using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionGetNorthTrashCups : TaskBase
    {
        private MissionGetNorthTrashCupsState state = MissionGetNorthTrashCupsState.Waiting;
        private DateTime timestamp;

        public enum MissionGetNorthTrashCupsState
        {
            Waiting,
            SetupRack,
            GetFirstTwoCups,
            GetTheBackTrashCups,
            GetTheCupNearTheRack
    
        }

        public MissionGetNorthTrashCups(StrategyEurobot p) : base(p)
        {
            Init();
        }
        public override void Init()
        {
            isFinished = false;
            state = MissionGetNorthTrashCupsState.Waiting;

        }

        public void Start()
        {
            ResetSubState();
            state = MissionGetNorthTrashCupsState.SetupRack;
        }

        public override void TaskStateMachine()
        {
            switch(state)
            {
                case MissionGetNorthTrashCupsState.Waiting:

                    break;

                case MissionGetNorthTrashCupsState.SetupRack:
                    switch(subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskTurbine.SetAllStatesTo(Constants.TurbineState.PrehensionHigh);
                            parent.taskRackPrehension.SetRackPositionToGetCups();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000 || parent.taskRackPrehension.isFinished)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionGetNorthTrashCupsState.GetFirstTwoCups;
                            break;
                    }
                    break;

                case MissionGetNorthTrashCupsState.GetFirstTwoCups:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == Constants.TeamColor.Yellow)
                                parent.OnSetWantedLocation(1.2, 0.45);
                            else if (parent.localWorldMap.Team == Constants.TeamColor.Blue)
                                parent.OnSetWantedLocation(-1.2, 0.45);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 15000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionGetNorthTrashCupsState.GetTheBackTrashCups;
                            break;
                    }
                    break;

                case MissionGetNorthTrashCupsState.GetTheBackTrashCups:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == Constants.TeamColor.Yellow)
                                parent.OnSetWantedLocation(0.725, 0.6, false, 3 * Math.PI / 4);
                            else if (parent.localWorldMap.Team == Constants.TeamColor.Blue)
                                parent.OnSetWantedLocation(-0.725, 0.6, false, 3 * Math.PI / 4);

                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 15000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionGetNorthTrashCupsState.GetTheCupNearTheRack;
                            break;
                    }
                    break;


                case MissionGetNorthTrashCupsState.GetTheCupNearTheRack:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                             if (parent.localWorldMap.Team == Constants.TeamColor.Yellow)
                                parent.OnSetWantedLocation(0.8, 0.8, false, Math.PI / 2);
                            else if (parent.localWorldMap.Team == Constants.TeamColor.Blue)
                                parent.OnSetWantedLocation(-0.8, 0.8, false, Math.PI / 2);

                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 15000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = MissionGetNorthTrashCupsState.Waiting;
                            break;
                    }
                    break;
            }
        }
    }
}
