
using Constants;
using EventArgsLibrary;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Model.DataSeries.Heatmap2DArrayDataSeries;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Core.Utility.Mouse;
using SciChart.Drawing.VisualXcceleratorRasterizer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Utilities;
using WorldMap;

namespace WpfWorldMapDisplay
{
    public class BindingClass
    {
        private string imagePath;

        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }

        private double x1, x2, y1, y2;
        public double X1 { get { return x1; } set { x1 = value; } }
        public double X2 { get { return x2; } set { x2 = value; } }
        public double Y1 { get { return y1; } set { y1 = value; } }
        public double Y2 { get { return y2; } set { y2 = value; } }
    }

    public enum LocalWorldMapDisplayType
    {
        StrategyMap,
        WayPointMap,
    }

    /// <summary>
    /// Logique d'interaction pour ExtendedHeatMap.xaml
    /// </summary>    /// 
    public partial class LocalWorldMapDisplay : UserControl
    {
        ////A enlever après debug
        //PolygonRenderableSeries PolygonTerrainSeries = new PolygonRenderableSeries();
        //XyScatterRenderableSeries LidarPoints = new XyScatterRenderableSeries();
        //XyScatterRenderableSeries LidarProcessedPoints = new XyScatterRenderableSeries();
        //PolygonRenderableSeries PolygonSeries = new PolygonRenderableSeries();
        //XyScatterRenderableSeries BallPoints = new XyScatterRenderableSeries();
        //PolygonRenderableSeries BallPolygon = new PolygonRenderableSeries();
        //PolygonRenderableSeries ObjectsPolygonSeries = new PolygonRenderableSeries();
        //XyScatterRenderableSeries ObstaclePoints = new XyScatterRenderableSeries();

        //

        LocalWorldMapDisplayType lwmdType = LocalWorldMapDisplayType.StrategyMap; //Par défaut

        GameMode competition;

        Random random = new Random();

        public bool IsExtended = false;

        double LengthGameArea = 0;
        double WidthGameArea = 0;
        double LengthDisplayArea = 0;
        double WidthDisplayArea = 0;

        //Liste des robots à afficher
        Dictionary<int, RobotDisplay> TeamMatesDisplayDictionary = new Dictionary<int, RobotDisplay>();

        ConcurrentDictionary<int, RobotDisplay> OpponentDisplayDictionary = new ConcurrentDictionary<int, RobotDisplay>();
        //Dictionary<int, RobotDisplay> OpponentDisplayDictionary = new Dictionary<int, RobotDisplay>();
        List<PolygonExtended> ObjectDisplayList = new List<PolygonExtended>();

        //Liste des balles vues par le robot à afficher
        List<BallDisplay> BallDisplayList = new List<BallDisplay>();

        //Liste des obstacles vus par le robot à afficher
        List<ObstacleDisplay> ObstacleDisplayList = new List<ObstacleDisplay>();

        string typeTerrain = "RoboCup";

        BindingClass imageBinding = new BindingClass();
        Thread tDisplayMap;
        AutoResetEvent waitForDisplayAuthorization = new AutoResetEvent(false);

        public LocalWorldMapDisplay()
        {
            InitializeComponent();
            this.DataContext = imageBinding;

            tDisplayMap = new Thread(DisplayWorldMap);
            tDisplayMap.ApartmentState = ApartmentState.STA;
            tDisplayMap.Start();
            //sciChart.ChartModifier = new ModifierGroup(new ZoomExtentsModifier());

        }

        /// <summary>
        /// Définit l'image en fond de carte
        /// </summary>
        /// <param name="imagePath">Chemin de l'image</param>
        public void SetFieldImageBackGround(string imagePath)
        {
            imageBinding.ImagePath = imagePath;
            imageBinding.X1 = -LengthGameArea / 2;
            imageBinding.X2 = +LengthGameArea / 2;
            imageBinding.Y1 = -WidthGameArea / 2;
            imageBinding.Y2 = +WidthGameArea / 2;
        }

        public void UpdateRobotLocation(int robotId, Location location)
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

        /// <summary>
        /// Initialise la Local World Map
        /// </summary>
        /// <param name="competition">Spécifie le type de compétition, le réglage des dimensions est automatique</param>
        /// <param name="type">Spécifie si on a une LWM affichant la stratégie ou les waypoints</param>
        /// <param name="imagePath">Chemin de l'image de fond</param>
        public void Init(GameMode competition, LocalWorldMapDisplayType type)
        {
            this.competition = competition;
            lwmdType = type;
            if (lwmdType == LocalWorldMapDisplayType.StrategyMap)
                LocalWorldMapTitle.Text = "LWM Strategy";
            if (lwmdType == LocalWorldMapDisplayType.WayPointMap)
                LocalWorldMapTitle.Text = "LWM Waypoint";

            switch (competition)
            {
                case GameMode.Cachan:
                    LengthDisplayArea = 10;
                    WidthDisplayArea = 6;
                    LengthGameArea = 8;
                    WidthGameArea = 4;
                    InitCachanField();
                    break;
                case GameMode.Eurobot:
                    LengthDisplayArea = 3.4;
                    WidthDisplayArea = 2.4;
                    LengthGameArea = 3.0;
                    WidthGameArea = 2.0;
                    InitEurobotField();
                    break;
                case GameMode.RoboCup:
                    LengthDisplayArea = 30;
                    WidthDisplayArea = 18;
                    LengthGameArea = 22;
                    WidthGameArea = 14;
                    InitSoccerField();
                    break;
                default:
                    LengthDisplayArea = 30;
                    WidthDisplayArea = 18;
                    LengthGameArea = 22;
                    WidthGameArea = 14;
                    InitSoccerField();
                    break;
            }

            this.sciChartSurface.XAxis.VisibleRange.SetMinMax(-LengthDisplayArea / 2, LengthDisplayArea / 2);
            this.sciChartSurface.YAxis.VisibleRange.SetMinMax(-WidthDisplayArea / 2, WidthDisplayArea / 2);

            if (sciChartSurface.RenderSurface.GetType() == typeof(VisualXcceleratorRenderSurface))
            {
                Console.WriteLine("Scichart LocalWorldMapDisplay : DirectX enabled");
            }
        }

        public void DisplayWorldMap()
        {
            while (true)
            {
                waitForDisplayAuthorization.WaitOne();

                Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    UpdateBallPolygons();
                    UpdateObstaclesPolygons();
                    if (TeamMatesDisplayDictionary.Count == 1) //Cas d'un affichage de robot unique (localWorldMap)
                    {
                        AnnotRobotRole.Text = TeamMatesDisplayDictionary.First().Value.robotRole.ToString();
                        //DrawLidar(); 
                        DrawHeatMap(TeamMatesDisplayDictionary.First().Key);
                    }

                    if (competition == GameMode.RoboCup)
                        PolygonTerrainSeries.RedrawAll();
                    PolygonSeries.RedrawAll();
                    SegmentSeries.RedrawAll();
                    LidarPtExtendedSeries.RedrawAll();
                    ObjectsPolygonSeries.RedrawAll();
                    BallPolygon.RedrawAll();
                    DrawTeam();
                }));
                Thread.Sleep(10);
            }
        }

        public void UpdateWorldMapDisplay()
        {
            waitForDisplayAuthorization.Set();
        }

        public void InitTeamMate(int robotId, GameMode gMode, string playerName)
        {
            switch (gMode)
            {
                case GameMode.Cachan:
                    {
                        PolygonExtended robotShape = new PolygonExtended();
                        robotShape.polygon.Points.Add(new System.Windows.Point(-0.14, -0.18));
                        robotShape.polygon.Points.Add(new System.Windows.Point(0.14, -0.18));
                        robotShape.polygon.Points.Add(new System.Windows.Point(0.10, 0));
                        robotShape.polygon.Points.Add(new System.Windows.Point(0.14, 0.18));
                        robotShape.polygon.Points.Add(new System.Windows.Point(-0.14, 0.18));
                        robotShape.polygon.Points.Add(new System.Windows.Point(-0.14, -0.18));
                        robotShape.borderColor = System.Drawing.Color.Blue;
                        robotShape.backgroundColor = System.Drawing.Color.Red;

                        PolygonExtended ghostShape = new PolygonExtended();
                        ghostShape.polygon.Points.Add(new Point(-0.16, -0.2));
                        ghostShape.polygon.Points.Add(new Point(0.16, -0.2));
                        ghostShape.polygon.Points.Add(new Point(0.12, 0));
                        ghostShape.polygon.Points.Add(new Point(0.16, 0.2));
                        ghostShape.polygon.Points.Add(new Point(-0.16, 0.2));
                        ghostShape.polygon.Points.Add(new Point(-0.16, -0.2));
                        ghostShape.backgroundColor = System.Drawing.Color.FromArgb(20, 0, 255, 0);
                        ghostShape.borderColor = System.Drawing.Color.Black;

                        RobotDisplay rd = new RobotDisplay(robotShape, ghostShape, playerName);
                        rd.SetLocation(new Location(0, 0, 0, 0, 0, 0));
                        TeamMatesDisplayDictionary.Add(robotId, rd);
                    }
                    break;
                case GameMode.Eurobot:
                    {
                        PolygonExtended robotShape = new PolygonExtended();
                        robotShape.polygon.Points.Add(new System.Windows.Point(-0.12, -0.12));
                        robotShape.polygon.Points.Add(new System.Windows.Point(0.12, -0.12));
                        robotShape.polygon.Points.Add(new System.Windows.Point(0.02, 0));
                        robotShape.polygon.Points.Add(new System.Windows.Point(0.12, 0.12));
                        robotShape.polygon.Points.Add(new System.Windows.Point(-0.12, 0.12));
                        robotShape.polygon.Points.Add(new System.Windows.Point(-0.12, -0.12));
                        robotShape.borderColor = System.Drawing.Color.Blue;
                        robotShape.backgroundColor = System.Drawing.Color.DarkRed;

                        PolygonExtended ghostShape = new PolygonExtended();
                        ghostShape.polygon.Points.Add(new Point(-0.14, -0.14));
                        ghostShape.polygon.Points.Add(new Point(0.14, -0.14));
                        ghostShape.polygon.Points.Add(new Point(0.14, 0.02));
                        ghostShape.polygon.Points.Add(new Point(0.14, 0.14));
                        ghostShape.polygon.Points.Add(new Point(-0.14, 0.14));
                        ghostShape.polygon.Points.Add(new Point(-0.14, -0.14));
                        ghostShape.backgroundColor = System.Drawing.Color.FromArgb(20, 0, 255, 0);
                        ghostShape.borderColor = System.Drawing.Color.Black;

                        RobotDisplay rd = new RobotDisplay(robotShape, ghostShape, playerName);
                        rd.SetLocation(new Location(0, 0, 0, 0, 0, 0));
                        TeamMatesDisplayDictionary.Add(robotId, rd);
                    }
                    break;
                default:
                    {
                        PolygonExtended robotShape = new PolygonExtended();
                        robotShape.polygon.Points.Add(new System.Windows.Point(-0.25, -0.25));
                        robotShape.polygon.Points.Add(new System.Windows.Point(0.25, -0.25));
                        robotShape.polygon.Points.Add(new System.Windows.Point(0.2, 0));
                        robotShape.polygon.Points.Add(new System.Windows.Point(0.25, 0.25));
                        robotShape.polygon.Points.Add(new System.Windows.Point(-0.25, 0.25));
                        robotShape.polygon.Points.Add(new System.Windows.Point(-0.25, -0.25));
                        robotShape.borderColor = System.Drawing.Color.Blue;
                        robotShape.backgroundColor = System.Drawing.Color.Red;

                        PolygonExtended ghostShape = new PolygonExtended();
                        ghostShape.polygon.Points.Add(new Point(-0.27, -0.27));
                        ghostShape.polygon.Points.Add(new Point(0.27, -0.27));
                        ghostShape.polygon.Points.Add(new Point(0.22, 0.02));
                        ghostShape.polygon.Points.Add(new Point(0.27, 0.27));
                        ghostShape.polygon.Points.Add(new Point(-0.27, 0.27));
                        ghostShape.polygon.Points.Add(new Point(-0.27, -0.27));
                        ghostShape.backgroundColor = System.Drawing.Color.FromArgb(20, 0, 255, 0);
                        ghostShape.borderColor = System.Drawing.Color.Black;

                        RobotDisplay rd = new RobotDisplay(robotShape, ghostShape, playerName);
                        rd.SetLocation(new Location(0, 0, 0, 0, 0, 0));
                        TeamMatesDisplayDictionary.Add(robotId, rd);
                    }
                    break;
            }

            LocalWorldMapTitle.Text = "LWM " + playerName.ToString();
        }

        public void AddOrUpdateTextAnnotation(string annotationName, string annotationText, double posX, double posY)
        {
            var textAnnotationList = sciChartSurface.Annotations.Where(annotation => annotation.GetType().Name == "TextAnnotation").ToList();
            var annot = textAnnotationList.FirstOrDefault(c => ((TextAnnotation)c).Name == annotationName);
            if (annot == null)
            {
                TextAnnotation textAnnot = new TextAnnotation();
                textAnnot.Text = annotationText;
                textAnnot.X1 = posX;
                textAnnot.Y1 = posY;
                sciChartSurface.Annotations.Add(textAnnot);
            }
        }


        public void UpdateLocalWorldMap(LocalWorldMap localWorldMap)
        {
            int robotId = localWorldMap.RobotId + localWorldMap.TeamId;
            UpdateRobotLocation(robotId, localWorldMap.RobotLocation);
            //UpdateRobotRole(robotId, localWorldMap.Robot);
            UpdatePlayingSide(robotId, localWorldMap.playingSide);
            UpdateRobotGhostLocation(robotId, localWorldMap.RobotGhostLocation);
            UpdateRobotDestination(robotId, localWorldMap.DestinationLocation);
            UpdateRobotWaypoint(robotId, localWorldMap.WaypointLocations);

            if (lwmdType == LocalWorldMapDisplayType.StrategyMap)
            {
                //if (localWorldMap.heatMapStrategy != null)
                //    UpdateHeatMap(robotId, localWorldMap.heatMapStrategy.BaseHeatMapData);
            }
            else if (lwmdType == LocalWorldMapDisplayType.WayPointMap)
            {
                //if (localWorldMap.heatMapWaypoint != null)
                //    UpdateHeatMap(robotId, localWorldMap.heatMapWaypoint.BaseHeatMapData);
            }
            //Affichage du lidar uniquement dans la strategy map
            if (lwmdType == LocalWorldMapDisplayType.StrategyMap)
            {
                UpdateLidarMap(robotId, localWorldMap.LidarMapRaw, LidarDataType.RawData);
                UpdateLidarMap(robotId, localWorldMap.LidarMapProcessed, LidarDataType.ProcessedData1);

                UpdateLidarSegments(robotId, localWorldMap.LidarSegment);
            }
            //UpdateLidarObjects(robotId, localWorldMap.lidarObjectList);

            /// Demande d'affichage de la World Map reçue
            UpdateWorldMapDisplay();
        }

        private void DrawHeatMap(int robotId)
        {
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                UniformHeatmapDataSeries<double, double, double> heatmapDataSeries = null;
                if (lwmdType == LocalWorldMapDisplayType.StrategyMap)
                {
                    if (TeamMatesDisplayDictionary[robotId].heatMapStrategy == null)
                        return;
                    //heatmapSeries.DataSeries = new UniformHeatmapDataSeries<double, double, double>(data, startX, stepX, startY, stepY);
                    double xStep = (LengthGameArea) / (TeamMatesDisplayDictionary[robotId].heatMapStrategy.GetUpperBound(1));
                    double yStep = (WidthGameArea) / (TeamMatesDisplayDictionary[robotId].heatMapStrategy.GetUpperBound(0));
                    heatmapDataSeries = new UniformHeatmapDataSeries<double, double, double>(TeamMatesDisplayDictionary[robotId].heatMapStrategy, -LengthGameArea / 2 - xStep / 2, xStep, -WidthGameArea / 2 - yStep / 2, yStep);
                }
                else
                {
                    if (TeamMatesDisplayDictionary[robotId].heatMapWaypoint == null)
                        return;
                    //heatmapSeries.DataSeries = new UniformHeatmapDataSeries<double, double, double>(data, startX, stepX, startY, stepY);
                    double xStep = (LengthGameArea) / (TeamMatesDisplayDictionary[robotId].heatMapWaypoint.GetUpperBound(1));
                    double yStep = (WidthGameArea) / (TeamMatesDisplayDictionary[robotId].heatMapWaypoint.GetUpperBound(0));
                    heatmapDataSeries = new UniformHeatmapDataSeries<double, double, double>(TeamMatesDisplayDictionary[robotId].heatMapWaypoint, -LengthGameArea / 2 - xStep / 2, xStep, -WidthGameArea / 2 - yStep / 2, yStep);
                }

                // Apply the dataseries to the heatmap
                if (heatmapDataSeries != null)
                {
                    heatmapSeries.DataSeries = heatmapDataSeries;
                    heatmapDataSeries.InvalidateParentSurface(RangeMode.None);
                }
            }
        }

        public void UpdateBallPolygons()
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
        public void UpdateObstaclesPolygons()
        {
            lock (ObstacleDisplayList)
            {
                int indexObstacle = 0;
                //ObstaclePolygons = new PolygonRenderableSeries();
                var obstaclesPointsList = ObstacleDisplayList.Select(x => new PointD(x.location.X, x.location.Y));
                var obstaclesPoints = GetXYDataSeriesFromPoints(obstaclesPointsList.ToList());
                ObstaclePoints.DataSeries = obstaclesPoints;

                //foreach (var obstacle in ObstacleDisplayList)
                //{
                //    //Affichage des obstacles
                //    ObstaclePolygons.AddOrUpdatePolygonExtended((int)ObstacleId.Obstacle + indexObstacle, obstacle.GetObstaclePolygon());
                //    //ObstaclePolygons.AddOrUpdatePolygonExtended((int)ObstacleId.Obstacle + indexBall + (int)Caracteristique.Speed, obstacle.GetObstacleSpeedArrow());
                //    indexObstacle++;
                //}
            }
        }

        public void DrawTeam()
        {
            XyDataSeries<double, double> lidarPts = new XyDataSeries<double, double>();
            XyDataSeries<double, double>[] lidarProcessedPts = new XyDataSeries<double, double>[3];
            //foreach(var l in lidarProcessedPts)
            //{
            //    l = new XyDataSeries<double, double>();
            //}

            //XyDataSeries<double, double> lidarProcessedPts2 = new XyDataSeries<double, double>();
            //XyDataSeries<double, double> lidarProcessedPts3 = new XyDataSeries<double, double>();
            ObjectsPolygonSeries.Clear();
            SegmentSeries.Clear();
            LidarPtExtendedSeries.Clear();

            foreach (var r in TeamMatesDisplayDictionary)
            {
                //Affichage des robots
                PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.Ghost, TeamMatesDisplayDictionary[r.Key].GetRobotGhostPolygon());
                PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.Speed, TeamMatesDisplayDictionary[r.Key].GetRobotSpeedArrow());
                PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.Destination, TeamMatesDisplayDictionary[r.Key].GetRobotDestinationArrow());
                PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.WayPoint, TeamMatesDisplayDictionary[r.Key].GetRobotWaypointArrow());
                //On trace le robot en dernier pour l'avoir en couche de dessus
                PolygonSeries.AddOrUpdatePolygonExtended(r.Key, TeamMatesDisplayDictionary[r.Key].GetRobotPolygon());

                //Rendering des points Lidar
                lidarPts.AcceptsUnsortedData = true;
                var lidarData = TeamMatesDisplayDictionary[r.Key].GetRobotLidarPoints(LidarDataType.RawData);
                lidarPts.Append(lidarData.XValues, lidarData.YValues);

                for (int i = 0; i < lidarProcessedPts.Length; i++)
                {
                    lidarProcessedPts[i] = new XyDataSeries<double, double>();
                    lidarProcessedPts[i].AcceptsUnsortedData = true;
                    switch (i)
                    {
                        case 0:
                            {
                                var lidarProcessedData = TeamMatesDisplayDictionary[r.Key].GetRobotLidarPoints(LidarDataType.ProcessedData1);
                                lidarProcessedPts[i].Append(lidarProcessedData.XValues, lidarProcessedData.YValues);
                            }
                            break;
                        case 1:
                            {
                                var lidarProcessedData = TeamMatesDisplayDictionary[r.Key].GetRobotLidarPoints(LidarDataType.ProcessedData2);
                                lidarProcessedPts[i].Append(lidarProcessedData.XValues, lidarProcessedData.YValues);
                            }
                            break;
                        case 2:
                            {
                                var lidarProcessedData = TeamMatesDisplayDictionary[r.Key].GetRobotLidarPoints(LidarDataType.ProcessedData3);
                                lidarProcessedPts[i].Append(lidarProcessedData.XValues, lidarProcessedData.YValues);
                            }
                            break;
                    }
                }

                //Rendering des objets Lidar
                
                foreach (var segment in TeamMatesDisplayDictionary[r.Key].GetRobotLidarSegments())
                {
                    SegmentSeries.AddSegmentExtended(0, segment);
                }

                foreach (var pt in TeamMatesDisplayDictionary[r.Key].GetRobotLidarExtendedPoints())
                {
                    LidarPtExtendedSeries.AddPtExtended(pt);
                }

                foreach (var polygonObject in TeamMatesDisplayDictionary[r.Key].GetRobotLidarObjects())
                    ObjectsPolygonSeries.AddOrUpdatePolygonExtended(ObjectsPolygonSeries.Count(), polygonObject);
            }

            foreach (var r in OpponentDisplayDictionary)
            {
                //Affichage des robots
                PolygonSeries.AddOrUpdatePolygonExtended(r.Key, OpponentDisplayDictionary[r.Key].GetRobotPolygon());
                //PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.Speed, OpponentDisplayDictionary[r.Key].GetRobotSpeedArrow());
                //PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.Destination, TeamMatesDictionary[r.Key].GetRobotDestinationArrow());
                //PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.WayPoint, TeamMatesDictionary[r.Key].GetRobotWaypointArrow());
            }
            //Affichage des points lidar
            LidarPoints.DataSeries = lidarPts;
            LidarProcessedPoints1.DataSeries = lidarProcessedPts[0];
            LidarProcessedPoints2.DataSeries = lidarProcessedPts[1];
            LidarProcessedPoints3.DataSeries = lidarProcessedPts[2];
        }

        public void DrawLidar()
        {
            XyDataSeries<double, double> lidarPts = new XyDataSeries<double, double>();
            XyDataSeries<double, double> lidarProcessedPts1 = new XyDataSeries<double, double>();
            XyDataSeries<double, double> lidarProcessedPts2 = new XyDataSeries<double, double>();
            XyDataSeries<double, double> lidarProcessedPts3 = new XyDataSeries<double, double>();
            foreach (var r in TeamMatesDisplayDictionary)
            {
                ////Affichage des robots
                //PolygonSeries.AddOrUpdatePolygonExtended(r.Key, TeamMatesDisplayDictionary[r.Key].GetRobotPolygon());
                //PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.Speed, TeamMatesDisplayDictionary[r.Key].GetRobotSpeedArrow());
                //PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.Destination, TeamMatesDisplayDictionary[r.Key].GetRobotDestinationArrow());
                //PolygonSeries.AddOrUpdatePolygonExtended(r.Key + (int)Caracteristique.WayPoint, TeamMatesDisplayDictionary[r.Key].GetRobotWaypointArrow());

                //Rendering des points Lidar
                lidarPts.AcceptsUnsortedData = true;
                var lidarData = TeamMatesDisplayDictionary[r.Key].GetRobotLidarPoints(LidarDataType.RawData);
                lidarPts.Append(lidarData.XValues, lidarData.YValues);
                LidarPoints.DataSeries = lidarPts;

                lidarProcessedPts1.AcceptsUnsortedData = true;
                var lidarProcessedData1 = TeamMatesDisplayDictionary[r.Key].GetRobotLidarPoints(LidarDataType.ProcessedData1);
                lidarProcessedPts1.Append(lidarProcessedData1.XValues, lidarProcessedData1.YValues);

                lidarProcessedPts2.AcceptsUnsortedData = true;
                var lidarProcessedData2 = TeamMatesDisplayDictionary[r.Key].GetRobotLidarPoints(LidarDataType.ProcessedData2);
                lidarProcessedPts2.Append(lidarProcessedData2.XValues, lidarProcessedData2.YValues);

                lidarProcessedPts3.AcceptsUnsortedData = true;
                var lidarProcessedData3 = TeamMatesDisplayDictionary[r.Key].GetRobotLidarPoints(LidarDataType.ProcessedData3);
                lidarProcessedPts3.Append(lidarProcessedData3.XValues, lidarProcessedData3.YValues);

                LidarProcessedPoints1.DataSeries = lidarProcessedPts1;
                LidarProcessedPoints2.DataSeries = lidarProcessedPts2;
                LidarProcessedPoints3.DataSeries = lidarProcessedPts3;
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
                //Console.WriteLine("UpdateRobotRole : Robot non trouvé");
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
                //Console.WriteLine("UpdatePlayingSide : Robot non trouvé");
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
                //Console.WriteLine("UpdateRobotGhostLocation : Robot non trouvé");
            }
        }

        private void UpdateHeatMap(int robotId, double[,] data)
        {
            if (data == null)
                return;
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                if (lwmdType == LocalWorldMapDisplayType.StrategyMap)
                    TeamMatesDisplayDictionary[robotId].SetHeatMapStrategy(data);
                if (lwmdType == LocalWorldMapDisplayType.WayPointMap)
                    TeamMatesDisplayDictionary[robotId].SetHeatMapWaypoint(data);
            }
        }

        private void UpdateLidarMap(int robotId, List<PointDExtended> lidarMap, LidarDataType type)
        {
            if (lidarMap == null)
                return;
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                TeamMatesDisplayDictionary[robotId].SetLidarMap(lidarMap, type);
            }
        }

        //private void UpdateLidarProcessedMap1(int robotId, List<PointD> lidarMapProcessed)
        //{
        //    if (lidarMapProcessed == null)
        //        return;
        //    if (TeamMatesDisplayDictionary.ContainsKey(robotId))
        //    {
        //        TeamMatesDisplayDictionary[robotId].SetLidarProcessedMap1(lidarMapProcessed);
        //    }
        //}

        //private void UpdateLidarProcessedMap2(int robotId, List<PointD> lidarMapProcessed)
        //{
        //    if (lidarMapProcessed == null)
        //        return;
        //    if (TeamMatesDisplayDictionary.ContainsKey(robotId))
        //    {
        //        TeamMatesDisplayDictionary[robotId].SetLidarProcessedMap2(lidarMapProcessed);
        //    }
        //}

        //private void UpdateLidarProcessedMap3(int robotId, List<PointD> lidarMapProcessed)
        //{
        //    if (lidarMapProcessed == null)
        //        return;
        //    if (TeamMatesDisplayDictionary.ContainsKey(robotId))
        //    {
        //        TeamMatesDisplayDictionary[robotId].SetLidarProcessedMap3(lidarMapProcessed);            }
        //}

        private void UpdateLidarSegments(int robotId, List<SegmentExtended> lidarSegmentList)
        {
            if (lidarSegmentList == null)
                return;
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                TeamMatesDisplayDictionary[robotId].SetLidarSegment(lidarSegmentList);
            }
        }

        //private void UpdateLidarObjects(int robotId, List<PolarPointListExtended> lidarObjectList)
        //{
        //    if (lidarObjectList == null)
        //        return;
        //    if (TeamMatesDisplayDictionary.ContainsKey(robotId))
        //    {
        //        TeamMatesDisplayDictionary[robotId].SetLidarObjectList(lidarObjectList);
        //    }
        //}

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

        public void UpdateObstacleList(List<LocationExtended> obstacleList)
        {
            if (obstacleList != null)
            {
                lock (ObstacleDisplayList)
                {
                    ObstacleDisplayList.Clear();
                    foreach (var obstacleLocation in obstacleList.ToList())
                    {
                        ObstacleDisplayList.Add(new ObstacleDisplay(obstacleLocation));
                    }
                }
            }
        }

        public void UpdateRobotWaypoint(int robotId, List<Location> waypoints)
        {
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                TeamMatesDisplayDictionary[robotId].SetWayPoint(waypoints);
            }
        }



        public void UpdateRobotDestination(int robotId, Location destinationLocation)
        {
            if (destinationLocation == null)
                return;
            if (TeamMatesDisplayDictionary.ContainsKey(robotId))
            {
                TeamMatesDisplayDictionary[robotId].SetDestination(destinationLocation);
            }
        }

        public void UpdateOpponentsLocation(int robotId, Location location)
        {
            if (location == null)
                return;
            if (OpponentDisplayDictionary.ContainsKey(robotId))
            {
                OpponentDisplayDictionary[robotId].SetLocation(location);
            }
            else
            {
                Console.WriteLine("UpdateOpponentsLocation : Robot non trouvé");
            }
        }


        void InitSoccerField()
        {
            int fieldLineWidth = 2;
            PolygonExtended p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-12, -8));
            p.polygon.Points.Add(new Point(12, -8));
            p.polygon.Points.Add(new Point(12, 8));
            p.polygon.Points.Add(new Point(-12, 8));
            p.polygon.Points.Add(new Point(-12, -8));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0x00, 0x00);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0x22, 0x22, 0x22);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.ZoneProtegee, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(11, -7));
            p.polygon.Points.Add(new Point(0, -7));
            p.polygon.Points.Add(new Point(0, 7));
            p.polygon.Points.Add(new Point(11, 7));
            p.polygon.Points.Add(new Point(11, -7));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x66, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.DemiTerrainDroit, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-11, -7));
            p.polygon.Points.Add(new Point(0, -7));
            p.polygon.Points.Add(new Point(0, 7));
            p.polygon.Points.Add(new Point(-11, 7));
            p.polygon.Points.Add(new Point(-11, -7));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x66, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.DemiTerrainGauche, p);


            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-11, -1.95));
            p.polygon.Points.Add(new Point(-10.25, -1.95));
            p.polygon.Points.Add(new Point(-10.25, 1.95));
            p.polygon.Points.Add(new Point(-11.00, 1.95));
            p.polygon.Points.Add(new Point(-11.00, -1.95));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.SurfaceButGauche, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(11.00, -1.95));
            p.polygon.Points.Add(new Point(10.25, -1.95));
            p.polygon.Points.Add(new Point(10.25, 1.95));
            p.polygon.Points.Add(new Point(11.00, 1.95));
            p.polygon.Points.Add(new Point(11.00, -1.95));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.SurfaceButDroit, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(11.00, -3.45));
            p.polygon.Points.Add(new Point(8.75, -3.45));
            p.polygon.Points.Add(new Point(8.75, 3.45));
            p.polygon.Points.Add(new Point(11.00, 3.45));
            p.polygon.Points.Add(new Point(11.00, -3.45));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.SurfaceReparationDroit, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-11.00, -3.45));
            p.polygon.Points.Add(new Point(-8.75, -3.45));
            p.polygon.Points.Add(new Point(-8.75, 3.45));
            p.polygon.Points.Add(new Point(-11.00, 3.45));
            p.polygon.Points.Add(new Point(-11.00, -3.45));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.SurfaceReparationGauche, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-11.00, -1.20));
            p.polygon.Points.Add(new Point(-11.00, 1.20));
            p.polygon.Points.Add(new Point(-11.50, 1.20));
            p.polygon.Points.Add(new Point(-11.50, -1.20));
            p.polygon.Points.Add(new Point(-11.00, -1.20));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.ButGauche, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(11.00, -1.20));
            p.polygon.Points.Add(new Point(11.00, 1.20));
            p.polygon.Points.Add(new Point(11.50, 1.20));
            p.polygon.Points.Add(new Point(11.50, -1.20));
            p.polygon.Points.Add(new Point(11.00, -1.20));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.ButDroit, p);


            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-12.00, -8.00));
            p.polygon.Points.Add(new Point(-12.00, -9.00));
            p.polygon.Points.Add(new Point(-4.00, -9.00));
            p.polygon.Points.Add(new Point(-4.00, -8.00));
            p.polygon.Points.Add(new Point(-12.00, -8.00));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0x00, 0x00);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x00, 0xFF);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.ZoneTechniqueGauche, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(+12.00, -8.00));
            p.polygon.Points.Add(new Point(+12.00, -9.00));
            p.polygon.Points.Add(new Point(+4.00, -9.00));
            p.polygon.Points.Add(new Point(+4.00, -8.00));
            p.polygon.Points.Add(new Point(+12.00, -8.00));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0x00, 0x00);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x00, 0xFF);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.ZoneTechniqueDroite, p);

            p = new PolygonExtended();
            int nbSteps = 30;
            for (int i = 0; i < nbSteps + 1; i++)
                p.polygon.Points.Add(new Point(1.0f * Math.Cos((double)i * (2 * Math.PI / nbSteps)), 1.0f * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.RondCentral, p);

            p = new PolygonExtended();
            for (int i = 0; i < (int)(nbSteps / 4) + 1; i++)
                p.polygon.Points.Add(new Point(-11.00 + 0.75 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), -7.0 + 0.75 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.CornerBasGauche, p);

            p = new PolygonExtended();
            for (int i = (int)(nbSteps / 4) + 1; i < (int)(2 * nbSteps / 4) + 1; i++)
                p.polygon.Points.Add(new Point(11 + 0.75 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), -7 + 0.75 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.CornerBasDroite, p);

            p = new PolygonExtended();
            for (int i = (int)(2 * nbSteps / 4); i < (int)(3 * nbSteps / 4) + 1; i++)
                p.polygon.Points.Add(new Point(11 + 0.75 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), 7 + 0.75 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.CornerHautDroite, p);

            p = new PolygonExtended();
            for (int i = (int)(3 * nbSteps / 4) + 1; i < (int)(nbSteps) + 1; i++)
                p.polygon.Points.Add(new Point(-11 + 0.75 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), 7 + 0.75 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.CornerHautGauche, p);

            p = new PolygonExtended();
            for (int i = 0; i < (int)(nbSteps) + 1; i++)
                p.polygon.Points.Add(new Point(-7.4 + 0.075 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), 0.075 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.PtAvantSurfaceGauche, p);

            p = new PolygonExtended();
            for (int i = 0; i < (int)(nbSteps) + 1; i++)
                p.polygon.Points.Add(new Point(7.4 + 0.075 * Math.Cos((double)i * (2 * Math.PI / nbSteps)), 0.075 * Math.Sin((double)i * (2 * Math.PI / nbSteps))));
            p.borderWidth = fieldLineWidth;
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0xFF, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.PtAvantSurfaceDroit, p);

        }
        void InitEurobotField()
        {
            double TerrainLowerX = -LengthGameArea / 2 - 0.2;
            double TerrainUpperX = LengthGameArea / 2 + 0.2;
            double TerrainLowerY = -WidthGameArea / 2 - 0.2;
            double TerrainUpperY = WidthGameArea / 2 + 0.2;

            int fieldLineWidth = 1;
            PolygonExtended p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-1.5, -1));
            p.polygon.Points.Add(new Point(1.5, -1));
            p.polygon.Points.Add(new Point(1.5, 1));
            p.polygon.Points.Add(new Point(-1.5, 1));
            p.polygon.Points.Add(new Point(-1.5, -1));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 46, 49, 146);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.TerrainComplet, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-1.5 - 0.1, 1));
            p.polygon.Points.Add(new Point(-1.5, 1));
            p.polygon.Points.Add(new Point(-1.5, 1 - 0.1));
            p.polygon.Points.Add(new Point(-1.5 - 0.1, 1 - 0.1));
            p.polygon.Points.Add(new Point(-1.5 - 0.1, 1));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0xA0, 0xA0, 0xA0);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.BaliseGaucheHaut, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(+1.5, -0.1));
            p.polygon.Points.Add(new Point(+1.5 + 0.1, -0.1));
            p.polygon.Points.Add(new Point(+1.5 + 0.1, 0.1));
            p.polygon.Points.Add(new Point(+1.5, 0.1));
            p.polygon.Points.Add(new Point(+1.5, -0.1));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0xA0, 0xA0, 0xA0);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.BaliseGaucheCentre, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-1.5 - 0.1, -1));
            p.polygon.Points.Add(new Point(-1.5, -1));
            p.polygon.Points.Add(new Point(-1.5, -1 + 0.1));
            p.polygon.Points.Add(new Point(-1.5 - 0.1, -1 + 0.1));
            p.polygon.Points.Add(new Point(-1.5 - 0.1, -1));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0xA0, 0xA0, 0xA0);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.BaliseGaucheBas, p);

        }

        void InitCachanField()
        {
            double TerrainLowerX = -LengthGameArea / 2;
            double TerrainUpperX = LengthGameArea / 2;
            double TerrainLowerY = -WidthGameArea / 2;
            double TerrainUpperY = WidthGameArea / 2;

            int fieldLineWidth = 1;
            PolygonExtended p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-4, -2));
            p.polygon.Points.Add(new Point(4, -2));
            p.polygon.Points.Add(new Point(4, 2));
            p.polygon.Points.Add(new Point(-4, 2));
            p.polygon.Points.Add(new Point(-4, -2));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 46, 49, 146);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.TerrainComplet, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-4, 1.5));
            p.polygon.Points.Add(new Point(-0.03, 1.5));
            p.polygon.Points.Add(new Point(-2.15, 1.5));
            p.polygon.Points.Add(new Point(-2.15, 0));
            p.polygon.Points.Add(new Point(-0.03, 0));
            p.polygon.Points.Add(new Point(-2.15, 0));
            p.polygon.Points.Add(new Point(-2.15, -1.5));
            p.polygon.Points.Add(new Point(-0.03, -1.5));
            p.polygon.Points.Add(new Point(-4, -1.5));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0, 0, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.LigneTerrainGauche, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(4, 1.5));
            p.polygon.Points.Add(new Point(0.03, 1.5));
            p.polygon.Points.Add(new Point(2.15, 1.5));
            p.polygon.Points.Add(new Point(2.15, 0));
            p.polygon.Points.Add(new Point(0.03, 0));
            p.polygon.Points.Add(new Point(2.15, 0));
            p.polygon.Points.Add(new Point(2.15, -1.5));
            p.polygon.Points.Add(new Point(0.03, -1.5));
            p.polygon.Points.Add(new Point(4, -1.5));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0, 0, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.LigneTerrainDroite, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-0.335, -2));
            p.polygon.Points.Add(new Point(0.335, -2));
            p.polygon.Points.Add(new Point(0.335, 2));
            p.polygon.Points.Add(new Point(-0.335, 2));
            p.polygon.Points.Add(new Point(-0.335, -2));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.LigneCentraleEpaisse, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-0.0095, -2));
            p.polygon.Points.Add(new Point(0.0095, -2));
            p.polygon.Points.Add(new Point(0.0095, 2));
            p.polygon.Points.Add(new Point(-0.0095, 2));
            p.polygon.Points.Add(new Point(-0.0095, -2));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.LigneCentraleFine, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-1 - 0.1, 2));
            p.polygon.Points.Add(new Point(-1 + 0.1, 2));
            p.polygon.Points.Add(new Point(-1 + 0.1, 2 + 0.2));
            p.polygon.Points.Add(new Point(-1 - 0.1, 2 + 0.2));
            p.polygon.Points.Add(new Point(-1 - 0.1, 2));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0xA0, 0xA0, 0xA0);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.BaliseGaucheHaut, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-4.2, -0.1));
            p.polygon.Points.Add(new Point(-4, -0.1));
            p.polygon.Points.Add(new Point(-4, 0.1));
            p.polygon.Points.Add(new Point(-4.2, 0.1));
            p.polygon.Points.Add(new Point(-4.2, -0.1));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0xA0, 0xA0, 0xA0);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.BaliseGaucheCentre, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(-1 - 0.1, -2));
            p.polygon.Points.Add(new Point(-1 + 0.1, -2));
            p.polygon.Points.Add(new Point(-1 + 0.1, -2 - 0.2));
            p.polygon.Points.Add(new Point(-1 - 0.1, -2 - 0.2));
            p.polygon.Points.Add(new Point(-1 - 0.1, -2));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0xA0, 0xA0, 0xA0);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.BaliseGaucheBas, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(1 - 0.1, 2));
            p.polygon.Points.Add(new Point(1 + 0.1, 2));
            p.polygon.Points.Add(new Point(1 + 0.1, 2 + 0.2));
            p.polygon.Points.Add(new Point(1 - 0.1, 2 + 0.2));
            p.polygon.Points.Add(new Point(1 - 0.1, 2));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0xA0, 0xA0, 0xA0);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.BaliseDroiteHaut, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(4.2, -0.1));
            p.polygon.Points.Add(new Point(4, -0.1));
            p.polygon.Points.Add(new Point(4, 0.1));
            p.polygon.Points.Add(new Point(4.2, 0.1));
            p.polygon.Points.Add(new Point(4.2, -0.1));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0xA0, 0xA0, 0xA0);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.BaliseDroiteCentre, p);

            p = new PolygonExtended();
            p.polygon.Points.Add(new Point(1 - 0.1, -2));
            p.polygon.Points.Add(new Point(1 + 0.1, -2));
            p.polygon.Points.Add(new Point(1 + 0.1, -2 - 0.2));
            p.polygon.Points.Add(new Point(1 - 0.1, -2 - 0.2));
            p.polygon.Points.Add(new Point(1 - 0.1, -2));
            p.borderWidth = fieldLineWidth;
            p.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            p.backgroundColor = System.Drawing.Color.FromArgb(0xFF, 0xA0, 0xA0, 0xA0);
            PolygonTerrainSeries.AddOrUpdatePolygonExtended((int)Terrain.BaliseDroiteBas, p);
        }


        //Récupération de la position cliquée sur la heatmap
        private void sciChart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                Console.WriteLine("CTRL+Click");
                // Perform the hit test relative to the GridLinesPanel
                var hitTestPoint = e.GetPosition(sciChartSurface.GridLinesPanel as UIElement);
                foreach (var serie in sciChartSurface.RenderableSeries)
                {
                    if (serie.GetType().Name == "FastUniformHeatmapRenderableSeries")
                    {
                        double xmin = (double)sciChartSurface.XAxes[0].VisibleRange.Min;
                        double xmax = (double)sciChartSurface.XAxes[0].VisibleRange.Max;
                        double ymin = (double)sciChartSurface.YAxes[0].VisibleRange.Min;
                        double ymax = (double)sciChartSurface.YAxes[0].VisibleRange.Max;

                        var width = sciChartSurface.ModifierSurface.ActualWidth;
                        var height = sciChartSurface.ModifierSurface.ActualHeight;

                        var xHeatMap = xmin + (xmax - xmin) * hitTestPoint.X / width;
                        var yHeatMap = -(ymin + (ymax - ymin) * hitTestPoint.Y / height);

                        Console.WriteLine("Click on : x=" + xHeatMap + " - y=" + yHeatMap);
                        OnCtrlClickOnHeatMap(xHeatMap, yHeatMap);
                    }
                }
            }
        }

        private void sciChart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            this.sciChartSurface.XAxis.VisibleRange.SetMinMax(-LengthDisplayArea / 2, LengthDisplayArea / 2);
            this.sciChartSurface.YAxis.VisibleRange.SetMinMax(-WidthDisplayArea / 2, WidthDisplayArea / 2);
        }

        //Event en cas de CTRL+click dans une heatmap
        public event EventHandler<PositionArgs> OnCtrlClickOnHeatMapEvent;
        public virtual void OnCtrlClickOnHeatMap(double x, double y)
        {
            var handler = OnCtrlClickOnHeatMapEvent;
            if (handler != null)
            {
                handler(this, new PositionArgs { X = x, Y = y });
            }
        }


        public XyDataSeries<double, double> GetXYDataSeriesFromPoints(List<PointD> ptList)
        {
            var dataSeries = new XyDataSeries<double, double>();
            var listX = ptList.Select(e => e.X);
            var listY = ptList.Select(e => e.Y);
            dataSeries.AcceptsUnsortedData = true;
            dataSeries.Append(listX, listY);
            return dataSeries;
        }

        public Point GetRelativeCoords(Point mousePoint)
        {
            var xCalc = sciChartSurface.XAxis.GetCurrentCoordinateCalculator();
            var yCalc = sciChartSurface.YAxis.GetCurrentCoordinateCalculator();

            double xDataValue = xCalc.GetDataValue(mousePoint.X);
            double yDataValue = yCalc.GetDataValue(mousePoint.Y);

            return new Point(xDataValue, yDataValue);
        }
    }

    public class CustomAnnotationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _logo;
        private string _text;

        // Provides a text for the watermark
        public string Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        // Provides the path to the image
        public string Logo
        {
            get { return _logo; }
            set
            {
                if (value != _logo)
                {
                    _logo = value;
                    OnPropertyChanged("Logo");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


