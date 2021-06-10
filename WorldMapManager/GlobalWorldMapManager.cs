using Constants;
using EventArgsLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Utilities;
using WorldMap;
using ZeroFormatter;

namespace WorldMapManager
{
    public class GlobalWorldMapManager
    {
        int RobotId;
        int TeamId;
        double freqRafraichissementWorldMap = 20;

        ConcurrentDictionary<int, LocalWorldMap> localWorldMapDictionary = new ConcurrentDictionary<int, LocalWorldMap>();
        //GlobalWorldMapStorage globalWorldMapStorage = new GlobalWorldMapStorage();
        GlobalWorldMap globalWorldMap = new GlobalWorldMap();
        Timer globalWorldMapSendTimer;
        //HighFreqTimer globalWorldMapSendTimer;
              

        public GlobalWorldMapManager(int robotId, int teamId)
        {
            RobotId = robotId;
            TeamId = teamId;
            //globalWorldMapSendTimer = new HighFreqTimer(freqRafraichissementWorldMap);
            //globalWorldMapSendTimer.Tick += GlobalWorldMapSendTimer_Tick; 
            globalWorldMapSendTimer = new Timer(freqRafraichissementWorldMap);            
            globalWorldMapSendTimer.Elapsed += GlobalWorldMapSendTimer_Tick;
            globalWorldMapSendTimer.Start();
        }

        private void GlobalWorldMapSendTimer_Tick(object sender, EventArgs e)
        {
            //ATTENTION : Starting point temporel pour beaucoup de processing, car cela envoie la GlobalWorldMap aux robots.
            MergeLocalWorldMaps();
        }

        public void OnLocalWorldMapReceived(object sender, LocalWorldMap e)
        {
            switch(sender.GetType().Name)
            {
                case "UDPMulticastInterpreter":
                    if (e.RobotId != RobotId)
                        AddOrUpdateLocalWorldMap(e);
                    else
                        ;
                    break;
                case "LocalWorldMapManager":
                    if (e.RobotId == RobotId)
                        AddOrUpdateLocalWorldMap(e);
                    else
                        Console.WriteLine("GlobalWorldMapManager : ceci ne devrait pas arriver");
                        ;
                    break;
                default:
                    AddOrUpdateLocalWorldMap(e);
                    break;
            }
            
        }

        private void AddOrUpdateLocalWorldMap(LocalWorldMap localWorldMap)
        {
            int robotId = localWorldMap.RobotId;
            int teamId = localWorldMap.TeamId;
            localWorldMapDictionary.AddOrUpdate(robotId, localWorldMap, (key, value) => localWorldMap);
        }

        DecimalJsonConverter decimalJsonConverter = new DecimalJsonConverter();

