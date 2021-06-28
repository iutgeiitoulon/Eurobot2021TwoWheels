using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using EventArgsLibrary;
using Constants;
using Lidar;
using MathNet.Numerics;
using MathNet.Numerics.LinearRegression;
using System.Drawing;

/// <summary>
/// Notes for myself Medium Kalman Filter
/// </summary>

namespace LidarProcessNS
{
    public class LidarProcess
    {
        #region Parameters
        int robotId, teamId, LidarFrame;
        List<PolarPointRssi> polarPointRssis;
        List<PointD> pointXYCoord;
        #endregion

        #region Constructor
        public LidarProcess(int robot, int team)
        {
            robotId = robot;
            teamId = team;
            LidarFrame = 0;
        }
        #endregion

        #region Callback
        public void OnNewLidarConnected(object sender, LidarDevice e)
        {
            LidarFrame = 0;
        }


        public void OnRawPointAvailable(object sender, LidarPointsReadyEventArgs lidarPoints)
        {
            RawLidarArgs rawLidar = new RawLidarArgs()
            {
                RobotId = robotId,
                LidarFrameNumber = LidarFrame,
                PtList = lidarPoints.LidarPoints.Select(x => new PolarPointRssi(x.Angle, x.Distance, x.RSSI)).ToList()
            };


            OnRawLidarArgs(sender, rawLidar);
        }

        public void OnRawLidarArgs(object sender, RawLidarArgs rawLidarArgs)
        {
            LidarFrame++;

            OnRawLidarDataEvent?.Invoke(this, rawLidarArgs);
            OnRawLidarPointPolarEvent?.Invoke(this, rawLidarArgs.PtList.Select(x => new PolarPointRssiExtended(x, 3, Color.Purple)).ToList());
            OnRawLidarPointXYEvent?.Invoke(this, rawLidarArgs.PtList.Select(x => new PointDExtended(Toolbox.ConvertPolarToPointD(x), Color.Blue, 2)).ToList());
            ProcessLidarData(rawLidarArgs.PtList);
        }
        #endregion

        #region Event
        public event EventHandler<RawLidarArgs> OnRawLidarDataEvent;
        public event EventHandler<RawLidarArgs> OnProcessLidarDataEvent;
        public event EventHandler<List<PolarPointRssiExtended>> OnRawLidarPointPolarEvent;
        public event EventHandler<List<PointDExtended>> OnRawLidarPointXYEvent;
        public event EventHandler<List<PolarPointRssiExtended>> OnProcessLidarPolarDataEvent;
        public event EventHandler<List<PointDExtended>> OnProcessLidarAbsoluteDataEvent;
        public event EventHandler<List<SegmentExtended>> OnProcessLidarLineDataEvent;
        public event EventHandler<List<LidarObject>> OnProcessLidarObjectsDataEvent;
        public event EventHandler<List<Location>> OnLidarMultiplePositionEvent;
        #endregion

