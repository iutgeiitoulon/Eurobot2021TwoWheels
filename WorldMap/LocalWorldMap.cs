using Constants;
using System;
using System.Collections.Generic;
using Utilities;
using EventArgsLibrary;
using System.Windows;
using Lidar;
using System.Linq;

namespace WorldMap
{
    public class LocalWorldMap
    {
        #region Params
        public virtual int RobotId { get; set; }
        public virtual int TeamId { get; set; }
        public virtual Location RobotLocation { get; set; }
        public virtual Equipe Team { get; set; }
        public virtual Location RobotGhostLocation { get; set; }
        public virtual Location DestinationLocation { get; set; }
        public virtual List<Location> WaypointLocations { get; set; }
        public virtual List<Location> RobotHistorical { get; set; }
        public virtual List<Location> BallLocationList { get; set; }
        public virtual List<LocationExtended> ObstaclesLocationList { get; set; }
        public virtual List<PolarPointRssi> LidarPoints { get; set; }
        public virtual List<PointDExtended> LidarMapRaw { get; set; }
        public virtual List<PointDExtended> LidarMapProcessed { get; set; }
        public virtual List<SegmentExtended> LidarSegment { get; set; }
        public virtual List<LidarObject> LidarObjectList { get; set; }
        public virtual List<Field> Fields { get; set; }

        // public virtual List<PointD> lidarMap { get; set; }
        // public virtual List<PointD> lidarMapProcessed { get; set; }
        // public virtual Heatmap heatMapStrategy { get; set; }
        // public virtual Heatmap heatMapWaypoint { get; set; }
        #endregion


        #region Constructors
        public LocalWorldMap(int robotId, int teamId)
        {
            RobotId = robotId;
            TeamId = teamId;
            RobotLocation = new Location(0, 0, 0, 0, 0, 0);
            RobotGhostLocation = new Location(0, 0, 0, 0, 0, 0);
            WaypointLocations = new List<Location> { };
            RobotHistorical = new List<Location> { RobotLocation };
            LidarObjectList = new List<LidarObject>();
        }
        #endregion
    }

    public enum GameState
    {
        STOPPED,
        STOPPED_GAME_POSITIONING,
        PLAYING,
    }

    public enum StoppedGameAction
    {
        NONE,
        KICKOFF,
        KICKOFF_OPPONENT,
        FREEKICK,
        FREEKICK_OPPONENT,
        GOALKICK,
        GOALKICK_OPPONENT,
        THROWIN,
        THROWIN_OPPONENT,
        CORNER,
        CORNER_OPPONENT,
        PENALTY,
        PENALTY_OPPONENT,
        PARK,
        DROPBALL,
        GOTO,
        GOTO_OPPONENT,
    }
}
