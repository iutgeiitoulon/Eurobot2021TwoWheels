using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using MIConvexHull;
using System.Drawing;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Constants;

namespace LidarProcessNS
{
    // C# program to find largest rectangle
    // with all 1s in a binary matrix
    using System;
    using System.Collections.Generic;

    public static class FindRectangle
    {
        public static Location GetBestAngularLocation(List<Location> list_of_locations, Location actual_location)
        {
            return list_of_locations.OrderBy(x => Math.Abs(Toolbox.Modulo2PiAngleRad(x.Theta) - Toolbox.Modulo2PiAngleRad(actual_location.Theta))).ToList()[0];
        }

        public static Location GetBestDistanceLocation(List<Location> list_of_locations, Location actual_location)
        {
            return list_of_locations.OrderBy(x => Toolbox.Distance(new PointD(x.X, x.Y), new PointD(actual_location.X, actual_location.Y))).FirstOrDefault();
        }

        public static List<Location> ListAllPossibleLocation(RectangleOriented rectangle)
        {
            Tuple<PointD, PointD, PointD, PointD> corners = Toolbox.GetCornerOfAnOrientedRectangle(rectangle);

            PointD corner_1 = corners.Item1;
            PointD corner_2 = corners.Item2;
            PointD corner_3 = corners.Item3;
            PointD corner_4 = corners.Item4;


            bool farthest_point_is_1 = Toolbox.ConvertPointDToPolar(corner_1).Distance > Toolbox.ConvertPointDToPolar(corner_2).Distance;
            bool corner_1_and_2_is_height = Math.Round(Toolbox.Distance(corner_1, corner_2)) == ConstVar.HEIGHT_BOXSIZE; // Math.Abs(rotation_angle) > Math.Acos(ConstVar.HEIGHT_BOXSIZE / Math.Sqrt(Math.Pow(ConstVar.HEIGHT_BOXSIZE, 2) + Math.Pow(ConstVar.WIDTH_BOXSIZE, 2)))



            double rotation_angle = rectangle.Angle;

            Matrix<double> rotation_matrix = DenseMatrix.OfArray(new double[,] {
                    { Math.Cos(rotation_angle), - Math.Sin(rotation_angle) },
                    { Math.Sin(rotation_angle), Math.Cos(rotation_angle) }
            });

            Vector<double> ref_center = Vector<double>.Build.DenseOfArray(new double[] { rectangle.Center.X, rectangle.Center.Y }) * rotation_matrix;
            PointD ref_center_point = new PointD(ref_center[0], ref_center[1]);

            Vector<double> ref_pt1 = Vector<double>.Build.DenseOfArray(new double[] { corners.Item1.X, corners.Item1.Y }) * rotation_matrix;
            Vector<double> ref_pt2 = Vector<double>.Build.DenseOfArray(new double[] { corners.Item2.X, corners.Item2.Y }) * rotation_matrix;
            Vector<double> ref_pt3 = Vector<double>.Build.DenseOfArray(new double[] { corners.Item3.X, corners.Item3.Y }) * rotation_matrix;
            Vector<double> ref_pt4 = Vector<double>.Build.DenseOfArray(new double[] { corners.Item4.X, corners.Item4.Y }) * rotation_matrix;

            PointD pt1 = new PointD(0, 0);

            PointD ref_robot_point = new PointD(-ref_center[0], -ref_center[1]);

            if (rotation_angle > 0 && !corner_1_and_2_is_height)
            {
                ref_center_point.X *= -1;
                ref_center_point.Y *= -1;
            }
                

            //if ()
            //    ref_center_point.X *= -1;

            Location location_1, location_2, location_3, location_4;

            //location_1 = new Location(ref_pt1[0] - ref_center_point.X, ref_pt1[1] - ref_center_point.Y, 0, 0, 0, 0);
            //location_2 = new Location(ref_pt1[0] - ref_center_point.X, ref_pt1[1] - ref_center_point.Y, 0, 0, 0, 0);
            //location_3 = new Location(ref_pt1[0] - ref_center_point.X, ref_pt1[1] - ref_center_point.Y, 0, 0, 0, 0);
            //location_4 = new Location(ref_pt1[0] - ref_center_point.X, ref_pt1[1] - ref_center_point.Y, 0, 0, 0, 0);
            if (rotation_angle > 0 && !corner_1_and_2_is_height)
            {
                location_1 = new Location(pt1.X - ref_center_point.X, pt1.Y - ref_center_point.Y, -rotation_angle + Math.PI, 0, 0, 0);
                location_4 = new Location(pt1.X + ref_center_point.X, pt1.Y + ref_center_point.Y, -rotation_angle , 0, 0, 0);
            }
            else
            {
                location_1 = new Location(pt1.X - ref_center_point.X, pt1.Y - ref_center_point.Y, -rotation_angle, 0, 0, 0);
                location_4 = new Location(pt1.X + ref_center_point.X, pt1.Y + ref_center_point.Y, -rotation_angle + Math.PI, 0, 0, 0);
            }
           

            return new List<Location>() { location_1, location_4 };

        }