        #region Main
        public void ProcessLidarData(List<PolarPointRssi> polarPointRssi)
        {
            double thresold = 0.1;
            List<PointDExtended> absolutePoints = new List<PointDExtended>();
            List<SegmentExtended> Lines = new List<SegmentExtended>();


            List<PolarPointRssi> validPoint = polarPointRssi.Where(x => x.Distance >= ConstVar.LIDAR_MIN_POINT_DISTANCE && x.Distance <= ConstVar.LIDAR_MAX_POINT_DISTANCE).ToList();
            List<PointD> validPointXY = validPoint.Select(x => Toolbox.ConvertPolarToPointD(x)).ToList();

            List<ClusterObjects> list_of_all_clusters = ClustersDetection.DetectClusterOfPoint(validPointXY, 0.05, 3);
            List<PointD> allPointXY = list_of_all_clusters.SelectMany(x => x.points).ToList().Select(x => Toolbox.ConvertPolarToPointD(x)).ToList().Select(x => x.Pt).ToList();


            RectangleOriented best_rectangle = FindRectangle.FindMbrBoxByOverlap(allPointXY);

            List<PolarPointRssiExtended> processedPoints = allPointXY.Select(x => new PolarPointRssiExtended(Toolbox.ConvertPointDToPolar(x), 2, Color.White)).ToList();

            if (best_rectangle.Lenght <= ConstVar.WIDTH_BOXSIZE + thresold && best_rectangle.Width <= ConstVar.WIDTH_BOXSIZE + thresold)
            {


                //List<PointD> border_points = FindRectangle.FindAllBorderPoints(validPointXY, best_rectangle, 0.05);
                List<ClusterObjects> borders_clusters = FindRectangle.FindAllBorderClusters(list_of_all_clusters, best_rectangle, 0.05);

                List<ClusterObjects> inside_clusters = list_of_all_clusters.Where(x => borders_clusters.IndexOf(x) == -1).ToList();
                processedPoints = ClustersDetection.SetColorsOfClustersObjects(inside_clusters);

                Tuple<PointD, PointD, PointD, PointD> corners = Toolbox.GetCornerOfAnOrientedRectangle(best_rectangle);



                List<SegmentExtended> rectangle_segments = FindRectangle.DrawRectangle(best_rectangle, Color.Green, 1);

                RectangleOriented resized_rectangle = FindRectangle.ResizeRectangle(best_rectangle, thresold);
                List<Location> list_of_possible_locations = new List<Location>();

                if (resized_rectangle != null)
                {
                    List<Location> green_location = FindRectangle.ListAllPossibleLocation(resized_rectangle);

                    list_of_possible_locations.AddRange(green_location);

                }
                else
                {
                    Tuple<RectangleOriented, RectangleOriented, RectangleOriented> list_of_possible_rectangles = FindRectangle.ListResisableRectangle(best_rectangle, thresold);

                    List<Location> blue_location = FindRectangle.ListAllPossibleLocation(list_of_possible_rectangles.Item1);

                    list_of_possible_locations.AddRange(blue_location);

                    List<Location> yellow_location = FindRectangle.ListAllPossibleLocation(list_of_possible_rectangles.Item2);

                    list_of_possible_locations.AddRange(yellow_location);

                    List<Location> red_location = FindRectangle.ListAllPossibleLocation(list_of_possible_rectangles.Item3);

                    list_of_possible_locations.AddRange(red_location);
                }



                OnLidarMultiplePositionEvent?.Invoke(this, list_of_possible_locations);



                List<LidarObject> list_of_objects = new List<LidarObject>();

                foreach (ClusterObjects c in inside_clusters)
                {
                    RectangleOriented cluster_rectangle = FindRectangle.FindMbrBoxByOverlap(c.points.Select(x => Toolbox.ConvertPolarToPointD(x.Pt)).ToList());
                    cluster_rectangle.Width += 0.1;
                    cluster_rectangle.Lenght += 0.1;



                    Color color = Toolbox.ColorFromHSL((list_of_objects.Count * 0.20) % 1, 1, 0.5);

                    LidarObject cup = DetectCup(c);
                    // The null condition is Bad need to edit
                    if (cup != null)
                    {
                        list_of_objects.Add(cup);
                    }
                    else
                    {
                        LidarObject lidarObject = DetectRobot(c);

                        if (lidarObject.Type == LidarObjectType.Null)
                            Lines.AddRange(FindRectangle.DrawRectangle(cluster_rectangle, c.points[0].Color, 3));
                        else
                            list_of_objects.Add(lidarObject);
                    }
                        
                }
                           
                OnProcessLidarObjectsDataEvent?.Invoke(this, list_of_objects);
                
                
                OnProcessLidarLineDataEvent?.Invoke(this, Lines);
            }

            RawLidarArgs processLidar = new RawLidarArgs() { RobotId = robotId, LidarFrameNumber = LidarFrame, PtList = processedPoints.Select(x => x.Pt).ToList() };
            OnProcessLidarDataEvent?.Invoke(this, processLidar);
            OnProcessLidarPolarDataEvent?.Invoke(this, processedPoints);


        }
        #endregion

