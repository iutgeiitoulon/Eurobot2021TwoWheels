using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using System.Drawing;

namespace LidarProcessNS
{
    public static class LineDetection
    {
        public static List<PolarPointRssiExtended> ExtractLinesFromCurvature(List<PolarPointRssiExtended> ptList, List<PolarCourbure> curvatureList, double seuilCourbure = 1.01)
        {
            List<PolarPointRssiExtended> linePoints = new List<PolarPointRssiExtended>();

            for (int i = 0; i < curvatureList.Count; i++)
            {

                if (curvatureList[i].Courbure < seuilCourbure)
                {
                    linePoints.Add(ptList[i]);
                }
            }
            return linePoints;
        }

        public static List<SegmentExtended> ExtractSegmentsFromCurvature(List<PolarPointRssiExtended> ptList, List<PolarCourbure> curvatureList, double seuilCourbure = 1.01, double minimum_distance = 0.3)
        {
            List<SegmentExtended> segmentList = new List<SegmentExtended>();


            bool segmentEnCours = false;
            PolarPointRssiExtended ptDebutSegmentCourant = new PolarPointRssiExtended(new PolarPointRssi(), 1, Color.White);
            PolarPointRssiExtended ptFinSegmentCourant = new PolarPointRssiExtended(new PolarPointRssi(), 1, Color.White);

            for (int i = 0; i < curvatureList.Count; i++)
            {
                if (curvatureList[i].Courbure < seuilCourbure && Toolbox.Distance(ptList[i].Pt, ptList[Math.Max(0, i - 1)].Pt) < 3)
                {
                    if (segmentEnCours == false)
                    {
                        //On a un nouveau segment
                        ptDebutSegmentCourant = ptList[i];
                    }
                    //linePoints.Add(ptList[i]);
                    segmentEnCours = true;
                }
                else
                {
                    if (segmentEnCours == true)
                    {
                        //On a une fin de segment
                        ptFinSegmentCourant = ptList[i - 1];
                        PointD segment_start_point = new PointD(ptDebutSegmentCourant.Pt.Distance * Math.Cos(ptDebutSegmentCourant.Pt.Angle), ptDebutSegmentCourant.Pt.Distance * Math.Sin(ptDebutSegmentCourant.Pt.Angle));
                        PointD segment_end_point = new PointD(ptFinSegmentCourant.Pt.Distance * Math.Cos(ptFinSegmentCourant.Pt.Angle), ptFinSegmentCourant.Pt.Distance * Math.Sin(ptFinSegmentCourant.Pt.Angle));
                        if (Toolbox.Distance(segment_start_point, segment_end_point) >= minimum_distance)
                        {
                            segmentList.Add(new SegmentExtended(segment_start_point, segment_end_point, Color.Orange, 5));
                        }

                    }
                    segmentEnCours = false;
                }
            }
            return segmentList;
        }

        public static List<SegmentExtended> SetColorsOfSegments(List<SegmentExtended> segments_objets)
        {
            int i;
            for (i = 0; i < segments_objets.Count; i++)
            {
                Color color = Toolbox.HLSToColor((35 * i) % 240, 120, 240);
                segments_objets[i].Color = color;

            }

            return segments_objets;
        }

        public static List<SegmentExtended> SetColorOfFamily(List<List<SegmentExtended>> list_of_family)
        {
            List<SegmentExtended> list_of_segments = new List<SegmentExtended>();
            int i = 0;
            foreach (List<SegmentExtended> family in list_of_family)
            {
                List<SegmentExtended> family_colorised = family;
                Color color = Toolbox.HLSToColor((35 * i++) % 240, 120, 240);

                family_colorised.ForEach(x => x.Color = color);
                list_of_segments.AddRange(family_colorised);
            }

            return list_of_segments;

        }

