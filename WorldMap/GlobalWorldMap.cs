using Constants;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Utilities;
using ZeroFormatter;

namespace WorldMap
{
    [ZeroFormattable]
    public class GlobalWorldMap : ZeroFormatterMsg
    {
        public override ZeroFormatterMsgType Type
        {
            get
            {
                return ZeroFormatterMsgType.GlobalWM;
            }
        }

        [Index(0)]
        public virtual int TeamId { get; set; }
        [Index(1)]
        public virtual int timeStampMs { get; set; }
        [Index(2)]
        //public virtual GameState gameState { get; set; } // = GameState.STOPPED;
        //[Index(3)]
        //public virtual StoppedGameAction stoppedGameAction { get; set; } // = StoppedGameAction.NONE;
        //[Index(4)]
        //public virtual PlayingSide playingSide { get; set; } // = PlayingSide.Left        
        //[Index(5)]
        public virtual List<Location> ballLocationList { get; set; }
        [Index(6)]
        public virtual Dictionary<int, Location> teammateLocationList { get; set; }
        [Index(7)]
        public virtual Dictionary<int, Location> teammateGhostLocationList { get; set; }
        [Index(8)]
        public virtual Dictionary<int, Location> teammateDestinationLocationList { get; set; }
        [Index(9)]
        public virtual Dictionary<int, Location> teammateWayPointList { get; set; }
        [Index(10)]
        public virtual List<Location> opponentLocationList { get; set; }
        [Index(11)]
        public virtual List<LocationExtended> obstacleLocationList { get; set; }
        [Index(12)]
        public virtual Dictionary<int, RoboCupRobotRole> teammateRoleList { get; set; }
        [Index(13)]
        public virtual Dictionary<int, BallHandlingState> teammateBallHandlingStateList { get; set; }
        [Index(14)]
        public virtual Dictionary<int, string> teammateDisplayMessageList { get; set; }
        [Index(15)]
        public virtual Dictionary<int, PlayingSide> teammatePlayingSideList { get; set; }

        public GlobalWorldMap()
        {
        }
        public GlobalWorldMap(int teamId)
        {
            TeamId = teamId;
        }

        public WorldStateMessage ConvertToWorldStateMessage()
        {
            WorldStateMessage wsm = new WorldStateMessage();
            foreach (var teamMate in teammateLocationList)
            {
                Robot r = new Robot();
                r.Id = teamMate.Key;
                r.Pose = new List<double>() { teamMate.Value.X, teamMate.Value.Y, teamMate.Value.Theta };
                r.TargetPose = new List<double>() { 0, 0, 0 };
                r.Velocity = new List<double>() { teamMate.Value.Vx, teamMate.Value.Vy, teamMate.Value.Vtheta };
                r.Intention = "";
                r.BatteryLevel = 100;
                r.BallEngaged = 0;
                wsm.Robots.Add(r);
            }

            //On prend par défaut la première balle du premier robot
            Ball b = new Ball();
            b.Position = new List<double?>() { ballLocationList[0].X, ballLocationList[0].X, 0 };
            b.Velocity = new List<double?>() { ballLocationList[0].Vx, ballLocationList[0].Vy, 0 };
            b.Confidence = 1;
            wsm.Balls.Add(b);

            foreach (var o in obstacleLocationList)
            {
                Obstacle obstacle = new Obstacle();
                obstacle.Position = new List<double>() { o.X, o.Y };
                obstacle.Velocity = new List<double>() { o.Vx, o.Vy };
                obstacle.Radius = 0.5;
                obstacle.Confidence = 1;
                wsm.Obstacles.Add(obstacle);
            }

            wsm.Intention = "Win";
            wsm.AgeMs = timeStampMs;
            wsm.TeamName = "RCT";
            wsm.Type = "worldstate";
            return wsm;
        }
    }

