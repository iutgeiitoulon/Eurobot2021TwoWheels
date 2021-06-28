using Constants;
using EventArgsLibrary;
using System;
using Utilities;


namespace TrajectoryPlannerNs
{
    public class TrajectoryPlanner
    {
        /// <summary>
        ///  Implementation base on: https://www.vgies.com/downloads/Cours/Enonce-Projet-electronique-Partie-Haut-Niveau.pdf
        /// </summary>
        #region Variables

        int robotId;
        Location RobotLocation;
        Location GhostLocation;
        Location WantedDestination;

        double MajorationLin, ecartement;

        bool isEndRotating = false;
        bool isEnslave = true;
        bool isReversed = false;
        bool isUrgence = false;

        AsservissementPID PID_Position_Lineaire;
        AsservissementPID PID_Position_Angulaire;

        GhostState state = GhostState.Wait;
        #endregion

        #region Fonctions
        public TrajectoryPlanner(int id)
        {
            robotId = id;
            InitRobotPosition(0, 0, 0);
            InitPositionPID();
        }

        void InitPositionPID()
        {
            PID_Position_Lineaire = new AsservissementPID(
                ConstVar.PLANNER_PID_POSITION_LINEAR_KP, ConstVar.PLANNER_PID_POSITION_LINEAR_KI, ConstVar.PLANNER_PID_POSITION_LINEAR_KD, 
                ConstVar.PLANNER_PID_POSITION_LINEAR_MAX_KP, ConstVar.PLANNER_PID_POSITION_LINEAR_MAX_KI, ConstVar.PLANNER_PID_POSITION_LINEAR_MAX_KD
                );

            PID_Position_Angulaire = new AsservissementPID(
                ConstVar.PLANNER_PID_POSITION_ANGULAR_KP, ConstVar.PLANNER_PID_POSITION_ANGULAR_KI, ConstVar.PLANNER_PID_POSITION_ANGULAR_KD,
                ConstVar.PLANNER_PID_POSITION_ANGULAR_MAX_KP, ConstVar.PLANNER_PID_POSITION_ANGULAR_MAX_KI, ConstVar.PLANNER_PID_POSITION_ANGULAR_MAX_KD
                );
        }

        public void InitRobotPosition(double x, double y, double theta)
        {
            RobotLocation = new Location(x, y, theta, 0, 0, 0);
            WantedDestination = new Location(x, y, theta, 0, 0, 0);
            GhostLocation = new Location(x, y, theta, 0, 0, 0);
            state = GhostState.Angular;
            PIDPositionReset();
        }



        public void SelectMajoration(double min, double max, double coef)
        {
            MajorationLin = Math.Pow(Toolbox.Distance(GhostLocation, WantedDestination), 2) * coef;
            MajorationLin = Math.Max(MajorationLin, min);
            MajorationLin = Math.Min(MajorationLin, max);
        }

        public void ResetGhost()
        {
            GhostLocation = new Location(RobotLocation.X, RobotLocation.Y, RobotLocation.Theta, 0, 0, 0);
        }

        public void Start(Location e)
        {
            WantedDestination = e;

            ResetGhost();
            OnDestinationSet(robotId, WantedDestination);
            SelectMajoration(ConstVar.PLANNER_MAJORATION_LINEAR_MIN, ConstVar.PLANNER_MAJORATION_LINEAR_MAX, ConstVar.PLANNER_MAJORATION_LINEAR_COEFF);
            state = GhostState.Angular;
        }

        public void Stop()
        {
            isUrgence = true;
            state = GhostState.Arret;
        }


        #endregion