        #region Clusters
        public List<ClusterObjects> DetectClusterOfPoint(List<PolarPointRssi> pointsList, double thresold, int mininum_amount_of_points)
        {
            /// ABD Stand for Adaptative breakpoint Detection
            List<ClusterObjects> listOfClusters = new List<ClusterObjects>();
            ClusterObjects cluster = new ClusterObjects();
            int i;
            for (i = 1; i < pointsList.Count - 1; i++)
            {
                PolarPointRssi point_n_minus_1 = pointsList[i - 1];
                PolarPointRssi point_n_plus_1 = pointsList[i + 1];
                PolarPointRssi point_n = pointsList[i];

                double dist_n_minus_1 = point_n_minus_1.Distance;
                double delta_theta = Math.Abs(point_n_minus_1.Angle - point_n.Angle);
                double lambda = point_n_plus_1.Angle - point_n_minus_1.Angle;

                double ABD_Thresold = thresold; // dist_n_minus_1 * (Math.Sin(delta_theta) / Math.Sin(lambda - delta_theta));
                double distance_between_point = Toolbox.Distance(point_n, point_n_minus_1);
                if (distance_between_point < ABD_Thresold)
                {
                    cluster.points.Add(new PolarPointRssiExtended(point_n, 3, Color.Purple));
                }
                else
                {
                    if (cluster.points.Count() > mininum_amount_of_points)
                    {
                        listOfClusters.Add(cluster);
                    }
                    cluster = new ClusterObjects();
                }


            }
            if (cluster.points.Count() > mininum_amount_of_points)
            {
                listOfClusters.Add(cluster);
            }

            return listOfClusters;
        }
        #endregion


        #region Small Line
        public Segment DetectLine(ClusterObjects blob, double thresold, double alignNbr, int moy = 5)
        {
            List<PolarPointRssi> pointList = blob.points.Select(x => x.Pt).ToList();

            List<double> derivate1 = new List<double>();
            List<double> derivate2 = new List<double>();
            Segment line = new Segment();

            int i;
            for (i = 0; i < pointList.Count - 1; i++)
            {
                derivate1.Add(Math.Abs(pointList[i].Distance - pointList[i + 1].Distance));
            }
            for (i = 0; i < derivate1.Count - 1; i++)
            {
                derivate2.Add(Math.Abs(derivate1[i] - derivate1[i + 1]));
            }

            uint nbrOfCurrentAlign = 0;
            for (i = 0; i < derivate2.Count - 1; i++)
            {
                if (derivate2[i] >= 0 && derivate2[i] <= thresold)
                {
                    nbrOfCurrentAlign++;
                }
                else if (nbrOfCurrentAlign > 0)
                {
                    if (nbrOfCurrentAlign >= alignNbr)
                    {
                        return CreateLineSegment(pointList, moy);
                    }
                    nbrOfCurrentAlign = 0;
                }

            }

            if (nbrOfCurrentAlign >= alignNbr)
            {
                return CreateLineSegment(pointList, moy);
            }

            return line;
        }
        #endregion