        public static Tuple<RectangleOriented, RectangleOriented, RectangleOriented> ListResisableRectangle(RectangleOriented rectangle, double thresold)
        {
            double  width_correction_angle_1, height_correction_angle_1, 
                    width_correction_angle_2, height_correction_angle_2, 
                    width_correction_distance_1, height_correction_distance_1, 
                    width_correction_distance_2, height_correction_distance_2;

            double Width = Math.Max(rectangle.Lenght, rectangle.Width);
            double Height = Math.Min(rectangle.Lenght, rectangle.Width);
            double Angle = rectangle.Angle;

            width_correction_distance_1 = (ConstVar.WIDTH_BOXSIZE - Height) / 2;
            height_correction_distance_1 = (ConstVar.HEIGHT_BOXSIZE - Width) / 2;

            width_correction_distance_2 = (ConstVar.WIDTH_BOXSIZE - Width) / 2;
            height_correction_distance_2 = (ConstVar.HEIGHT_BOXSIZE - Height) / 2;


            if (Width >= ConstVar.HEIGHT_BOXSIZE - thresold)
            {
                //width_correction_distance_1 = 0;
                height_correction_distance_1 = 0;
            }

            if (Height >= ConstVar.HEIGHT_BOXSIZE - thresold)
            {
                height_correction_distance_2 = 0;
                //width_correction_distance_2 = 0;
            }

            /// 1st
            width_correction_angle_1 = Toolbox.ModuloPiAngleRadian(Angle - Math.PI / 2) + Math.PI;
            height_correction_angle_1 = Toolbox.ModuloPiAngleRadian(Angle) + Math.PI;

            PointD width_correction_point_1 = Toolbox.ConvertPolarToPointD(new PolarPointRssi(width_correction_angle_1, width_correction_distance_1, 0));
            PointD height_correction_point_1 = Toolbox.ConvertPolarToPointD(new PolarPointRssi(height_correction_angle_1, height_correction_distance_1, 0));

            PointD correct_center_point_1 = new PointD(rectangle.Center.X + width_correction_point_1.X + height_correction_point_1.X, rectangle.Center.Y + width_correction_point_1.Y + height_correction_point_1.Y);


            /// 2nd & 3rd
            width_correction_angle_2 = Toolbox.ModuloPiAngleRadian(Angle) + Math.PI;
            height_correction_angle_2 = Toolbox.ModuloPiAngleRadian(Angle - Math.PI / 2) + Math.PI;

            PointD width_correction_point_2 = Toolbox.ConvertPolarToPointD(new PolarPointRssi(width_correction_angle_2, width_correction_distance_2, 0));
            PointD height_correction_point_2 = Toolbox.ConvertPolarToPointD(new PolarPointRssi(height_correction_angle_2, height_correction_distance_2, 0));

            PointD correct_center_point_2 = new PointD(rectangle.Center.X + width_correction_point_2.X + height_correction_point_2.X, rectangle.Center.Y + width_correction_point_2.Y + height_correction_point_2.Y);
            PointD correct_center_point_3 = new PointD(rectangle.Center.X - width_correction_point_2.X + height_correction_point_2.X, rectangle.Center.Y - width_correction_point_2.Y + height_correction_point_2.Y);



            RectangleOriented rectangle1 = new RectangleOriented(correct_center_point_1, ConstVar.WIDTH_BOXSIZE, ConstVar.HEIGHT_BOXSIZE, Angle + Math.PI / 2);
            RectangleOriented rectangle2 = new RectangleOriented(correct_center_point_2, ConstVar.WIDTH_BOXSIZE, ConstVar.HEIGHT_BOXSIZE, Angle);
            RectangleOriented rectangle3 = new RectangleOriented(correct_center_point_3, ConstVar.WIDTH_BOXSIZE, ConstVar.HEIGHT_BOXSIZE, Angle);

            return new Tuple<RectangleOriented, RectangleOriented, RectangleOriented>(rectangle1, rectangle2, rectangle3);
        }

