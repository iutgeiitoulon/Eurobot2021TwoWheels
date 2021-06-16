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
    public class TaskCalibrate
    {
        Thread TaskThread;
        StrategyEurobot parentManager;
        Stopwatch sw = new Stopwatch();
        TaskCalibrateState state = TaskCalibrateState.Wait;
        public bool isFinished = false;

        enum TaskCalibrateState
        {
            Init,
            Wait,
            TurnTo1Third,
            TurnTo1ThirdWaiting,
            TurnTo2Third,
            TurnTo2ThirdWaiting,
            Finished,
        }

        public TaskCalibrate(StrategyEurobot manager)
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
            state = TaskCalibrateState.Init;
            isFinished = false;
            StopSw();
        }
        public void Start()
        {
            state = TaskCalibrateState.TurnTo1Third;
            isFinished = false;
            StopSw();
        }
        public void Pause()
        {
            state = TaskCalibrateState.Wait;
            isFinished = false;
            StopSw();
        }

        void TaskThreadProcess()
        {
            while (true)
            {
                switch (state)
                {
                    case TaskCalibrateState.Wait:
                        break;
                    case TaskCalibrateState.Init:
                        state = TaskCalibrateState.Wait;
                        break;
                    case TaskCalibrateState.TurnTo1Third:
                        parentManager.OnCalibatrionAsked();
                        Location location1 = new Location(parentManager.localWorldMap.RobotLocation.X,
                            parentManager.localWorldMap.RobotLocation.Y,
                            Toolbox.Modulo2PiAngleRad(parentManager.localWorldMap.RobotLocation.Theta + 120 * Math.PI / 180), 0, 0, 0);
                        parentManager.OnSetWantedLocation(location1, true);
                        state = TaskCalibrateState.TurnTo1ThirdWaiting;
                        StartSw();
                        break;
                    case TaskCalibrateState.TurnTo1ThirdWaiting:
                        if (parentManager.isDeplacementFinished || sw.ElapsedMilliseconds > 5000)
                        {
                            state = TaskCalibrateState.TurnTo2Third;
                            StopSw();
                        }
                        break;
                    case TaskCalibrateState.TurnTo2Third:
                        Location location2 = new Location(parentManager.localWorldMap.RobotLocation.X,
                            parentManager.localWorldMap.RobotLocation.Y,
                            Toolbox.Modulo2PiAngleRad(parentManager.localWorldMap.RobotLocation.Theta + 120 * Math.PI / 180), 0, 0, 0);
                        parentManager.OnSetWantedLocation(location2, true);

                        state = TaskCalibrateState.TurnTo2ThirdWaiting;
                        StartSw();
                        break;
                    case TaskCalibrateState.TurnTo2ThirdWaiting:
                        if (parentManager.isDeplacementFinished || sw.ElapsedMilliseconds > 5000)
                        {
                            state = TaskCalibrateState.Finished;
                            StopSw();
                        }
                        break;
                    case TaskCalibrateState.Finished:
                        isFinished = true;
                        state = TaskCalibrateState.Wait;
                        break;
                }
                Thread.Sleep(10);
            }
        }
    }
}

