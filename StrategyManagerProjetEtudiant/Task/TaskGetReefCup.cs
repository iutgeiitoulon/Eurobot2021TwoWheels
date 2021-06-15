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
    class TaskGetReefCup
    {
        Thread TaskThread;
        StrategyEurobot parentManager;
        Stopwatch sw = new Stopwatch();
        TaskReefCupState state = TaskReefCupState.Wait;
        public bool isFinished = false;
        public ReefType Reef;

        /// Reef Number:
        /// 0: Private
        /// 1: Commun Team Side
        /// 2: Commun Opponent Side
        
        enum TaskReefCupState
        {
            Init,
            Wait,
            MoveToReefSafety,
            MoveToReefSafetyWaiting,
            MoveToReef,
            MoveToReefWaiting,
            StepBackToSafety,
            StepBackToSafetyWaiting,
            Finished,
        }

        public TaskGetReefCup(StrategyEurobot manager)
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
        public void Init(ReefType type)
        {
            Reef = type;
            state = TaskReefCupState.Init;
            isFinished = false;
            StopSw();
        }
        public void Start()
        {
            state = TaskReefCupState.MoveToReefSafety;
            isFinished = false;
            StopSw();
        }
        public void Pause()
        {
            state = TaskReefCupState.Wait;
            isFinished = false;
            StopSw();
        }

        void TaskThreadProcess()
        {
            while (true)
            {
                switch (state)
                {
                    case TaskReefCupState.Wait:
                        break;
                    case TaskReefCupState.Init:
                        state = TaskReefCupState.Wait;
                        break;
                    //case TaskReefCupState.MoveToReefSafety:
                    //    parentManager.OnEnableDisableRotation(false);
                    //    if (parentManager.localWorldMap.Team == TeamColor.Blue)
                    //    {
                    //        switch (Reef)
                    //        {
                    //            //case ReefType.Private:

                    //        }

                    //    }
                    //    else if (parentManager.localWorldMap.Team == TeamColor.Yellow)
                    //    {
                    //        parentManager.OnSetWantedLocation(new Location(1.3, 0.8, 0, 0, 0, 0));
                    //    }


                    //    state = TaskReefCupState.MoveToReefSafetyWaiting;
                    //    StartSw();
                    //    break;
                    //case TaskReefCupState.MoveToReefSafetyWaiting:
                    //    if (parentManager.isDeplacementFinished || sw.ElapsedMilliseconds > 5000)
                    //    {
                    //        state = TaskReefCupState.PushBeacon;
                    //        StopSw();
                    //    }
                    //    break;
                    //case TaskReefCupState.PushBeacon:
                    //    parentManager.OnEnableDisableRotation(false);
                    //    if (parentManager.localWorldMap.Team == TeamColor.Blue)
                    //        parentManager.OnSetWantedLocation(new Location(-1, 0.8, 0, 0, 0, 0));
                    //    else if (parentManager.localWorldMap.Team == TeamColor.Yellow)
                    //        parentManager.OnSetWantedLocation(new Location(1, 0.8, 0, 0, 0, 0));

                    //    state = TaskReefCupState.PushBeaconWaiting;
                    //    StartSw();
                    //    break;
                    //case TaskReefCupState.PushBeaconWaiting:
                    //    if (parentManager.isDeplacementFinished || sw.ElapsedMilliseconds > 5000)
                    //    {
                    //        state = TaskReefCupState.Finished;
                    //        StopSw();
                    //    }
                    //    break;
                    //case TaskReefCupState.Finished:
                    //    isFinished = true;
                    //    state = TaskReefCupState.Wait;
                    //    break;
                }
                Thread.Sleep(10);
            }
        }
    }
}

