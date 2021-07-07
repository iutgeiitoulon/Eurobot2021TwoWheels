using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;
using EventArgsLibrary;
using Utilities;
using WorldMap;

namespace TrajectoryAvoidanceNs
{
    public class TrajectoryAvoidance
    {
        public int robotID;
        LocalWorldMap localWorldMap;        

        public TrajectoryAvoidance(int robotId)
        {
            robotID = robotId;
        }

        #region Methods

        public void DetectObstacleCollision()
        {
            if (localWorldMap == null)
                return;
            lock (localWorldMap)
            {
                if (localWorldMap.RobotLocation == null || localWorldMap.DestinationLocation == null || localWorldMap.Fields == null || localWorldMap.LidarObjectList == null)
                    return;

                Segment robot_to_destination = new Segment(new PointD(localWorldMap.RobotLocation.X, localWorldMap.RobotLocation.Y), new PointD(localWorldMap.DestinationLocation.X, localWorldMap.DestinationLocation.Y));

                List<RectangleOriented> list_of_obstacle = localWorldMap.Fields.Where(x => x.Type == FieldType.DeadZone).Select(x => x.Shape).ToList();
                list_of_obstacle.AddRange(localWorldMap.LidarObjectList.Where(x => x.Type == LidarObjectType.Robot && x.LIFE >= ConstVar.LIDAR_OBJECT_VALID_LIFE).Select(x => x.Shape).ToList());

                foreach (RectangleOriented obstacle in list_of_obstacle)
                {
                    if (Toolbox.testIfSegmentIntersectRectangle(robot_to_destination, obstacle) && Toolbox.Distance(localWorldMap.RobotLocation, obstacle.Center) <= 0.70)
                    {
                        OnCollisionDetected(true);
                        return;
                    }
                    else if (Toolbox.Distance(localWorldMap.RobotLocation, obstacle.Center) <= 0.5)
                    {
                        OnCollisionDetected(true);
                        return;
                    }    
                }

                OnCollisionDetected(false);
            }
        }

        public void UpdateLocalMap(LocalWorldMap localWorld)
        {
            localWorldMap = localWorld;
            DetectObstacleCollision();
        }

        #endregion

        #region InputsCallback

        public void OnLocalWorldMapUpdate(object sender, LocalWorldMap localWorld)
        {
            UpdateLocalMap(localWorld);
        }

        #endregion

        #region Events
        public event EventHandler<bool> OnCollisionDetectedEvent;

        public virtual void OnCollisionDetected(bool collision)
        {
            Console.WriteLine(collision);
            OnCollisionDetectedEvent?.Invoke(this, collision);
        }

        #endregion
    }
}
