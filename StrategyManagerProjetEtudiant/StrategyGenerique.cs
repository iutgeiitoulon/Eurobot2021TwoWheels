using Constants;
using EventArgsLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Utilities;
using WorldMap;

namespace StrategyManagerProjetEtudiantNS
{
    /****************************************************************************/
    /// <summary>
    /// Il y a un Strategy Manager par robot, qui partage la même Global World Map -> les stratégies collaboratives sont possibles
    /// Le Strategy Manager a pour rôle de déterminer les déplacements et les actions du robot auquel il appartient
    /// 
    /// Il implante implante à minima le schéma de fonctionnement suivant
    /// - Récupération asynchrone de la Global World Map décrivant l'état du monde autour du robot
    ///     La Global World Map inclus en particulier l'état du jeu (à voir pour changer cela)
    /// - Sur Timer Strategy : détermination si besoin du rôle du robot :
    ///         - simple si Eurobot car les rôles sont figés
    ///         - complexe dans le cas de la RoboCup car les rôles sont changeant en fonction des positions et du contexte.
    /// - Sur Timer Strategy : Itération des machines à état de jeu définissant les déplacements et actions
    ///         - implante les machines à état de jeu à Eurobot, ainsi que les règles spécifiques 
    ///         de jeu (déplacement max en controlant le ballon par exemple à la RoboCup).
    ///         - implante les règles de mise à jour 
    ///             des zones préférentielles de destination (par exemple la balle pour le joueur qui la conteste à la RoboCup), 
    ///             des zones interdites (par exemple les zones de départ à Eurobot), d
    ///             es zones à éviter (par exemple pour se démarquer à la RoboCup)...
    /// - DONE - Sur Timer Strategy : génération de la HeatMap de positionnement X Y donnant l'indication d'intérêt de chacun des points du terrain
    ///     et détermination de la destination théorique (avant inclusion des masquages waypoint)
    /// - DONE - Sur Timer Strategy : prise en compte de la osition des obstacles pour générer la HeatMap de WayPoint 
    ///     et trouver le WayPoint courant.
    /// - Sur Timer Strategy : gestion des actions du robot en fonction du contexte
    ///     Il est à noter que la gestion de l'orientation du robot (différente du cap en déplacement de celui-ci)
    ///     est considérée comme une action, et non comme un déplacement car celle-ci dépend avant tout du contexte du jeu
    ///     et non pas de la manière d'aller à un point.
    /// </summary>

    /****************************************************************************/
    public abstract class StrategyGenerique
    {
        public int robotId = 0;
        public int teamId = 0;
        public string teamIpAddress = "";
        public string DisplayName;

        public GlobalWorldMap globalWorldMap;
        public LocalWorldMap localWorldMap;
        public Location robotCurrentLocation = new Location(0, 0, 0, 0, 0, 0);
        public double robotOrientation;

        
        System.Timers.Timer timerStrategy;

        public StrategyGenerique(int robotId, int teamId, string teamIpAddress)
        {
            this.teamId = teamId;
            this.robotId = robotId;
            this.teamIpAddress = teamIpAddress;

            globalWorldMap = new GlobalWorldMap();

            timerStrategy = new System.Timers.Timer();
            timerStrategy.Interval = 50;
            timerStrategy.Elapsed += TimerStrategy_Elapsed;
            timerStrategy.Start();
        }

        public abstract void InitStrategy();

        //************************ Events reçus ************************************************/
        //public abstract void OnRefBoxMsgReceived(object sender, WorldMap.RefBoxMessageArgs e);
        public void OnLocalWorldMapReceived(object sender, LocalWorldMap e)
        {
            //On récupère la nouvelle worldMap
            lock (localWorldMap)
            {
                localWorldMap = e;
            }
        }


        /// Evènement envoyé par le module de gestion de la LocalWorldMap
        public void OnGlobalWorldMapReceived(object sender, GlobalWorldMap e)
        {
            //On récupère la nouvelle worldMap
            lock (globalWorldMap)
            {
                globalWorldMap = e;
            }
        }

        /// Evènement envoyé par le module de calcul de Positioning
        public void OnPositionRobotReceived(object sender, LocationArgs location)
        {

            robotCurrentLocation.X = location.Location.X;
            robotCurrentLocation.Y = location.Location.Y;
            robotCurrentLocation.Theta = location.Location.Theta;

            robotCurrentLocation.Vx = location.Location.Vx;
            robotCurrentLocation.Vy = location.Location.Vy;
            robotCurrentLocation.Vtheta = location.Location.Vtheta;
        }

        private void TimerStrategy_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Mise à jour de l'affichage de la world map
            OnUpdateWorldMapDisplay(robotId);

        }


