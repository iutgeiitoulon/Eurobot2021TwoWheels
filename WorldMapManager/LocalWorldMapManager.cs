using Constants;
using EventArgsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using WorldMap;
using ZeroFormatter;

namespace WorldMapManager
{
    public class LocalWorldMapManager
    {
        LocalWorldMap localWorldMap;

        public LocalWorldMapManager(int robotId, int teamId)
        {
            localWorldMap = new LocalWorldMap(robotId, teamId);
        }
        #region Methods
        public void Init()
        {
            localWorldMap.RobotLocation = new Location(0, 0, 0, 0, 0, 0);
            localWorldMap.RobotGhostLocation = new Location(0, 0, 0, 0, 0, 0);
            localWorldMap.WaypointLocations = new List<Location> { };
            localWorldMap.RobotHistorical = new List<Location> { localWorldMap.RobotLocation };
            OnLocalWorldMapChange();
        }

        public void OnUpdateRobotLocation(Location location)
        {
            localWorldMap.RobotLocation = location;
            OnUpdateHistoricalLocation(location);
            OnUpdateRobotLocationEvent?.Invoke(this, new LocationArgs() { RobotId = localWorldMap.RobotId, Location = location });
            OnLocalWorldMapChange();
        }

        public void OnUpdateHistoricalLocation(Location location)
        {
            if (localWorldMap.RobotHistorical.Count != 0)
            {
                Location lastHistorical = localWorldMap.RobotHistorical[localWorldMap.RobotHistorical.Count - 1];

                PointD p1 = new PointD(location.X, location.Y);
                PointD p2 = new PointD(lastHistorical.X, lastHistorical.Y);

                double distance = Toolbox.Distance(p1, p2);
                if (distance >= ConstVar.MINIMAL_WORLD_HISTORICAL_DIST)
                {
                    localWorldMap.RobotHistorical.Add(location);
                    OnNewHistoricalPositionEvent?.Invoke(this, new LocationArgs() { RobotId = localWorldMap.RobotId, Location = location });
                }

            }
            OnLocalWorldMapChange();
            OnLocalWorldMapChange();
        }

        public void AddNewWaypoints(Location location)
        {
            localWorldMap.WaypointLocations.Add(location);
            OnLocalWorldMapChange();
        }

        public void SetDestinationLocation(Location location)
        {
            localWorldMap.DestinationLocation = location;
            OnLocalWorldMapChange();
        }

        public void ResetWaypoints()
        {
            localWorldMap.WaypointLocations = new List<Location> { };
            OnLocalWorldMapChange();
        }

        public void SetWaypoints(List<Location> locations)
        {
            localWorldMap.WaypointLocations = locations;
            OnLocalWorldMapChange();
        }

        public void ResetDestination()
        {
            localWorldMap.DestinationLocation = null;
            OnLocalWorldMapChange();
        }

        public void DeleteFirstWaypoint()
        {
            localWorldMap.WaypointLocations.RemoveAt(0);
            OnLocalWorldMapChange();
        }

        public void SetRobotLocation(Location location)
        {
            localWorldMap.RobotLocation = location;
            OnLocalWorldMapChange();
        }

        public void SetGhostRobotLocation(Location location)
        {
            localWorldMap.RobotGhostLocation = location;
            OnLocalWorldMapChange();
        }

        public void ResetRobot()
        {
            ResetRobot(new Location(0, 0, 0, 0, 0, 0));
        }

        public void ResetRobot(Location location)
        {
            localWorldMap.RobotLocation = location;
            localWorldMap.RobotGhostLocation = location;
            localWorldMap.WaypointLocations = new List<Location> { };
            localWorldMap.RobotHistorical = new List<Location> { localWorldMap.RobotLocation };
            OnResetRobotEvent?.Invoke(this, new LocationArgs() { RobotId = localWorldMap.RobotId, Location = location });
            OnLocalWorldMapChange();
        }

        public void SetupDeadZone(List<RectangleOriented> list_of_deadzones)
        {
            localWorldMap.DeadZones = list_of_deadzones;
            OnLocalWorldMapChange();
        }
        #endregion


        #region Input Callback
        public void SetNewWaypointsList(object sender, List<Location> locations)
        {
            SetWaypoints(locations);
        }

        public void AddNewWaypointsEvent(object sender, Location location)
        {
            AddNewWaypoints(location);
        }