        #region Ghost
        void CalculateGhostPosition()
        {

            ecartement = Toolbox.Distance(GhostLocation.X, GhostLocation.Y, RobotLocation.X, RobotLocation.Y);
            if (ecartement > ConstVar.PLANNER_MAX_SPACING)
            {
                PIDPositionReset();
                ResetGhost();
                state = GhostState.Angular;
            }

            if (state != GhostState.Wait && state != GhostState.Arret && WantedDestination != null )
            {
                OnDestinationSet(robotId, WantedDestination);
            }

            switch (state)
            {
                case GhostState.Wait:
                    GhostLocation.Vtheta = 0;
                    GhostLocation.Vx = 0;

                    if (Toolbox.Distance(RobotLocation, GhostLocation) <= ConstVar.PLANNER_LINEAR_ROBOT_DEAD_ZONE)
                        OnRobotDestinationReached();
                    else
                    {
                        ResetGhost();
                        state = GhostState.Arret;
                    }

                    break;

                case GhostState.Arret:
                    if (Math.Abs(GhostLocation.Vx) >= ConstVar.PLANNER_LINEAR_SPEED_MIN)
                    {
                        if (isReversed)
                            GhostLocation.Vx += ConstVar.PLANNER_MAX_LINEAR_ACCELERATION / ConstVar.ODOMETRY_FREQ_IN_HZ;
                        else
                            GhostLocation.Vx -= ConstVar.PLANNER_MAX_LINEAR_ACCELERATION / ConstVar.ODOMETRY_FREQ_IN_HZ;
                    }
                    else
                    {
                        ResetGhost();
                        if (!isUrgence)
                            state = GhostState.Angular;
                    }

                    GhostLocation.X += GhostLocation.Vx / ConstVar.ODOMETRY_FREQ_IN_HZ * Math.Cos(GhostLocation.Theta);
                    GhostLocation.Y += GhostLocation.Vx / ConstVar.ODOMETRY_FREQ_IN_HZ * Math.Sin(GhostLocation.Theta);
                    OnGhostLocation(robotId, GhostLocation);
                    break;

                case GhostState.Angular:
                    GenerateGhostRotation();
                    break;
                case GhostState.AngularEnd:
                    state = GhostState.Linear;
                    break;
                case GhostState.Linear:
                    GenerateGhostShifting();
                    break;
                case GhostState.LinearEnd:
                    if (isEndRotating)
                        state = GhostState.AngularTurn;
                    else
                        state = GhostState.Wait;
                    break;
                case GhostState.AngularTurn:
                    GenerateGhostRotation(true);
                    break;
                case GhostState.AngularTurnEnd:
                    state = GhostState.Wait;
                    break;
            }
        }

        void GenerateGhostRotation(bool isEndTurn = false)
        {
            double WantedTheta;

            if (!isEndTurn)
                WantedTheta = Math.Atan2(WantedDestination.Y - GhostLocation.Y, WantedDestination.X - GhostLocation.X);
            else
                WantedTheta = WantedDestination.Theta;

            

            WantedTheta += isReversed ? Math.PI : 0;

            

            double dThetaFreinage = Math.Pow(GhostLocation.Vtheta, 2) / (2 * ConstVar.PLANNER_MAX_ANGULAR_ACCELERATION);
            double dThetaRestant = WantedTheta - Toolbox.ModuloByAngle(WantedTheta, GhostLocation.Theta);

            if ((Toolbox.Distance(GhostLocation, WantedDestination) == 0) && !isEndTurn)
                dThetaRestant = 0;

            if (dThetaFreinage * ConstVar.PLANNER_MAJORATION_ANGULAR < Math.Abs(dThetaRestant))
                GhostLocation.Vtheta += Math.Sign(dThetaRestant) * ConstVar.PLANNER_MAX_ANGULAR_ACCELERATION / ConstVar.ODOMETRY_FREQ_IN_HZ;
            else
                GhostLocation.Vtheta -= Math.Sign(GhostLocation.Vtheta) * ConstVar.PLANNER_MAX_ANGULAR_ACCELERATION / ConstVar.ODOMETRY_FREQ_IN_HZ;

            GhostLocation.Vtheta = Toolbox.LimitToInterval(GhostLocation.Vtheta, -ConstVar.PLANNER_MAX_ANGULAR_SPEED, ConstVar.PLANNER_MAX_ANGULAR_SPEED);
            GhostLocation.Theta += GhostLocation.Vtheta / ConstVar.ODOMETRY_FREQ_IN_HZ;

            OnGhostLocation(robotId, GhostLocation);

            if (Math.Abs(dThetaRestant) <= Toolbox.DegToRad(ConstVar.PLANNER_ANGULAR_GHOST_DEAD_ZONE))
                if (!isEndTurn)
                    state = GhostState.AngularEnd;
                else
                    state = GhostState.AngularTurnEnd;
        }