        /// <summary>
        /// Return a list of Segment where all parallel and continuous segment are Merge
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static List<SegmentExtended> MergeSegment(List<SegmentExtended> segments, double threshold)
        {
            List<SegmentExtended> merged_segment;

            merged_segment = new List<SegmentExtended>();

            for (int i=0; i < segments.Count; i++)
            {
                /// On ajoute le segment courant à la liste des segments fusionnés
                /// il faudra donc éliminer les segments mergeable au fur et à mesure de l'algo
                merged_segment.Add(segments[i]);
                for (int j = i+1; j < segments.Count; j++)
                {
                    if (testIfSegmentAreParrallel(merged_segment[i], segments[j]))
                    {
                        /// Les segments sont bien parallèles
                        if (Toolbox.DistancePointToLine(new PointD(segments[j].Segment.X1, segments[j].Segment.Y1),
                                                        new PointD(merged_segment[i].Segment.X1, merged_segment[i].Segment.Y1),
                                                        new PointD(merged_segment[i].Segment.X2, merged_segment[i].Segment.Y2)) < threshold)
                        {
                            /// Le pt 1 appartient au merged_segment[i]
                            if (Toolbox.DistancePointToLine(new PointD(segments[j].Segment.X2, segments[j].Segment.Y2),
                                                        new PointD(merged_segment[i].Segment.X1, merged_segment[i].Segment.Y1),
                                                        new PointD(merged_segment[i].Segment.X2, merged_segment[i].Segment.Y2)) < threshold)
                            {
                                /// Le pt 2 appartient au merged_segment[i]
                                /// On fusionne les segments !!!
                                double xMin = Math.Min(Math.Min(merged_segment[i].Segment.X1, merged_segment[i].Segment.X2), Math.Min(segments[j].Segment.X1, segments[j].Segment.X2));
                                double xMax = Math.Max(Math.Max(merged_segment[i].Segment.X1, merged_segment[i].Segment.X2), Math.Max(segments[j].Segment.X1, segments[j].Segment.X2));
                                double yMin = Math.Min(Math.Min(merged_segment[i].Segment.Y1, merged_segment[i].Segment.Y2), Math.Min(segments[j].Segment.Y1, segments[j].Segment.Y2));
                                double yMax = Math.Max(Math.Max(merged_segment[i].Segment.Y1, merged_segment[i].Segment.Y2), Math.Max(segments[j].Segment.Y1, segments[j].Segment.Y2));

                                if (Toolbox.ModuloPiAngleRadian(Math.Atan2(merged_segment[i].Segment.Y2 - merged_segment[i].Segment.Y1, merged_segment[i].Segment.X2 - merged_segment[i].Segment.X1)) > 0)
                                    merged_segment[i] = new SegmentExtended(new PointD(xMin, yMin), new PointD(xMax, yMax), merged_segment[i].Color, merged_segment[i].Width);
                                else
                                    merged_segment[i] = new SegmentExtended(new PointD(xMin, yMax), new PointD(xMax, yMin), merged_segment[i].Color, merged_segment[i].Width);

                                /// On supprime le segment fusionné
                                segments.RemoveAt(j);
                            }
                        }
                    }
                }
            }

            return merged_segment;
        }

        public static List<List<SegmentExtended>> FindFamilyOfSegment(List<SegmentExtended> list_of_segments, double minimum_size = 0.0)
        {
            List<SegmentExtended> copy_of_list_of_segment = list_of_segments;
            List<List<SegmentExtended>> list_of_family = new List<List<SegmentExtended>>();

            while (copy_of_list_of_segment.Count > 0)
            {
                SegmentExtended selected_segment = copy_of_list_of_segment[0];
                if (Toolbox.Distance(selected_segment.Segment.X1, selected_segment.Segment.Y1, selected_segment.Segment.X2, selected_segment.Segment.Y2) > minimum_size)
                {
                    int index_of_this_family = list_of_family.Count;
                    list_of_family.Add(new List<SegmentExtended>());

                    list_of_family[index_of_this_family].Add(selected_segment);

                    for (int j = 0; j < copy_of_list_of_segment.Count; j++)
                    {
                        SegmentExtended tested_segment = copy_of_list_of_segment[j];
                        if (testIfSegmentAreParrallel(selected_segment, tested_segment) || testIfSegmentArePerpendicular(selected_segment, tested_segment))
                        {
                            list_of_family[index_of_this_family].Add(tested_segment);
                        }
                    }

                    foreach (SegmentExtended element in list_of_family[index_of_this_family])
                    {
                        copy_of_list_of_segment.Remove(element);
                    }
                }
                else
                {
                    copy_of_list_of_segment.RemoveAt(0);
                }
            }
            return list_of_family;
        }


