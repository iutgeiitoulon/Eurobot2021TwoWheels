using Constants;
using SciChart.Charting.Visuals.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Linq;
using Utilities;
using WorldMap;
using System.Windows;
using System.Windows.Input;

namespace WpfWorldMapDisplay
{

    /// <summary>
    /// Logique d'interaction pour ExtendedHeatMap.xaml
    /// </summary>
    public partial class GlobalWorldMapDisplay : UserControl
    {
        Random random = new Random();
        DispatcherTimer timerAffichage;

        public bool IsExtended = false;

        //double TerrainLowerX = -11;
        //double TerrainUpperX = 11;
        //double TerrainLowerY = -7;
        //double TerrainUpperY = 7;

        //Liste des robots à afficher
        Dictionary<int, RobotDisplay> TeamMatesDisplayDictionary = new Dictionary<int, RobotDisplay>();
        Dictionary<int, RobotDisplay> OpponentDisplayDictionary = new Dictionary<int, RobotDisplay>();

        //Liste des objets à afficher
        List<PolygonExtended> ObjectDisplayList = new List<PolygonExtended>();

        //Liste des balles à afficher
        List<BallDisplay> BallDisplayList = new List<BallDisplay>();

        public GlobalWorldMapDisplay()
        {
            InitializeComponent();

            InitRoboCupSoccerField();
        }

        public void InitTeamMate(int robotId, string name)
        {
            PolygonExtended robotShape = new PolygonExtended();
            robotShape.polygon.Points.Add(new System.Windows.Point(-0.25, -0.25));
            robotShape.polygon.Points.Add(new System.Windows.Point(0.25, -0.25));
            robotShape.polygon.Points.Add(new System.Windows.Point(0.2, 0));
            robotShape.polygon.Points.Add(new System.Windows.Point(0.25, 0.25));
            robotShape.polygon.Points.Add(new System.Windows.Point(-0.25, 0.25));
            robotShape.polygon.Points.Add(new System.Windows.Point(-0.25, -0.25));
            robotShape.borderColor = Color.Black;
            robotShape.backgroundColor = Color.FromArgb(255, 0, 0, 200);

            PolygonExtended ghostShape = new PolygonExtended();
            ghostShape.polygon.Points.Add(new System.Windows.Point(-0.27, -0.27));
            ghostShape.polygon.Points.Add(new System.Windows.Point(0.27, -0.27));
            ghostShape.polygon.Points.Add(new System.Windows.Point(0.22, 0.02));
            ghostShape.polygon.Points.Add(new System.Windows.Point(0.27, 0.27));
            ghostShape.polygon.Points.Add(new System.Windows.Point(-0.27, 0.27));
            ghostShape.polygon.Points.Add(new System.Windows.Point(-0.27, -0.27));
            ghostShape.backgroundColor = System.Drawing.Color.FromArgb(20, 0, 255, 0);
            ghostShape.borderColor = System.Drawing.Color.Black;

            RobotDisplay rd = new RobotDisplay(robotShape, ghostShape, name);
            rd.SetLocation(new Location(0, 0, 0, 0, 0, 0));
            TeamMatesDisplayDictionary.Add(robotId, rd);
        }

        public void InitOpponent(int robotId, string name)
        {
            PolygonExtended robotShape = new PolygonExtended();
            for (int i = 0; i < 7; i++)
                robotShape.polygon.Points.Add(new System.Windows.Point(0.25 * Math.Cos(i * 2 * Math.PI / 6), 0.25 * Math.Sin(i * 2 * Math.PI / 6)));
            robotShape.borderColor = Color.Black;
            robotShape.backgroundColor = Color.FromArgb(255, 200, 0, 0);

            PolygonExtended ghostShape = new PolygonExtended();
            for (int i = 0; i < 7; i++)
                ghostShape.polygon.Points.Add(new System.Windows.Point(0.27 * Math.Cos(i * 2 * Math.PI / 6), 0.27 * Math.Sin(i * 2 * Math.PI / 6)));
            ghostShape.backgroundColor = System.Drawing.Color.FromArgb(20, 0, 255, 0);
            ghostShape.borderColor = System.Drawing.Color.Black;

            RobotDisplay rd = new RobotDisplay(robotShape, ghostShape, name);
            rd.SetLocation(new Location(0, 0, 0, 0, 0, 0));
            OpponentDisplayDictionary.Add(robotId, rd);
        }

