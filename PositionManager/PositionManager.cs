using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventArgsLibrary;
using Constants;
using Utilities;

namespace PositionManagerNs
{
    public class PositionManager
    {
        int robotId;
        Location lidar_location;
        Location odometry_location;


        public PositionManager(int robotID)
        {
            robotId = robotID;
        }

        public Location GetBestAngularLocation(List<Location> list_of_locations, Location actual_location)
        {
            return list_of_locations.OrderBy(x => Math.Abs(Toolbox.Modulo2PiAngleRad(x.Theta) - Toolbox.Modulo2PiAngleRad(actual_location.Theta))).FirstOrDefault();
        }

        public Location GetBestDistanceLocation(List<Location> list_of_locations, Location actual_location)
        {
            return list_of_locations.OrderBy(x => Toolbox.Distance(x, actual_location)).FirstOrDefault();
        }

        //public void OnNewSafeLidarLocation(List<Location> )



        #region InputsCallback
        public void OnCalibatrionAsked(object sender, EventArgs e)
        {

        }

        public void OnLidarMultiplePostionReceived(object sender, List<Location> list_of_possibles_locations)
        {

        }


        public void OnOdometryPositionReceived(object sender, LocationArgs e)
        {
            if (e.RobotId == robotId)
            {

            }
        }
        #endregion

        #region Events
        public event EventHandler<LocationArgs> OnPositionMergedEvent;

        public void OnPositionMerged(Location location)
        {
            OnPositionMergedEvent?.Invoke(this, new LocationArgs { RobotId = robotId, Location = location });
        }

        #endregion
    }
}
