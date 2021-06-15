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
    public class TaskActivateBeacon
    {
        Thread TaskThread;
        StrategyEurobot parentManager;
        Stopwatch sw = new Stopwatch();
        TaskActivateBeaconState state = TaskActivateBeaconState.Wait;
        public bool isFinished = false;

        enum TaskActivateBeaconState
        {
            Init,
            Wait,
            MoveToBeacon,
            MoveToBeaconWaiting,
            PushBeacon,
            PushBeaconWaiting,
            Finished,
        }

        public TaskActivateBeacon(StrategyEurobot manager)
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
            state = TaskActivateBeaconState.Init;
            isFinished = false;
            StopSw();
        }
        public void Start()
        {
            state = TaskActivateBeaconState.MoveToBeacon;
            isFinished = false;
            StopSw();
        }
        public void Pause()
        {
            state = TaskActivateBeaconState.Wait;
            isFinished = false;
            StopSw();
        }

        void TaskThreadProcess()
        {
            while (true)
            {
                switch (state)
                {
                    case TaskActivateBeaconState.Wait:
                        break;
                    case TaskActivateBeaconState.Init:
                        state = TaskActivateBeaconState.Wait;
                        break;
                    case TaskActivateBeaconState.MoveToBeacon:
                        parentManager.OnEnableDisableRotation(false);
                        if (parentManager.localWorldMap.Team == TeamColor.Blue)
                            parentManager.OnSetWantedLocation(new Location(-1.3, 0.8, 0, 0, 0, 0));
                        else if (parentManager.localWorldMap.Team == TeamColor.Yellow)
                            parentManager.OnSetWantedLocation(new Location(1.3, 0.8, 0, 0, 0, 0));

                        state = TaskActivateBeaconState.MoveToBeaconWaiting;
                        StartSw();
                        break;
                    case TaskActivateBeaconState.MoveToBeaconWaiting:
                        if (parentManager.isDeplacementFinished || sw.ElapsedMilliseconds > 5000)
                        {
                            state = TaskActivateBeaconState.PushBeacon;
                            StopSw();
                        }
                        break;
                    case TaskActivateBeaconState.PushBeacon:
                        parentManager.OnEnableDisableRotation(false);
                        if (parentManager.localWorldMap.Team == TeamColor.Blue)
                            parentManager.OnSetWantedLocation(new Location(-1, 0.8, 0, 0, 0, 0));
                        else if (parentManager.localWorldMap.Team == TeamColor.Yellow)
                            parentManager.OnSetWantedLocation(new Location(1, 0.8, 0, 0, 0, 0));

                        state = TaskActivateBeaconState.PushBeaconWaiting;
                        StartSw();
                        break;
                    case TaskActivateBeaconState.PushBeaconWaiting:
                        if (parentManager.isDeplacementFinished || sw.ElapsedMilliseconds > 5000)
                        {
                            state = TaskActivateBeaconState.Finished;
                            StopSw();
                        }
                        break;
                    case TaskActivateBeaconState.Finished:
                        isFinished = true;
                        state = TaskActivateBeaconState.Wait;
                        break;
                }
                Thread.Sleep(10);
            }
        }
    }

}
