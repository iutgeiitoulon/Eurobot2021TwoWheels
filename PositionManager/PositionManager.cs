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
        int errorCount;
        bool askForCalibration;
        List<Location> moving_list;
        Location RobotLocation;
        Location BestLocation;


        public PositionManager(int robotID)
        {
            robotId = robotID;
            errorCount = 0;
            askForCalibration = false;
            RobotLocation = new Location(0, 0, 0, 0, 0, 0);
        }

        public Location GetBestAngularLocation(List<Location> list_of_locations, Location actual_location)
        {
            return list_of_locations.OrderBy(x => Math.Abs(Toolbox.Modulo2PiAngleRad(x.Theta) - Toolbox.Modulo2PiAngleRad(actual_location.Theta))).FirstOrDefault();
        }

        public Location GetBestDistanceLocation(List<Location> list_of_locations, Location actual_location)
        {
            return list_of_locations.OrderBy(x => Toolbox.Distance(x, actual_location)).FirstOrDefault();
        }

        public void OnNewSafeLidarLocation(List<Location> list_of_locations)
        {
            /// TEMP:

            if (askForCalibration)
            {
                OnPositionMerged(GetBestDistanceLocation(list_of_locations, RobotLocation));
                askForCalibration = false;
            }
                
        }

        public void ResetSafeLidarLocation()
        {
            moving_list = new List<Location>();
        }

        //public void OnTest 

        #region InputsCallback
        public void OnCalibatrionAsked(object sender, EventArgs e)
        {
            askForCalibration = true;
        }

        public void OnLidarMultiplePostionReceived(object sender, List<Location> list_of_possibles_locations)
        {
            if (list_of_possibles_locations == null)
                return;

            if (list_of_possibles_locations.Count == 2)
                OnNewSafeLidarLocation(list_of_possibles_locations);
            else if (list_of_possibles_locations.Count > 2)
                ResetSafeLidarLocation();

            Location bestLocation = GetBestAngularLocation(list_of_possibles_locations, RobotLocation);
            if (Toolbox.Distance(bestLocation, RobotLocation) >= 0.2 || Math.Abs(bestLocation.Theta - RobotLocation.Theta) >= 5 * Math.PI / 180)
                errorCount++;
            else
                errorCount = 0;

            //if (errorCount >= 10)
                //askForCalibration = true;
        }

        

        public void OnOdometryPositionReceived(object sender, LocationArgs e)
        {
            if (e.RobotId == robotId)
            {
                OnPositionMerged(e.Location);
                //if (Math.Abs(e.Location.X) > (ConstVar.WIDTH_BOXSIZE / 2) || Math.Abs(e.Location.Y) > (ConstVar.HEIGHT_BOXSIZE / 2))
                //    askForCalibration = true;
            }
        }

        public void OnPositionReceived(object sender, LocationArgs e)
        {
            if (e.RobotId == robotId)
            {
                RobotLocation = e.Location;
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