        public static RectangleOriented ResizeRectangle(RectangleOriented rectangle, double thresold)
        {
            Tuple<PointD, PointD, PointD, PointD> corners = Toolbox.GetCornerOfAnOrientedRectangle(rectangle);

            PointD point_1 = corners.Item1;
            PointD point_2 = corners.Item2;
            PointD point_3 = corners.Item3;
            PointD point_4 = corners.Item4;

            double Width = Math.Max(rectangle.Lenght, rectangle.Width);
            double Height = Math.Min(rectangle.Lenght, rectangle.Width);
            double Angle = rectangle.Angle;

            PointD correct_center_point = new PointD(0, 0);

            if (Width >= ConstVar.WIDTH_BOXSIZE - thresold)
            {
                if (Height >= ConstVar.HEIGHT_BOXSIZE - thresold)
                {
                    /// Whe have the full box
                    correct_center_point = rectangle.Center;
                }
                else
                {

                    /// We resize the Height of the box
                    double correction_angle;
                    double correction_distance = (ConstVar.HEIGHT_BOXSIZE - Height) / 2;

                    if (Toolbox.Distance(point_1, point_2) > ConstVar.HEIGHT_BOXSIZE - thresold)
                        correction_angle = Toolbox.ModuloPiAngleRadian(Toolbox.Angle(point_1, point_2) - Math.PI / 2) + Math.PI;
                    else
                        correction_angle = Toolbox.ModuloPiAngleRadian(Toolbox.Angle(point_1, point_2)) + Math.PI;

                    PointD correction_point = Toolbox.ConvertPolarToPointD(new PolarPointRssi(correction_angle, correction_distance, 0));

                    correct_center_point = new PointD(rectangle.Center.X + correction_point.X, rectangle.Center.Y + correction_point.Y);
                }
            }
            else if (Width >= ConstVar.HEIGHT_BOXSIZE + thresold)
            {
                if (Height >= ConstVar.HEIGHT_BOXSIZE - thresold)
                {
                    /// Whe only resize the Width
                    double correction_angle;
                    double correction_distance = (ConstVar.WIDTH_BOXSIZE - Width) / 2;

                    if (Toolbox.Distance(point_1, point_2) <= ConstVar.HEIGHT_BOXSIZE + thresold)
                        correction_angle = Toolbox.ModuloPiAngleRadian(Toolbox.Angle(point_1, point_2) - Math.PI / 2) + Math.PI;
                    else
                        correction_angle = Toolbox.ModuloPiAngleRadian(Toolbox.Angle(point_1, point_2)) + Math.PI;

                    PointD correction_point = Toolbox.ConvertPolarToPointD(new PolarPointRssi(correction_angle, correction_distance, 0));

                    correct_center_point = new PointD(rectangle.Center.X + correction_point.X, rectangle.Center.Y + correction_point.Y);
                }
                else
                {
                    /// Whe resize the Width
                    double width_correction_angle, height_correction_angle;
                    double width_correction_distance = (ConstVar.WIDTH_BOXSIZE - Width) / 2;
                    double height_correction_distance = (ConstVar.HEIGHT_BOXSIZE - Height) / 2;

                    if (Toolbox.Distance(point_1, point_2) <= ConstVar.HEIGHT_BOXSIZE + thresold)
                    {
                        width_correction_angle = Toolbox.ModuloPiAngleRadian(Toolbox.Angle(point_1, point_2) - Math.PI / 2) + Math.PI;
                        height_correction_angle = Toolbox.ModuloPiAngleRadian(Toolbox.Angle(point_1, point_2)) + Math.PI;
                    }
                    else
                    {
                        width_correction_angle = Toolbox.ModuloPiAngleRadian(Toolbox.Angle(point_1, point_2)) + Math.PI;
                        height_correction_angle = Toolbox.ModuloPiAngleRadian(Toolbox.Angle(point_1, point_2) - Math.PI / 2) + Math.PI;
                    }

                    PointD width_correction_point = Toolbox.ConvertPolarToPointD(new PolarPointRssi(width_correction_angle, width_correction_distance, 0));
                    PointD height_correction_point = Toolbox.ConvertPolarToPointD(new PolarPointRssi(height_correction_angle, height_correction_distance, 0));

                    correct_center_point = new PointD(rectangle.Center.X + width_correction_point.X + height_correction_point.X, rectangle.Center.Y + width_correction_point.Y + height_correction_point.Y);
                }
            }
            else
            {
                if (Math.Abs(Toolbox.ModuloPiAngleRadian(Angle + Math.PI / 2)) <= Math.Acos(ConstVar.WIDTH_BOXSIZE / Math.Sqrt(Math.Pow(ConstVar.WIDTH_BOXSIZE, 2) + Math.Pow(ConstVar.HEIGHT_BOXSIZE, 2))) && Width >= ConstVar.HEIGHT_BOXSIZE - thresold)
                {
                    return null;
                    //if (Width <= ConstVar.HEIGHT_BOXSIZE - thresold)
                    //	Console.WriteLine("Fuck");

                    //return null; // Not Safe Need Improvement
                    //if (Toolbox.Distance(point_1, point_2) < ConstVar.HEIGHT_BOXSIZE - thresold || Toolbox.Distance(point_1, point_2) > ConstVar.HEIGHT_BOXSIZE + thresold) // Not Good -> False Positive + False Negative
                    //	return null;


                    /// Whe only resize the Width
                    double correction_angle = Toolbox.ModuloPiAngleRadian(Toolbox.Angle(point_1, point_2) - Math.PI / 2) + Math.PI;
                    double correction_distance = (ConstVar.WIDTH_BOXSIZE - Height) / 2;

                    PointD correction_point = Toolbox.ConvertPolarToPointD(new PolarPointRssi(correction_angle, correction_distance, 0));

                    Angle += Math.PI / 2;

                    correct_center_point = new PointD(rectangle.Center.X + correction_point.X, rectangle.Center.Y + correction_point.Y);
                }
                else
                    return null;
            }

            return new RectangleOriented(correct_center_point, ConstVar.WIDTH_BOXSIZE, ConstVar.HEIGHT_BOXSIZE, Angle);
        }