        void GenerateGhostShifting()
        {
            PointD xy_ghost = new PointD(GhostLocation.X, GhostLocation.Y);
            PointD xy_wanted = new PointD(WantedDestination.X, WantedDestination.Y);

            PointD project_point = new PointD(xy_ghost.X + Math.Cos(GhostLocation.Theta), xy_ghost.Y + Math.Sin(GhostLocation.Theta));

            /// Calcul du point projeté + remplacement du waypoint par ce point
            PointD projectedGhost = Toolbox.ProjectedPointOnLineFromWaypoint(xy_wanted, xy_ghost, project_point); /// Make A projection for avoiding infinity movement

            double dLinFreinage = Math.Pow(GhostLocation.Vx, 2) / (2 * ConstVar.PLANNER_MAX_LINEAR_ACCELERATION);
            double dLinRestant = Toolbox.Distance(GhostLocation.X, GhostLocation.Y, projectedGhost.X, projectedGhost.Y);

            double thetaCor = Math.Atan2(projectedGhost.Y - GhostLocation.Y, projectedGhost.X - GhostLocation.X);
            double thetaRestant = thetaCor - Toolbox.ModuloByAngle(thetaCor, GhostLocation.Theta);

            bool cibleDevantLin = Math.Abs(thetaRestant) < Math.PI / 2;

            if (cibleDevantLin)
            {
                if (GhostLocation.Vx < 0)
                    GhostLocation.Vx += ConstVar.PLANNER_MAX_LINEAR_ACCELERATION / ConstVar.ODOMETRY_FREQ_IN_HZ;

                else
                {
                    if (dLinFreinage * MajorationLin < dLinRestant)
                    {
                        if (Math.Abs(GhostLocation.Vx) < ConstVar.PLANNER_MAX_LINEAR_SPEED)
                            GhostLocation.Vx += ConstVar.PLANNER_MAX_LINEAR_ACCELERATION / ConstVar.ODOMETRY_FREQ_IN_HZ;
                    }
                    else
                        GhostLocation.Vx -= ConstVar.PLANNER_MAX_LINEAR_ACCELERATION / ConstVar.ODOMETRY_FREQ_IN_HZ;
                }

            }
            else if (!cibleDevantLin)
            {
                if (GhostLocation.Vx > 0)
                    GhostLocation.Vx -= ConstVar.PLANNER_MAX_LINEAR_ACCELERATION / ConstVar.ODOMETRY_FREQ_IN_HZ;

                else
                {
                    if (dLinFreinage * MajorationLin < dLinRestant)
                    {
                        if (Math.Abs(GhostLocation.Vx) < ConstVar.PLANNER_MAX_LINEAR_SPEED)
                            GhostLocation.Vx -= ConstVar.PLANNER_MAX_LINEAR_ACCELERATION / ConstVar.ODOMETRY_FREQ_IN_HZ;
                    }
                    else
                        GhostLocation.Vx += ConstVar.PLANNER_MAX_LINEAR_ACCELERATION / ConstVar.ODOMETRY_FREQ_IN_HZ;
                }
            }

            GhostLocation.X += GhostLocation.Vx / ConstVar.ODOMETRY_FREQ_IN_HZ * Math.Cos(GhostLocation.Theta);
            GhostLocation.Y += GhostLocation.Vx / ConstVar.ODOMETRY_FREQ_IN_HZ * Math.Sin(GhostLocation.Theta);

            OnGhostLocation(robotId, GhostLocation);

            if (dLinRestant <= ConstVar.PLANNER_LINEAR_GHOST_DEAD_ZONE && Math.Abs(GhostLocation.Vx) <= ConstVar.PLANNER_LINEAR_SPEED_MIN)
                state = GhostState.LinearEnd;
        }

        #endregion

        #region PID

