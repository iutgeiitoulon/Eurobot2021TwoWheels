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
    public enum TaskDestinationState
    {
        Arret,
        Avance,
        Waypoint,
        Destination,
        AvanceEnCours,
    }

    public enum TaskDestinationMode
    { 
        Waypoint,
        Destination,
        Manual,
        Stop,
    }

    public class TaskDestination
    {
        StrategyEurobot parent;
        Thread TaskThread;
        public TaskDestinationState state = TaskDestinationState.Arret;
        public TaskDestinationMode mode = TaskDestinationMode.Stop;

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

        public void SetTaskState(TaskDestinationState state)
        {
            this.state = state;
        }
           
        public void TaskReached()
        {
            switch (mode)
            {
                case TaskDestinationMode.Destination:
                    parent.OnDestinationReached(parent.localWorldMap.DestinationLocation);
                    break;
                case TaskDestinationMode.Waypoint:
                    parent.OnWaypointsReached(parent.localWorldMap.WaypointLocations[0]);

                    //Random rand = new Random();
                    //parent.OnSetNewWaypoint(new Location((rand.NextDouble() - 0.5) * 2.6, (rand.NextDouble() - 0.3) * 1, 0, 0, 0, 0)); /// Only for fun
                    break;
                default:
                    break;
            }
            mode = TaskDestinationMode.Stop;
        }
        
        void UpdateTaskMode()
        {
            if (parent.localWorldMap == null)
                return;
            switch (mode)
            {
                case TaskDestinationMode.Stop:
                    if (parent.localWorldMap.DestinationLocation != null)
                    {
                        mode = TaskDestinationMode.Destination;
                        UpdateAndLaunch(parent.localWorldMap.DestinationLocation);

                    }
                    else if (parent.localWorldMap.WaypointLocations.Count >= 1)
                    {
                        mode = TaskDestinationMode.Waypoint;
                        Location location = parent.localWorldMap.WaypointLocations[0];
                        UpdateAndLaunch(location);
                    }
                    else
                    {
                        state = TaskDestinationState.Arret;
                    }
                    break;
                case TaskDestinationMode.Waypoint:
                    if (parent.localWorldMap.DestinationLocation != null)
                    {
                        mode = TaskDestinationMode.Destination;
                    }
                    state = TaskDestinationState.Avance;
                    break;
                case TaskDestinationMode.Destination:
                    state = TaskDestinationState.Avance;
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
                    case TaskDestinationState.Arret:
                        sw.Restart();
                        UpdateTaskMode();
                        break;                    
                    case TaskDestinationState.AvanceEnCours:
                        if (sw.ElapsedMilliseconds > 500)
                            state = TaskDestinationState.Avance;                           
                        break;
                    case TaskDestinationState.Avance:
                        sw.Restart();
                        state = TaskDestinationState.AvanceEnCours;
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