    //public class GlobalWorldMapStorage
    //{
    //    public ConcurrentDictionary<int, Location> robotLocationDictionary { get; set; }
    //    public ConcurrentDictionary<int, Location> ghostLocationDictionary { get; set; }
    //    public ConcurrentDictionary<int, Location> destinationLocationDictionary { get; set; }
    //    public ConcurrentDictionary<int, Location> waypointLocationDictionary { get; set; }
    //    public ConcurrentDictionary<int, List<Location>> ballLocationListDictionary { get; set; }
    //    public ConcurrentDictionary<int, List<LocationExtended>> ObstaclesLocationListDictionary { get; set; }
    //    public ConcurrentDictionary<int, RobotRole> robotRoleDictionary { get; set; }
    //    public ConcurrentDictionary<int, BallHandlingState> ballHandlingStateDictionary { get; set; }
    //    public ConcurrentDictionary<int, string> robotMessageDisplayDictionary { get; set; }
    //    public ConcurrentDictionary<int, PlayingSide> robotPlayingSideDictionary { get; set; }

    //    public GlobalWorldMapStorage()
    //    {
    //        robotLocationDictionary = new ConcurrentDictionary<int, Location>();
    //        ghostLocationDictionary = new ConcurrentDictionary<int, Location>();
    //        destinationLocationDictionary = new ConcurrentDictionary<int, Location>();
    //        waypointLocationDictionary = new ConcurrentDictionary<int, Location>();
    //        ballLocationListDictionary = new ConcurrentDictionary<int, List<Location>>();
    //        ObstaclesLocationListDictionary = new ConcurrentDictionary<int, List<LocationExtended>>();
    //        robotRoleDictionary = new ConcurrentDictionary<int, RobotRole>();
    //        ballHandlingStateDictionary = new ConcurrentDictionary<int, BallHandlingState>();
    //        robotMessageDisplayDictionary = new ConcurrentDictionary<int, string>();
    //        robotPlayingSideDictionary = new ConcurrentDictionary<int, PlayingSide>();
    //    }

    //    public void AddOrUpdateRobotLocation(int id, Location loc)
    //    {
    //        robotLocationDictionary.AddOrUpdate(id, loc, (key, value) => loc);
    //    }
    //    public void AddOrUpdateGhostLocation(int id, Location loc)
    //    {
    //        ghostLocationDictionary.AddOrUpdate(id, loc, (key, value) => loc);
    //    }

    //    public void AddOrUpdateRobotDestination(int id, Location loc)
    //    {
    //        destinationLocationDictionary.AddOrUpdate(id, loc, (key, value) => loc);           
    //    }

    //    public void AddOrUpdateRobotWayPoint(int id, Location loc)
    //    {
    //        waypointLocationDictionary.AddOrUpdate(id, loc, (key, value) => loc);
    //    }
    //    public void AddOrUpdateRobotRole(int id, RobotRole role)
    //    {
    //        robotRoleDictionary.AddOrUpdate(id, role, (key, value) => role);
    //    }
    //    public void AddOrUpdateBallHandlingState(int id, BallHandlingState state)
    //    {
    //        ballHandlingStateDictionary.AddOrUpdate(id, state, (key, value) => state);
    //    }
    //    public void AddOrUpdateMessageDisplay(int id, string  message)
    //    {
    //        robotMessageDisplayDictionary.AddOrUpdate(id, message, (key, value) => message);
    //    }

    //    public void AddOrUpdateRobotPlayingSide(int id, PlayingSide playSide)
    //    {
    //        robotPlayingSideDictionary.AddOrUpdate(id, playSide, (key, value) => playSide);
    //    }

    //    public void AddOrUpdateBallLocationList(int id, List<Location> ballLocationList)
    //    {
    //        ballLocationListDictionary.AddOrUpdate(id, ballLocationList, (key, value) => ballLocationList);
    //    }

    //    public void AddOrUpdateObstaclesList(int id, List<LocationExtended> locList)
    //    {
    //        ObstaclesLocationListDictionary.AddOrUpdate(id, locList, (key, value) => locList);
    //    }
    //}
}
