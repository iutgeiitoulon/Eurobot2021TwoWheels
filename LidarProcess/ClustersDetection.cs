using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using System.Drawing;
using EventArgsLibrary;

namespace LidarProcessNS
{
    public static class ClustersDetection
    {

        /// <summary>
        /// Detect and return a list of Clusters point in pointList
        /// </summary>
        /// <param name="pointsList"></param>
        /// <param name="thresold"></param>
        /// <param name="mininum_amount_of_points"></param>
        /// <returns></returns>
        public static List<ClusterObjects> DetectClusterOfPoint(List<PolarPointRssiExtended> pointsList, double thresold, int mininum_amount_of_points = 5)
        {
            /// ABD Stand for Adaptative breakpoint Detection
            List<ClusterObjects> listOfClusters = new List<ClusterObjects>();
            ClusterObjects cluster = new ClusterObjects();
            int i;
            for (i = 1; i < pointsList.Count - 1; i++)
            {
                PolarPointRssiExtended point_n_minus_1 = pointsList[i - 1];
                PolarPointRssiExtended point_n_plus_1 = pointsList[i + 1];
                PolarPointRssiExtended point_n = pointsList[i];

                

                //double dist_n_minus_1 = point_n_minus_1.Distance;
                //double delta_theta = Math.Abs(point_n_minus_1.Angle - point_n.Angle);
                //double lambda = point_n_plus_1.Angle - point_n_minus_1.Angle;

                double ABD_Thresold = thresold; // dist_n_minus_1 * (Math.Sin(delta_theta) / Math.Sin(lambda - delta_theta));
                double distance_between_point = Toolbox.Distance(point_n, point_n_minus_1);
                if (distance_between_point < ABD_Thresold)
                {

                    cluster.points.Add(point_n);
                }
                else
                {

                    if (Toolbox.Distance(point_n_plus_1, point_n_minus_1) <= 2 * thresold)
                    {
                        //cluster.points.Add(point_n);
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
            }

            if (cluster.points.Count() > mininum_amount_of_points)
            {
                listOfClusters.Add(cluster);
            }

            return listOfClusters;
        }

        public static List<ClusterObjects> ExtractClusterFromIEPF(List<PolarPointRssiExtended> ptList, List<PolarPointRssiExtended> ptIEPF)
        {
            List<ClusterObjects> list_of_clusters = new List<ClusterObjects>(); 


            /// Maybe sort
            for (int i = 0; i < ptIEPF.Count - 2; i += 2)
            {
                ClusterObjects cluster = new ClusterObjects(ptList.GetRange(ptList.IndexOf(ptIEPF[i]), ptList.IndexOf(ptIEPF[i + 1]) - ptList.IndexOf(ptIEPF[i])));
                list_of_clusters.Add(cluster);
            }
            return list_of_clusters;
        }

        /// <summary>
        /// Notes maybe use pointer insted of list searching...
        /// </summary>
        /// <param name="list_of_points"></param>
        /// <param name="epsilon"></param>
        /// <param name="min_points"></param>
        /// 
        public static List<ClusterObjects> ExtractClusterByDBScan (List<PointD> list_of_points, double epsilon, int min_points)
        {
            /// The byte is just a variable representing the code:
            ///     - 0x00 : Unvisited
            ///     - 0x01 : Visited
            ///     - 0X02 : Visited + Part of a Cluster
            ///     - 0xFF : Noise

            Dictionary<PointD, byte> DictionnaryOfDBScan = new Dictionary<PointD, byte>();
            List<ClusterObjects> list_of_clusters = new List<ClusterObjects>();

            list_of_points.Distinct().ToList().ForEach(x => DictionnaryOfDBScan.Add(x, 0x00));

            foreach (PointD point in DictionnaryOfDBScan.Keys.ToList())
            {
                /// If the point is Unvisited
                if (DictionnaryOfDBScan[point] == 0x00)
                {
                    /// The point is marked as Visited
                    DictionnaryOfDBScan[point] = 0x01;
                    List<PointD> neighbors_points = Get_neighbors_points_for_DbScan(DictionnaryOfDBScan, point, epsilon);

                    if (neighbors_points.Count() < min_points)
                    {
                        /// The point is marked as Noise
                        DictionnaryOfDBScan[point] = 0xFF;
                    }
                    else
                    {
                        DictionnaryOfDBScan[point] = 0x02;
                        list_of_clusters.Add(new ClusterObjects( new List<PolarPointRssiExtended>() { new PolarPointRssiExtended( Toolbox.ConvertPointDToPolar( point ), 1, Color.White) } ) );

                        for (int j = 0; j < neighbors_points.Count; j++)
                        {
                            PointD selected_point = neighbors_points[j];

                            if (DictionnaryOfDBScan[selected_point] == 0x00)
                            {
                                DictionnaryOfDBScan[selected_point] = 0x01;

                                List<PointD> neighbors_points_prime = Get_neighbors_points_for_DbScan(DictionnaryOfDBScan, selected_point, epsilon);
                                if (neighbors_points_prime.Count() >= min_points)
                                {
                                    neighbors_points.AddRange(neighbors_points_prime);
                                    neighbors_points = neighbors_points.Distinct().ToList(); // Supress the duplicates
                                }
                            }

                            if (DictionnaryOfDBScan[selected_point] == 0x01)
                            {
                                DictionnaryOfDBScan[selected_point] = 0x02;
                                list_of_clusters[list_of_clusters.Count - 1].points.Add(new PolarPointRssiExtended(Toolbox.ConvertPointDToPolar(selected_point), 1, Color.White));
                            }
                        }
                    }
                }
            }

            return list_of_clusters;
        }


        /// <summary>
        /// Only useful for DBScan but actually pretty terrible in term of optimisation... -> O(n) 
        /// </summary>
        /// <param name="D"></param>
        /// <param name="P"></param>
        /// <param name="epsilon"></param>
        /// <returns>The list of points which are closest to the point P</returns>
        public static List<PointD> Get_neighbors_points_for_DbScan (Dictionary<PointD, byte> D, PointD P, double epsilon)
        {
            List<PointD> neighbors_list = new List<PointD>();

            foreach (PointD tested_point in D.Keys)
            {
                if (Toolbox.Distance(P, tested_point) < epsilon)
                {
                    neighbors_list.Add(tested_point);
                }
            }

            return neighbors_list;
        }

        public static void ExtractClustersFromOPTICSOrderedList(List<PointD> OrderedList, Dictionary<PointD, Tuple<bool, double?>> DictionaryOPTICS, double epsilon)
        {
            List<ClusterObjects> cluster_list = new List<ClusterObjects>();

            for (int i = 0; i < OrderedList.Count(); i++)
            {


                // if (OrderedList[i] < epsilon)
                {

                }
            }
        }

        //public static List<PointD> ExtractOrderListFromOPTICS (List<PointD> list_of_points, double epsilon, int min_points)
        //{
        //    List<PointD> OrderedList = new List<PointD>();
        //    Dictionary<PointD, Tuple<bool, double?>> DictionnaryOfOPTICS = new Dictionary<PointD, Tuple<bool, double?>>();

        //    foreach (PointD point in list_of_points.Distinct().ToList())
        //    {
        //        DictionnaryOfOPTICS.Add(point, new Tuple<bool, double?>(false, null));
        //    }

        //    foreach (PointD point in DictionnaryOfOPTICS.Keys.ToList())
        //    {
        //        if (DictionnaryOfOPTICS[point].Item1 == false)
        //        {
        //            DictionnaryOfOPTICS[point] = new Tuple<bool, double?>(true, DictionnaryOfOPTICS[point].Item2);
        //            OrderedList.Add(point);
                    
        //            if (Core_distance(DictionnaryOfOPTICS, point, epsilon, min_points) != null)
        //            {
        //                List<PointD> list_of_neighbors = Get_neighbors_points_for_Optics(DictionnaryOfOPTICS.Keys.ToList() , point, epsilon); // Get Neighbors

        //                StablePriorityQueue<PointDQueue> seeds = new StablePriorityQueue<PointDQueue>(1440);
        //                Update_OPTICS(ref DictionnaryOfOPTICS, list_of_neighbors, point, ref seeds, epsilon, min_points);
        //                while (seeds.Count > 0)
        //                {
        //                    PointDQueue point_temp = seeds.Dequeue();
        //                    PointD neighbor_point = new PointD(point_temp.x, point_temp.y);
        //                    DictionnaryOfOPTICS[neighbor_point] = new Tuple<bool, double?>(true, DictionnaryOfOPTICS[neighbor_point].Item2);

        //                    OrderedList.Add(neighbor_point);

        //                    if (Core_distance(DictionnaryOfOPTICS, point, epsilon, min_points) != null)
        //                    {
        //                        Update_OPTICS(ref DictionnaryOfOPTICS, list_of_neighbors, neighbor_point, ref seeds, epsilon, min_points);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return OrderedList;
        //}

        //private static void Update_OPTICS(ref Dictionary<PointD, Tuple<bool, double?>> DictionnaryOfOPTICS, List<PointD> list_of_neighbors, PointD point, ref StablePriorityQueue<PointDQueue> seeds, double epsilon, int min_points)
        //{
        //    double? core_dist = Core_distance(DictionnaryOfOPTICS, point, epsilon, min_points);

        //    foreach (PointD O in list_of_neighbors)
        //    {
        //        if (DictionnaryOfOPTICS[O].Item1 == false)
        //        {
        //            double? reachability = Reachability_distance(DictionnaryOfOPTICS, point, O, epsilon, min_points);
        //            if (DictionnaryOfOPTICS[O].Item2 == null)
        //            {
        //                DictionnaryOfOPTICS[O] = new Tuple<bool, double?>(DictionnaryOfOPTICS[O].Item1, reachability);
        //                seeds.Enqueue(new PointDQueue(O.X, O.Y), (float) reachability);
        //            }
        //            else if (reachability < DictionnaryOfOPTICS[O].Item2)
        //            {
        //                DictionnaryOfOPTICS[O] = new Tuple<bool, double?>(DictionnaryOfOPTICS[O].Item1, reachability);
        //                seeds.UpdatePriority(new PointDQueue(O.X, O.Y), (float) reachability);
        //            }
        //        }
        //    }
        //}

        //private static double? Core_distance(Dictionary<PointD, Tuple<bool, double?>> D, PointD P, double epsilon, int min_pts)
        //{
        //    List<PointD> neigbors = Get_neighbors_points_for_Optics(D.Keys.ToList() , P, epsilon);

        //    if (neigbors.Count() >= min_pts)
        //    {
        //        return Toolbox.Distance(neigbors.OrderBy(x => Toolbox.Distance(P, x)).FirstOrDefault(), P);
        //    }

        //    return null;
        //}

        //private static double? Reachability_distance(Dictionary<PointD, Tuple<bool, double?>> D, PointD P, PointD O, double epsilon, int min_pts) 
        //{
        //    double? core_dist = Core_distance(D, P, epsilon, min_pts);
            
        //    if (core_dist != null)
        //    {
        //        return Math.Max((double) core_dist, (double) Toolbox.Distance(P, O));
        //    }
        //    return null;
        //}

        //public static List<PointD> Get_neighbors_points_for_Optics(List<PointD> D, PointD P, double epsilon)
        //{
        //    List<PointD> neighbors_list = new List<PointD>();

        //    foreach (PointD tested_point in D)
        //    {
        //        if (Toolbox.Distance(P, tested_point) < epsilon)
        //        {
        //            neighbors_list.Add(tested_point);
        //        }
        //    }

        //    return neighbors_list;
        //}

        public static List<PolarPointRssiExtended> SetColorsOfClustersObjects(List<ClusterObjects> clusterObjects)
        {
            int i;
            List<PolarPointRssiExtended> array_of_points = new List<PolarPointRssiExtended>();

            for (i = 0; i < clusterObjects.Count; i++)
            {
                Color color = Toolbox.HLSToColor((35 * i) % 240 , 120, 240);

                foreach (PolarPointRssiExtended points in clusterObjects[i].points)
                {
                    points.Color = color;
                    points.Width = 2;

                    array_of_points.Add(points);
                }
                
            }

            return array_of_points;
        }

        //public class PointDQueue : StablePriorityQueueNode
        //{
        //    public double x { get; set; }
        //    public double y { get; set; }
        //    public PointDQueue(double x, double y)
        //    {
        //        this.x = x;
        //        this.y = y;
        //    }
        //}
    }
}