        public static List<PointD> FindValidCorners(RectangleOriented rectangle, List<PointD> list_of_points, double thresold, double min_points)
        {
            List<PointD> list_of_valid_corners = new List<PointD>();
            Tuple<PointD, PointD, PointD, PointD> corners = Toolbox.GetCornerOfAnOrientedRectangle(rectangle);
            double angle = Math.Abs(Toolbox.ModuloPiAngleRadian(rectangle.Angle));

            double[,] point_array = new double[list_of_points.Count, 2];
            for (int i = 0; i < list_of_points.Count; i++)
            {
                point_array[i, 0] = list_of_points[i].X;
                point_array[i, 1] = list_of_points[i].Y;
            }
            Matrix<double> points_matrix = DenseMatrix.OfArray(point_array);



            Matrix<double> rotation_matrix = DenseMatrix.OfArray(new double[,] {
                    { Math.Cos(angle), Math.Cos(angle - Math.PI / 2) },
                    { Math.Cos(angle + Math.PI / 2), Math.Cos(angle) }
            });

            Matrix<double> rotated_points = rotation_matrix * points_matrix.Transpose();
            Vector<double> rotated_center = rotation_matrix * Vector<double>.Build.DenseOfArray(new double[] { rectangle.Center.X, rectangle.Center.Y });

            double min_x = rotated_center[0] - rectangle.Width / 2;
            double max_x = rotated_center[0] + rectangle.Width / 2;

            double min_y = rotated_center[1] - rectangle.Lenght / 2;
            double max_y = rotated_center[1] + rectangle.Lenght / 2;

            int right_border = 0, left_border = 0, top_border = 0, bottom_border = 0;

            for (int i = 0; i < rotated_points.ColumnCount; i++)
            {
                double current_x = rotated_points[0, i];
                double current_y = rotated_points[1, i];

                if (Math.Abs(min_x - current_x) <= thresold)
                    left_border++;
                if (Math.Abs(max_x - current_x) <= thresold)
                    right_border++;
                if (Math.Abs(min_y - current_y) <= thresold)
                    top_border++;
                if (Math.Abs(max_y - current_y) <= thresold)
                    bottom_border++;
            }

            if (top_border >= min_points && right_border >= min_points)
                list_of_valid_corners.Add(corners.Item2);
            if (bottom_border >= min_points && right_border >= min_points)
                list_of_valid_corners.Add(corners.Item4);
            if (top_border >= min_points && left_border >= min_points)
                list_of_valid_corners.Add(corners.Item1);
            if (bottom_border >= min_points && left_border >= min_points)
                list_of_valid_corners.Add(corners.Item4);

            return list_of_valid_corners;
        }

