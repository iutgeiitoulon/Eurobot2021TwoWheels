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
    class TaskUpdateCupWaypoints
    {
        StrategyEurobot parent;
        Thread TaskThread;
        Stopwatch sw = new Stopwatch();

        public TaskUpdateCupWaypoints(StrategyEurobot parent)
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
                lock (parent.localWorldMap)
                {
                    /// NOT FOR FINAL PROD /!\
                    if (parent.localWorldMap.LidarObjectList.Count == 0)
                    {
                        parent.OnSetWaypointsList(new List<Location>());
                    }
                    else if (parent.localWorldMap.WaypointLocations.Count >= 0)
                    {
                        List<LidarObject> valid_Objects = parent.localWorldMap.LidarObjectList.Where(x => x.LIFE >= ConstVar.LIDAR_OBJECT_VALID_LIFE && x.Type == LidarObjectType.Cup).ToList();

                        foreach (Field field in parent.localWorldMap.Fields)
                        {                            
                            if (field.Type == FieldType.DeadZone || field.Type == FieldType.Harbor)
                            {
                                valid_Objects = valid_Objects.Where(x => !Toolbox.TestIfPointInsideAnOrientedRectangle(field.Shape, x.Shape.Center)).ToList();
                            }
                        }
                        //List<LidarObject> valid_Cups = valid_Objects.Where(x => Toolbox.TestIfPointInsideAnOrientedRectangle())

                        List<PointD> false_ordered_list_of_cups = valid_Objects.OrderBy(x => Toolbox.Distance(x.Shape.Center, new PointD(parent.localWorldMap.RobotLocation.X, parent.localWorldMap.RobotLocation.Y))).Select(x => x.Shape.Center).ToList();
                        List<PointD> real_ordred_list_of_cups = new List<PointD>();

                        while (false_ordered_list_of_cups.Count != 0)
                        {
                            PointD point = false_ordered_list_of_cups[0];
                            real_ordred_list_of_cups.Add(point);
                            false_ordered_list_of_cups.RemoveAt(0);
                            false_ordered_list_of_cups = false_ordered_list_of_cups.OrderBy(x => Toolbox.Distance(x, point)).ToList();
                        }

                        parent.OnSetWaypointsList(real_ordred_list_of_cups.Select(x => new Location(x.X, x.Y, 0, 0, 0, 0)).ToList());
                    }
                }
            }
        }
    }
}
