using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
    public static class ConstVar
    {
        public const byte START_OF_FRAME = 0xFE;
        public const byte MAX_MSG_LENGHT = 255;

        public const int SICK_LIDAR_SERIAL_NUMBER = 19061439;

        public const string SCICHART_RUNTIME_KEY = "Wh4gbehyfiMgkJA2zpELG3Z1hR4/RsihR9mWQXQ4oz3z/12i6QyyTgoag5ThU5WmVg3FVgxOAS3stZHqZrMg0btp5DhpXrOEp8zPV/UBZcZO3W9yEzvIuu0odvX0tx8s+EG27UvGlc6qUQXylWIRiG/Pyz9BN4CYd7bHOMQKTEnx6vpexCxfZYK930TYzo2hCJJTaty93ld1feRDCYlGubQMDcV3+9Ogsc+P67ldqTghn5pUZfukASkp3Pw1wui/TPR3iLp1rDTN7RJWzfFkMhTPjDLw46hW18ixWCzWb1F2fI1OghfjcLQFCAVw+v90KsP3V71OaVtW9Ur2WcH0/p0Rlzi+HjmE2rvQkiJjiOr5AoP/tKTM6AmfsCUA3WtHQ/gDpUkkXcadoIZ9YPkt9mR0WdmzsPFMuQ56L38ZpIRVVytLFrPH9yfsDbffsfRMwTU99x9OOgozJorss2WXY4Da8EROiVvcyS1/2UP/KRoYvbTZGrEVe/a/ii4DIhZOvg==";
        public const double EUROBOT_ODOMETRY_POINT_TO_METER = 1.178449e-06;
        public const double EUROBOT_WHEELS_ANGLE = 1.570796e+00;
        public const double EUROBOT_MATRIX_X_COEFF = 5.000000e-01;
        public const double EUROBOT_MATRIX_THETA_COEFF = 4.166667e+00;

        public const double LIDAR_FREQ_IN_HZ = 15d;
        public const double ODOMETRY_FREQ_IN_HZ = 50.0d;

        public const string PROJECT_NAME = "Eurobot2021TwoWheels";
        public const string LOG_FOLDER_NAME = "LogFiles";
        public const long MAX_LOG_FILE_SIZE = 90000000;

        public const int WIDTH_BOXSIZE = 3;
        public const int HEIGHT_BOXSIZE = 2;

        public const double LIDAR_MIN_POINT_DISTANCE = 0.03;
        public const double LIDAR_MAX_POINT_DISTANCE = 3.60555;/// Math.Sqrt(Math.Pow(WIDTH_BOXSIZE, 2) + Math.Pow(HEIGHT_BOXSIZE, 2));

        public const double LIDAR_OBJECT_MAX_ASSOCIATION_DISTANCE = 0.4;
        public const int LIDAR_OBJECT_DEFAULT_LIFE = 1;
        public const int LIDAR_OBJECT_DEATH_LIFE = 0;
        public const int LIDAR_OBJECT_VALID_LIFE = 10;
        public const int LIDAR_OBJECT_MAX_LIFE = 30;
        public const int LIDAR_OBJECT_GAIN_LIFE = 1;
        public const int LIDAR_OBJECT_LOSE_LIFE = 3;

        public const double MINIMAL_WORLD_HISTORICAL_DIST = 0.010d;

        public const double PID_SPEED_MOTOR1_KP = 5.0d;
        public const double PID_SPEED_MOTOR1_KI = 50.0d;
        public const double PID_SPEED_MOTOR1_KD = 0.0d;

        public const double PID_SPEED_MOTOR2_KP = 5.0d;
        public const double PID_SPEED_MOTOR2_KI = 50.0d;
        public const double PID_SPEED_MOTOR2_KD = 0.0d;

        public const double PID_SPEED_MOTOR1_KP_MAX = 10.0d;
        public const double PID_SPEED_MOTOR1_KI_MAX = 15.0d;
        public const double PID_SPEED_MOTOR1_KD_MAX = 0.0d;

        public const double PID_SPEED_MOTOR2_KP_MAX = 10.0d;
        public const double PID_SPEED_MOTOR2_KI_MAX = 15.0d;
        public const double PID_SPEED_MOTOR2_KD_MAX = 0.0d;


        public const double PLANNER_MAX_LINEAR_ACCELERATION = 1.0d;
        public const double PLANNER_MAX_ANGULAR_ACCELERATION = Math.PI;
        public const double PLANNER_MAX_LINEAR_SPEED = 1.4d;
        public const double PLANNER_MAX_ANGULAR_SPEED = 2.8d * Math.PI;

        public const double PLANNER_MAJORATION_ANGULAR = 1.525d;

        public const double PLANNER_MAJORATION_LINEAR_MIN = 1.25d;
        public const double PLANNER_MAJORATION_LINEAR_MAX = 1.70d;
        public const double PLANNER_MAJORATION_LINEAR_COEFF = 0.45d;

        public const double PLANNER_MAX_SPACING = 0.4d;

        public const double PLANNER_LINEAR_ROBOT_DEAD_ZONE = 0.01d;
        public const double PLANNER_ANGULAR_ROBOT_DEAD_ZONE = 0.095d;

        public const double PLANNER_LINEAR_GHOST_DEAD_ZONE = 0.005d;
        public const double PLANNER_ANGULAR_GHOST_DEAD_ZONE = 0.075d;
        public const double PLANNER_LINEAR_SPEED_MIN = 0.02d;

        public const double PLANNER_PID_POSITION_LINEAR_KP = 15d;
        public const double PLANNER_PID_POSITION_LINEAR_KI = 0d;
        public const double PLANNER_PID_POSITION_LINEAR_KD = 1.5d;
        public const double PLANNER_PID_POSITION_LINEAR_MAX_KP = 5d;
        public const double PLANNER_PID_POSITION_LINEAR_MAX_KI = 0d;
        public const double PLANNER_PID_POSITION_LINEAR_MAX_KD = 5d;

        public const double PLANNER_PID_POSITION_ANGULAR_KP = 22.0d;
        public const double PLANNER_PID_POSITION_ANGULAR_KI = 0d;
        public const double PLANNER_PID_POSITION_ANGULAR_KD = 0.6d;  
        public const double PLANNER_PID_POSITION_ANGULAR_MAX_KP = 5d * Math.PI;
        public const double PLANNER_PID_POSITION_ANGULAR_MAX_KI = 0d * Math.PI;
        public const double PLANNER_PID_POSITION_ANGULAR_MAX_KD = 5d * Math.PI;

        public const double ROBOT_ENCODER_DIST_WHEELS = 0.297d; 
        public const double ROBOT_ENCODER_WHEELS_DIAMETER = 0.0426d;
        public const double ROBOT_ENCODER_POINT_TO_METER = ROBOT_ENCODER_WHEELS_DIAMETER * Math.PI / 8192.0d;


        public const byte HKLX_PLAYTIME = 50;

    }
}
