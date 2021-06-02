
using System;
using System.Windows;
using Utilities;

namespace WpfWorldMapDisplay
{
    public class ObstacleDisplay
    {
        private Random rand = new Random();
        public LocationExtended location = new LocationExtended(0, 0, 0, 0, 0, 0, ObjectType.Obstacle);
        private System.Drawing.Color backgroundColor = System.Drawing.Color.Yellow;
        private System.Drawing.Color borderColor = System.Drawing.Color.DarkRed;
        private int borderWidth = 5;

        public ObstacleDisplay(LocationExtended loc)
        {
            location = loc;
        }

        public PolygonExtended GetObstaclePolygon()
        {
            PolygonExtended polygonToDisplay = new PolygonExtended();
            if (location != null)
            {
                int nbSegments = 1;
                double radius = 0.05;
                for (double theta = 0; theta <= Math.PI * 2; theta += Math.PI * 2 / nbSegments)
                {
                    Point pt = new Point(radius * Math.Cos(theta), radius * Math.Sin(theta));
                    pt.X += location.X;
                    pt.Y += location.Y;
                    polygonToDisplay.polygon.Points.Add(pt);
                    polygonToDisplay.backgroundColor = backgroundColor;
                    polygonToDisplay.borderColor = borderColor;
                    polygonToDisplay.borderWidth = borderWidth;
                }
            }
            return polygonToDisplay;
        }
        public PolygonExtended GetObstacleSpeedArrow()
        {
            PolygonExtended polygonToDisplay = new PolygonExtended();
            if (location != null)
            {
                double angleTeteFleche = Math.PI / 6;
                double longueurTeteFleche = 0.30;
                double LongueurFleche = Math.Sqrt(location.Vx * location.Vx + location.Vy * location.Vy);
                double headingAngle = Math.Atan2(location.Vy, location.Vx) + location.Theta;
                double xTete = LongueurFleche * Math.Cos(headingAngle);
                double yTete = LongueurFleche * Math.Sin(headingAngle);

                polygonToDisplay.polygon.Points.Add(new Point(location.X, location.Y));
                polygonToDisplay.polygon.Points.Add(new Point(location.X + xTete, location.Y + yTete));
                double angleTeteFleche1 = headingAngle + angleTeteFleche;
                double angleTeteFleche2 = headingAngle - angleTeteFleche;
                polygonToDisplay.polygon.Points.Add(new Point(location.X + xTete - longueurTeteFleche * Math.Cos(angleTeteFleche1), location.Y + yTete - longueurTeteFleche * Math.Sin(angleTeteFleche1)));
                polygonToDisplay.polygon.Points.Add(new Point(location.X + xTete, location.Y + yTete));
                polygonToDisplay.polygon.Points.Add(new Point(location.X + xTete - longueurTeteFleche * Math.Cos(angleTeteFleche2), location.Y + yTete - longueurTeteFleche * Math.Sin(angleTeteFleche2)));
                polygonToDisplay.polygon.Points.Add(new Point(location.X + xTete, location.Y + yTete));
                polygonToDisplay.borderWidth = 2;
                polygonToDisplay.borderColor = System.Drawing.Color.FromArgb(0xFF, 0xFF, 0x00, 0x00);
                polygonToDisplay.borderDashPattern = new double[] { 3, 3 };
                polygonToDisplay.borderOpacity = 1;
                polygonToDisplay.backgroundColor = System.Drawing.Color.FromArgb(0x00, 0x00, 0x00, 0x00);
            }
            return polygonToDisplay;
        }
    }
}
