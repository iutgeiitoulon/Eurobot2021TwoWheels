﻿using System;
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
        public double BackSensorAnalogValue;
        bool firstAnalogValueReceived = false;
        LocalWorldMap localWorldMap;
        private bool enableAvoidance = true;

        public TrajectoryAvoidance(int robotId)
        {
            robotID = robotId;
        }

        #region Methods

        public void DetectObstacleCollision()
        {
            if(enableAvoidance)
            {
                if (firstAnalogValueReceived) //on attend de recevoir au moins un retour du capteur sick
                {
                    if (localWorldMap == null)
                        return;
                    lock (localWorldMap)
                    {
                        if (localWorldMap.RobotLocation == null || localWorldMap.DestinationLocation == null || localWorldMap.Fields == null || localWorldMap.LidarObjectList == null)
                            return;

                        Segment robot_to_destination = new Segment(new PointD(localWorldMap.RobotLocation.X, localWorldMap.RobotLocation.Y), new PointD(localWorldMap.DestinationLocation.X, localWorldMap.DestinationLocation.Y));

                        List<RectangleOriented> list_of_obstacle = localWorldMap
                                                                    .Fields
                                                                    .Where(x => x.Type == FieldType.DeadZone)
                                                                    .Select(x => x.Shape)
                                                                    .ToList();

                        List<LidarObject> valid_robot = localWorldMap.LidarObjectList.Where(x => x.Type == LidarObjectType.Robot && x.LIFE >= ConstVar.LIDAR_OBJECT_VALID_LIFE).ToList();

                        List<RectangleOriented> list_of_personnal_field = localWorldMap.Fields.Where(x => x.Type == FieldType.Harbor).Select(x => x.Shape).ToList();


                        foreach (RectangleOriented rectangle in list_of_personnal_field)
                        {
                            valid_robot = valid_robot.Where(x => Toolbox.TestIfPointInsideAnOrientedRectangle(rectangle, x.Shape.Center)).ToList();
                        }
                        list_of_obstacle.AddRange(valid_robot.Select(x => x.Shape).ToList());
                        ;

                        //on check le backsensor si on recule
                        if (BackSensorAnalogValue <= ConstVar.BACK_SENSOR_AVOID_THRESHOLD && localWorldMap.RobotGhostLocation.Vx < 0) //Si on dépasse la distance min et qu'on recule
                        {
                            OnCollisionDetected(true);
                            return;
                        }


                        foreach (RectangleOriented obstacle in list_of_obstacle)
                        {

                            if (Toolbox.testIfSegmentIntersectRectangle(robot_to_destination, obstacle) && Toolbox.Distance(localWorldMap.RobotLocation, obstacle.Center) <= 0.70)
                            {
                                //Console.Write("A: ");
                                OnCollisionDetected(true);
                                return;
                            }
                            else if (Toolbox.Distance(localWorldMap.RobotLocation, obstacle.Center) <= 0.15)
                            {
                                //Console.Write("B: ");
                                OnCollisionDetected(true);
                                return;
                            }
                        }


                        OnCollisionDetected(false);
                    }
                }
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
            //Console.WriteLine("Collision?: " + collision);
            OnCollisionDetectedEvent?.Invoke(this, collision);
        }


        public void OnDisableAvoidance(object sender, EventArgs e)
        {
            enableAvoidance = false;
        }

        public void OnEnableAvoidance(object sender, EventArgs e)
        {
            enableAvoidance = true;
        }

        public void OnBackSensorAnalogReceived(object sender, IOAnalogValuesEventArgs e)
        {
            firstAnalogValueReceived = true;
            BackSensorAnalogValue = Math.Min(e.An1, e.An2);
            //Console.WriteLine("BackSensor Analog: An1: " + e.An1 + " - An2: " + e.An2);
        }

        #endregion
    }
}