        public static RectangleOriented FindMbrBoxByArea(List<PointD> list_of_points)
        {
            if (list_of_points.Count <= 1)
                return new RectangleOriented(new PointD(0, 0), 0, 0, 0);

            ConvexHullCreationResult<DefaultVertex2D> hull = ConvexHull.Create2D(list_of_points.Select(x => new double[2] { x.X, x.Y }).ToList());

            double[,] hull_array = new double[hull.Result.Count, 2];

            for (int i = 0; i < hull.Result.Count; i++)
            {
                hull_array[i, 0] = hull.Result[i].X;
                hull_array[i, 1] = hull.Result[i].Y;
            }

            Matrix<double> hull_matrix = DenseMatrix.OfArray(hull_array);

            List<double> edge_angles = Rotating_Caliper(hull_array);

            double? min_area = null;
            RectangleOriented best_rectangle = new RectangleOriented();
            /// Test each angle to find bounding box with smallest area
            foreach (double angle in edge_angles)
            {
                RectangleOriented rectangle = FindMinimumBoundingRectangle(hull_matrix, angle);


                double area = rectangle.Width * rectangle.Lenght;

                if (area < min_area || min_area == null)
                {
                    min_area = area;
                    best_rectangle = rectangle;
                }
            }

            return best_rectangle;
        }


        public static RectangleOriented FindMbrBoxByOverlap(List<PointD> list_of_points)
        {
            ConvexHullCreationResult<DefaultVertex2D> hull = ConvexHull.Create2D(list_of_points.Select(x => new double[2] { x.X, x.Y }).ToList());

            if (list_of_points.Count <= 1)
                return new RectangleOriented(new PointD(0, 0), 0, 0, 0);

            double[,] hull_array = new double[hull.Result.Count, 2];

            for (int i = 0; i < hull.Result.Count; i++)
            {
                hull_array[i, 0] = hull.Result[i].X;
                hull_array[i, 1] = hull.Result[i].Y;
            }

            Matrix<double> hull_matrix = DenseMatrix.OfArray(hull_array);

            List<double> edge_angles = Rotating_Caliper(hull_array);

            int? max_overlap = null;
            RectangleOriented best_rectangle = new RectangleOriented();
            /// Test each angle to find bounding box with smallest area
            foreach (double angle in edge_angles)
            {
                RectangleOriented rectangle = FindMinimumBoundingRectangle(hull_matrix, angle);


                int overlap = FindAllBorderPoints(list_of_points, rectangle, 0.05).Count;

                if (overlap > max_overlap || max_overlap == null)
                {
                    max_overlap = overlap;
                    best_rectangle = rectangle;
                }
            }

            return best_rectangle;
        }



        /// <summary>
        /// Implementation of the Rotating Calipers Algorithm
        /// </summary>
        /// <param name="list_of_points"></param>
        /// <returns></returns>
        public static List<double> Rotating_Caliper(double[,] hull_array)
        {
            List<Tuple<double, double>> edges = new List<Tuple<double, double>>();

            /// Compute edges (x2 - x1, y2 - y1)
            for (int i = 0; i < hull_array.Length / 2 - 1; i++)
            {
                double edge_x = hull_array[i + 1, 0] - hull_array[i, 0];
                double edge_y = hull_array[i + 1, 1] - hull_array[i, 1];

                edges.Add(new Tuple<double, double>(edge_x, edge_y));
            }

            /// Calculate Edges Angles
            List<double> edge_angles = edges.Select(x => Math.Atan2(x.Item2, x.Item1)).ToList();

            /// Check for angle in 1st quadrant
            edge_angles.ForEach(x => Math.Abs(x % (Math.PI / 2)));


            /// Remove Duplicates
            return edge_angles.Distinct().ToList();
        }

