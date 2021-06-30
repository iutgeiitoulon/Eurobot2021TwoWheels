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
            parent.OnAddServo(HerkulexManagerNS.ServoId.Rack1, HerkulexManagerNS.HerkulexDescription.JOG_MODE.positionControlJOG);
            parent.OnAddServo(HerkulexManagerNS.ServoId.Rack2, HerkulexManagerNS.HerkulexDescription.JOG_MODE.positionControlJOG);
            parent.OnAddServo(HerkulexManagerNS.ServoId.Drapeau, HerkulexManagerNS.HerkulexDescription.JOG_MODE.positionControlJOG);

            parent.OnSetTorqueMode(HerkulexManagerNS.ServoId.Rack1, HerkulexManagerNS.HerkulexDescription.TorqueControl.TorqueOn);
            parent.OnSetTorqueMode(HerkulexManagerNS.ServoId.Rack2, HerkulexManagerNS.HerkulexDescription.TorqueControl.TorqueOn);
            parent.OnSetTorqueMode(HerkulexManagerNS.ServoId.Drapeau, HerkulexManagerNS.HerkulexDescription.TorqueControl.TorqueOn);
            TaskThread.Start();

        }

     

        void TaskThreadProcess()
        {
            while(true)
            {
                //parent.OnSetPosition(HerkulexManagerNS.ServoId.Rack1, (ushort)HerkulexManagerNS.Positions.RackHorizontal, 50);
                //parent.OnSetPosition(HerkulexManagerNS.ServoId.Rack2, (ushort)HerkulexManagerNS.Positions.RackHorizontal, 50);
                //Thread.Sleep(1000);
                //parent.OnSetPosition(HerkulexManagerNS.ServoId.Rack1, (ushort)HerkulexManagerNS.Positions.RackVertical, 50);
                //parent.OnSetPosition(HerkulexManagerNS.ServoId.Rack2, (ushort)HerkulexManagerNS.Positions.RackVertical, 50);
                //Thread.Sleep(500);
                //parent.OnSetPosition(HerkulexManagerNS.ServoId.Drapeau, (ushort)HerkulexManagerNS.Positions.DapeauLeve, 5);
                //Thread.Sleep(300);
                //parent.OnSetPosition(HerkulexManagerNS.ServoId.Drapeau, 200, 5);
                //Thread.Sleep(300);
                //parent.OnSetPosition(HerkulexManagerNS.ServoId.Drapeau, (ushort)HerkulexManagerNS.Positions.DapeauLeve, 5);
                //Thread.Sleep(300);
                //parent.OnSetPosition(HerkulexManagerNS.ServoId.Drapeau, 200, 5);
                //Thread.Sleep(300);
                //parent.OnSetPosition(HerkulexManagerNS.ServoId.Drapeau, (ushort)HerkulexManagerNS.Positions.DapeauLeve, 5);
                //Thread.Sleep(500);
                //parent.OnSetPosition(HerkulexManagerNS.ServoId.Drapeau, 1, 5);
                // 2000 to 1000
                parent.OnPololuSetUs(0, 2000);
                Thread.Sleep(1000);


            }


        }
    }
}
