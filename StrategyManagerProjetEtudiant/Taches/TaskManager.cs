
using Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace StrategyManagerProjetEtudiantNS
{
    public enum TaskMoveState
    {
        Arret,
        Avance,
        Waypoint,
        AvanceEnCours,
    }

    public enum TaskMode
    {
        Action,
        Move,
        Stop,
        Halt
    }

    public class TaskDestination
    {
        StrategyEurobot parent;
        Thread TaskThread;
        public TaskMoveState state = TaskMoveState.Arret;
        public TaskMode mode = TaskMode.Stop;
        Location Destination = new Location();

        Stopwatch sw = new Stopwatch();

        public TaskDestination(StrategyEurobot parent)
        {
            this.parent = parent;
            TaskThread = new Thread(TaskThreadProcess);
            TaskThread.IsBackground = true;
            TaskThread.Start();
            sw.Stop();
            sw.Reset();
        }

        public void SetTaskState(TaskMoveState state)
        {
            this.state = state;
        }

        public void TaskReached()
        {
            switch (mode)
            {
                case TaskMode.Move:
                    parent.OnWaypointsReached(parent.localWorldMap.WaypointLocations[0]);
                    break;
                default:
                    break;
            }
            mode = TaskMode.Stop;
        }

        void UpdateTaskMode()
        {
            if (parent.localWorldMap == null)
                return;
            if (parent.JackState)
                mode = TaskMode.Halt;

            Destination = parent.localWorldMap.DestinationLocation;
            List<Location> WaypointListCopy = parent.localWorldMap.WaypointLocations.ToList();
            switch (mode)
            {
                case TaskMode.Halt:
                    if (!parent.JackState)
                        mode = TaskMode.Stop;
                    break;
                case TaskMode.Stop:
                    if (WaypointListCopy.Count == 0)
                    {
                        state = TaskMoveState.Arret;
                        Destination = parent.localWorldMap.RobotLocation;
                        parent.OnDestination(parent.robotId, Destination);
                        UpdateAndLaunch(Destination);
                    }
                    else
                    {
                        mode = TaskMode.Move;
                        Destination = WaypointListCopy[0];
                        parent.OnDestination(parent.robotId, Destination);
                        UpdateAndLaunch(Destination);
                    }                    
                    break;
                case TaskMode.Move:
                    if (WaypointListCopy.Count == 0)
                        mode = TaskMode.Stop;
                    else
                    {
                        state = TaskMoveState.Avance;
                        if (Toolbox.Distance(WaypointListCopy[0], Destination) >= 0.5)
                        {
                            Destination = WaypointListCopy[0];

                            parent.OnDestination(parent.robotId, Destination);
                            UpdateAndLaunch(Destination);
                        }
                    }
                    break;
                default:
                    break;

            }
        }

        void TaskThreadProcess()
        {
            while (true)
            {
                switch (state)
                {
                    case TaskMoveState.Arret:
                        sw.Restart();
                        UpdateTaskMode();
                        break;
                    case TaskMoveState.AvanceEnCours:
                        if (sw.ElapsedMilliseconds > 500)
                            state = TaskMoveState.Avance;
                        break;
                    case TaskMoveState.Avance:
                        sw.Restart();
                        state = TaskMoveState.AvanceEnCours;
                        UpdateTaskMode();
                        break;
                    default:
                        break;
                }
                Thread.Sleep(100);
            }
        }

        void UpdateAndLaunch(Location location)
        {
            parent.OnSetWantedLocation(location);
        }
    }
}