        void PIDPosition()
        {
            double VxeaireRobot, vAngulaireRobot;
            PointD PID_ProjectionPoint;

            // calcul de l'erreur angulaire
            double Erreur_AnglualireReel = GhostLocation.Theta - RobotLocation.Theta;
            vAngulaireRobot = PID_Position_Angulaire.CalculatePIDoutput(Erreur_AnglualireReel, 1 / ConstVar.ODOMETRY_FREQ_IN_HZ);

            double thetaCible = Math.Atan2(GhostLocation.Y - RobotLocation.Y, GhostLocation.X - RobotLocation.X);
            double thetaRestant = thetaCible - Toolbox.ModuloByAngle(thetaCible, RobotLocation.Theta);

            PointD xy_robot = new PointD(RobotLocation.X, RobotLocation.Y);
            PointD xy_wanted = new PointD(GhostLocation.X, GhostLocation.Y);

            PointD project_point = new PointD(xy_robot.X + Math.Cos(RobotLocation.Theta), xy_robot.Y + Math.Sin(RobotLocation.Theta));

            PID_ProjectionPoint = Toolbox.ProjectedPointOnLineFromWaypoint(xy_wanted, xy_robot, project_point); /// Make A projection for avoiding infinity movement

            int IsInFront = Math.Abs(thetaRestant) <= (Math.PI / 2) ? 1 : -1;

            double Erreur_LinReel = Toolbox.Distance(PID_ProjectionPoint, xy_robot) * IsInFront;

            VxeaireRobot = PID_Position_Lineaire.CalculatePIDoutput(Erreur_LinReel, 1 / ConstVar.ODOMETRY_FREQ_IN_HZ);

            /// On envoie les vitesses consigne.
            /// Indispensable en permanence, sinon la sécurité de l'embarqué reset le contrôle moteur
            /// en l'absence d'orde pendant 200ms
            if (isEnslave)
                OnSpeedConsigneToRobot(robotId, VxeaireRobot, vAngulaireRobot);
            else
                OnSpeedConsigneToRobot(robotId, 0, 0);
        }

        void PIDPositionReset()
        {
            if (PID_Position_Angulaire != null && PID_Position_Lineaire != null)
            {
                PID_Position_Lineaire.ResetPID(0);
                PID_Position_Angulaire.ResetPID(0);
            }
        }

        #endregion

        #region Inputs Callback
        public void OnPhysicalPositionReceived(object sender, LocationArgs e)
        {
            if (robotId == e.RobotId)
            {
                RobotLocation = e.Location;
                CalculateGhostPosition();
                PIDPosition();
            }
        }

        public void SetDestination(object sender, PositionArgs e)
        {
            if (e.RobotId == robotId)
            {
                Start(new Location(e.X, e.Y, e.Theta, 0, 0, 0));
            }
        }

        public void OnResetOrder(object sender, EventArgs e)
        {
            Start(WantedDestination);
        }

        public void OnEnableDisableAsservReceived(object sender, bool e)
        {
            isEnslave = e;
        }

        public void OnEndRotateEnableDisableReceived(object sender, bool e)
        {
            isEndRotating = e;
        }

        public void OnCollisionDetect(object sender, bool e)
        {
            if (e)
            {
                isUrgence = true;
                state = GhostState.Arret;
            }
            else 
            {
                isUrgence = false;
            }
        }
        #endregion

        #region Events
        public event EventHandler<LocationArgs> OnGhostLocationEvent;
        public event EventHandler<PolarSpeedArgs> OnSpeedConsigneEvent;
        public event EventHandler<LocationArgs> OnGhostDestinationReachedEvent;
        public event EventHandler<LocationArgs> OnRobotDestinationReachedEvent;
        public event EventHandler<LocationArgs> OnDestinationSetEvent;

        public virtual void OnGhostLocation(int id, Location loc)
        {
            OnGhostLocationEvent?.Invoke(this, new LocationArgs { RobotId = id, Location = loc });
        }

        public virtual void OnSpeedConsigneToRobot(int id, double Vxeaire, double vAngulaire)
        {
            OnSpeedConsigneEvent?.Invoke(this, new PolarSpeedArgs { RobotId = id, Vx = Vxeaire, Vy = 0, Vtheta = vAngulaire });
        }

        public virtual void OnGhostDestinationReached()
        {
            OnGhostDestinationReachedEvent?.Invoke(this, new LocationArgs { RobotId = robotId, Location = GhostLocation });
        }

        public virtual void OnRobotDestinationReached()
        {
            OnRobotDestinationReachedEvent?.Invoke(this, new LocationArgs { RobotId = robotId, Location = RobotLocation });
        }

        public virtual void OnDestinationSet(int robotId, Location destination)
        {
            OnDestinationSetEvent?.Invoke(this, new LocationArgs { RobotId = robotId, Location = destination });
        }

        
        #endregion
    }

}

