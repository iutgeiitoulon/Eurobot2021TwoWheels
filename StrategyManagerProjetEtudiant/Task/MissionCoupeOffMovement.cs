using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionCoupeOffMovement : TaskBase
    {
        bool invert;
        DateTime timestamp;
        DateTime global_timestamp;
        public enum MissionCoupeOffMovementState
        {
            Waiting,
            Twist,

        }

        MissionCoupeOffMovementState state = MissionCoupeOffMovementState.Waiting;

        public MissionCoupeOffMovement(StrategyEurobot p) : base(p)
        {
            Init();
        }

        public override void Init()
        {
            isFinished = false;
            state = MissionCoupeOffMovementState.Waiting;
        }

        public void Start()
        {
            ResetSubState();
            state = MissionCoupeOffMovementState.Twist;
        }

        public void Abort()
        {
            isFinished = true;
            state = MissionCoupeOffMovementState.Waiting;
        }

        public override void TaskStateMachine()
        {
            switch(state)
            {
                case MissionCoupeOffMovementState.Waiting:
                    switch (subState)
                    {
                        case SubTaskState.Entry:

                            break;
                        case SubTaskState.EnCours:
                            parent.GenerateSpeedConsigne(0, 0);
                            break;

                        case SubTaskState.Exit:

                            break;
                    }
                    break;

                case MissionCoupeOffMovementState.Twist:

                    switch (subState)
                    {
                        case SubTaskState.Entry:
                            timestamp = DateTime.Now;
                            global_timestamp = DateTime.Now;
                            break;
                        case SubTaskState.EnCours:
                            if (DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 200)
                            {
                                timestamp = DateTime.Now;
                                invert = !invert;
                            }

                            parent.GenerateSpeedConsigne(0, invert ? 3: -3);
                            if (DateTime.Now.Subtract(global_timestamp).TotalMilliseconds >= 4000)

                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            state = MissionCoupeOffMovementState.Waiting;
                            break;
                    }
                    break;

            }
        }
    }
}