        public static RectangleOriented FindMinimumBoundingRectangle(Matrix<double> hull_matrix, double angle)
        {
            /// Create rotation matrix to shift points to baseline
            /// R = [cos(theta)      , cos(theta-PI/2)]
            ///		[cos(theta+PI/2) , cos(theta)     ]
            Matrix<double> rotation_matrix = DenseMatrix.OfArray(new double[,] {
                    { Math.Cos(angle), Math.Cos(angle - Math.PI / 2) },
                    { Math.Cos(angle + Math.PI / 2), Math.Cos(angle) }
                });

            Matrix<double> rotated_points = rotation_matrix * hull_matrix.Transpose();

            double max_x = rotated_points[0, 0];
            double max_y = rotated_points[1, 0];
            double min_x = rotated_points[0, 0];
            double min_y = rotated_points[1, 0];

            for (int i = 0; i < rotated_points.ColumnCount; i++)
            {
                double current_x = rotated_points[0, i];
                double current_y = rotated_points[1, i];

                if (max_x < current_x)
                    max_x = current_x;
                if (min_x > current_x)
                    min_x = current_x;

                if (max_y < current_y)
                    max_y = current_y;
                if (min_y > current_y)
                    min_y = current_y;
            }

            double width = max_x - min_x;
            double height = max_y - min_y;

            double center_x = (min_x + max_x) / 2;
            double center_y = (min_y + max_y) / 2;

            Vector<double> center_point = Vector<double>.Build.DenseOfArray(new double[] { center_x, center_y });

            Vector<double> ref_center = center_point * rotation_matrix;

            return new RectangleOriented(new PointD(ref_center[0], ref_center[1]), width, height, angle);
        }

        public static List<RectangleOriented> FindAllPossibleRectangle(List<List<PointDExtended>> list_of_family_corners, double thresold)
        {
            List<RectangleOriented> list_of_rectangles = new List<RectangleOriented>();
            foreach (List<PointDExtended> family in list_of_family_corners)
            {
                List<int> list_of_case = Enumerable.Range(0, family.Count).ToList(); /// [0,1,2,3,...,n]
				List<List<int>> list_of_combinations_of_corner_index = Toolbox.GetKCombs(list_of_case, 2).ToList().Select(x => x.ToList()).ToList(); /// [[0,1],[0,2],[0,3],[1,2],[1,3],[2,3],...]

                                                                                                                                                     /// [[Dist_0-1, Corner0, Corner1],[Dist_0-2, Corner0,Corner2],[Dist_0-3, Corner0,Corner3],[Dist_1-2, Corner1,Corner2],etc...]
                List<Tuple<double, PointDExtended, PointDExtended>> list_of_combinations_corners_and_distance = list_of_combinations_of_corner_index.Select(
                    x => new Tuple<double, PointDExtended, PointDExtended>(
                        Toolbox.Distance(family[x[0]].Pt, family[x[1]].Pt),
                        family[x[0]],
                        family[x[1]]
                    )
                ).ToList();



                double previous_distance = 0;
                PointD previous_vector_point_a = new PointD(0, 0);
                PointD previous_vector_point_b = new PointD(0, 0);
                PointD previous_center_vector_point = new PointD(0, 0);


                /// Now we sort the list by distance (Greater -> Smaller)
                foreach (var vector_distance in list_of_combinations_corners_and_distance.OrderByDescending(x => x.Item1))
                {

                    PointD actual_vector_point_a = vector_distance.Item2.Pt;
                    PointD actual_vector_point_b = vector_distance.Item3.Pt;
                    PointD actual_center_vector_point = new PointD((actual_vector_point_b.X + actual_vector_point_a.X) / 2, (actual_vector_point_b.Y + actual_vector_point_a.Y) / 2);

                    /// We check if all points are not similar
                    if (Toolbox.Distance(actual_vector_point_a, previous_vector_point_a) != 0 && Toolbox.Distance(actual_vector_point_a, previous_vector_point_b) != 0 && Toolbox.Distance(actual_vector_point_b, previous_vector_point_a) != 0 && Toolbox.Distance(actual_vector_point_b, previous_vector_point_b) != 0)
                    {
                        /// We calculate the center of the two vector
                        /// and if the two center points are close enought whe have a rectangle
                        if (previous_distance + thresold >= vector_distance.Item1 && previous_distance - thresold <= vector_distance.Item1)
                        {
                            if (Toolbox.Distance(actual_center_vector_point, previous_center_vector_point) < thresold)
                            {
                                PointD mean_center_point = new PointD((actual_center_vector_point.X + previous_center_vector_point.X) / 2, (actual_center_vector_point.Y + previous_center_vector_point.Y) / 2);

                                double lenght = Toolbox.Distance(actual_vector_point_a, previous_vector_point_a);
                                double width = Toolbox.Distance(actual_vector_point_b, previous_vector_point_a);
                                double angle = Math.Atan2(actual_vector_point_a.Y - previous_vector_point_a.Y, actual_vector_point_a.X - previous_vector_point_a.X);

                                RectangleOriented rectangle = new RectangleOriented(mean_center_point, lenght, width, angle);
                                list_of_rectangles.Add(rectangle);
                            }
                        }
                    }
                    previous_distance = vector_distance.Item1;
                    previous_vector_point_a = actual_vector_point_a;
                    previous_vector_point_b = actual_vector_point_b;
                    previous_center_vector_point = actual_center_vector_point;
                }
            }
            return list_of_rectangles;
        }