        public void AddOrUpdateTextAnnotation(string annotationName, string annotationText, double posX, double posY)
        {
            var textAnnotationList = sciChart.Annotations.Where(annotation => annotation.GetType().Name == "TextAnnotation").ToList();
            var annot = textAnnotationList.FirstOrDefault(c => ((TextAnnotation)c).Name == "R" + annotationName + "r");
            if (annot == null)
            {
                TextAnnotation textAnnot = new TextAnnotation();
                textAnnot.Text = annotationText;
                textAnnot.Name = "R"+annotationName+"r";
                textAnnot.X1 = posX;
                textAnnot.Y1 = posY;
                textAnnot.HorizontalAnchorPoint = HorizontalAnchorPoint.Center;
                textAnnot.VerticalAnchorPoint = VerticalAnchorPoint.Bottom;
                textAnnot.FontSize = 10;
                textAnnot.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0x88, 0xFF, 0xFF, 0xFF));
                textAnnot.FontWeight = FontWeights.Bold;
                sciChart.Annotations.Add(textAnnot);
            }
            else
            {
                ((TextAnnotation)annot).Text = annotationText;
                ((TextAnnotation)annot).Name = "R" + annotationName + "r";
                annot.X1 = posX;
                annot.Y1 = posY+0.5;
                ((TextAnnotation)annot).Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0x88, 0xFF, 0xFF, 0xFF));
            }
        }

        public void UpdateWorldMapDisplay()
        {
            DrawBalls();
            DrawTeam();
            PolygonSeries.RedrawAll();
            RobotGhostSeries.RedrawAll();
            RobotShapesSeries.RedrawAll();
            ObjectsPolygonSeries.RedrawAll();
            BallPolygon.RedrawAll();
        }

        public void UpdateGlobalWorldMap(GlobalWorldMap globalWorldMap)
        {
            lock (globalWorldMap.teammateLocationList)
            {
                foreach (var robotLoc in globalWorldMap.teammateLocationList)
                {
                    UpdateRobotLocation(robotLoc.Key, robotLoc.Value);
                }
            }

            lock (globalWorldMap.teammateRoleList)
            {
                foreach (var robotRole in globalWorldMap.teammateRoleList)
                {
                    UpdateRobotRole(robotRole.Key, robotRole.Value);
                }
            }

            lock (globalWorldMap.teammateDisplayMessageList)
            {
                foreach (var messageDisplay in globalWorldMap.teammateDisplayMessageList)
                {
                    UpdateDisplayMessage(messageDisplay.Key, messageDisplay.Value);
                }
            }

            lock (globalWorldMap.teammatePlayingSideList)
            {
                foreach (var playingSide in globalWorldMap.teammatePlayingSideList)
                {
                    UpdatePlayingSide(playingSide.Key, playingSide.Value);
                }
            }

            lock (globalWorldMap.teammateGhostLocationList)
            {
                foreach (var robotGhostLoc in globalWorldMap.teammateGhostLocationList)
                {
                    UpdateRobotGhostLocation(robotGhostLoc.Key, robotGhostLoc.Value);
                }
            }

            lock (globalWorldMap.teammateDestinationLocationList)
            {
                foreach (var robotDest in globalWorldMap.teammateDestinationLocationList)
                {
                    UpdateRobotDestination(robotDest.Key, robotDest.Value);
                }
            }

            lock (globalWorldMap.teammateWayPointList)
            {
                foreach (var robotWayPointt in globalWorldMap.teammateWayPointList)
                {
                    UpdateRobotWaypoint(robotWayPointt.Key, robotWayPointt.Value);
                }
            }

            lock (globalWorldMap.obstacleLocationList)
            {
                int i = 0;
                foreach (var opponentLocation in globalWorldMap.obstacleLocationList)
                {
                    if (globalWorldMap.TeamId == (int)TeamId.Team1)
                        UpdateOpponentLocation((int)TeamId.Team2 + i, new Location(opponentLocation.X, opponentLocation.Y, 0, 0, 0, 0));
                    else if (globalWorldMap.TeamId == (int)TeamId.Team2)
                        UpdateOpponentLocation((int)TeamId.Team1 + i, new Location(opponentLocation.X, opponentLocation.Y, 0, 0, 0, 0));
                    i++;
                }
            }
            UpdateBallLocationList(globalWorldMap.ballLocationList);
        }
        
        public void DrawBalls()
        {
            lock (BallDisplayList)
            {
                int indexBall = 0;
                foreach (var ball in BallDisplayList)
                {
                    //Affichage de la balle
                    BallPolygon.AddOrUpdatePolygonExtended((int)BallId.Ball + indexBall, ball.GetBallPolygon());
                    BallPolygon.AddOrUpdatePolygonExtended((int)BallId.Ball + indexBall + (int)Caracteristique.Speed, ball.GetBallSpeedArrow());
                    indexBall++;
                }
            }
        }

        public void DrawTeam()
        {
            //XyDataSeries<double, double> lidarPts = new XyDataSeries<double, double>();
            ObjectsPolygonSeries.Clear();

            foreach (var r in TeamMatesDisplayDictionary)
            {
                //Affichage des robots
                RobotGhostSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.Ghost, TeamMatesDisplayDictionary[r.Key].GetRobotGhostPolygon());
                PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.Speed, TeamMatesDisplayDictionary[r.Key].GetRobotSpeedArrow());
                PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.Destination, TeamMatesDisplayDictionary[r.Key].GetRobotDestinationArrow());
                PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.WayPoint, TeamMatesDisplayDictionary[r.Key].GetRobotWaypointArrow());

                //On trace le robot en dernier pour l'avoir en couche de dessus
                RobotShapesSeries.AddOrUpdatePolygonExtended(r.Key, TeamMatesDisplayDictionary[r.Key].GetRobotPolygon());

                AddOrUpdateTextAnnotation(r.Key.ToString(), r.Value.robotName, TeamMatesDisplayDictionary[r.Key].GetRobotLocation().X, TeamMatesDisplayDictionary[r.Key].GetRobotLocation().Y);
                AddOrUpdateTextAnnotation(r.Key.ToString()+"Role", r.Value.robotRole.ToString(), TeamMatesDisplayDictionary[r.Key].GetRobotLocation().X, TeamMatesDisplayDictionary[r.Key].GetRobotLocation().Y-1.4);
                AddOrUpdateTextAnnotation(r.Key.ToString() + "Console", r.Value.DisplayMessage.ToString(), TeamMatesDisplayDictionary[r.Key].GetRobotLocation().X, TeamMatesDisplayDictionary[r.Key].GetRobotLocation().Y - 1.9);

                ////Rendering des objets Lidar
                //foreach (var polygonObject in TeamMatesDisplayDictionary[r.Key].GetRobotLidarObjects())
                //    ObjectsPolygonSeries.AddOrUpdatePolygonExtended(ObjectsPolygonSeries.Count(), polygonObject);
            }
            
            foreach (var r in OpponentDisplayDictionary)
            {
                //Affichage des robots adverses
                PolygonSeries.AddOrUpdatePolygonExtended(r.Key, OpponentDisplayDictionary[r.Key].GetRobotPolygon());
            }
        }
        private void UpdateRobotRole(int robotId, RoboCupRobotRole role)
        {
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                TeamMatesDisplayDictionary[robotId].SetRole(role);
            }
            else
            {
                // Console.WriteLine("UpdateRobotRole : Robot non trouvé");
            }
        }
        private void UpdateDisplayMessage(int robotId, string message)
        {
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                TeamMatesDisplayDictionary[robotId].SetDisplayMessage(message);
            }
            else
            {
                // Console.WriteLine("UpdateDisplayMessage : Robot non trouvé");
            }
        }
        private void UpdatePlayingSide(int robotId, PlayingSide playSide)
        {
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                TeamMatesDisplayDictionary[robotId].SetPlayingSide(playSide);
            }
            else
            {
                // Console.WriteLine("UpdateRobotPlayingSide : Robot non trouvé");
            }
        }
        private void UpdateRobotLocation(int robotId, Location location)
        {
            if (location == null)
                return;
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                TeamMatesDisplayDictionary[robotId].SetLocation(location);
            }
            else
            {
                // Console.WriteLine("UpdateRobotLocation : Robot non trouvé");
            }
        }

        private void UpdateRobotGhostLocation(int robotId, Location location)
        {
            if (location == null)
                return;
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                TeamMatesDisplayDictionary[robotId].SetGhostLocation(location);
            }
            else
            {
                // Console.WriteLine("UpdateRobotGhostLocation : Robot non trouvé");
            }
        }

        public void UpdateBallLocationList(List<Location> ballLocationList)
        {
            if (ballLocationList != null)
            {
                lock (BallDisplayList)
                {
                    BallDisplayList.Clear();
                    foreach (var ballLocation in ballLocationList)
                    {
                        BallDisplayList.Add(new BallDisplay(ballLocation));
                    }
                }
            }
        }

        public void UpdateRobotWaypoint(int robotId, Location waypointLocation)
        {
            if (waypointLocation == null)
                return;
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                // TeamMatesDisplayDictionary[robotId].SetWayPoint(waypointLocation.X, waypointLocation.Y, waypointLocation.Theta);
            }
        }

        public void UpdateRobotDestination(int robotId, Location destinationLocation)
        {
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                TeamMatesDisplayDictionary[robotId].SetDestination(destinationLocation);
            }
        }

        public void UpdateOpponentLocation(int robotId, Location location)
        {
            if (location == null)
                return;
            if (OpponentDisplayDictionary.ContainsKey(robotId))
            {
                OpponentDisplayDictionary[robotId].SetLocation(location);
            }
            else
            {
                //Console.WriteLine("UpdateOpponentsLocation : Robot non trouvé");
            }
        }        

        void InitRoboCupSoccerField()
        {
            double TerrainLowerX = -11;
            double TerrainUpperX = 11;
            double TerrainLowerY = -7;
            double TerrainUpperY = 7;

            int fieldLineWidth = 2;
            PolygonExtended p = new PolygonExtended();
            p.polygon.Points.Add(new System.Windows.Point(-12, -8));
            p.polygon.Points.Add(new System.Windows.Point(12, -8));
            p.polygon.Points.Add(new System.Windows.Point(12, 8));
            p.polygon.Points.Add(new System.Windows.Point(-12, 8));
            p.polygon.Points.Add(new System.Windows.Point(-12, -8));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0x00, 0x00);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0x22, 0x22, 0x22);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.ZoneProtegee, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new System.Windows.Point(11, -7));
            p.polygon.Points.Add(new System.Windows.Point(0, -7));
            p.polygon.Points.Add(new System.Windows.Point(0, 7));
            p.polygon.Points.Add(new System.Windows.Point(11, 7));
            p.polygon.Points.Add(new System.Windows.Point(11, -7));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x66, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.DemiTerrainDroit, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new System.Windows.Point(-11, -7));
            p.polygon.Points.Add(new System.Windows.Point(0, -7));
            p.polygon.Points.Add(new System.Windows.Point(0, 7));
            p.polygon.Points.Add(new System.Windows.Point(-11, 7));
            p.polygon.Points.Add(new System.Windows.Point(-11, -7));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x66, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.DemiTerrainGauche, p);


            p = new PolygonExtended();
            p.polygon.Points.Add(new System.Windows.Point(-11, -1.95));
            p.polygon.Points.Add(new System.Windows.Point(-10.25, -1.95));
            p.polygon.Points.Add(new System.Windows.Point(-10.25, 1.95));
            p.polygon.Points.Add(new System.Windows.Point(-11.00, 1.95));
            p.polygon.Points.Add(new System.Windows.Point(-11.00, -1.95));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.SurfaceButGauche, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new System.Windows.Point(11.00, -1.95));
            p.polygon.Points.Add(new System.Windows.Point(10.25, -1.95));
            p.polygon.Points.Add(new System.Windows.Point(10.25, 1.95));
            p.polygon.Points.Add(new System.Windows.Point(11.00, 1.95));
            p.polygon.Points.Add(new System.Windows.Point(11.00, -1.95));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.SurfaceButDroit, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new System.Windows.Point(11.00, -3.45));
            p.polygon.Points.Add(new System.Windows.Point(8.75, -3.45));
            p.polygon.Points.Add(new System.Windows.Point(8.75, 3.45));
            p.polygon.Points.Add(new System.Windows.Point(11.00, 3.45));
            p.polygon.Points.Add(new System.Windows.Point(11.00, -3.45));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.SurfaceReparationDroit, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new System.Windows.Point(-11.00, -3.45));
            p.polygon.Points.Add(new System.Windows.Point(-8.75, -3.45));
            p.polygon.Points.Add(new System.Windows.Point(-8.75, 3.45));
            p.polygon.Points.Add(new System.Windows.Point(-11.00, 3.45));
            p.polygon.Points.Add(new System.Windows.Point(-11.00, -3.45));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.SurfaceReparationGauche, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new System.Windows.Point(-11.00, -1.20));
            p.polygon.Points.Add(new System.Windows.Point(-11.00, 1.20));
            p.polygon.Points.Add(new System.Windows.Point(-11.50, 1.20));
            p.polygon.Points.Add(new System.Windows.Point(-11.50, -1.20));
            p.polygon.Points.Add(new System.Windows.Point(-11.00, -1.20));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.ButGauche, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new System.Windows.Point(11.00, -1.20));
            p.polygon.Points.Add(new System.Windows.Point(11.00, 1.20));
            p.polygon.Points.Add(new System.Windows.Point(11.50, 1.20));
            p.polygon.Points.Add(new System.Windows.Point(11.50, -1.20));
            p.polygon.Points.Add(new System.Windows.Point(11.00, -1.20));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.ButDroit, p);


            p = new PolygonExtended();
            p.polygon.Points.Add(new System.Windows.Point(-12.00, -8.00));
            p.polygon.Points.Add(new System.Windows.Point(-12.00, -9.00));
            p.polygon.Points.Add(new System.Windows.Point(-4.00, -9.00));
            p.polygon.Points.Add(new System.Windows.Point(-4.00, -8.00));
            p.polygon.Points.Add(new System.Windows.Point(-12.00, -8.00));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0x00, 0x00);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x00, 0xFF);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.ZoneTechniqueGauche, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new System.Windows.Point(+12.00, -8.00));
            p.polygon.Points.Add(new System.Windows.Point(+12.00, -9.00));
            p.polygon.Points.Add(new System.Windows.Point(+4.00, -9.00));
            p.polygon.Points.Add(new System.Windows.Point(+4.00, -8.00));
            p.polygon.Points.Add(new System.Windows.Point(+12.00, -8.00));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0x00, 0x00);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x00, 0xFF);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.ZoneTechniqueDroite, p);

            p = new PolygonExtended();
            int nbSteps = 30;
            for (int i = 0; i < nbSteps + 1; i++)
                p.polygon.Points.Add(new System.Windows.Point(1.0f * Math.Cos((double)i * (2 * Math.PI / nbSteps)), 1.0f * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.RondCentral, p);

            p = new PolygonExtended();
            for (int i = 0; i < (int)(nbSteps / 4) + 1; i++)
                p.polygon.Points.Add(new System.Windows.Point(-11.00 + 0.75 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), -7.0 + 0.75 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.CornerBasGauche, p);

            p = new PolygonExtended();
            for (int i = (int)(nbSteps / 4) + 1; i < (int)(2 * nbSteps / 4) + 1; i++)
                p.polygon.Points.Add(new System.Windows.Point(11 + 0.75 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), -7 + 0.75 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.CornerBasDroite, p);

            p = new PolygonExtended();
            for (int i = (int)(2 * nbSteps / 4); i < (int)(3 * nbSteps / 4) + 1; i++)
                p.polygon.Points.Add(new System.Windows.Point(11 + 0.75 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), 7 + 0.75 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.CornerHautDroite, p);

            p = new PolygonExtended();
            for (int i = (int)(3 * nbSteps / 4) + 1; i < (int)(nbSteps) + 1; i++)
                p.polygon.Points.Add(new System.Windows.Point(-11 + 0.75 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), 7 + 0.75 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.CornerHautGauche, p);

            p = new PolygonExtended();
            for (int i = 0; i < (int)(nbSteps) + 1; i++)
                p.polygon.Points.Add(new System.Windows.Point(-7.4 + 0.075 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), 0.075 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.PtAvantSurfaceGauche, p);

            p = new PolygonExtended();
            for (int i = 0; i < (int)(nbSteps) + 1; i++)
                p.polygon.Points.Add(new System.Windows.Point(7.4 + 0.075 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), 0.075 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonSeries.AddOrUpdatePolygonExtended((int)Terrain.PtAvantSurfaceDroit, p);

        }

        private void sciChart_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void heatmapSeries_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                // Perform the hit test relative to the GridLinesPanel
                var hitTestPoint = e.GetPosition(sciChart.GridLinesPanel as UIElement);
                
            }
        }
    }    
}

