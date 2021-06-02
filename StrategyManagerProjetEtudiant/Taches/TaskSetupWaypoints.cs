using Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using EventArgsLibrary;

namespace StrategyManagerProjetEtudiantNS
{
    class TaskSetupWaypoints
    {
        StrategyEurobot parent;
        Thread TaskThread;
        Stopwatch sw = new Stopwatch();

        List<Cup> list_of_finished_cups;

        public TaskSetupWaypoints(StrategyEurobot parent)
        {
            this.parent = parent;
            TaskThread = new Thread(TaskThreadProcess);
            TaskThread.IsBackground = true;
            TaskThread.Start();
            sw.Start();
            sw.Reset();
        }

        private void TaskThreadProcess()
        {
            while (true)
            {
                Thread.Sleep(100);
                //if (sw.ElapsedMilliseconds > 500)
                //{
                //    sw.Restart();

                    if (parent.localWorldMap.WaypointLocations.Count == 0 && parent.localWorldMap.LidarCup.Count > 0)
                    {
                        PointD point = parent.localWorldMap.LidarCup.OrderBy(x => Toolbox.Distance(x.center, new PointD(parent.localWorldMap.RobotLocation.X, parent.localWorldMap.RobotLocation.Y))).FirstOrDefault().center;
                        parent.OnSetNewWaypoint(new Location(point.X, point.Y, 0, 0, 0, 0));
                    }
                //}
                
            }
        }
    }
}
