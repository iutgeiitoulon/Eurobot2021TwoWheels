using Constants;
using EventArgsLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using WorldMap;

namespace StrategyManagerProjetEtudiantNS
{
    public class TaskStrategy
    {
        Thread TaskThread;
        TaskStrategyState state = TaskStrategyState.Wait;
        StrategyEurobot parentManager;

        public bool isStoped = false;
        public bool Jack = true;

        Stopwatch timeStamp = new Stopwatch();
        private void StopSw()
        {
            timeStamp.Stop();
            timeStamp.Reset();
        }
        private void StartSw()
        {
            timeStamp.Reset();
            timeStamp.Start();
        }

        enum TaskStrategyState
        {
            InitialPositioning,
            InitialPositioningWaiting,
            Wait,
            PushFlags,
            PushFlagsWaiting,
            ReturnHome,
            ReturnHomeWaiting,
            Finished
        }

        public TaskStrategy(StrategyEurobot strategyManager)
        {
            parentManager = strategyManager;
            TaskThread = new Thread(TaskThreadProcess);
            TaskThread.IsBackground = true;
            TaskThread.Start();
        }

        public void Init()
        {
            parentManager.taskWindFlag.Init();
            parentManager.taskReturnHarbor.Init();
            state = TaskStrategyState.InitialPositioning;
        }

        void TaskThreadProcess()
        {
            while (true)
            {
                if (Jack)
                    Init();
                if (timeStamp.ElapsedMilliseconds < 99999)
                {
                   

                    switch (state)
                    {
                        case TaskStrategyState.Wait:
                            break;
                        case TaskStrategyState.InitialPositioning:  //Le positionnement initial est manuel de manière à pouvoir coller deux robots très proches sans mouvement parasite
                            parentManager.OnEnableDisableMotors(false);
                            parentManager.OnSetWantedLocation(parentManager.localWorldMap.RobotLocation);
                            state = TaskStrategyState.InitialPositioningWaiting;
                            break;
                        case TaskStrategyState.InitialPositioningWaiting:
                            if (!Jack)
                            {
                                parentManager.OnEnableDisableMotors(true);
                                parentManager.taskWindFlag.Start();
                                state = TaskStrategyState.PushFlags;
                                StartSw();

                            }
                            break;

                        case TaskStrategyState.PushFlags:
                            parentManager.taskWindFlag.Start();
                            state = TaskStrategyState.PushFlagsWaiting;
                            break;
                        case TaskStrategyState.PushFlagsWaiting:
                            if (parentManager.taskWindFlag.isFinished)
                                state = TaskStrategyState.ReturnHome;
                            break;
                        case TaskStrategyState.ReturnHome:
                            parentManager.taskReturnHarbor.Start();
                            state = TaskStrategyState.ReturnHomeWaiting;
                            break;
                        case TaskStrategyState.ReturnHomeWaiting:
                            if (parentManager.taskReturnHarbor.isFinished)
                                state = TaskStrategyState.Wait;
                            break;
                    }
                }
                else
                {
                    Init();
                    if (!isStoped)
                    {
                        //parentManager.taskFinDeMatch.Start();
                        timeStamp.Stop();
                    }
                    isStoped = true;
                }
                Thread.Sleep(1);
            }
        }
        
    }
}