        public void AddNewWaypointsEvent(object sender, PointD point)
        {
            AddNewWaypoints(new Location(point.X, point.Y, 0, 0, 0, 0));
        }

        public void SetDestinationLocationEvent(object sender, Location location)
        {
            SetDestinationLocation(location);
        }

        public void SetDestinationLocationEvent(object sender, PointD point)
        {
            SetDestinationLocation(new Location(point.X, point.Y, 0, 0, 0, 0));
        }

        public void ResetWaypointDestinationEvent(object sender, PointD point)
        {
            ResetWaypoints();
            ResetDestination();
        }

        public void ResetDestinationEvent(object sender, EventArgs e)
        {
            ResetDestination();
        }

        public void OnLidarProcesObjectsReceived(object sender, List<LidarObject> lidarObjects)
        {
            localWorldMap.LidarObjectList = lidarObjects;
            OnLocalWorldMapChange();
        }

        public void OnGameStateChange(object sender, GameState gameState_a)
        {
            OnLocalWorldMapChange();
        }

        public void OnGhostLocation(object sender, LocationArgs e)
        {
            SetGhostRobotLocation(e.Location);
        }

        public void OnRobotLocation(object sender, Location location)
        {
            OnUpdateRobotLocation(location);
        }

        public void OnRobotLocationArgs(object sender, LocationArgs location)
        {
            OnUpdateRobotLocation(location.Location);
        }

        public void OnWaypointReached(object sender, Location point)
        {
            DeleteFirstWaypoint();
        }

        public void OnDestinationReached(object sender, Location location)
        {
            ResetDestination();
        }

        public void OnPhysicalPositionReceived(object sender, LocationArgs e)
        {
            if (localWorldMap == null)
                return;
            if (localWorldMap.RobotId == e.RobotId)
            {
                SetRobotLocation(e.Location); //Update de la robot Location dans la local world map
                OnLocalWorldMapForDisplayOnly(localWorldMap); //Event de transmission de la local world map
            }
        }

        public void OnWaypointReceived(object sender, LocationArgs e)
        {
            if (localWorldMap == null)
                return;
            if (localWorldMap.RobotId == e.RobotId)
            {
                /// MULTI WAYPOINTS
                //localWorldMap.waypointLocation = e.Location;
            }
        }

        public void OnGhostLocationReceived(object sender, LocationArgs e)
        {
            if (localWorldMap == null)
                return;
            if (localWorldMap.RobotId == e.RobotId)
            {
                SetGhostRobotLocation(e.Location);
            }
        }

        public void OnDestinationReceived(object sender, LocationArgs e)
        {
            if (localWorldMap == null)
                return;
            if (localWorldMap.RobotId == e.RobotId)
            {
                SetDestinationLocation(e.Location);
            }
        }

        public void OnTeamChangeReceived(object sender, Equipe e)
        {
            if (localWorldMap == null)
                return;

            localWorldMap.Team = e;
        }

        public void OnRawLidarDataReceived(object sender, List<PolarPointRssiExtended> e)
        {
            if (localWorldMap == null || localWorldMap.RobotLocation == null)
                return;
            if (e.Count != 0)
            {
                List<PointDExtended> listPtLidar = e.Select(x => new PointDExtended(
                    new PointD(
                        localWorldMap.RobotLocation.X + (x.Pt.Distance * Math.Cos(localWorldMap.RobotLocation.Theta + x.Pt.Angle)),
                        localWorldMap.RobotLocation.Y + (x.Pt.Distance * Math.Sin(localWorldMap.RobotLocation.Theta + x.Pt.Angle))
                    ),
                    x.Color,
                    x.Width
                    )
                ).ToList();

                localWorldMap.LidarMapRaw = listPtLidar;
            }
        }
        public void OnProcessedLidarDataReceived(object sender, List<PolarPointRssiExtended> e)
        {
            if (localWorldMap == null || localWorldMap.RobotLocation == null)
                return;

            if (e.Count != 0)
            {
                List<PointDExtended> listPtLidar = e.Select(x => new PointDExtended(
                   new PointD(
                       localWorldMap.RobotLocation.X + (x.Pt.Distance * Math.Cos(localWorldMap.RobotLocation.Theta + x.Pt.Angle)),
                       localWorldMap.RobotLocation.Y + (x.Pt.Distance * Math.Sin(localWorldMap.RobotLocation.Theta + x.Pt.Angle))
                   ),
                   x.Color,
                   x.Width
                   )
                ).ToList();


                localWorldMap.LidarMapProcessed = listPtLidar;
            }
        }

