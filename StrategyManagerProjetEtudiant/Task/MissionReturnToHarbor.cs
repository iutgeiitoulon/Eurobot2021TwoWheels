using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;
using Utilities;

namespace StrategyManagerProjetEtudiantNS
{
    public class MissionReturnToHarbor : TaskBase
    {
        DateTime timestamp;
        public enum MissionReturnToHarborState
        {
            Waiting,
            GotoHarbor,

        }

        MissionReturnToHarborState state = MissionReturnToHarborState.Waiting;

        public MissionReturnToHarbor(StrategyEurobot p) : base(p)
        {
            Init();
        
        }

        public void Start()
        {
            ResetSubState();
            state = MissionReturnToHarborState.GotoHarbor;
        }

        public override void Init()
        {
            isFinished = false;
            state = MissionReturnToHarborState.Waiting;
        }

        public override void TaskStateMachine()
        {
            switch (state)
            {

                case MissionReturnToHarborState.Waiting:
                    break;

                case MissionReturnToHarborState.GotoHarbor:
                    switch(subState)
                    {
                        case SubTaskState.Entry:
                            timestamp = DateTime.Now;
                            PointD harbor = parent.localWorldMap.Fields.Where(x => x.Type == FieldType.StartZone).FirstOrDefault().Shape.Center;
                            parent.OnSetWantedLocation(harbor.X, harbor.Y, false, Math.PI);
                            break;

                        case SubTaskState.EnCours:
                            if(parent.isDeplacementFinished || DateTime.Now.Subtract(timestamp).TotalMilliseconds >= 10000)
                                ExitState();
                            break;

                        case SubTaskState.Exit:
                            isFinished = true;
                            state = MissionReturnToHarborState.Waiting;
                            break;
                    }
                    break;
            }
        }
    }
}
