using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
    public enum TeamId
    {
        Team1 = 10,
        Team2 = 20,
        Opponents = 100,
    }

    public enum BallId
    {
        Ball = 1000,
    }
    public enum ObstacleId
    {
        Obstacle = 2000,
    }
    public enum RobotId
    {
        Robot1 = 0,
        Robot2 = 1,
        Robot3 = 2,
        Robot4 = 3,
        Robot5 = 4,
        Robot6 = 5,
    }
    public enum Caracteristique
    {
        Speed = 100,
        Destination = 200,
        WayPoint = 300,
        Ghost = 400,
        Ball = 500,
    }

    public enum Terrain
    {
        ZoneProtegee=1,
        DemiTerrainDroit=2,
        DemiTerrainGauche=3,
        SurfaceButGauche=4,
        SurfaceButDroit=5,
        SurfaceReparationDroit=6,
        SurfaceReparationGauche=7,
        ButGauche=8,
        ButDroit=9,
        ZoneTechniqueGauche=10,
        ZoneTechniqueDroite=11,
        RondCentral=12,
        CornerBasGauche=13,
        CornerBasDroite=14,
        CornerHautDroite=15,
        CornerHautGauche=16,
        PtAvantSurfaceGauche=17,
        PtAvantSurfaceDroit=18,


        TerrainComplet = 19,
        LigneTerrainGauche = 20,
        LigneTerrainDroite = 21,
        LigneCentraleEpaisse = 22,
        LigneCentraleFine = 23,
        BaliseGaucheHaut = 24,
        BaliseGaucheCentre = 25,
        BaliseGaucheBas = 26,
        BaliseDroiteHaut = 27,
        BaliseDroiteCentre = 28,
        BaliseDroiteBas = 29
    }
}