        public void OnLidarProcessedLineReceived(object sender, List<SegmentExtended> list_of_segments)
        {
            List<SegmentExtended> corrected_list_segment = new List<SegmentExtended>();

            foreach (SegmentExtended segment in list_of_segments)
            {
                PolarPointRssi point_a = Toolbox.ConvertPointDToPolar(new PointD(segment.Segment.X1, segment.Segment.Y1));
                PolarPointRssi point_b = Toolbox.ConvertPointDToPolar(new PointD(segment.Segment.X2, segment.Segment.Y2));

                PointD correct_point_a = new PointD(
                        localWorldMap.RobotLocation.X + (point_a.Distance * Math.Cos(localWorldMap.RobotLocation.Theta + point_a.Angle)),
                        localWorldMap.RobotLocation.Y + (point_a.Distance * Math.Sin(localWorldMap.RobotLocation.Theta + point_a.Angle))
                    );

                PointD correct_point_b = new PointD(
                        localWorldMap.RobotLocation.X + (point_b.Distance * Math.Cos(localWorldMap.RobotLocation.Theta + point_b.Angle)),
                        localWorldMap.RobotLocation.Y + (point_b.Distance * Math.Sin(localWorldMap.RobotLocation.Theta + point_b.Angle))
                    );
                corrected_list_segment.Add(new SegmentExtended(correct_point_a, correct_point_b, segment.Color, segment.Width));
            }

            localWorldMap.LidarSegment = corrected_list_segment;
            OnLocalWorldMapChange();
        }

        public void OnLidarObjectReceived(object sender, List<LidarObject> list_of_object)
        {
            localWorldMap.LidarObjectList = list_of_object;
        }

        public void OnLidarAbsoluteProcessPointReceived(object sender, List<PointDExtended> list_of_points)
        {
            /// NOT SAFE DO NOT USE UNTIL LOCKED

            if (localWorldMap.LidarMapProcessed != null)
                localWorldMap.LidarMapProcessed.AddRange(list_of_points);
            else
                localWorldMap.LidarMapProcessed = list_of_points.ToList();
        }

        public void OnNewDeadZonesReceived(object sender, List<RectangleOriented> list_of_deadzones)
        {
            SetupDeadZone(list_of_deadzones);
        }
        #endregion

        #region Events
        public event EventHandler<LocationArgs> OnUpdateRobotLocationEvent;
        public event EventHandler<LocationArgs> OnUpdateGhostRobotLocation;
        public event EventHandler<LocationArgs> OnSetRobotLocationEvent;
        public event EventHandler<LocationArgs> OnSetGhostRobotLocationEvent;
        public event EventHandler<LocationArgs> OnNewHistoricalPositionEvent;
        public event EventHandler<LocationArgs> OnNewWaypointLocationEvent;
        public event EventHandler<LocationArgs> OnResetRobotEvent;
        public event EventHandler<LocalWorldMap> OnLocalWorldMapEvent;
        public event EventHandler<DataReceivedArgs> OnMulticastSendLocalWorldMapEvent;
        public event EventHandler<LocalWorldMap> OnLocalWorldMapToGlobalWorldMapGeneratorEvent;
        public event EventHandler<LocalWorldMap> OnLocalWorldMapForDisplayOnlyEvent;

        public virtual void OnLocalWorldMapChange()
        {
            OnLocalWorldMapEvent?.Invoke(this, localWorldMap);
        }

        public virtual void OnMulticastSendLocalWorldMapCommand(byte[] data)
        {
            OnMulticastSendLocalWorldMapEvent?.Invoke(this, new DataReceivedArgs { Data = data });
        }

        
        public virtual void OnLocalWorldMapToGlobalWorldMapGenerator(LocalWorldMap data)
        {
            OnLocalWorldMapToGlobalWorldMapGeneratorEvent?.Invoke(this, data);
        }

        public virtual void OnLocalWorldMapForDisplayOnly(LocalWorldMap map)
        {
            OnLocalWorldMapForDisplayOnlyEvent?.Invoke(this, map);
        }
        #endregion
    }
}
