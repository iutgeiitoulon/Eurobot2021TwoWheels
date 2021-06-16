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
    public class TaskReturnHarbor
    {
        Thread TaskThread;
        StrategyEurobot parentManager;
        Stopwatch sw = new Stopwatch();
        TaskReturnHarborState state = TaskReturnHarborState.Wait;
        public bool isFinished = false;

        enum TaskReturnHarborState
        {
            Init,
            Wait,
            MoveToHarbor,
            MoveToHarborWaiting,
            Finished,
        }

        public TaskReturnHarbor(StrategyEurobot manager)
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
            state = TaskReturnHarborState.Init;
            isFinished = false;
            StopSw();
        }
        public void Start()
        {
            state = TaskReturnHarborState.MoveToHarbor;
            isFinished = false;
            StopSw();
        }
        public void Pause()
        {
            state = TaskReturnHarborState.Wait;
            isFinished = false;
            StopSw();
        }

        void TaskThreadProcess()
        {
            while (true)
            {
                switch (state)
                {
                    case TaskReturnHarborState.Wait:
                        break;
                    case TaskReturnHarborState.Init:
                        state = TaskReturnHarborState.Wait;
                        break;
                    case TaskReturnHarborState.MoveToHarbor:
                        PointD home_point = parentManager.localWorldMap.Fields.Where(x => x.Type == FieldType.StartZone).FirstOrDefault().Shape.Center;
                        Location destination = new Location(home_point.X, home_point.Y, Toolbox.Angle(home_point, new PointD(0, 0)), 0, 0, 0);
                        parentManager.OnSetWantedLocation(destination, true);
                        state = TaskReturnHarborState.MoveToHarborWaiting;
                        StartSw();
                        break;
                    case TaskReturnHarborState.MoveToHarborWaiting:
                        if (parentManager.isDeplacementFinished || sw.ElapsedMilliseconds > 5000)
                        {
                            state = TaskReturnHarborState.Finished;
                            StopSw();
                        }
                        break;
                    case TaskReturnHarborState.Finished:
                        isFinished = true;
                        state = TaskReturnHarborState.Wait;
                        break;
                }
                Thread.Sleep(10);
            }
        }
    }
}
