using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventArgsLibrary;
using Constants;
using Utilities;

namespace ObjectTrackersNs
{
    public class ObjectTrackers
    {
        int RobotId;
        private Location RobotLocation;
        private List<LidarObject> current_object_list;
        public ObjectTrackers(int robotId)
        {
            RobotId = robotId;
            RobotLocation = new Location();
            current_object_list = new List<LidarObject>();
        }

        public void Update(List<LidarObject> list_of_objects)
        {
            List<LidarObject> new_current_object_list = new List<LidarObject>();

            for (int i = 0; i < current_object_list.Count; i++)
            {
                LidarObject current_object = current_object_list[i];
                LidarObject associated_object = Associate(current_object, list_of_objects);
                if (associated_object != null)
                {
                    current_object.Shape = associated_object.Shape;
                    current_object.color = associated_object.color;
                    current_object.Gain();
                    new_current_object_list.Add(current_object);
                    list_of_objects.Remove(associated_object);
                }
                else
                {
                    if (isVisible(current_object))
                    {   
                        current_object.Lose();
                    }
                    if (current_object.LIFE > ConstVar.LIDAR_OBJECT_DEATH_LIFE)
                        new_current_object_list.Add(current_object);
                }
            }

            foreach (LidarObject new_object in list_of_objects)
            {
                new_object.LIFE = ConstVar.LIDAR_OBJECT_DEFAULT_LIFE;
                new_current_object_list.Add(new_object);
            }

            current_object_list = new_current_object_list;
            OnObjectListUpdate(new_current_object_list);
        }

        public LidarObject Associate(LidarObject sourceObject, List<LidarObject> list_of_objects)
        {
            List<LidarObject> valid_Objects = list_of_objects;

            if (sourceObject.Type != LidarObjectType.Unknow)
                valid_Objects = list_of_objects.Where(x => x.Type == sourceObject.Type).ToList();

            List<LidarObject> associated_objects = valid_Objects.Where(x => Toolbox.Distance(x.Shape.Center, sourceObject.Shape.Center) <= ConstVar.LIDAR_OBJECT_MAX_ASSOCIATION_DISTANCE).ToList();

            if (associated_objects.Count != 0)
                return associated_objects.OrderBy(x => Toolbox.Distance(x.Shape.Center, sourceObject.Shape.Center)).FirstOrDefault();
            else
                return null;

        }

        public bool isVisible(LidarObject lidar_object)
        {
            /// TODO

            return false;
        }

        #region Input Callback
        public void OnNewRobotLocationReceived(object sender, LocationArgs e)
        {
            if (e.RobotId == RobotId)
            {
                RobotLocation = e.Location;
            }
        }

        public void OnNewObjectListReceived(object sender, List<LidarObject> list_of_objects)
        {
            Update(list_of_objects);
        }
        #endregion

        #region Events
        public event EventHandler<List<LidarObject>> OnObjectListUpdateEvent;

        public void OnObjectListUpdate(List<LidarObject> objectList)
        {
            OnObjectListUpdateEvent?.Invoke(this, objectList);
        }
        #endregion
    }
}