        #region Cups
        public LidarObject DetectCup(ClusterObjects cluster)
        {
            RectangleOriented box_of_cluster = FindRectangle.FindMbrBoxByOverlap(cluster.points.Select(x => Toolbox.ConvertPolarToPointD(x.Pt)).ToList());
            //box_of_cluster.Width += 0.1;
            //box_of_cluster.Lenght += 0.1;

            if (box_of_cluster.Width / box_of_cluster.Lenght <= 5 && box_of_cluster.Width <= 0.08 && box_of_cluster.Width >= 0.02)
            {
                List<PointD> pointDs = cluster.points.Select(x => Toolbox.ConvertPolarToPointD(x.Pt)).ToList();

                double median = 0.80;

                double b = cluster.points[(int)(cluster.points.Count() * median)].Pt.Rssi;
                double e = cluster.points[(int)(cluster.points.Count() * (1 - median))].Pt.Rssi;

                double moyenne = (b + e) / 2;
                Color color = Color.White;

                if (moyenne >= 9000 && moyenne <= 12000)
                {
                    color = Color.LimeGreen;
                }
                else if (moyenne >= 12000 && moyenne <= 14000)
                {
                    color = Color.Red;
                }
                else
                {
                    return new LidarObject();
                }

                PointD center_point = new PointD(pointDs.Sum(x => x.X) / pointDs.Count, pointDs.Sum(x => x.Y) / pointDs.Count, pointDs.Sum(x => x.Rssi) / pointDs.Count);

                return new LidarObject(box_of_cluster, color, LidarObjectType.Cup);
            }
            else if (box_of_cluster.Width / box_of_cluster.Lenght <= 3 && box_of_cluster.Width >= 0.05 && box_of_cluster.Width <= 0.15)
            {
                List<PointD> pointDs = cluster.points.Select(x => Toolbox.ConvertPolarToPointD(x.Pt)).ToList();

                double median = 0.80;

                double b = cluster.points[(int)(cluster.points.Count() * median)].Pt.Rssi;
                double e = cluster.points[(int)(cluster.points.Count() * (1 - median))].Pt.Rssi;

                double moyenne = (b + e) / 2;
                Color color = Color.White;

                if (moyenne >= 9000 && moyenne <= 12000)
                {
                    color = Color.DarkGreen;
                }
                else if (moyenne >= 12000 && moyenne <= 14000)
                {
                    color = Color.DarkRed;
                }
                else
                {
                    return new LidarObject();
                }

                PointD center_point = new PointD(pointDs.Sum(x => x.X) / pointDs.Count, pointDs.Sum(x => x.Y) / pointDs.Count, pointDs.Sum(x => x.Rssi) / pointDs.Count);
                return new LidarObject(box_of_cluster, color, LidarObjectType.Cup);
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region Robot
        public LidarObject DetectRobot(ClusterObjects cluster)
        {
            RectangleOriented box_of_cluster = FindRectangle.FindMbrBoxByOverlap(cluster.points.Select(x => Toolbox.ConvertPolarToPointD(x.Pt)).ToList());

            if (box_of_cluster.Width >= 0.20 || box_of_cluster.Lenght >= 0.20)
                return new LidarObject(new RectangleOriented(box_of_cluster.Center, 0.3, 0.3, box_of_cluster.Angle), Color.Red, LidarObjectType.Robot);
            else
                return new LidarObject();
        }

        #endregion

        #region Spatial SubSampling
        private List<PolarPointRssi> FixedStepLidarMap(List<PolarPointRssi> ptList, double step, double minAngle = -Math.PI / 2, double maxAngle = Math.PI / 2)
        {
            /// On construit une liste de points ayant le double du nombre de points de la liste d'origine, de manière 
            /// à avoir un recoupement d'un tour complet 
            List<PolarPointRssi> ptListFixedStep = new List<PolarPointRssi>();
            double currentAngle = minAngle;
            double constante = (ptList.Count) / (maxAngle - minAngle);
            while (currentAngle < maxAngle && currentAngle >= minAngle)
            {
                /// On détermine l'indice du point d'angle courant dans la liste d'origine
                int ptIndex = (int)((currentAngle - minAngle) * constante);
                var ptCourant = ptList[ptIndex];

                /// On ajoute ce point à la liste des points de sortie
                ptListFixedStep.Add(ptCourant);

                /// On calcule l'incrément d'angle de manière à avoir une résolution constante si la paroi est orthogonale au rayon issu du robot
                double incrementAngle = step / Math.Max(ptCourant.Distance, 0.1);

                /// On regarde le ratio entre la distance entre les pts à droite
                int n = 5;
                var ptDroite = ptList[Math.Min(ptIndex + n, ptList.Count - 1)];
                var ptGauche = ptList[Math.Max(ptIndex - n, 0)];
                var distancePtGauchePtDroit = Toolbox.Distance(ptDroite, ptGauche);
                var distancePtGauchePtDroitCasOrthogonal = (ptDroite.Angle - ptGauche.Angle) * ptCourant.Distance;
                var ratioAngle = distancePtGauchePtDroit / distancePtGauchePtDroitCasOrthogonal;
                ratioAngle = Toolbox.LimitToInterval(ratioAngle, 1, 5);

                currentAngle += Math.Min(0.3, incrementAngle * step / ratioAngle);
            }

            return ptListFixedStep;
        }
        #endregion

        #region Gies Detection
        List<PolarCourbure> ExtractCurvature(List<PolarPointRssi> ptList, int tailleNoyau = 5)
        {
            List<PolarPointRssi> curvatureListDebug = new List<PolarPointRssi>();
            /// Implanation basée sur 
            /// "Natural landmark extraction for mobile robot navigation based on an adaptive curvature estimation"
            /// P. Nunez, R. Vazquez-Martın, J.C. del Toro, A. Bandera, F. Sandoval
            /// Robotics and Autonomous Systems 56 (2008) 247–264
            /// 

            List<PolarCourbure> curvatureList = new List<PolarCourbure>();

            for (int i = 0; i < ptList.Count; i++)
            {
                double normeContour = 0;
                for (int j = -tailleNoyau / 2; j < tailleNoyau / 2 + 1; j++)
                {
                    if (i + j >= 0 && i + j + 1 < ptList.Count)
                    {
                        normeContour += Toolbox.Distance(ptList[i + j], ptList[i + j + 1]);
                    }
                }
                double normeDirecte = Toolbox.Distance(ptList[Math.Max(0, i - tailleNoyau / 2)], ptList[Math.Min(i + tailleNoyau / 2 + 1, ptList.Count - 1)]);
                curvatureList.Add(new PolarCourbure(ptList[i].Angle, normeContour / normeDirecte, false));

                curvatureListDebug.Add(new PolarPointRssi(ptList[i].Angle, normeContour / normeDirecte, 0));
            }
            return curvatureList;

        }

        List<Segment> ExtractLinesFromCurvature(List<PointD> ptList, List<PolarCourbure> curvatureList, double seuilCourbure = 1.01)
        {
            bool isLineStarted = false;

            List<PointD> linePoints = new List<PointD>();
            List<Segment> list_of_segment = new List<Segment>();

            for (int i = 0; i < curvatureList.Count; i++)
            {
                if (curvatureList[i].Courbure < seuilCourbure)
                {
                    isLineStarted = true;
                    linePoints.Add(ptList[i]);
                }
                else if (isLineStarted)
                {
                    isLineStarted = false;
                    list_of_segment.Add(new Segment(linePoints[0], linePoints[linePoints.Count - 1]));
                    linePoints = new List<PointD>();
                }
            }
            return list_of_segment;
        }

        List<PolarPointRssi> ExtractCornersFromCurvature(List<PolarPointRssi> ptList, List<PolarCourbure> curvatureList)
        {
            List<PolarPointRssi> cornerPoints = new List<PolarPointRssi>();
            for (int i = 0; i < curvatureList.Count; i++)
            {
                int i_Moins1 = i - 1;
                if (i_Moins1 < 0)
                    i_Moins1 += ptList.Count;
                int i_Plus1 = i + 1;
                if (i_Plus1 >= ptList.Count)
                    i_Plus1 -= ptList.Count;
                if (curvatureList[i].Courbure > curvatureList[i_Moins1].Courbure && curvatureList[i].Courbure > curvatureList[i_Plus1].Courbure && curvatureList[i].Courbure > 1) //On a maximum local de courbure
                {
                    cornerPoints.Add(ptList[i]);
                }
            }
            return cornerPoints;
        }
        #endregion

        #region Utils
        #region Segments
        private Segment CreateLineSegment(List<PolarPointRssi> pointList, double moy)
        {
            int j;
            double X1 = 0, Y1 = 0, X2 = 0, Y2 = 0;
            for (j = 0; j < moy; j++)
            {
                PointD point = Toolbox.ConvertPolarToPointD(pointList[j]);
                X1 += point.X;
                Y1 += point.Y;
            }
            for (j = 0; j < moy; j++)
            {
                PointD point = Toolbox.ConvertPolarToPointD(pointList[pointList.Count - 1 - j]);
                X2 += point.X;
                Y2 += point.Y;
            }
            X1 /= moy;
            Y1 /= moy;
            X2 /= moy;
            Y2 /= moy;
            return new Segment(X1, Y1, X2, Y2);
        }
        private Segment CreateLineSegment(List<PolarPointRssi> pointList, int first, int last, double moy)
        {

            int i;
            double X1 = 0, Y1 = 0, X2 = 0, Y2 = 0;
            for (i = 0; i < moy; i++)
            {
                PointD point = Toolbox.ConvertPolarToPointD(pointList[first + i]);
                X1 += point.X;
                Y1 += point.Y;
            }
            for (i = 0; i < moy; i++)
            {
                PointD point = Toolbox.ConvertPolarToPointD(pointList[last - i]);
                X2 += point.X;
                Y2 += point.Y;
            }
            X1 /= moy;
            Y1 /= moy;
            X2 /= moy;
            Y2 /= moy;
            return new Segment(X1, Y1, X2, Y2);
        }
        #endregion
        #region Conversion



        #endregion
        #region Others
        private int GetIndexOfAngle(List<double> angle_array, double angle)
        {
            if (angle_array.Count == 0)
            {
                return 1;
            }
            int i, index = 0;
            double min = Math.Abs(angle - angle_array[0]);
            for (i = 0; i < angle_array.Count; i++)
            {
                double delta = Math.Abs(angle - angle_array[i]);
                if (delta < min)
                {
                    min = delta;
                    index = i;
                }
            }
            return index;
        }
        #endregion
        #endregion
    }
}
