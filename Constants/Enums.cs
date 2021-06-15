using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
    public enum TeamColor
    {
        Yellow,
        Blue,
    }

    public enum GameMode
    {
        RoboCup,
        Eurobot,
        Cachan,
        Demo
    }

    public enum PlayingSide
    {
        Left,
        Right
    }

    public enum GhostState
    {
        Angular,
        AngularEnd,
        Linear,
        LinearEnd,
        AngularTurn,
        AngularTurnEnd,
        Wait,
        Arret,
        ArretUrgence
    }

    public enum LidarObjectType
    { 
        Cup,
        Robot,
        Unknow,
        Null
    }

    public enum FieldType
    {
        DeadZone,
        StartZone,
        NorthField,
        SouthField,
        Harbor,
        RedHarbor,
        GreenHarbor,
        None
    }
}