        public static bool testIfSegmentAreParrallel(SegmentExtended segment_1, SegmentExtended segment_2, double thresold = 3 * (Math.PI / 180))
        {
            double segment_1_angle = Toolbox.ModuloPiAngleRadian(Math.Atan2(segment_1.Segment.Y2 - segment_1.Segment.Y1, segment_1.Segment.X2 - segment_1.Segment.X1));
            double segment_2_angle = Toolbox.ModuloPiAngleRadian(Math.Atan2(segment_2.Segment.Y2 - segment_2.Segment.Y1, segment_2.Segment.X2 - segment_2.Segment.X1));

            if (segment_1_angle - thresold <= segment_2_angle && segment_1_angle + thresold >= segment_2_angle)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool testIfSegmentArePerpendicular(SegmentExtended segment_1, SegmentExtended segment_2, double thresold = 3 * (Math.PI / 180))
        {
            double segment_1_angle = Toolbox.ModuloPiAngleRadian(Math.Atan2(segment_1.Segment.Y2 - segment_1.Segment.Y1, segment_1.Segment.X2 - segment_1.Segment.X1));
            double segment_2_angle = Toolbox.ModuloPiAngleRadian(Toolbox.ModuloPiAngleRadian(Math.Atan2(segment_2.Segment.Y2 - segment_2.Segment.Y1, segment_2.Segment.X2 - segment_2.Segment.X1)) + Math.PI / 2);

            if (segment_1_angle - thresold <= segment_2_angle && segment_1_angle + thresold >= segment_2_angle)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Implementation of the Ramer–Douglas–Peucker algorithm, which consist for reducing the nomber of usefull point for creating a line;
        /// en.wikipedia.org/wiki/Ramer-Douglas-Peucker_algorithm
        /// </summary>
        /// <param name="list_of_points"></param>
        /// <param name="epsilon"></param>
        public static List<PointD> IEPF_Algorithm(List<PointD> list_of_points, double epsilon)
        {
            double dmax = 0;
            int index = 0;
            int end = list_of_points.Count();

            PointD first_point = list_of_points[0];
            PointD end_point = list_of_points[end - 1];
            double angle = Math.Atan2(first_point.Y - end_point.Y, first_point.X - end_point.X);

            for (int i = 1; i < end - 1; i++)
            {
                double distance = Toolbox.DistancePointToLine(list_of_points[i], first_point, angle);
                if (distance > dmax)
                {
                    index = i;
                    dmax = distance;
                }

            }

            List<PointD> ResultList = new List<PointD>();

            /// If max dist is greater than epsilon -> Recursively Simplify
            if (dmax > epsilon)
            {
                List<PointD> recursiveResult1 = IEPF_Algorithm(list_of_points.GetRange(0, index), epsilon);
                List<PointD> recursiveResult2 = IEPF_Algorithm(list_of_points.GetRange(index + 1, end - 1 - index), epsilon);

                ResultList.AddRange(recursiveResult1);
                ResultList.AddRange(recursiveResult2);

            }
            else
            {
                ResultList.Add(list_of_points[0]);
                ResultList.Add(list_of_points[end - 1]);
            }
            return ResultList;
        }

        public static List<SegmentExtended> MergeSegmentWithLSM(List<SegmentExtended> list_of_segments, double spatial_distance, double tau_theta)
        {
            int n;
            List<SegmentExtended> list_of_merged_segments = list_of_segments;

            do
            {
                n = list_of_merged_segments.Count();

                list_of_merged_segments = list_of_merged_segments.OrderByDescending(x => Toolbox.Distance(x)).ToList();

                for (int i = 0; i < list_of_merged_segments.Count; i++)
                {
                    SegmentExtended L1 = list_of_merged_segments[i];

                    double l1 = Toolbox.Distance(L1);
                    double theta_1 = Toolbox.Angle(L1);

                    double tau_s = spatial_distance * l1;

                    double x_11 = L1.Segment.X1;
                    double x_12 = L1.Segment.X2;
                    double y_11 = L1.Segment.Y1;
                    double y_12 = L1.Segment.Y2;

                    List<SegmentExtended> P = list_of_merged_segments.Where(x => Math.Abs(Toolbox.Angle(x) - theta_1) < tau_theta).ToList();
                    P = P.Where(
                        s => Math.Abs(x_11 - s.Segment.X1) < tau_s || Math.Abs(x_11 - s.Segment.X2) < tau_s || Math.Abs(x_12 - s.Segment.X1) < tau_s || Math.Abs(x_12 - s.Segment.X2) < tau_s
                    ).ToList();

                    P = P.Where(
                        s => Math.Abs(y_11 - s.Segment.Y1) < tau_s || Math.Abs(y_11 - s.Segment.Y2) < tau_s || Math.Abs(y_12 - s.Segment.Y1) < tau_s || Math.Abs(y_12 - s.Segment.Y2) < tau_s
                    ).ToList();

                    List<SegmentExtended> R = new List<SegmentExtended>();
                    foreach (SegmentExtended L2 in P)
                    {
                        SegmentExtended M = LSMSegmentMerger(L1, L2, spatial_distance, tau_theta);
                        if (M != null)
                        {
                            L1 = M;
                            list_of_merged_segments[i] = M;
                            R.Add(L2);
                        }
                    }

                    R.ForEach(x => list_of_merged_segments.Remove(x));
                }
            }
            while (n != list_of_merged_segments.Count());


            return list_of_merged_segments;
        }

        /// <summary>
        /// Implemenation of segment merger "LSM: perceptually accurate linesegment merging" 
        /// https://doi.org/10.1117/1.JEI.25.6.061620
        /// </summary>
        /// <returns>Return the merged Segment or null if the condition are not </returns>
        public static SegmentExtended LSMSegmentMerger(SegmentExtended Global_L1, SegmentExtended Global_L2, double xi_s, double tau_theta)
        {
            SegmentExtended L1 = Global_L1;
            SegmentExtended L2 = Global_L2;

            double l1 = Toolbox.Distance(L1);
            double l2 = Toolbox.Distance(L2);

            double theta1 = Toolbox.Angle(L1);
            double theta2 = Toolbox.Angle(L2);

            if (l1 < l2)
            {
                /// Yeah i know i could have made it here but for now, it's fancier to do this 
                Toolbox.SwapNum(ref L1,ref L2);
                Toolbox.SwapNum(ref l1, ref l2);
                Toolbox.SwapNum(ref theta1, ref theta2);
            }

            PointD L1_a = new PointD(L1.Segment.X1, L1.Segment.Y1);
            PointD L1_b = new PointD(L1.Segment.X2, L1.Segment.Y2);
            PointD L2_a = new PointD(L2.Segment.X1, L2.Segment.Y1);
            PointD L2_b = new PointD(L2.Segment.X2, L2.Segment.Y2);

            /// Now we Compute d (c1 and c2 are useless)
            Tuple<PointD, PointD, double> F1F2D = ComputeAndReturnF1F2AndD(L1_a, L1_b, L2_a, L2_b);

            PointD f1 = F1F2D.Item1;
            PointD f2 = F1F2D.Item2;
            double d = F1F2D.Item3;


            double tau_s = xi_s * l1;

            if (d > tau_s)
            {
                /// We don't merge
                return null;
            }

            /// Now we compute the adaptive angluar thresold tau_theta_prime
            double normalise_l2 = l2 / l1;
            double normalise_d = d / tau_s;

            double lambda = normalise_l2 + normalise_d;

            double tau_theta_prime = (1 - 1 / (1 + Math.Exp(-2 * (lambda - 1.5)))) * tau_theta;

            double theta = Math.Abs(theta2 - theta1);

            if ((theta < tau_theta_prime) || (theta > (Math.PI - tau_theta_prime)))
            {
                SegmentExtended M = new SegmentExtended(f1, f2, L1.Color, L1.Width);
                double theta_M = Toolbox.Angle(M);

                if (Math.Abs(theta1 - theta_M) <= tau_theta / 2)
                {
                    /// We merge
                    return M;
                }
            }
            
            return null;
        }

        private static Tuple<PointD, PointD, double> ComputeAndReturnF1F2AndD(PointD L1_a, PointD L1_b, PointD L2_a, PointD L2_b)
        {
            double d_a_a = Toolbox.Distance(L1_a, L2_a);
            double d_a_b = Toolbox.Distance(L1_a, L2_b);
            double d_b_a = Toolbox.Distance(L1_b, L2_a);
            double d_b_b = Toolbox.Distance(L1_b, L2_b);

            double d = Math.Min(Math.Min(d_a_a, d_a_b), Math.Min(d_b_a, d_b_b));

            PointD f1, f2;

            if (d == d_a_a)
            {
                f1 = L1_b;
                f2 = L2_b;
            }
            else if (d == d_a_b)
            {
                f1 = L1_b;
                f2 = L2_a;
            }
            else if (d == d_b_a)
            {
                f1 = L1_a;
                f2 = L2_b;
            }
            else
            {
                 f1 = L1_a;
                 f2 = L2_a;
            }

            return new Tuple<PointD, PointD, double>(f1, f2, d);
        }
    }
}