        public abstract void OnGhostLocationReached(object sender, LocationArgs e);
        public abstract void OnRobotLocationReached(object sender, LocationArgs e);


        /****************************************** Events envoyés ***********************************************/

        public event EventHandler<LocationArgs> OnDestinationEvent;
        public event EventHandler<EventArgs> OnUpdateWorldMapDisplayEvent;
        public event EventHandler<IndependantPIDSetupArgs> On2WheelsIndependantSpeedPIDSetupEvent;
        public event EventHandler<ByteEventArgs> OnSetAsservissementModeEvent;
        public event EventHandler<PolarPIDSetupArgs> OnPolarPIDSetupEvent;
        public event EventHandler<IndependantPIDSetupArgs> OnIndependantPIDSetupEvent;
        public event EventHandler<SpeedConsigneToMotorArgs> OnSetSpeedConsigneToMotor;
        public event EventHandler<BoolEventArgs> OnEnableDisableMotorCurrentDataEvent;
        public event EventHandler<CollisionEventArgs> OnCollisionEvent;
        public event EventHandler<IOValuesEventArgs> OnIOValuesFromRobotEvent;
        public event EventHandler<DoubleEventArgs> OnOdometryPointToMeterSetupEvent;
        public event EventHandler<TwoWheelsAngleArgs> On2WheelsAngleSetupEvent;
        public event EventHandler<TwoWheelsToPolarMatrixArgs> On2WheelsToPolarMatrixSetupEvent;
        public event EventHandler<StringEventArgs> OnTextMessageEvent;
        public event EventHandler<Location> OnWaypointsReachedEvent;
        public event EventHandler<Location> OnDestinationReachedEvent;
        public event EventHandler<LocationArgs> OnSetActualLocationEvent;
        public event EventHandler<PositionArgs> OnSetWantedLocationEvent;
        public event EventHandler<List<Location>> OnSetWaypointsListEvent;
        public event EventHandler<Location> OnSetNewWaypointEvent;
        public event EventHandler<Location> OnSetNewDestinationEvent;
        public event EventHandler<List<RectangleOriented>> OnNewDeadZonesEvents;



