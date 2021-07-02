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
            ActivateBeacon,
            ActivateBeaconWaiting,
            ReturnHome,
            ReturnHomeWaiting,
            Calibrate1,
            Calibrate1Waiting,
            Calibrate2,
            Calibrate2Waiting,
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
            timeStamp.Reset();
            parentManager.taskWindFlag.Init();
            parentManager.taskReturnHarbor.Init();
            parentManager.taskActivateBeacon.Init();
            parentManager.taskCalibrate.Init();

            if (state != TaskStrategyState.InitialPositioning && state != TaskStrategyState.InitialPositioningWaiting)
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
                                state = TaskStrategyState.PushFlags;
                                StartSw();
                            }
                            else
                            {
                                if (parentManager.localWorldMap == null)
                                    break;
                                if (parentManager.localWorldMap.Fields == null)
                                    break;

                                RectangleOriented spawn = parentManager.localWorldMap.Fields.Where(x => x.Type == FieldType.StartZone).FirstOrDefault().Shape;

                                if (spawn != null)
                                {
                                    PointD home_point = new PointD(spawn.Center.X, spawn.Center.Y);
                                    Location pos = new Location(home_point.X, home_point.Y, Toolbox.Angle(home_point, new PointD(0, 0)), 0, 0, 0);
                                    parentManager.OnSetActualLocation(pos);
                                }
                            }
                            break;
                        case TaskStrategyState.Calibrate1:
                            parentManager.taskCalibrate.Start();
                            state = TaskStrategyState.Calibrate1Waiting;
                            break;
                        case TaskStrategyState.Calibrate1Waiting:
                            if (parentManager.taskCalibrate.isFinished)
                                state = TaskStrategyState.PushFlags;
                            break;
                        case TaskStrategyState.Calibrate2:
                            parentManager.taskCalibrate.Start();
                            state = TaskStrategyState.Calibrate2Waiting;
                            break;
                        case TaskStrategyState.Calibrate2Waiting:
                            if (parentManager.taskCalibrate.isFinished)
                                state = TaskStrategyState.ActivateBeacon;
                            break;
                        case TaskStrategyState.PushFlags:
                            parentManager.taskWindFlag.Start();
                            state = TaskStrategyState.PushFlagsWaiting;
                            break;
                        case TaskStrategyState.PushFlagsWaiting:
                            if (parentManager.taskWindFlag.isFinished)
                                state = TaskStrategyState.Calibrate2;
                            break;
                        case TaskStrategyState.ActivateBeacon:
                            parentManager.taskActivateBeacon.Start();
                            state = TaskStrategyState.ActivateBeaconWaiting;
                            break;
                        case TaskStrategyState.ActivateBeaconWaiting:
                            if (parentManager.taskActivateBeacon.isFinished)
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
                    //Init();
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
