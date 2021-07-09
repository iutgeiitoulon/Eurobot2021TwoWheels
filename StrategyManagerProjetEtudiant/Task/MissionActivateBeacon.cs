using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionActivateBeacon : TaskBase
    {
        DateTime timestamp;
        public enum MissionActivateBeaconState
        {
            Waiting,
            GoToBeacon,
            DeployArm,
            PushBeacon,
            FoldArm

        }

        MissionActivateBeaconState state = MissionActivateBeaconState.Waiting;

        public MissionActivateBeacon(StrategyEurobot p) : base(p)
        {
            Init();
        }

        public override void Init()
        {
            isFinished = false;
            state = MissionActivateBeaconState.Waiting;
        }

        public void Start()
        {
            ResetSubState();
            state = MissionActivateBeaconState.GoToBeacon;
        }

        public void Abort()
        {
            isFinished = true;
            state = MissionActivateBeaconState.Waiting;
        }

        public override void TaskStateMachine()
        {
            switch(state)
            {
                case MissionActivateBeaconState.Waiting:
                    break;

                case MissionActivateBeaconState.GoToBeacon:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(1.25, 0.80, false, 0);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-1.25, 0.80, false, Math.PI);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished)
                                ExitState();
                            else if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 10000)
                                Abort();
                            break;

                        case SubTaskState.Exit:
                            state = MissionActivateBeaconState.DeployArm;
                            break;
                    }
                    break;
                case MissionActivateBeaconState.DeployArm:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskRackPrehension.SetRackPositionToHorizontal();
                            parent.taskArm.SetArmPush();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionActivateBeaconState.PushBeacon;
                            break;
                    }
                    break;
                case MissionActivateBeaconState.PushBeacon:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            if (parent.localWorldMap.Team == TeamColor.Yellow)
                                parent.OnSetWantedLocation(0.9, 0.80);
                            else if (parent.localWorldMap.Team == TeamColor.Blue)
                                parent.OnSetWantedLocation(-0.75, 0.80, true);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 10000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionActivateBeaconState.FoldArm;
                            break;
                    }
                    break;

                case MissionActivateBeaconState.FoldArm:
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.taskRackPrehension.SetRackPositionToVertical();
                            parent.taskArm.SetArmDown();
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = MissionActivateBeaconState.Waiting;
                            break;
                    }
                    break;

            }
        }
    }
}