        public virtual void OnDestination(int id, Location location)
        {
            OnDestinationEvent?.Invoke(this, new LocationArgs { RobotId = id, Location = location });
        }

        
        public virtual void OnUpdateWorldMapDisplay(int id)
        {
            OnUpdateWorldMapDisplayEvent?.Invoke(this, new EventArgs());
        }

        
        public virtual void On2WheelsIndependantSpeedPIDSetup(double pM1, double iM1, double dM1, double pM2, double iM2, double dM2,
            double pM1Limit, double iM1Limit, double dM1Limit, double pM2Limit, double iM2Limit, double dM2Limit)
        {
            On2WheelsIndependantSpeedPIDSetupEvent?.Invoke(this, new IndependantPIDSetupArgs
            {
                P_M1 = pM1,
                I_M1 = iM1,
                D_M1 = dM1,
                P_M2 = pM2,
                I_M2 = iM2,
                D_M2 = dM2,
                P_M1_Limit = pM1Limit,
                I_M1_Limit = iM1Limit,
                D_M1_Limit = dM1Limit,
                P_M2_Limit = pM2Limit,
                I_M2_Limit = iM2Limit,
                D_M2_Limit = dM2Limit,
            });
        }

        
        public virtual void OnSetRobotSpeedPolarPID(double px, double ix, double dx, double py, double iy, double dy, double ptheta, double itheta, double dtheta,
            double pxLimit, double ixLimit, double dxLimit, double pyLimit, double iyLimit, double dyLimit, double pthetaLimit, double ithetaLimit, double dthetaLimit
            )
        {
            OnPolarPIDSetupEvent?.Invoke(this, new PolarPIDSetupArgs
            {
                P_x = px,
                I_x = ix,
                D_x = dx,
                P_y = py,
                I_y = iy,
                D_y = dy,
                P_theta = ptheta,
                I_theta = itheta,
                D_theta = dtheta,
                P_x_Limit = pxLimit,
                I_x_Limit = ixLimit,
                D_x_Limit = dxLimit,
                P_y_Limit = pyLimit,
                I_y_Limit = iyLimit,
                D_y_Limit = dyLimit,
                P_theta_Limit = pthetaLimit,
                I_theta_Limit = ithetaLimit,
                D_theta_Limit = dthetaLimit
            });
        }

        
        public virtual void OnSetRobotSpeedIndependantPID(double pM1, double iM1, double dM1, double pM2, double iM2, double dM2, double pM3, double iM3, double dM3, double pM4, double iM4, double dM4,
            double pM1Limit, double iM1Limit, double dM1Limit, double pM2Limit, double iM2Limit, double dM2Limit, double pM3Limit, double iM3Limit, double dM3Limit, double pM4Limit, double iM4Limit, double dM4Limit
            )
        {
            OnIndependantPIDSetupEvent?.Invoke(this, new IndependantPIDSetupArgs
            {
                P_M1 = pM1,
                I_M1 = iM1,
                D_M1 = dM1,
                P_M2 = pM2,
                I_M2 = iM2,
                D_M2 = dM2,
                P_M3 = pM3,
                I_M3 = iM3,
                D_M3 = dM3,
                P_M4 = pM4,
                I_M4 = iM4,
                D_M4 = dM4,
                P_M1_Limit = pM1Limit,
                I_M1_Limit = iM1Limit,
                D_M1_Limit = dM1Limit,
                P_M2_Limit = pM2Limit,
                I_M2_Limit = iM2Limit,
                D_M2_Limit = dM2Limit,
                P_M3_Limit = pM3Limit,
                I_M3_Limit = iM3Limit,
                D_M3_Limit = dM3Limit,
                P_M4_Limit = pM4Limit,
                I_M4_Limit = iM4Limit,
                D_M4_Limit = dM4Limit
            });
        }

        
        public virtual void OnSetAsservissementMode(byte val)
        {
            OnSetAsservissementModeEvent?.Invoke(this, new ByteEventArgs { Value = val });
        }

        
        public virtual void OnSetSpeedConsigneToMotorEvent(object sender, SpeedConsigneToMotorArgs e)
        {
            OnSetSpeedConsigneToMotor?.Invoke(sender, e);
        }

       
        public virtual void OnEnableDisableMotorCurrentData(bool val)
        {
            OnEnableDisableMotorCurrentDataEvent?.Invoke(this, new BoolEventArgs { value = val });
        }

        
        public virtual void OnCollision(int id, Location robotLocation)
        {
            OnCollisionEvent?.Invoke(this, new CollisionEventArgs { RobotId = id, RobotRealPositionRefTerrain = robotLocation });
        }

        
        public void OnIOValuesFromRobot(object sender, IOValuesEventArgs e)
        {
            OnIOValuesFromRobotEvent?.Invoke(sender, e);
        }

       
        public void OnOdometryPointToMeter(double value)
        {
            OnOdometryPointToMeterSetupEvent?.Invoke(this, new DoubleEventArgs { Value = value });
        }

        
        public void On2WheelsAngleSetup(double angleM1, double angleM2)
        {
            On2WheelsAngleSetupEvent?.Invoke(this, new TwoWheelsAngleArgs { angleMotor1 = angleM1, angleMotor2 = angleM2});
        }

        
        public void On2WheelsToPolarMatrixSetup(double mX1, double mX2, double mTheta1, double mTheta2)
        {
            On2WheelsToPolarMatrixSetupEvent?.Invoke(this, new TwoWheelsToPolarMatrixArgs
            {
                mx1 = mX1,
                mx2 = mX2,
                mtheta1 = mTheta1,
                mtheta2 = mTheta2,
            });
        }

        
        public virtual void OnTextMessage(string str)
        {
            OnTextMessageEvent?.Invoke(this, new StringEventArgs { value = str });
        }

        
        public virtual void OnWaypointsReached(Location location)
        {
            OnWaypointsReachedEvent?.Invoke(this, location);
        }

        
        public virtual void OnDestinationReached(Location location)
        {
            OnDestinationReachedEvent?.Invoke(this, location);
        }

       

        public void OnSetActualLocation(Location location)
        {
            OnSetActualLocationEvent?.Invoke(this, new LocationArgs { RobotId = robotId , Location = location });
        }

        public void OnSetWantedLocation(Location location)
        {
            OnSetWantedLocationEvent?.Invoke(this, new PositionArgs { RobotId = robotId, X = location.X, Y = location.Y, Theta = location.Theta, Reliability = 0 });
        }

        public void OnSetWaypointsList(List<Location> list_of_location)
        {
            OnSetWaypointsListEvent?.Invoke(this, list_of_location);
        }

        public void OnSetNewWaypoint(Location location)
        {
            OnSetNewWaypointEvent?.Invoke(this, location);
        }

        public void OnSetNewWaypoint(PointD point)
        {
            OnSetNewWaypointEvent?.Invoke(this, new Location(point.X, point.Y, 0, 0, 0, 0)); 
        }

        public void OnNewDeadZones(List<RectangleOriented> list_of_deadzones)
        {
            OnNewDeadZonesEvents?.Invoke(this, list_of_deadzones);
        }

    }    
}
