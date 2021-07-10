using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{
    enum MissionDemoState
    {
        Waiting,
        Twinkle,
        RackUp,
        RackDown,
        RackVertical,
        RackHorizontal,
        TurbineWave
    }



    public class MissionDemo : TaskBase
    {

        MissionDemoState state = MissionDemoState.Waiting;

        private ushort turbine_level = 1100;

        DateTime timestamp;

        public MissionDemo(StrategyEurobot p) : base(p) { }

        public override void Init()
        {
            isFinished = false;
            state = MissionDemoState.Waiting;
        }

        public override void TaskStateMachine()
        {
            switch (state)
            {
                case MissionDemoState.Waiting:
                    break;
                case MissionDemoState.Twinkle:

                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetWantedLocation(parent.localWorldMap.RobotLocation.X, parent.localWorldMap.RobotLocation.Y, false, -Math.PI / 6);
                            Thread.Sleep(1000);
                            parent.OnSetWantedLocation(parent.localWorldMap.RobotLocation.X, parent.localWorldMap.RobotLocation.Y, false, Math.PI / 6);
                            Thread.Sleep(1000);
                            parent.OnSetWantedLocation(parent.localWorldMap.RobotLocation.X, parent.localWorldMap.RobotLocation.Y, false, -Math.PI / 6);
                            Thread.Sleep(1000);
                            parent.OnSetWantedLocation(parent.localWorldMap.RobotLocation.X, parent.localWorldMap.RobotLocation.Y, false, Math.PI / 6);
                            Thread.Sleep(1000);
                            parent.OnSetWantedLocation(parent.localWorldMap.RobotLocation.X, parent.localWorldMap.RobotLocation.Y, false, -Math.PI / 6);
                            Thread.Sleep(1000);
                            parent.OnSetWantedLocation(parent.localWorldMap.RobotLocation.X, parent.localWorldMap.RobotLocation.Y, false, Math.PI / 6);
                            Thread.Sleep(1000);
                            parent.OnSetWantedLocation(parent.localWorldMap.RobotLocation.X, parent.localWorldMap.RobotLocation.Y, false, -Math.PI / 6);
                            Thread.Sleep(1000);
                            parent.OnSetWantedLocation(parent.localWorldMap.RobotLocation.X, parent.localWorldMap.RobotLocation.Y, false, Math.PI / 6);
                            Thread.Sleep(1000);

                            break;
                        case SubTaskState.EnCours:
                            ExitState();

                            break;
                        case SubTaskState.Exit:
                            state = MissionDemoState.RackUp;
                            break;
                    }

                    break;
                case MissionDemoState.RackUp:

                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnPololuSetUs(PololuActuators.ServoAscenseur, (ushort)GruePositions.High);
                            timestamp = DateTime.Now;
                            break;
                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;
                        case SubTaskState.Exit:
                            state = MissionDemoState.RackVertical;
                            break;
                    }

                    break;
                case MissionDemoState.RackDown:

                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetHerkulexPosition(HerkulexManagerNs.ServoId.Drapeau, HerkulexManagerNs.Positions.DrapeauRange);
                            parent.OnPololuSetUs(PololuActuators.ServoAscenseur, (ushort)GruePositions.Low);
                            timestamp = DateTime.Now;
                            break;

                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionDemoState.RackHorizontal;
                            break;
                    }

                    break;
                case MissionDemoState.RackVertical:
                    
                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetHerkulexPosition(HerkulexManagerNs.ServoId.Rack1, HerkulexManagerNs.Positions.RackVertical, 40);
                            parent.OnSetHerkulexPosition(HerkulexManagerNs.ServoId.Rack2, HerkulexManagerNs.Positions.RackVertical, 40);
                            timestamp = DateTime.Now;
                            break;
                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;
                        case SubTaskState.Exit:
                            state = MissionDemoState.TurbineWave;
                            break;
                    }

                    break;
                case MissionDemoState.RackHorizontal:

                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            parent.OnSetHerkulexPosition(HerkulexManagerNs.ServoId.Rack1, HerkulexManagerNs.Positions.RackHorizontal, 40);
                            parent.OnSetHerkulexPosition(HerkulexManagerNs.ServoId.Rack2, HerkulexManagerNs.Positions.RackHorizontal, 40);
                            timestamp = DateTime.Now;
                            break;
                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;
                        case SubTaskState.Exit:
                            state = MissionDemoState.RackUp;
                            break;
                    }

                    break;
                case MissionDemoState.TurbineWave:

                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            int delay = 250;
                            parent.OnPololuSetUs(PololuActuators.Turbine1, (ushort)(turbine_level + 50));
                            Thread.Sleep(delay);
                            parent.OnPololuSetUs(PololuActuators.Turbine1, 1000);
                            Thread.Sleep(delay);
                            parent.OnPololuSetUs(PololuActuators.Turbine2, turbine_level);
                            Thread.Sleep(delay);
                            parent.OnPololuSetUs(PololuActuators.Turbine2, 1000);
                            Thread.Sleep(delay);
                            parent.OnPololuSetUs(PololuActuators.Turbine3, turbine_level);
                            Thread.Sleep(delay);
                            parent.OnPololuSetUs(PololuActuators.Turbine3, 1000);
                            Thread.Sleep(delay);
                            parent.OnPololuSetUs(PololuActuators.Turbine4, (ushort)(turbine_level + 50));
                            Thread.Sleep(delay);
                            parent.OnPololuSetUs(PololuActuators.Turbine4, 1000);
                            Thread.Sleep(delay);
                            parent.OnPololuSetUs(PololuActuators.Turbine5, turbine_level);
                            Thread.Sleep(delay);
                            parent.OnPololuSetUs(PololuActuators.Turbine5, 1000);
                            Thread.Sleep(delay);
                            parent.OnSetHerkulexPosition(HerkulexManagerNs.ServoId.Drapeau, HerkulexManagerNs.Positions.DrapeauLeve);
                            timestamp = DateTime.Now;
                            break;
                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 1000)
                                ExitState();
                            break;
                        case SubTaskState.Exit:
                            state = MissionDemoState.RackDown;
                            break;
                    }

                    break;

            }
        }

        internal void Start()
        {
            ResetSubState();
            state = MissionDemoState.RackUp;
        }
    }
}
