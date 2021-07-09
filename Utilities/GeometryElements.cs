using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Shapes;
using ZeroFormatter;
using Constants;

namespace Utilities
{
    public class PointD
    {
        public double X;// { get; set; }
        public double Y;// { get; set; }
        public double Rssi;



        public PointD()
        {
            X = 0;
            Y = 0;
            Rssi = 0;
        }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
            Rssi = 0;
        }

        public PointD(double x, double y, double rssi)
        {
            X = x;
            Y = y;
            Rssi = rssi;
        }


    }

    public class PointDExtended
    {
        public PointD Pt;
        public Color Color;
        public double Width;

        public PointDExtended(PointD pt, Color c, double size)
        {
            Pt = pt;
            Color = c;
            Width = size;
        }
    }

    public class Point3D
    {
        public double X;// { get; set; }
        public double Y;// { get; set; }
        public double Z;// { get; set; }
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public class RectangleD
    {
        public double Xmin;
        public double Xmax;// { get; set; }
        public double Ymin;
        public double Ymax;// { get; set; }
        public RectangleD(double xMin, double xMax, double yMin, double yMax)
        {
            Xmin = xMin;
            Xmax = xMax;
            Ymin = yMin;
            Ymax = yMax;
        }
    }

    public class RectangleOriented
    {
        public PointD Center { get; set; }
        public double Width { get; set; }
        public double Lenght { get; set; }
        public double Angle { get; set; }

        public RectangleOriented()
        {
            Center = new PointD(0, 0);
            Width = 0;
            Lenght = 0;
            Angle = 0;
        }

        public RectangleOriented(PointD center, double width, double lenght, double angle)
        {
            Center = center;

            if (width > lenght)
            {
                Width = width;
                Lenght = lenght;
                Angle = Toolbox.ModuloPiAngleRadian(angle);
            }
            else
            {
                Width = lenght;
                Lenght = width;
                Angle = Toolbox.ModuloPiAngleRadian(angle + Math.PI / 2);
            }
            

            
        }
    }

    public class Field
    {
        public RectangleOriented Shape { get; set; }
        public FieldType Type { get; set; }

        public Field()
        {
            Shape = new RectangleOriented();
            Type = FieldType.None;
        }

        public Field(RectangleOriented shape, FieldType type)
        {
            Shape = shape;
            Type = type;
        }
    }
    public class PolarPoint
    {
        public double Distance;
        public double Angle;

        public PolarPoint(double angle, double distance)
        {
            Distance = distance;
            Angle = angle;
        }
    }
    [ZeroFormattable]
    public class PolarPointRssi
    {
        [Index(0)]
        public virtual double Distance { get; set; }
        [Index(1)]
        public virtual double Angle { get; set; }
        [Index(2)]
        public virtual double Rssi { get; set; }

        public PolarPointRssi(double angle, double distance, double rssi)
        {
            Distance = distance;
            Angle = angle;
            Rssi = rssi;
        }
        public PolarPointRssi()
        {

        }
    }

    public class ClusterObjects
    {
        public List<PolarPointRssiExtended> points { get; set; }

        public ClusterObjects()
        {
            points = new List<PolarPointRssiExtended>();
        }
        public ClusterObjects(List<PolarPointRssiExtended> polarPointRssis)
        {
            points = polarPointRssis;
        }
    }

    public class PolarPointRssiExtended
    {
        public PolarPointRssi Pt { get; set; }
        public double Width { get; set; }
        public Color Color { get; set; }

        public PolarPointRssiExtended(PolarPointRssi pt, double width, Color c)
        {
            Pt = pt;
            Width = width;
            Color = c;
        }
    }

    public class PolarCourbure
    {
        public virtual double Courbure { get; set; }
        public virtual double Angle { get; set; }
        public virtual bool Discontinuity { get; set; }
        public PolarCourbure(double angle, double courbure, bool discontinuity)
        {            
            Angle = angle;
            Courbure = courbure;
            Discontinuity = discontinuity;
        }
    }

    public class Line
    {
        public double slope { get; set; }
        public double y_intercept { get; set; }
        
        public Line()
        {
            slope = 0;
            y_intercept = 0;
        }

        public Line(double slope, double y_intercept)
        {
            this.slope = slope;
            this.y_intercept = y_intercept;
        }
    }

    public class Segment
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }

        public Segment()
        {
            X1 = 0;
            Y1 = 0;
            X2 = 0;
            Y2 = 0;
        }

        public Segment(double x1, double y1, double x2, double y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public Segment(PointD a, PointD b)
        {
            X1 = a.X;
            Y1 = a.Y;
            X2 = b.X;
            Y2 = b.Y;
        }
    }

    public class SegmentExtended
    {
        public Segment Segment;
        public double Width = 10;
        public System.Drawing.Color Color = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
        public double Opacity = 1;
        public double[] DashPattern = new double[] { 1.0 };

        public SegmentExtended(PointD ptDebut, PointD ptFin, System.Drawing.Color color, double width = 1)
        {
            Segment = new Segment(ptDebut, ptFin);
            Color = color;
            Width = width;
        }

        public SegmentExtended(Segment segment, System.Drawing.Color color, double width = 1)
        {
            Segment = segment;
            Color = color;
            Width = width;
        }
    }

    [ZeroFormattable]
    public class Location
    {
        [Index(0)]
        public virtual double X { get; set; }
        [Index(1)]
        public virtual double Y { get; set; }
        [Index(2)]
        public virtual double Theta { get; set; }
        [Index(3)]
        public virtual double Vx { get; set; }
        [Index(4)]
        public virtual double Vy { get; set; }
        [Index(5)]
        public virtual double Vtheta { get; set; }

        public Location()
        {

        }
        public Location(double x, double y, double theta, double vx, double vy, double vtheta)
        {
            X = x;
            Y = y;
            Theta = theta;
            Vx = vx;
            Vy = vy;
            Vtheta = vtheta;
        }
    }

    //Pose probleme
    [ZeroFormattable]
    public class LocationExtended
    {
        [Index(0)]
        public virtual double X { get; set; }
        [Index(1)]
        public virtual double Y { get; set; }
        [Index(2)]
        public virtual double Theta { get; set; }
        [Index(3)]
        public virtual double Vx { get; set; }
        [Index(4)]
        public virtual double Vy { get; set; }
        [Index(5)]
        public virtual double Vtheta { get; set; }
        [Index(6)]
        public virtual ObjectType Type { get; set; }

        public LocationExtended()
        {

        }
        public LocationExtended(double x, double y, double theta, double vx, double vy, double vtheta, ObjectType type)
        {
            X = x;
            Y = y;
            Theta = theta;
            Vx = vx;
            Vy = vy;
            Vtheta = vtheta;
            Type = type;
        }
    }

    public class PolygonExtended
    {
        public Polygon polygon = new Polygon();
        public float borderWidth = 1;
        public System.Drawing.Color borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
        public double borderOpacity = 1;
        public double[] borderDashPattern = new double[] { 1.0 };
        public System.Drawing.Color backgroundColor = System.Drawing.Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF);
    }

    public class PolarPointListExtended
    {
        public List<PolarPointRssi> polarPointList;
        public ObjectType type;
        //public System.Drawing.Color displayColor;
        //public double displayWidth=1;
    }

    public class Zone
    {
        public PointD center;
        public double radius; //Le rayon correspond à la taille la zone - à noter que l'intensité diminuera avec le rayon
        public double strength; //La force correspond à l'intensité du point central de la zone
        public Zone(PointD center, double radius, double strength)
        {
            this.radius = radius;
            this.center = center;
            this.strength = strength;
        }
    }
    public class RectangleZone
    {
        public RectangleD rectangularZone;
        public double strength; //La force correspond à l'intensité du point central de la zone
        public RectangleZone(RectangleD rect, double strength = 1)
        {
            this.rectangularZone = rect;
            this.strength = strength;
        }
    }

    public class ConicalZone
    {
        public PointD InitPoint;
        public PointD Cible;
        public double Radius;
        public ConicalZone(PointD initPt, PointD ciblePt, double radius)
        {
            InitPoint = initPt;
            Cible = ciblePt;
            Radius = radius;
        }
    }
    public class SegmentZone
    {
        public PointD PointA;
        public PointD PointB;
        public double Radius;
        public double Strength;
        public SegmentZone(PointD ptA, PointD ptB, double radius, double strength)
        {
            PointA = ptA;
            PointB = ptB;
            Radius = radius;
            Strength = strength;
        }
    }

    public enum ObjectType
    {
        Balle,
        Obstacle,
        Robot,
        Poteau,
        Balise,
        LimiteHorizontaleHaute,
        LimiteHorizontaleBasse,
        LimiteVerticaleGauche,
        LimiteVerticaleDroite,

    }
}
