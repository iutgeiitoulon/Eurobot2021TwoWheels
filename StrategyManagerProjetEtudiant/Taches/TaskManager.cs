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
        Destination,
        AvanceEnCours,
    }

    public enum TaskMode
    { 
        Action,
        Move,
        Stop,
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

                    //Random rand = new Random();
                    //parent.OnSetNewWaypoint(new Location((rand.NextDouble() - 0.5) * 2.6, (rand.NextDouble() - 0.3) * 1, 0, 0, 0, 0)); /// Only for fun
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
            switch (mode)
            {
                case TaskMode.Stop:
                    if (parent.localWorldMap.WaypointLocations.Count >= 1)
                    {
                        mode = TaskMode.Move;
                        Location location = parent.localWorldMap.WaypointLocations[0];
                        UpdateAndLaunch(location);
                    }
                    else
                    {
                        state = TaskMoveState.Arret;
                    }
                    break;
                case TaskMode.Move:
                    if (parent.localWorldMap.WaypointLocations.Count == 0)
                    {
                        mode = TaskMode.Stop;
                        state = TaskMoveState.Arret;
                        UpdateAndLaunch(parent.robotCurrentLocation);
                    }
                    else 
                    {
                        state = TaskMoveState.Avance;
                        if (Toolbox.Distance(parent.localWorldMap.WaypointLocations[0], Destination) >= 0.05)
                        {
                            Location location = parent.localWorldMap.WaypointLocations[0];
                            UpdateAndLaunch(location);
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
