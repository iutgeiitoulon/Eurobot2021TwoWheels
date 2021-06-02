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
        Location robotLocation;
        List<PolarPointRssi> polarPointRssis;
        List<PointD> pointXYCoord;
        #endregion

        #region Constructor
        public LidarProcess(int robot, int team)
        {
            robotId = robot;
            teamId = team;
            robotLocation = new Location() { };
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

        public void OnRobotLocation(object sender, LocationArgs robot)
        {
            if (robot.RobotId == robotId)
            {
                robotLocation = robot.Location;
            }
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
        public event EventHandler<List<LidarObjects>> OnProcessLidarObjectsDataEvent;
        public event EventHandler<List<Cup>> OnProcessLidarCupDataEvent;
        public event EventHandler<Location> OnLidarSetupRobotLocationEvent;
        #endregion

        #region Main
        public void ProcessLidarData(List<PolarPointRssi> polarPointRssi)
        {
            List<SegmentExtended> Lines = new List<SegmentExtended>();

            List<PolarPointRssi> validPoint = polarPointRssi.Where(x => x.Distance <= Math.Sqrt(Math.Pow(3, 2) + Math.Pow(2, 2))).ToList();
            List<PointD> validPointXY = validPoint.Select(x => Toolbox.ConvertPolarToPointD(x)).ToList();

            List<PointDExtended> absolutePoints = new List<PointDExtended>();

            validPointXY = ClustersDetection.ExtractClusterByDBScan(validPointXY, 0.05, 3).SelectMany(x => x.points).ToList().Select(x => Toolbox.ConvertPolarToPointD(x)).ToList().Select(x => x.Pt).ToList();
            

            RectangleOriented best_rectangle = FindRectangle.FindMbrBoxByOverlap(validPointXY);
            List<PointD> border_points = FindRectangle.FindAllBorderPoints(validPointXY, best_rectangle, 0.05);
            List<PolarPointRssi> border_point_polar = border_points.Select(x => Toolbox.ConvertPointDToPolar(x)).ToList();

            List<ClusterObjects> inside_clusters = ClustersDetection.ExtractClusterByDBScan(validPointXY.Where(x => border_points.IndexOf(x) == -1).ToList(), 0.045, 3);
            List<PolarPointRssiExtended> processedPoints = ClustersDetection.SetColorsOfClustersObjects(inside_clusters);
            

            List<ClusterObjects> border_clusters = ClustersDetection.ExtractClusterByDBScan(border_points, 0.05, 10);

            List<SegmentExtended> iepfs_lines = new List<SegmentExtended>();

            foreach (ClusterObjects c in border_clusters)
            {
                var iepf_border_points = LineDetection.IEPF_Algorithm(c.points.Select(x => Toolbox.ConvertPolarToPointD(x).Pt).ToList(), 0.05);

                for (int i = 1; i < iepf_border_points.Count; i++)
                {
                    iepfs_lines.Add(new SegmentExtended(iepf_border_points[i - 1], iepf_border_points[i], Color.Black, 2));
                }
            }




            iepfs_lines = LineDetection.MergeSegmentWithLSM(iepfs_lines, 0.5, 3 * Math.PI / 180);
            Lines.AddRange(iepfs_lines);

            List<List<SegmentExtended>> list_of_family = LineDetection.FindFamilyOfSegment(iepfs_lines);

            List<PointD> corners_points = CornerDetection.FindAllValidCrossingPoints(list_of_family).SelectMany(x => x).ToList().Select(x => x.Pt).ToList();
            Tuple<PointD, PointD, PointD, PointD> corners = Toolbox.GetCornerOfAnOrientedRectangle(best_rectangle);

            double thresold = 0.09;

            double width = Math.Max(best_rectangle.Lenght, best_rectangle.Width);
            double height = Math.Min(best_rectangle.Lenght, best_rectangle.Width);

            List<SegmentExtended> rectangle_segments = FindRectangle.DrawRectangle(best_rectangle, Color.Green, 1);


            processedPoints.Add(new PolarPointRssiExtended(Toolbox.ConvertPointDToPolar(best_rectangle.Center), 10, Color.Black));


            //Lines.AddRange(rectangle_segments);

            //Console.WriteLine("Corners: " + FindRectangle.GetNumberOfVisibleCorners(best_rectangle));

            RectangleOriented resized_rectangle = FindRectangle.ResizeRectangle(best_rectangle, thresold);
            List<Location> list_of_possible_locations;

            if (resized_rectangle != null)
            {
                //Lines.AddRange(FindRectangle.DrawRectangle(resized_rectangle, Color.LightGreen, 8));
                processedPoints.Add(new PolarPointRssiExtended(Toolbox.ConvertPointDToPolar(resized_rectangle.Center), 10, Color.Green));

                list_of_possible_locations = FindRectangle.ListAllPossibleLocation(resized_rectangle);
                //processedPoints.AddRange(list_of_possible_locations.Select(x => new PolarPointRssiExtended(Toolbox.ConvertPointDToPolar(new PointD(x.X, x.Y)), 10, (x.Theta != 1) ? Color.Red : Color.DarkRed)).ToList());
            }
            else
            {
                Tuple<RectangleOriented, RectangleOriented, RectangleOriented> list_of_possible_rectangles = FindRectangle.ListResisableRectangle(best_rectangle, thresold);
                list_of_possible_locations = FindRectangle.ListAllPossibleLocation(list_of_possible_rectangles.Item2);

                //Lines.AddRange(FindRectangle.DrawRectangle(list_of_possible_rectangles.Item1, Color.Yellow, 4));
                //Lines.AddRange(FindRectangle.DrawRectangle(list_of_possible_rectangles.Item2, Color.Blue, 4));
                //Lines.AddRange(FindRectangle.DrawRectangle(list_of_possible_rectangles.Item3, Color.Red, 4));

            }
            //processedPoints.AddRange(list_of_possible_locations.Select(x => new PolarPointRssiExtended(Toolbox.ConvertPointDToPolar(new PointD(x.X, x.Y)), 10, (x.Theta != 1) ? Color.Red: Color.DarkBlue)).ToList());

            absolutePoints = list_of_possible_locations.Select(x => new PointDExtended(new PointD(x.X, x.Y), x.Theta > 0 ? Color.Red : Color.Blue, 10)).ToList();
            Location best_location = FindRectangle.GetBestLocation(list_of_possible_locations, robotLocation);

            //processedPoints.Add(new PolarPointRssiExtended(Toolbox.ConvertPointDToPolar(new PointD(best_location.X, best_location.Y)), 10, Color.DarkGreen));
            //OnLidarSetupRobotLocationEvent?.Invoke(this, best_location);








            List<Cup> list_of_cups = new List<Cup>();
            List<LidarObjects> list_of_objects = new List<LidarObjects>();

            foreach (ClusterObjects c in inside_clusters)
            {
                RectangleOriented cluster_rectangle = FindRectangle.FindMbrBoxByArea(c.points.Select(x => Toolbox.ConvertPolarToPointD(x.Pt)).ToList());
                cluster_rectangle.Width += 0.1;
                cluster_rectangle.Lenght += 0.1;

                Lines.AddRange(FindRectangle.DrawRectangle(cluster_rectangle, c.points[0].Color, 3));

                Color color = Toolbox.ColorFromHSL((list_of_objects.Count * 0.20) % 1, 1, 0.5);
                list_of_objects.Add(new LidarObjects(c.points.Select(x => Toolbox.ConvertPolarToPointD(x.Pt)).ToList(), color));


                Cup cup = DetectCup(c);
                // The null condition is Bad need to edit
                if (cup != null)
                {
                    list_of_cups.Add(cup);
                }
            }

            OnProcessLidarObjectsDataEvent?.Invoke(this, list_of_objects);

            OnProcessLidarLineDataEvent?.Invoke(this, Lines);

            OnProcessLidarCupDataEvent?.Invoke(this, list_of_cups);


            RawLidarArgs processLidar = new RawLidarArgs() { RobotId = robotId, LidarFrameNumber = LidarFrame, PtList = processedPoints.Select(x => x.Pt).ToList() };
            OnProcessLidarDataEvent?.Invoke(this, processLidar);
            
            OnProcessLidarPolarDataEvent?.Invoke(this, processedPoints);
            OnProcessLidarAbsoluteDataEvent?.Invoke(this, absolutePoints);
            //OnProcessLidarXYDataEvent?.Invoke(this, processedPoints.Select(x => new PointDExtended(Toolbox.ConvertPolarToPointD(x), Color.Blue, 2)).ToList());
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
        public Cup DetectCup(ClusterObjects cluster)
        {
            /// TEMPORARY NEED TO EDIT: ONLY FOR DEBUG PURPOSE

            PolarPointRssi begin_point = cluster.points[0].Pt;
            PolarPointRssi end_point = cluster.points[cluster.points.Count - 1].Pt;

            double lenght_of_cluster = Toolbox.Distance(begin_point, end_point);

            if (lenght_of_cluster >= 0.040 && lenght_of_cluster <= 0.08)
            {
                List<PointD> pointDs = cluster.points.Select(x => Toolbox.ConvertPolarToPointD(x.Pt)).ToList();

                double median = 0.80;

                double b = cluster.points[(int)(cluster.points.Count() * median)].Pt.Rssi;
                double e = cluster.points[(int)(cluster.points.Count() * (1 - median))].Pt.Rssi;

                double moyenne = (b + e) / 2;
                Color color = Color.White;
                if (moyenne >= 9000 && moyenne <= 12000)
                {
                    color = Color.Green;
                }
                else if (moyenne >= 12000 && moyenne <= 14000)
                {
                    color = Color.Red;
                }
                else
                {
                    return new Cup();
                }
                //Console.WriteLine(moyenne);
                PointD center_point = GetMediumPoint(pointDs);
                return new Cup(center_point, 0.065, color);
            }
            else
            {
                return null;
            }

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

        private PointD GetMediumPoint(List<PointD> points)
        {
            double X = 0;
            double Y = 0;
            foreach (PointD point in points)
            {
                X += point.X;
                Y += point.Y;
            }
            X /= points.Count();
            Y /= points.Count();

            return new PointD(X, Y);
        }
        #endregion
        #endregion
    }
}
