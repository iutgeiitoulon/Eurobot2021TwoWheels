using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FieldSetterNs;
using EventArgsLibrary;
using Constants;
using Utilities;
using WorldMapManager;
using WorldMap;

namespace TrajectoryGeneratorNs
{
    public class Waypoint
    {
        private static int sampling = 100;

        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public Waypoint Parent;

        public Waypoint(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Waypoint(Location location)
        {
            X = (int) (location.X * sampling);
            Y = (int) (location.Y * sampling);
        }

        internal static List<Location> waypointsToLocations(List<Waypoint> waypoints)
        {
            List<Location> locations = new List<Location>();
            foreach (Waypoint waypoint in waypoints)
            {
                locations.Add(new Location((double) waypoint.X / sampling, (double) waypoint.Y / sampling, 0, 0, 0, 0));
            }
            return locations;
        }

        internal static Location waypointToLocation(Waypoint waypoint)
        {
            return new Location((double) waypoint.X / sampling, (double) waypoint.Y / sampling, 0, 0, 0, 0);
        }
    }

    public class Obstacle : Field
    {
        public Obstacle(PointD center, double width, double height, double angle) :
            base(new RectangleOriented(center, width, height, angle), FieldType.DeadZone) 
        { }

        internal static List<Field> obstaclesToFields(List<Obstacle> obstacles) // For graphic visualization
        {
            List<Field> fields = new List<Field>();
            fields.AddRange(obstacles);
            return fields;
        }
    }

    

    public class TrajectoryGenerator
    {

        private List<Obstacle> getObstacles() // For graphic visualization
        {
            List<Obstacle> obstacles = new List<Obstacle>();

            obstacles.Add(new Obstacle(new PointD(0, 0), 0.2, 0.2, 0));

            return obstacles;
        }

        public void GenerateTrajectory(object sender, LocalWorldMap map) // For graphic visualization
        {
            List<Obstacle> obstacles = getObstacles();

            ((LocalWorldMapManager)sender).SetupFieldsZone(Obstacle.obstaclesToFields(obstacles));

            GenerateTrajectory(new Waypoint(map.RobotLocation), new Waypoint(map.DestinationLocation), obstacles);
        }

        public List<Waypoint> GenerateTrajectory(Waypoint start, Waypoint destination, List<Obstacle> obstacles)
        {

            Waypoint current = null;
            List<Waypoint> open_list = new List<Waypoint>();
            List<Waypoint> closed_list = new List<Waypoint>();
            int g = 0;

            open_list.Add(start);

            while (open_list.Count > 0)
            {
                double lowest = open_list.Min(w => w.F);
                current = open_list.First(w => w.F == lowest);

                closed_list.Add(current);
                open_list.Remove(current);

                if (closed_list.FirstOrDefault(w =>w.X == destination.X && w.Y == destination.Y) != null)
                    break;

                List<Waypoint> adjacent_waypoints = GetWalkableAdjacentWaypoint(current.X, current.Y, destination.X, destination.Y, obstacles);
                g++;

                foreach (Waypoint adjacent_waypoint in adjacent_waypoints)
                {
                    if (closed_list.FirstOrDefault(w => w.X == adjacent_waypoint.X && w.Y == adjacent_waypoint.Y) == null)
                    {
                        adjacent_waypoint.G = g;
                        adjacent_waypoint.H = ComputeHScore(adjacent_waypoint.X, adjacent_waypoint.Y, destination.X, destination.Y);
                        adjacent_waypoint.F = adjacent_waypoint.G + adjacent_waypoint.H;
                        adjacent_waypoint.Parent = current;

                        open_list.Insert(0, adjacent_waypoint);
                    }
                    else
                    {
                        if (g + adjacent_waypoint.H < adjacent_waypoint.F)
                        {
                            adjacent_waypoint.G = g;
                            adjacent_waypoint.F = adjacent_waypoint.G + adjacent_waypoint.H;
                            adjacent_waypoint.Parent = current;
                        }
                    }
                }
            }

            List<Waypoint> waypoints = new List<Waypoint>();
            while (current != null)
            {
                waypoints.Add(current);
                current = current.Parent;
            }
            waypoints.Reverse();

            LocationListArgs e = new LocationListArgs();
            e.LocationList = Waypoint.waypointsToLocations(waypoints);
            OnTrajectoryGenerated?.Invoke(this, e);
            return waypoints;
        }

        private List<Waypoint> GetWalkableAdjacentWaypoint(int x, int y, int destination_x, int destination_y, List<Obstacle> obstacles)
        {
            List<Waypoint> proposed_waypoint = new List<Waypoint>()
            {
                new Waypoint(x, y - 1),
                new Waypoint(x, y + 1),
                new Waypoint(x - 1, y),
                new Waypoint(x + 1, y),
                new Waypoint(x - 1, y - 1),
                new Waypoint(x - 1, y + 1),
                new Waypoint(x + 1, y + 1),
                new Waypoint(x + 1, y - 1)
            };

            return proposed_waypoint.Where( w =>
                !Toolbox.TestIfPointInsideAnOrientedRectangle(obstacles[0].Shape, Waypoint.waypointToLocation(w).PointD) ||
                (w.X == destination_x && w.Y == destination_y)
            ).ToList();
        }

        private int ComputeHScore(int x, int y, int destination_x, int destination_y)
        {
            return Math.Abs(destination_x - x) + Math.Abs(destination_y - y);
        }


        public event EventHandler<LocationListArgs> OnTrajectoryGenerated;
    }
}
