using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
    public enum Equipe
    {
        Jaune,
        Bleue,
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
        Theta,
        Lin,
        Wait,
        Arret,
        ArretUrgence
    }
}
