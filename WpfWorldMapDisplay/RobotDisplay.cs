using Constants;
using SciChart.Charting.Model.DataSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Utilities;
using EventArgsLibrary;

namespace WpfWorldMapDisplay
{
    public class RobotDisplay
    {
        private PolygonExtended robotShape;
        private PolygonExtended ghostShape;
        private Random rand = new Random();

        private Location robotLocation;
        private Location ghostLocation;
        private Location destinationLocation;
        private List<Location> waypointLocation;

        public string robotName = "";
        public RoboCupRobotRole robotRole = RoboCupRobotRole.Stopped;
        public string DisplayMessage = "";
        public PlayingSide playingSide = PlayingSide.Left;

        public double[,] heatMapStrategy;
        public double[,] heatMapWaypoint;

        List<PointDExtended> LidarRawPoints;
        private List<PointDExtended>[] LidarProcessedPoints = new List<PointDExtended>[3];
        List<SegmentExtended> LidarSegment;
        List<LidarObject> LidarObjectList;
        public List<Location> ballLocationList;

        public RobotDisplay(PolygonExtended rbtShape, PolygonExtended ghstShape, string name)
        {
            robotLocation = new Location(0, 0, 0, 0, 0, 0);
            destinationLocation = new Location(0, 0, 0, 0, 0, 0);
            waypointLocation = new List<Location> { };
            ghostLocation = new Location(0, 0, 0, 0, 0, 0);

            robotShape = rbtShape;
            ghostShape = ghstShape;
            robotName = name;

            ballLocationList = new List<Location>();
        }

        public void SetLocation(Location loc)
        {
            robotLocation = loc;
        }

        public void SetRole(RoboCupRobotRole role)
        {
            robotRole = role;
        }
        public void SetDisplayMessage(string displayMessage)
        {
            DisplayMessage = displayMessage;
        }

        public void SetPlayingSide(PlayingSide playSide)
        {
            this.playingSide = playSide;
        }

        public void SetGhostLocation(Location loc)
        {
            ghostLocation = loc;
        }
        public void SetDestination(Location location)
        {
            destinationLocation = location;
        }

        public void SetWayPoint(List<Location> locations)
        {
            waypointLocation = locations;
        }

        public void AddNewWayPoint(Location location)
        {
            waypointLocation.Add(location);
        }

        public void ResetWayPoints()
        {
            waypointLocation = new List<Location> { };
        }

        public void SetHeatMapStrategy(double[,] heatMap)
        {
            this.heatMapStrategy = heatMap;
        }
        public void SetHeatMapWaypoint(double[,] heatMap)
        {
            this.heatMapWaypoint = heatMap;
        }


        public void SetLidarMap(List<PointDExtended> lidarMap, LidarDataType dataType)
        {
            switch (dataType)
            {
                case LidarDataType.RawData:
                    this.LidarRawPoints = lidarMap;
                    break;
                case LidarDataType.ProcessedData1:
                    this.LidarProcessedPoints[0] = lidarMap;
                    break;
                default:
                    break;
            }  
        }

        public void SetLidarSegment(List<SegmentExtended> segments)
        {
            LidarSegment = segments;
        }

        public void SetLidarObjectList(List<LidarObject> lidarObjectList)
        {
            this.LidarObjectList = lidarObjectList;
        }

        public void SetBallList(List<Location> ballLocationList)
        {
            this.ballLocationList = ballLocationList;
        }

        public Location GetRobotLocation()
        {
            return robotLocation;
        }


        public PolygonExtended GetRobotPolygon()
        {
            PolygonExtended polygonToDisplay = new PolygonExtended();
            foreach (var pt in robotShape.polygon.Points)
            {
                Point polyPt = new Point(pt.X * Math.Cos(robotLocation.Theta) - pt.Y * Math.Sin(robotLocation.Theta), pt.X * Math.Sin(robotLocation.Theta) + pt.Y * Math.Cos(robotLocation.Theta));
                polyPt.X += robotLocation.X;
                polyPt.Y += robotLocation.Y;
                polygonToDisplay.polygon.Points.Add(polyPt);
                polygonToDisplay.backgroundColor = robotShape.backgroundColor;
                polygonToDisplay.borderColor = robotShape.borderColor;
                polygonToDisplay.borderWidth = robotShape.borderWidth;
            }
            return polygonToDisplay;
        }
        
