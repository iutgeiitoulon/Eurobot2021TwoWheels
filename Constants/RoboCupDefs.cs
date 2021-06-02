using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
    public enum RefBoxCommand
    {
        START,
        STOP,
        DROP_BALL,
        HALF_TIME,
        END_GAME,
        GAME_OVER,
        PARK,
        FIRST_HALF,
        SECOND_HALF,
        FIRST_HALF_OVER_TIME,
        RESET,
        WELCOME,
        KICKOFF,
        FREEKICK,
        GOALKICK,
        THROWIN,
        CORNER,
        PENALTY,
        GOAL,
        SUBGOAL,
        REPAIR,
        YELLOW_CARD,
        DOUBLE_YELLOW,
        RED_CARD,
        SUBSTITUTION,
        IS_ALIVE,


        //Added commands for debug
        GOTO,
        PLAYLEFT,
        PLAYRIGHT,
    }

    public enum RoboCupRobotRole
    {
        Unassigned,
        Stopped,
        Positioning,
        Stone,
        Gardien,
        DefenseurContesteur,
        DefenseurMarquage,
        DefenseurIntercepteur,
        AttaquantAvecBalle,
        AttaquantIntercepteur,
        AttaquantDemarque,
        Eurobot_petit_robot,
        Eurobot_gros_robot,
        Adversaire
    }

    public enum BallHandlingState
    {
        NoBall,
        HasBall,
        PassInProgress,
        ShootInProgress,
    }
}
