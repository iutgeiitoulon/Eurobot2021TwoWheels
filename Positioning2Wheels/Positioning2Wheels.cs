using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using EventArgsLibrary;
using Constants;

namespace Positioning2WheelsNs
{
    public class Positioning2Wheels
    {
        int robotId;
        public Positioning2Wheels(int id)
        {
            robotId = id;
        }

        // Init values
        Location locationRefTerrain = new Location(0, 0, 0, 0, 0, 0);

        public void OnOdometryRobotSpeedReceived(object sender, PolarSpeedArgs e)
        {

            locationRefTerrain.X = (double) (locationRefTerrain.X + (e.Vx / ConstVar.ODOMETRY_FREQ_IN_HZ) * Math.Cos(locationRefTerrain.Theta));
            locationRefTerrain.Y = (double) (locationRefTerrain.Y + (e.Vx / ConstVar.ODOMETRY_FREQ_IN_HZ) * Math.Sin(locationRefTerrain.Theta));
            locationRefTerrain.Theta = (double) (locationRefTerrain.Theta + e.Vtheta / ConstVar.ODOMETRY_FREQ_IN_HZ);

            OnCalculatedLocation(robotId, locationRefTerrain);
        }

        public void OnSetRobotLocation(object sender, LocationArgs e)
        {
            if (e.RobotId == robotId)
            {
                locationRefTerrain = e.Location;
            }
        }

        //Output events
        public event EventHandler<LocationArgs> OnCalculatedLocationEvent;
        public virtual void OnCalculatedLocation(int id, Location locationRefTerrain)
        {
            OnCalculatedLocationEvent?.Invoke(this, new LocationArgs
            {
                RobotId = id,
                Location = locationRefTerrain
            });
        }
    }
}
