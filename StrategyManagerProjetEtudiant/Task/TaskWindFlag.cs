using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using EventArgsLibrary;
using Constants;
using System.Threading;
using System.Diagnostics;

namespace StrategyManagerProjetEtudiantNS
{
    public class TaskWindFlag
    {
        Thread TaskThread;
        StrategyEurobot parentManager;
        Stopwatch sw = new Stopwatch();
        TaskWindFlagStates state = TaskWindFlagStates.Wait;
        public bool isFinished = false;

        enum TaskWindFlagStates
        { 
            Init,
            Wait,
            MoveToFlag,
            MoveToFlagWaiting,
            MoveToPushedFlag2,
            MoveToPushedFlag2Waiting,
            Finished,
        }

        public TaskWindFlag(StrategyEurobot manager)
        {
            isFinished = false;
            parentManager = manager;
            TaskThread = new Thread(TaskThreadProcess);
            TaskThread.IsBackground = true;
            TaskThread.Start();
        }

        private void StopSw()
        {
            sw.Stop();
            sw.Reset();
        }
        private void StartSw()
        {
            sw.Reset();
            sw.Start();
        }
        public void Init()
        {
            state = TaskWindFlagStates.Init;
            isFinished = false;
            StopSw();
        }
        public void Start()
        {
            state = TaskWindFlagStates.MoveToFlag;
            isFinished = false;
            StopSw();
        }
        public void Pause()
        {
            state = TaskWindFlagStates.Wait;
            isFinished = false;
            StopSw();
        }

        void TaskThreadProcess()
        {
            while (true)
            {
                switch (state)
                {
                    case TaskWindFlagStates.Wait:
                        break;
                    case TaskWindFlagStates.Init:
                        state = TaskWindFlagStates.Wait;
                        break;
                    case TaskWindFlagStates.MoveToFlag:
                        if (parentManager.localWorldMap.Team == TeamColor.Blue)
                            parentManager.OnSetWantedLocation(new Location(-1.25, -0.75, 0, 0, 0, 0));
                        else if (parentManager.localWorldMap.Team == TeamColor.Yellow)
                            parentManager.OnSetWantedLocation(new Location(1.25, -0.75, 0, 0, 0, 0));

                        state = TaskWindFlagStates.MoveToFlagWaiting;
                        StartSw();
                        break;
                    case TaskWindFlagStates.MoveToFlagWaiting:
                        if (parentManager.isDeplacementFinished || sw.ElapsedMilliseconds > 5000)
                        {
                            state = TaskWindFlagStates.MoveToPushedFlag2;
                            StopSw();
                        }
                        break;
                    case TaskWindFlagStates.MoveToPushedFlag2:
                        if (parentManager.localWorldMap.Team == TeamColor.Blue)
                            parentManager.OnSetWantedLocation(new Location(-0.8, -0.75, 0, 0, 0, 0));
                        else if (parentManager.localWorldMap.Team == TeamColor.Yellow)
                            parentManager.OnSetWantedLocation(new Location(0.8, -0.75, 0, 0, 0, 0));

                        state = TaskWindFlagStates.MoveToPushedFlag2Waiting;
                        StartSw();
                        break;
                    case TaskWindFlagStates.MoveToPushedFlag2Waiting:
                        if(parentManager.isDeplacementFinished || sw.ElapsedMilliseconds > 5000)
                        {
                            state = TaskWindFlagStates.Finished;
                            StopSw();
                        }
                        break;
                    case TaskWindFlagStates.Finished:
                        isFinished = true;
                        state = TaskWindFlagStates.Wait;
                        break;
                }
                Thread.Sleep(10);
            }
        }
    }
}
