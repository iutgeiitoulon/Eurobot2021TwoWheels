using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
    public enum GruePositions
    {
        Low = 2000,
        High = 1720,
        PreshensionRack = 1910

    }

    public enum TurbineState
    {
        Off = 1000,
        PrehensionHigh = 1300,
        PreshensionLow = 1200
    }

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

    public enum ReefType
    { 
        Private,
        TeamSide,
        OpponentSide
    }
}