        public static List<SegmentExtended> DrawRectangle(RectangleOriented rectangle, Color color, double width = 3)
        {
            Tuple<PointD, PointD, PointD, PointD> corners = Toolbox.GetCornerOfAnOrientedRectangle(rectangle);

            List<SegmentExtended> list_of_segments = new List<SegmentExtended>()
            {
                new SegmentExtended(corners.Item1, corners.Item2, color, width),
                new SegmentExtended(corners.Item1, corners.Item3, color, width),
                new SegmentExtended(corners.Item4, corners.Item2, color, width),
                new SegmentExtended(corners.Item4, corners.Item3, color, width),
            };


            return list_of_segments;

        }

        public static List<PointD> FindAllBorderPoints(List<PointD> list_of_points, RectangleOriented rectangle, double thresold)
        {
            double[,] point_array = new double[list_of_points.Count, 2];

            for (int i = 0; i < list_of_points.Count; i++)
            {
                point_array[i, 0] = list_of_points[i].X;
                point_array[i, 1] = list_of_points[i].Y;
            }

            Matrix<double> points_matrix = DenseMatrix.OfArray(point_array);

            double angle = rectangle.Angle;

            Matrix<double> rotation_matrix = DenseMatrix.OfArray(new double[,] {
                    { Math.Cos(angle), Math.Cos(angle - Math.PI / 2) },
                    { Math.Cos(angle + Math.PI / 2), Math.Cos(angle) }
                });

            Matrix<double> rotated_points = rotation_matrix * points_matrix.Transpose();

            Vector<double> center_point = Vector<double>.Build.DenseOfArray(new double[] { rectangle.Center.X, rectangle.Center.Y });

            Vector<double> rotated_center = rotation_matrix * center_point;

            double min_x = rotated_center[0] - rectangle.Width / 2;
            double max_x = rotated_center[0] + rectangle.Width / 2;

            double min_y = rotated_center[1] - rectangle.Lenght / 2;
            double max_y = rotated_center[1] + rectangle.Lenght / 2;

            bool[] list_of_rotated_border_points = new bool[rotated_points.ColumnCount];

            for (int i = 0; i < rotated_points.ColumnCount; i++)
            {
                double current_x = rotated_points[0, i];
                double current_y = rotated_points[1, i];

                if (Math.Abs(min_x - current_x) <= thresold || Math.Abs(max_x - current_x) <= thresold)
                {
                    list_of_rotated_border_points[i] = true;
                }

                if (Math.Abs(min_y - current_y) <= thresold || Math.Abs(max_y - current_y) <= thresold)
                {
                    list_of_rotated_border_points[i] = true;
                }
            }

            List<PointD> list_of_border_points = new List<PointD>();
            for (int i = 0; i < list_of_points.Count; i++)
            {
                if (list_of_rotated_border_points[i])
                {
                    list_of_border_points.Add(list_of_points[i]);
                }
            }

            return list_of_border_points;
        }
        public static List<ClusterObjects> FindAllBorderClusters(List<ClusterObjects> list_of_clusters, RectangleOriented rectangle, double thresold)
        {
            List<ClusterObjects> list_of_border_clusters = new List<ClusterObjects>();

            double angle = rectangle.Angle;

            Matrix<double> rotation_matrix = DenseMatrix.OfArray(new double[,] {
                    { Math.Cos(angle), Math.Cos(angle - Math.PI / 2) },
                    { Math.Cos(angle + Math.PI / 2), Math.Cos(angle) }
                });



            Vector<double> center_point = Vector<double>.Build.DenseOfArray(new double[] { rectangle.Center.X, rectangle.Center.Y });

            Vector<double> rotated_center = rotation_matrix * center_point;

            double min_x = rotated_center[0] - rectangle.Width / 2;
            double max_x = rotated_center[0] + rectangle.Width / 2;

            double min_y = rotated_center[1] - rectangle.Lenght / 2;
            double max_y = rotated_center[1] + rectangle.Lenght / 2;


            foreach (ClusterObjects cluster in list_of_clusters)
            {

                double[,] point_array = new double[cluster.points.Count, 2];

                for (int i = 0; i < cluster.points.Count; i++)
                {
                    point_array[i, 0] = cluster.points[i].Pt.Distance * Math.Cos(cluster.points[i].Pt.Angle);
                    point_array[i, 1] = cluster.points[i].Pt.Distance * Math.Sin(cluster.points[i].Pt.Angle);
                }

                Matrix<double> points_matrix = DenseMatrix.OfArray(point_array);
                Matrix<double> rotated_points = rotation_matrix * points_matrix.Transpose();

                bool[] list_of_rotated_border_points = new bool[rotated_points.ColumnCount];

                for (int i = 0; i < rotated_points.ColumnCount; i++)
                {
                    double current_x = rotated_points[0, i];
                    double current_y = rotated_points[1, i];

                    if (Math.Abs(min_x - current_x) <= thresold || Math.Abs(max_x - current_x) <= thresold)
                    {
                        list_of_border_clusters.Add(cluster);
                        break;
                    }

                    if (Math.Abs(min_y - current_y) <= thresold || Math.Abs(max_y - current_y) <= thresold)
                    {
                        list_of_border_clusters.Add(cluster);
                        break;
                    }
                }
            }

            return list_of_border_clusters;
        }
    }