        double distanceMaxFusionObstacle = 0.2;
        double distanceMaxFusionTeamMate = 0.2;
        private void MergeLocalWorldMaps()
        {
            //Fusion des World Map locales pour construire la world map globale            

            //Génération de la carte fusionnée à partir des perceptions des robots de l'équipe
            //La fusion porte avant tout sur la balle et sur les adversaires.

            //TODO : faire un algo de fusion robuste pour la balle
            globalWorldMap = new WorldMap.GlobalWorldMap(TeamId);

            //Pour l'instant on prend la position de balle vue par le robot 1 comme vérité, mais c'est à améliorer !
            if (localWorldMapDictionary.Count > 0)
                globalWorldMap.ballLocationList = localWorldMapDictionary.First().Value.BallLocationList;
            globalWorldMap.teammateLocationList = new Dictionary<int, Location>();
            globalWorldMap.teammateGhostLocationList = new Dictionary<int, Location>();
            globalWorldMap.teammateDestinationLocationList = new Dictionary<int, Location>();
            globalWorldMap.teammateBallHandlingStateList = new Dictionary<int, BallHandlingState>();
            globalWorldMap.teammateWayPointList = new Dictionary<int, Location>();
            globalWorldMap.opponentLocationList = new List<Location>();
            globalWorldMap.obstacleLocationList = new List<LocationExtended>();
            globalWorldMap.teammateRoleList = new Dictionary<int, RoboCupRobotRole>();
            globalWorldMap.teammateDisplayMessageList = new Dictionary<int, string>();
            globalWorldMap.teammatePlayingSideList = new Dictionary<int, PlayingSide>();

            //On place tous les robots de l'équipe dans la global map
            foreach (var localMap in localWorldMapDictionary)
            {
                //On ajoute la position des robots de l'équipe dans la WorldMap
                globalWorldMap.teammateLocationList.Add(localMap.Key, localMap.Value.RobotLocation);
                //On ajoute le rôle des robots de l'équipe dans la WorldMap
                //globalWorldMap.teammateRoleList.Add(localMap.Key, localMap.Value.RobotRole);
                //On ajoute l'état de Ball Handling des robots de l'équipe dans la WorldMap
                //globalWorldMap.teammateBallHandlingStateList.Add(localMap.Key, localMap.Value.ballHandlingState);
                //On ajoute le message à afficher des robots de l'équipe dans la WorldMap
                //globalWorldMap.teammateDisplayMessageList.Add(localMap.Key, localMap.Value.messageDisplay);
                //On ajoute le playing Side des robots de l'équipe dans la WorldMap
                //globalWorldMap.teammatePlayingSideList.Add(localMap.Key, localMap.Value.playingSide);
                //On ajoute le ghost (position théorique) des robots de l'équipe dans la WorldMap
                globalWorldMap.teammateGhostLocationList.Add(localMap.Key, localMap.Value.RobotGhostLocation);
                //On ajoute la destination des robots de l'équipe dans la WorldMap
                globalWorldMap.teammateDestinationLocationList.Add(localMap.Key, localMap.Value.DestinationLocation);
                //On ajoute le waypoint courant des robots de l'équipe dans la WorldMap
                //globalWorldMap.teammateWayPointList.Add(localMap.Key, localMap.Value.waypointLocation);
            }

            try
            {
                //TODO : Fusion des obstacles vus par chacun des robots
                //foreach (var localMap in localWorldMapDictionary)
                //{

                //    foreach (var obstacle in localMap.Value.obstaclesLocationList)
                //    {
                //        if (obstacle != null)
                //        {
                //            bool skipNext = false;
                //            /// On itère sur chacun des obstacles perçus par chacun des robots
                //            /// On commence regarde pour chaque obstacle perçu si il ne correspond pas à une position de robot de l'équipe
                //            ///     Si c'est le cas, on abandonne cet obstacle
                //            ///     Si ce n'est pas le cas, on regarde si il ne correspond pas à un obstacle déjà présent dans la liste des obstacles
                //            ///         Si ce n'est pas le cas, on l'ajoute à la liste des obtacles
                //            ///         Si c'est le cas, on le fusionne en moyennant ses coordonnées de manière pondérée 
                //            ///             et on renforce le poids de cet obstacle
                //            foreach (var teamMateRobot in globalWorldMap.teammateLocationList)
                //            {

                //                if (Toolbox.Distance(new PointD(obstacle.X, obstacle.Y), new PointD(teamMateRobot.Value.X, teamMateRobot.Value.Y)) < distanceMaxFusionTeamMate)
                //                {
                //                    /// L'obstacle est un robot, on abandonne
                //                    skipNext = true;
                //                    break;
                //                }
                //            }
                //            if (skipNext == false)
                //            {
                //                /// Si on arrive ici c'est que l'obstacle n'est pas un robot de l'équipe
                //                foreach (var obstacleConnu in globalWorldMap.obstacleLocationList)
                //                {
                //                    if (Toolbox.Distance(new PointD(obstacle.X, obstacle.Y), new PointD(obstacleConnu.X, obstacleConnu.Y)) < distanceMaxFusionObstacle)
                //                    {
                //                        //L'obstacle est déjà connu, on le fusionne /TODO : améliorer la fusion avec pondération
                //                        obstacleConnu.X = (obstacleConnu.X + obstacle.X) / 2;
                //                        obstacleConnu.Y = (obstacleConnu.Y + obstacle.Y) / 2;
                //                        skipNext = true;
                //                        break;
                //                    }
                //                }
                //            }
                //            if (skipNext == false)
                //            {
                //                /// Si on arrive ici, c'est que l'obstacle n'était pas connu, on l'ajoute
                //                globalWorldMap.obstacleLocationList.Add(obstacle);
                //            }
                //        }
                //    }
                //}
            }
            catch { }
            
            /// Transfert de la globalworldmap via le Multicast UDP
            //var s = ZeroFormatterSerializer.Serialize<WorldMap.ZeroFormatterMsg>(globalWorldMap);
            OnGlobalWorldMap(globalWorldMap);
            //OnMulticastSendGlobalWorldMap(s);
            //GWMEmiseMonitoring.GWMEmiseMonitor(s.Length);
        }
        
        //Output events
        public event EventHandler<DataReceivedArgs> OnMulticastSendGlobalWorldMapEvent;
        public virtual void OnMulticastSendGlobalWorldMap(byte[] data)
        {
            var handler = OnMulticastSendGlobalWorldMapEvent;
            if (handler != null)
            {
                handler(this, new DataReceivedArgs { Data = data });
            }
        }

        ////Output event for Multicast Bypass : NO USE at RoboCup !
        public event EventHandler<GlobalWorldMap> OnGlobalWorldMapEvent;
        public virtual void OnGlobalWorldMap(GlobalWorldMap map)
        {
            var handler = OnGlobalWorldMapEvent;
            if (handler != null)
            {
                handler(this, map);
            }
        }
    }
}

