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
            //locationRefTerrain.X = (double)(locationRefTerrain.X + (e.Vx / ConstVar.ODOMETRY_FREQ_IN_HZ) * Math.Cos(locationRefTerrain.Theta));
            //locationRefTerrain.Y = (double)(locationRefTerrain.Y + (e.Vx / ConstVar.ODOMETRY_FREQ_IN_HZ) * Math.Sin(locationRefTerrain.Theta));
            //locationRefTerrain.Theta = (double)(locationRefTerrain.Theta + e.Vtheta / ConstVar.ODOMETRY_FREQ_IN_HZ);
            //locationRefTerrain.Vx = e.Vx;
            //locationRefTerrain.Vy = 0;
            //locationRefTerrain.Vtheta = e.Vtheta;


            //OnCalculatedLocation(robotId, locationRefTerrain);
        }

        public void OnExternalOdometryRobotSpeedReceived(object sender, IndependantSpeedEventArgs e)
        {
            double right_motor_speed = - e.VitesseMoteur3 * (ConstVar.ROBOT_ENCODER_POINT_TO_METER / ConstVar.EUROBOT_ODOMETRY_POINT_TO_METER);
            double left_motor_speed = e.VitesseMoteur4 * (ConstVar.ROBOT_ENCODER_POINT_TO_METER / ConstVar.EUROBOT_ODOMETRY_POINT_TO_METER);

            double Vx =     (right_motor_speed + left_motor_speed) / 2;
            double Vtheta = (right_motor_speed - left_motor_speed) / ConstVar.ROBOT_ENCODER_DIST_WHEELS;


            locationRefTerrain.X = (double) (locationRefTerrain.X + (Vx / ConstVar.ODOMETRY_FREQ_IN_HZ) * Math.Cos(locationRefTerrain.Theta));
            locationRefTerrain.Y = (double) (locationRefTerrain.Y + (Vx / ConstVar.ODOMETRY_FREQ_IN_HZ) * Math.Sin(locationRefTerrain.Theta));
            locationRefTerrain.Theta = (double) (locationRefTerrain.Theta + Vtheta / ConstVar.ODOMETRY_FREQ_IN_HZ);
            locationRefTerrain.Vx = Vx;
            locationRefTerrain.Vy = 0;
            locationRefTerrain.Vtheta = Vtheta;

            OnExternalEncoderPolarSpeedEvent?.Invoke(this, new PolarSpeedEventArgs { RobotId = robotId, timeStampMs = e.timeStampMs, Vx = Vx, Vy = 0, Vtheta = Vtheta });
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
        public event EventHandler<PolarSpeedEventArgs> OnExternalEncoderPolarSpeedEvent;

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