    class GFG
    {
        // Finds the maximum area under the
        // histogram represented by histogram.
        // See below article for details.
        // https://
        // www.geeksforgeeks.org/largest-rectangle-under-histogram/
        public static int maxHist(int R, int C, int[] row)
        {
            // Create an empty stack. The stack
            // holds indexes of hist[] array.
            // The bars stored in stack are always
            // in increasing order of their heights.
            Stack<int> result = new Stack<int>();

            int top_val; // Top of stack

            int max_area = 0; // Initialize max area in
                              // current row (or histogram)

            int area = 0; // Initialize area with
                          // current top

            // Run through all bars of
            // given histogram (or row)
            int i = 0;
            while (i < C)
            {
                // If this bar is higher than the
                // bar on top stack, push it to stack
                if (result.Count == 0
                    || row[result.Peek()] <= row[i])
                {
                    result.Push(i++);
                }

                else
                {
                    // If this bar is lower than top
                    // of stack, then calculate area of
                    // rectangle with stack top as
                    // the smallest (or minimum height)
                    // bar. 'i' is 'right index' for
                    // the top and element before
                    // top in stack is 'left index'
                    top_val = row[result.Peek()];
                    result.Pop();
                    area = top_val * i;

                    if (result.Count > 0)
                    {
                        area
                            = top_val * (i - result.Peek() - 1);
                    }
                    max_area = Math.Max(area, max_area);
                }
            }

            // Now pop the remaining bars from
            // stack and calculate area with
            // every popped bar as the smallest bar
            while (result.Count > 0)
            {
                top_val = row[result.Peek()];
                result.Pop();
                area = top_val * i;
                if (result.Count > 0)
                {
                    area = top_val * (i - result.Peek() - 1);
                }

                max_area = Math.Max(area, max_area);
            }
            return max_area;
        }

        // Returns area of the largest
        // rectangle with all 1s in A[][]
        public static int maxRectangle(int R, int C, int[][] A)
        {
            // Calculate area for first row
            // and initialize it as result
            int result = maxHist(R, C, A[0]);

            // iterate over row to find
            // maximum rectangular area
            // considering each row as histogram
            for (int i = 1; i < R; i++)
            {
                for (int j = 0; j < C; j++)
                {

                    // if A[i][j] is 1 then
                    // add A[i -1][j]
                    if (A[i][j] == 1)
                    {
                        A[i][j] += A[i - 1][j];
                    }
                }

                // Update result if area with current
                // row (as last row of rectangle) is more
                result = Math.Max(result, maxHist(R, C, A[i]));
            }

            return result;
        }

        // Driver code
        public static void Main(string[] args)
        {
            int R = 4;
            int C = 4;

            int[][] A
                = new int[][] { new int[] { 0, 1, 1, 0 },
                            new int[] { 1, 1, 1, 1 },
                            new int[] { 1, 1, 1, 1 },
                            new int[] { 1, 1, 0, 0 } };
            Console.Write("Area of maximum rectangle is "
                        + maxRectangle(R, C, A));
        }
    }

    // This code is contributed by Shrikant13

}