        public PolygonExtended GetRobotGhostPolygon()
        {
            PolygonExtended polygonToDisplay = new PolygonExtended();
            foreach (var pt in ghostShape.polygon.Points)
            {
                Point polyPt = new Point(pt.X * Math.Cos(ghostLocation.Theta) - pt.Y * Math.Sin(ghostLocation.Theta), pt.X * Math.Sin(ghostLocation.Theta) + pt.Y * Math.Cos(ghostLocation.Theta));
                polyPt.X += ghostLocation.X;
                polyPt.Y += ghostLocation.Y;
                polygonToDisplay.polygon.Points.Add(polyPt);
                polygonToDisplay.backgroundColor = ghostShape.backgroundColor;
                polygonToDisplay.borderColor = ghostShape.borderColor;
                polygonToDisplay.borderWidth = ghostShape.borderWidth;
            }
            return polygonToDisplay;
        }

        public PolygonExtended GetRobotSpeedArrow()
        {
            PolygonExtended polygonToDisplay = new PolygonExtended();
            double angleTeteFleche = Math.PI / 6;
            double longueurTeteFleche = 0.10;
            double LongueurFleche = Math.Sqrt(robotLocation.Vx * robotLocation.Vx + robotLocation.Vy * robotLocation.Vy);
            if (LongueurFleche != 0)
            {
                double headingAngle = Math.Atan2(robotLocation.Vy, robotLocation.Vx) + robotLocation.Theta;
                double xTete = LongueurFleche * Math.Cos(headingAngle);
                double yTete = LongueurFleche * Math.Sin(headingAngle);

                polygonToDisplay.polygon.Points.Add(new Point(robotLocation.X, robotLocation.Y));
                polygonToDisplay.polygon.Points.Add(new Point(robotLocation.X + xTete, robotLocation.Y + yTete));
                double angleTeteFleche1 = headingAngle + angleTeteFleche;
                double angleTeteFleche2 = headingAngle - angleTeteFleche;
                polygonToDisplay.polygon.Points.Add(new Point(robotLocation.X + xTete - longueurTeteFleche * Math.Cos(angleTeteFleche1), robotLocation.Y + yTete - longueurTeteFleche * Math.Sin(angleTeteFleche1)));
                polygonToDisplay.polygon.Points.Add(new Point(robotLocation.X + xTete, robotLocation.Y + yTete));
                polygonToDisplay.polygon.Points.Add(new Point(robotLocation.X + xTete - longueurTeteFleche * Math.Cos(angleTeteFleche2), robotLocation.Y + yTete - longueurTeteFleche * Math.Sin(angleTeteFleche2)));
                polygonToDisplay.polygon.Points.Add(new Point(robotLocation.X + xTete, robotLocation.Y + yTete));
                polygonToDisplay.borderWidth = 2;
                polygonToDisplay.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0x00, 0x00);
                polygonToDisplay.borderDashPattern = new double[] { 3, 3 };
                polygonToDisplay.borderOpacity = 1;
                polygonToDisplay.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0x00, 0x00);
            }
            return polygonToDisplay;
        }


        public PolygonExtended GetRobotDestinationArrow()
        {
            PolygonExtended polygonToDisplay = new PolygonExtended();
            if (destinationLocation == null)
                return polygonToDisplay;
            double angleTeteFleche = Math.PI / 6;
            double longueurTeteFleche = 0.10;
            double headingAngle = Math.Atan2(destinationLocation.Y - robotLocation.Y, destinationLocation.X - robotLocation.X);
            double LongeurFleche = Math.Sqrt(Math.Pow(destinationLocation.X - robotLocation.X, 2.0) + Math.Pow(destinationLocation.Y - robotLocation.Y, 2.0));
            if (LongeurFleche >= 0.100)
            {
                polygonToDisplay.polygon.Points.Add(new Point(robotLocation.X, robotLocation.Y));
                polygonToDisplay.polygon.Points.Add(new Point(destinationLocation.X, destinationLocation.Y));
                double angleTeteFleche1 = headingAngle + angleTeteFleche;
                double angleTeteFleche2 = headingAngle - angleTeteFleche;
                polygonToDisplay.polygon.Points.Add(new Point(destinationLocation.X - longueurTeteFleche * Math.Cos(angleTeteFleche1), destinationLocation.Y - longueurTeteFleche * Math.Sin(angleTeteFleche1)));
                polygonToDisplay.polygon.Points.Add(new Point(destinationLocation.X, destinationLocation.Y));
                polygonToDisplay.polygon.Points.Add(new Point(destinationLocation.X - longueurTeteFleche * Math.Cos(angleTeteFleche2), destinationLocation.Y - longueurTeteFleche * Math.Sin(angleTeteFleche2)));
                polygonToDisplay.polygon.Points.Add(new Point(destinationLocation.X, destinationLocation.Y));
                polygonToDisplay.borderWidth = 3;
                polygonToDisplay.borderColor = System.Drawing.Color.FromArgb(0xFF, 0x00, 0x00, 0xFF);
                polygonToDisplay.borderOpacity = 0.8;
                polygonToDisplay.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0x00, 0x00);
            }
            return polygonToDisplay;
        }

        public PolygonExtended GetRobotWaypointArrow()
        {
            PolygonExtended polygonToDisplay = new PolygonExtended();
            double angleTeteFleche = Math.PI / 6;
            double longueurTeteFleche = 0.10;
            polygonToDisplay.borderWidth = 2;
            polygonToDisplay.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);
            polygonToDisplay.borderDashPattern = new double[] { 5, 5 };
            polygonToDisplay.borderOpacity = 0.8;
            polygonToDisplay.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0x00, 0x00);

            if (waypointLocation.Count == 0)
            {
                return polygonToDisplay;
            }

            Location lastLocation = waypointLocation[0];
            Location bfLastLocation = robotLocation;

            polygonToDisplay.polygon.Points.Add(new Point(robotLocation.X, robotLocation.Y));
            polygonToDisplay.polygon.Points.Add(new Point(waypointLocation[0].X, waypointLocation[0].Y));

            if (waypointLocation.Count > 1)
            {
                lastLocation = waypointLocation[waypointLocation.Count - 1];
                bfLastLocation = waypointLocation[waypointLocation.Count - 2];


                int i;
                for (i = 1; i < waypointLocation.Count; i++)
                {
                    polygonToDisplay.polygon.Points.Add(new Point(waypointLocation[i].X, waypointLocation[i].Y));
                }
            }



            double headingAngle = Math.Atan2(lastLocation.Y - bfLastLocation.Y, lastLocation.X - bfLastLocation.X);
            double angleTeteFleche1 = headingAngle + angleTeteFleche;
            double angleTeteFleche2 = headingAngle - angleTeteFleche;
            polygonToDisplay.polygon.Points.Add(new Point(lastLocation.X - longueurTeteFleche * Math.Cos(angleTeteFleche1), lastLocation.Y - longueurTeteFleche * Math.Sin(angleTeteFleche1)));
            polygonToDisplay.polygon.Points.Add(new Point(lastLocation.X, lastLocation.Y));
            polygonToDisplay.polygon.Points.Add(new Point(lastLocation.X - longueurTeteFleche * Math.Cos(angleTeteFleche2), lastLocation.Y - longueurTeteFleche * Math.Sin(angleTeteFleche2)));
            polygonToDisplay.polygon.Points.Add(new Point(lastLocation.X, lastLocation.Y));

            return polygonToDisplay;
        }

        public XyDataSeries<double, double> GetRobotLidarPoints(LidarDataType type)
        {
            var dataSeries = new XyDataSeries<double, double>();
            if (LidarRawPoints == null)
                return dataSeries;

            IEnumerable<double> listX = new List<double>();
            IEnumerable<double> listY = new List<double>();

            switch (type)
            {
                case LidarDataType.RawData:
                    listX = LidarRawPoints.Select(e => e.Pt.X);
                    listY = LidarRawPoints.Select(e => e.Pt.Y);
                    break;

            }

            if (listX.Count() == listY.Count())
            {
                dataSeries.AcceptsUnsortedData = true;
                dataSeries.Append(listX, listY);
            }
            return dataSeries;
        }

        public XyDataSeries<double, double> GetRobotLidarProcessedPoints()
        {
            var dataSeries = new XyDataSeries<double, double>();
            if (LidarProcessedPoints == null)
                return dataSeries;

            List<double> listX = new List<double>();
            List<double> listY = new List<double>();

            foreach (List<PointDExtended> list_of_points in LidarProcessedPoints)
            {
                if (list_of_points != null)
                {
                    listX.AddRange(list_of_points.Select(e => e.Pt.X).ToList());
                    listY.AddRange(list_of_points.Select(e => e.Pt.Y).ToList());
                }
            }
            
            if (listX.Count() == listY.Count())
            {
                dataSeries.AcceptsUnsortedData = true;
                dataSeries.Append(listX, listY);
            }
            return dataSeries;
        }

        public List<PointDExtended> GetRobotLidarExtendedPoints()
        {

            return (LidarProcessedPoints[0] == null)? new List<PointDExtended>():LidarProcessedPoints[0];
        }

        public List<SegmentExtended> GetRobotLidarSegments()
        {
            return LidarSegment == null?new List<SegmentExtended>():LidarSegment;
        }

        public List<LidarObject> GetRobotLidarObjects()
        {
            if (this.LidarObjectList == null)
                return new List<LidarObject>();

            return this.LidarObjectList;
        }
    }
}
