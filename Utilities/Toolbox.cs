using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace Utilities
{
    /// <summary>
    /// Contient plusieurs fonctions mathématiques utiles.
    /// </summary>
    public static class Toolbox
    {
        /// <summary>
        /// Renvoie la valeur max d'une liste de valeurs
        /// </summary>
        public static double Max(params double[] values)
            => values.Max();

        /// <summary>Converti un angle en degrés en un angle en radians.</summary>
        public static float DegToRad(float angleDeg)
            => angleDeg * (float)Math.PI / 180f;

        /// <summary>Converti un angle en degrés en un angle en radians.</summary>
        public static double DegToRad(double angleDeg)
            => angleDeg * Math.PI / 180;

        /// <summary>Converti un angle en degrés en un angle en radians.</summary>
        public static float RadToDeg(float angleRad)
            => angleRad / (float)Math.PI * 180f;

        /// <summary>Converti un angle en radians en un angle en degrés.</summary>
        public static double RadToDeg(double angleRad)
            => angleRad / Math.PI * 180;

        /// <summary>Renvoie l'angle modulo 2*pi entre -pi et pi.</summary>
        public static double Modulo2PiAngleRad(double angleRad)
        {
            double angleTemp = (angleRad - Math.PI) % (2 * Math.PI) + Math.PI;
            return (angleTemp + Math.PI) % (2 * Math.PI) - Math.PI;
        }

        /// <summary>Renvoie l'angle modulo pi entre -pi/2 et pi/2.</summary>
        public static double ModuloPiAngleRadian(double angleRad)
        {
            double angleTemp = (angleRad - Math.PI / 2.0) % Math.PI + Math.PI / 2.0;
            return (angleTemp + Math.PI / 2.0) % Math.PI - Math.PI / 2.0;
        }


        /// <summary>Renvoie l'angle modulo pi entre -pi et pi.</summary>
        public static double ModuloPiDivTwoAngleRadian(double angleRad)
        {
            double angleTemp = (angleRad - Math.PI / 4.0) % (Math.PI / 2) + Math.PI / 4.0;
            return (angleTemp + Math.PI / 4.0) % (Math.PI / 2) - Math.PI / 4.0;
        }

        /// <summary>Borne la valeur entre les deux valeurs limites données.</summary>
        public static double LimitToInterval(double value, double lowLimit, double highLimit)
        {
            if (value > highLimit)
                return highLimit;
            else if (value < lowLimit)
                return lowLimit;
            else
                return value;
        }

        /// <summary>Décale un angle dans un intervale de [-PI, PI] autour d'un autre.</summary>
        public static double ModuloByAngle(double angleToCenterAround, double angleToCorrect)
        {
            // On corrige l'angle obtenu pour le moduloter autour de l'angle Kalman
            int decalageNbTours = (int)Math.Round((angleToCorrect - angleToCenterAround) / (2 * Math.PI));
            double thetaDest = angleToCorrect - decalageNbTours * 2 * Math.PI;

            return thetaDest;
        }

        public static double Distance(Location l1, PointD p2)
        {
            return Distance(new PointD(l1.X, l1.Y), p2);
        }
        public static double Distance(PointD p1, Location l2)
        {
            return Distance(p1, new PointD(l2.X, l2.Y));
        }

        public static double Distance(Location l1, Location l2)
        {
            return Distance(new PointD(l1.X, l1.Y), new PointD(l2.X, l2.Y));
        }

        public static double Distance(PointD pt1, PointD pt2)
        {
            return Math.Sqrt((pt2.X - pt1.X) * (pt2.X - pt1.X) + (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y));
            //return Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2));
        }
        public static double Distance(PolarPointRssi pt1, PolarPointRssi pt2)
        {
            return Math.Sqrt(pt1.Distance * pt1.Distance + pt2.Distance * pt2.Distance - 2 * pt1.Distance * pt2.Distance * Math.Cos(pt1.Angle - pt2.Angle));
        }

        public static double Distance(PolarPointRssiExtended pt1, PolarPointRssiExtended pt2)
        {
            return Math.Sqrt(pt1.Pt.Distance * pt1.Pt.Distance + pt2.Pt.Distance * pt2.Pt.Distance - 2 * pt1.Pt.Distance * pt2.Pt.Distance * Math.Cos(pt1.Pt.Angle - pt2.Pt.Angle));
        }


        public static double DistanceL1(PointD pt1, PointD pt2)
        {
            return Math.Abs(pt2.X - pt1.X) + Math.Abs(pt2.Y - pt1.Y);
            //return Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2));
        }

        public static double Distance(double xPt1, double yPt1, double xPt2, double yPt2)
        {
            return Math.Sqrt(Math.Pow(xPt2 - xPt1, 2) + Math.Pow(yPt2 - yPt1, 2));
        }

        public static double DistancePointToLine(PointD pt, PointD LinePt, double LineAngle)
        {
            var xLineVect = Math.Cos(LineAngle);
            var yLineVect = Math.Sin(LineAngle);
            var dot = (pt.X - LinePt.X) * (yLineVect) - (pt.Y - LinePt.Y) * (xLineVect);
            return Math.Abs(dot);
        }
        public static double DistancePointToLine(PointD pt, PointD LinePt1, PointD LinePt2)
        {
            var lineAngle = Math.Atan2(LinePt2.Y - LinePt1.Y, LinePt2.X - LinePt1.X);
            var xLineVect = Math.Cos(lineAngle);
            var yLineVect = Math.Sin(lineAngle);
            var dot = (pt.X - LinePt1.X) * (yLineVect) - (pt.Y - LinePt1.Y) * (xLineVect);
            return Math.Abs(dot);
        }

        public static double DistancePointToSegment(PointD pt, PointD ptSeg1, PointD ptSeg2)
        {
            var A = pt.X - ptSeg1.X;
            var B = pt.Y - ptSeg1.Y;
            var C = ptSeg2.X - ptSeg1.X;
            var D = ptSeg2.Y - ptSeg1.Y;

            double dot = A * C + B * D;
            double len_sq = C * C + D * D;
            double param = -1;
            if (len_sq != 0) //in case of 0 length line
                param = dot / len_sq;

            double xx, yy;

            if (param < 0)
            {
                xx = ptSeg1.X;
                yy = ptSeg1.Y;
            }
            else if (param > 1)
            {
                xx = ptSeg2.X;
                yy = ptSeg2.Y;
            }
            else
            {
                xx = ptSeg1.X + param * C;
                yy = ptSeg1.Y + param * D;
            }

            var dx = pt.X - xx;
            var dy = pt.Y - yy;

            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance;
        }

        public static PointD GetInterceptionLocation(Location target, Location hunter, double huntingSpeed)
        {
            //D'après Al-Kashi, si d est la distance entre le pt target et le pt chasseur, que les vitesses sont constantes 
            //et égales à Vtarget et Vhunter
            //Rappel Al Kashi : A² = B²+C²-2BCcos(alpha) , alpha angle opposé au segment A
            //On a au moment de l'interception à l'instant Tinter: 
            //A = Vh * Tinter
            //B = VT * Tinter
            //C = initialDistance;
            //alpha = Pi - capCible - angleCible

            double targetSpeed = Math.Sqrt(Math.Pow(target.Vx, 2) + Math.Pow(target.Vy, 2));
            double initialDistance = Toolbox.Distance(new PointD(hunter.X, hunter.Y), new PointD(target.X, target.Y));
            double capCible = Math.Atan2(target.Vy, target.Vx);
            double angleCible = Math.Atan2(target.Y - hunter.Y, target.X - hunter.X);
            double angleCapCibleDirectionCibleChasseur = Math.PI - capCible + angleCible;

            //Résolution de ax²+bx+c=0 pour trouver Tinter
            double a = Math.Pow(huntingSpeed, 2) - Math.Pow(targetSpeed, 2);
            double b = 2 * initialDistance * targetSpeed * Math.Cos(angleCapCibleDirectionCibleChasseur);
            double c = -Math.Pow(initialDistance, 2);

            double delta = b * b - 4 * a * c;
            double t1 = (-b - Math.Sqrt(delta)) / (2 * a);
            double t2 = (-b + Math.Sqrt(delta)) / (2 * a);

            if (delta > 0 && t2 < 10)
            {
                double xInterception = target.X + targetSpeed * Math.Cos(capCible) * t2;
                double yInterception = target.Y + targetSpeed * Math.Sin(capCible) * t2;
                return new PointD(xInterception, yInterception);
            }
            else
                return null;
        }

        static public PointDExtended GetCrossingPointBetweenSegment(SegmentExtended segment_a, SegmentExtended segment_b)
        {
            PointDExtended crossing_point = new PointDExtended(new PointD(0, 0), segment_a.Color, segment_a.Width);

            if (segment_a.Segment.X1 == segment_a.Segment.X2 || segment_b.Segment.X1 == segment_b.Segment.X2)
            {
                return crossing_point;
            }
            double slope_a = (segment_a.Segment.Y2 - segment_a.Segment.Y1) / (segment_a.Segment.X2 - segment_a.Segment.X1);
            double y_intercept_a = segment_a.Segment.Y1 - (segment_a.Segment.X1) * slope_a;

            double slope_b = (segment_b.Segment.Y2 - segment_b.Segment.Y1) / (segment_b.Segment.X2 - segment_b.Segment.X1);
            double y_intercept_b = segment_b.Segment.Y1 - (segment_b.Segment.X1) * slope_b;

            if (slope_a == slope_b)
            {
                return crossing_point;
            }

            double x = (y_intercept_b - y_intercept_a) / (slope_a - slope_b);
            double y = slope_a * x + y_intercept_a;

            crossing_point.Pt = new PointD(x, y);

            return crossing_point;

        }

        [DllImport("shlwapi.dll")]
        public static extern int ColorHLSToRGB(int H, int L, int S);

        static public System.Drawing.Color HLSToColor(int H, int L, int S)
        {
            //
            // Convert Hue, Luminance, and Saturation values to System.Drawing.Color structure.
            // H, L, and S are in the range of 0-240.
            // ColorHLSToRGB returns a Win32 RGB value (0x00BBGGRR).  To convert to System.Drawing.Color
            // structure, use ColorTranslator.FromWin32.
            //
            return ColorTranslator.FromWin32(ColorHLSToRGB(H, L, S));

        }

        static public PointDExtended ConvertPolarToPointD(PolarPointRssiExtended point)
        {
            return new PointDExtended(new PointD(point.Pt.Distance * Math.Cos(point.Pt.Angle), point.Pt.Distance * Math.Sin(point.Pt.Angle), point.Pt.Rssi), point.Color, point.Width);
        }

        static public PolarPointRssiExtended ConvertPointDToPolar(PointDExtended point)
        {
            return new PolarPointRssiExtended(new PolarPointRssi(Math.Atan2(point.Pt.Y, point.Pt.X), Math.Sqrt(Math.Pow(point.Pt.X, 2) + Math.Pow(point.Pt.Y, 2)), point.Pt.Rssi), point.Width, point.Color);
        }
        static public PointD ConvertPolarToPointD(PolarPointRssi point)
        {
            return new PointD(point.Distance * Math.Cos(point.Angle), point.Distance * Math.Sin(point.Angle), point.Rssi);
        }

        static public PolarPointRssi ConvertPointDToPolar(PointD point)
        {
            return new PolarPointRssi(Math.Atan2(point.Y, point.X), Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2)), point.Rssi);
        }

        public static PointD GetCrossingPoint(Line line_1, Line line_2)
        {
            double estimated_X = (line_1.y_intercept - line_2.y_intercept) / (line_2.slope - line_1.slope);
            double estimated_Y = line_1.slope * estimated_X + line_1.y_intercept;

            return new PointD(estimated_X, estimated_Y);
        }

        public static PointD GetCrossingPoint(double slope_a, double y_intercept_a, double slope_b, double y_intercept_b)
        {
            return GetCrossingPoint(new Line(slope_a, y_intercept_a), new Line(slope_b, y_intercept_b));
        }


        static public PointDExtended GetCrossingPoint(SegmentExtended segment_a, SegmentExtended segment_b)
        {
            PointDExtended crossing_point = new PointDExtended(new PointD(0, 0), segment_a.Color, segment_a.Width);

            if (segment_a.Segment.X1 == segment_a.Segment.X2 || segment_b.Segment.X1 == segment_b.Segment.X2)
            {
                return crossing_point;
            }
            double slope_a = (segment_a.Segment.Y2 - segment_a.Segment.Y1) / (segment_a.Segment.X2 - segment_a.Segment.X1);
            double y_intercept_a = segment_a.Segment.Y1 - (segment_a.Segment.X1) * slope_a;

            double slope_b = (segment_b.Segment.Y2 - segment_b.Segment.Y1) / (segment_b.Segment.X2 - segment_b.Segment.X1);
            double y_intercept_b = segment_b.Segment.Y1 - (segment_b.Segment.X1) * slope_b;

            if (slope_a == slope_b)
            {
                return crossing_point;
            }

            double x = (y_intercept_b - y_intercept_a) / (slope_a - slope_b);
            double y = slope_a * x + y_intercept_a;

            crossing_point.Pt = new PointD(x, y);

            return crossing_point;

        }

        public static PointD GetPerpendicularPoint(PointD point, double slope, double y_intercept)
        {
            return GetPerpendicularPoint(point, new Line(slope, y_intercept));
        }

        public static PointD GetPerpendicularPoint(PointD point, Line line)
        {
            Line perpendicular_line = new Line(-line.slope, point.Y - (-line.slope * point.X));

            PointD target_point = GetCrossingPoint(line, perpendicular_line);
            return target_point;
        }


        /// <summary>
        /// Get all Combination of list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetKCombs(list, length - 1).SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0), (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static Tuple<PointD, PointD, PointD, PointD> GetCornerOfAnOrientedRectangle(RectangleOriented rectangle)
        {
            double radius_of_the_circle = Math.Sqrt(Math.Pow(rectangle.Width, 2) + Math.Pow(rectangle.Lenght, 2)) / 2;


            double angle_1 = ModuloPiAngleRadian(Math.Atan2(rectangle.Lenght, rectangle.Width) + rectangle.Angle);
            double angle_2 = ModuloPiAngleRadian(Math.Atan2(- rectangle.Lenght, rectangle.Width) + rectangle.Angle);

            if (angle_1 < 0)
                SwapNum(ref angle_1, ref angle_2);

            PointD polar_a_1 = ConvertPolarToPointD(new PolarPointRssi(angle_1, radius_of_the_circle, 0));
            PointD polar_a_2 = ConvertPolarToPointD(new PolarPointRssi(angle_2, radius_of_the_circle, 0));
            PointD polar_a_3 = ConvertPolarToPointD(new PolarPointRssi(angle_2, - radius_of_the_circle, 0));
            PointD polar_a_4 = ConvertPolarToPointD(new PolarPointRssi(angle_1, - radius_of_the_circle, 0));

            if (polar_a_1.Y < 0)
            {
                SwapNum(ref polar_a_1, ref polar_a_2);
                SwapNum(ref polar_a_3, ref polar_a_4);
            }
                

            PointD a1 = new PointD(polar_a_1.X + rectangle.Center.X, polar_a_1.Y + rectangle.Center.Y);
            PointD a2 = new PointD(polar_a_2.X + rectangle.Center.X, polar_a_2.Y + rectangle.Center.Y);
            PointD a3 = new PointD(polar_a_3.X + rectangle.Center.X, polar_a_3.Y + rectangle.Center.Y);
            PointD a4 = new PointD(polar_a_4.X + rectangle.Center.X, polar_a_4.Y + rectangle.Center.Y);

            return new Tuple<PointD, PointD, PointD, PointD>(a1, a2, a3, a4);
        }

        public static bool TestIfPointInsideAnOrientedRectangle(RectangleOriented rectangle, PointD m)
        {
            /// Whe simply make the dot product with each angle
            Tuple<PointD, PointD, PointD, PointD> corners = GetCornerOfAnOrientedRectangle(rectangle);

            PointD a = corners.Item1;
            PointD b = corners.Item2;
            PointD c = corners.Item3;

            PointD vector_a_b = new PointD(b.X - a.X, b.Y - a.Y);
            PointD vector_a_c = new PointD(c.X - a.X, c.Y - a.Y);
            PointD vector_a_m = new PointD(m.X - a.X, m.Y - a.Y);

            double dot_product_m_b = (vector_a_b.X * vector_a_m.X) + (vector_a_b.Y * vector_a_m.Y);
            double dot_product_m_c = (vector_a_c.X * vector_a_m.X) + (vector_a_c.Y * vector_a_m.Y);

            double dot_product_b_b = Math.Pow(vector_a_b.X, 2) + Math.Pow(vector_a_b.Y, 2);
            double dot_product_c_c = Math.Pow(vector_a_c.X, 2) + Math.Pow(vector_a_c.Y, 2);
            
            return (0 <= dot_product_m_b && dot_product_m_b <= dot_product_b_b) && (0 <= dot_product_m_c  && dot_product_m_c <= dot_product_c_c);
        }

        public static double DotProduct(PointD vector_a, PointD vector_b)
        {
            return (vector_a.X * vector_b.X) + (vector_a.Y * vector_b.Y);
        }

        public static double Distance(SegmentExtended segment)
        {
            return Math.Sqrt(Math.Pow(segment.Segment.X2 - segment.Segment.X1, 2) + Math.Pow(segment.Segment.Y2 - segment.Segment.Y1, 2));
        }
        
        public static double Angle(Segment segment)
        {
            return Math.Atan2(segment.Y2 - segment.Y1, segment.X2 - segment.X1);
        }
        public static double Angle(SegmentExtended segment)
        {
            return Math.Atan2(segment.Segment.Y2 - segment.Segment.Y1, segment.Segment.X2 - segment.Segment.X1);
        }

        public static double Angle(PointD pt1, PointD pt2)
        {
            return Math.Atan2(pt2.Y - pt1.Y, pt2.X - pt1.X);
        }

        public static double ClockWiseAngle(PointD pt1, PointD pt2)
        {
            return Toolbox.Modulo2PiAngleRad(- Angle(pt1, pt2));
        }

        public static void SwapNum(ref double x, ref double y)
        {
            x += y;
            y = x - y;
            x -= y;
        }

        public static void SwapNum(ref SegmentExtended s1, ref SegmentExtended s2)
        {
            SegmentExtended temporary_segment = s1;
            s1 = s2;
            s2 = temporary_segment;
        }

        public static void SwapNum(ref PointD p1, ref PointD p2)
        {
            PointD temporary_point = p1;
            p1 = p2;
            p2 = temporary_point;
        }

        public static Color ColorFromHSL(double h, double s, double l)
        {
            double r = 0, g = 0, b = 0;
            if (l != 0)
            {
                if (s == 0)
                    r = g = b = l;
                else
                {
                    double temp2;
                    if (l < 0.5)
                        temp2 = l * (1.0 + s);
                    else
                        temp2 = l + s - (l * s);

                    double temp1 = 2.0 * l - temp2;

                    r = GetColorComponent(temp1, temp2, h + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, h);
                    b = GetColorComponent(temp1, temp2, h - 1.0 / 3.0);
                }
            }

            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;

            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
            else
                return temp1;
        }

        public static PointD ProjectedPointOnLineFromWaypoint(PointD pt, PointD ptSeg1, PointD ptSeg2)
        {
            /// STOLEN FROM KEENAN

            var A = pt.X - ptSeg1.X;
            var B = pt.Y - ptSeg1.Y;
            var C = ptSeg2.X - ptSeg1.X;
            var D = ptSeg2.Y - ptSeg1.Y;

            double dot = A * C + B * D;
            double len_sq = C * C + D * D;
            double param = -1;
            if (len_sq != 0) //in case of 0 length line
                param = dot / len_sq;

            double xx, yy;

            xx = ptSeg1.X + param * C;
            yy = ptSeg1.Y + param * D;

            return new PointD(xx, yy);
        }

        public static double Area_of_a_Triangle (PointD a, PointD b, PointD c)
        {
            double a_b = Toolbox.Distance(a, b);
            double a_c = Toolbox.Distance(a, c);
            double b_c = Toolbox.Distance(b, c);

            double s = (a_b + a_c + b_c) / 2;

            return Math.Sqrt(s * ((s - a_b) * (s - a_c) * (s - b_c)));
        }
        public static int TestOrientationOfOrderedPoint(PointD p, PointD q, PointD r)
        {
            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (val == 0)
                return 0;
            else if (val > 0)
                return 1;
            else
                return 2;

        }

        public static bool testIfOnSegment(PointD p, PointD q, PointD r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
                return true;

            return false;
        }

        public static bool testIfTwoSegmentsIntersect(Segment s1, Segment s2)
        {
            int o1 = TestOrientationOfOrderedPoint(new PointD(s1.X1, s1.Y1), new PointD(s1.X2, s1.Y2), new PointD(s2.X1, s2.Y1));
            int o2 = TestOrientationOfOrderedPoint(new PointD(s1.X1, s1.Y1), new PointD(s1.X2, s1.Y2), new PointD(s2.X2, s2.Y2));
            int o3 = TestOrientationOfOrderedPoint(new PointD(s2.X1, s2.Y1), new PointD(s2.X2, s2.Y2), new PointD(s1.X1, s1.Y1));
            int o4 = TestOrientationOfOrderedPoint(new PointD(s2.X1, s2.Y1), new PointD(s2.X2, s2.Y2), new PointD(s1.X2, s1.Y2));

            if (o1 != o2 && o3 != o4)
                return true;

            if (o1 == 0 && testIfOnSegment(new PointD(s1.X1, s1.Y1), new PointD(s2.X1, s2.Y1), new PointD(s1.X2, s1.Y2)))
                return true;

            if (o2 == 0 && testIfOnSegment(new PointD(s1.X1, s1.Y1), new PointD(s2.X2, s2.Y2), new PointD(s1.X2, s1.Y2)))
                return true;

            if (o3 == 0 && testIfOnSegment(new PointD(s2.X1, s2.Y1), new PointD(s1.X1, s1.Y1), new PointD(s2.X2, s2.Y2)))
                return true;

            if (o4 == 0 && testIfOnSegment(new PointD(s2.X1, s2.Y1), new PointD(s1.X2, s1.Y2), new PointD(s2.X2, s2.Y2)))
                return true;

            return false;

        }

        

        public static bool testIfSegmentIntersectRectangle(Segment s1, RectangleOriented r1)
        {
            if (TestIfPointInsideAnOrientedRectangle(r1, new PointD(s1.X1, s1.Y1)) || TestIfPointInsideAnOrientedRectangle(r1, new PointD(s1.X2, s1.Y2)))
                return true;

            Tuple<PointD, PointD, PointD, PointD> corners = GetCornerOfAnOrientedRectangle(r1);
            Segment s_1_2 = new Segment(corners.Item1, corners.Item2);
            Segment s_1_3 = new Segment(corners.Item1, corners.Item3);
            Segment s_2_4 = new Segment(corners.Item2, corners.Item4);
            Segment s_3_4 = new Segment(corners.Item3, corners.Item4);

            if (testIfTwoSegmentsIntersect(s1, s_1_2))
                return true;

            if (testIfTwoSegmentsIntersect(s1, s_1_3))
                return true;

            if (testIfTwoSegmentsIntersect(s1, s_2_4))
                return true;

            if (testIfTwoSegmentsIntersect(s1, s_3_4))
                return true;

            return false;
            
        }

    }
}

