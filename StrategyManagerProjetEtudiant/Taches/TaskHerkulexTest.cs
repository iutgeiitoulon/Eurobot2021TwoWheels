using Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace StrategyManagerProjetEtudiantNS
{


    public class TaskHerkulexTest
    {


        StrategyEurobot parent;
        Thread TaskThread;
        public TaskDemoMoveState state = TaskDemoMoveState.Arret;


        public TaskHerkulexTest(StrategyEurobot parent)
        {
            this.parent = parent;
            TaskThread = new Thread(TaskThreadProcess);
            TaskThread.IsBackground = true;
            parent.OnAddServo(HerkulexManagerNS.ServoId.FrontArm, HerkulexManagerNS.HerkulexDescription.JOG_MODE.positionControlJOG);
            TaskThread.Start();

        }

     

        void TaskThreadProcess()
        {
            while(true)
            {
                parent.OnSetTorqueMode(HerkulexManagerNS.ServoId.FrontArm, HerkulexManagerNS.HerkulexDescription.TorqueControl.TorqueOn);
                Thread.Sleep(1000);
                parent.OnSetTorqueMode(HerkulexManagerNS.ServoId.FrontArm, HerkulexManagerNS.HerkulexDescription.TorqueControl.TorqueFree);
                Thread.Sleep(1000);

            }


        }
    }
}
