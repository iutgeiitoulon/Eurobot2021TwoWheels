using Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using HerkulexManagerNs;
using Constants;

namespace StrategyManagerProjetEtudiantNS
{


    public class TaskHerkulexTest
    {


        StrategyEurobot parent;
        Thread TaskThread;



        public TaskHerkulexTest(StrategyEurobot parent)
        {
            this.parent = parent;
            TaskThread = new Thread(TaskThreadProcess);
            TaskThread.IsBackground = true;
            parent.OnAddServo(ServoId.Rack1, HerkulexDescription.JOG_MODE.positionControlJOG);
            parent.OnAddServo(ServoId.Rack2, HerkulexDescription.JOG_MODE.positionControlJOG);
            parent.OnAddServo(ServoId.Drapeau, HerkulexDescription.JOG_MODE.positionControlJOG);

            parent.OnSetTorqueMode(ServoId.Rack1, HerkulexDescription.TorqueControl.TorqueOn);
            parent.OnSetTorqueMode(ServoId.Rack2, HerkulexDescription.TorqueControl.TorqueOn);
            parent.OnSetTorqueMode(ServoId.Drapeau, HerkulexDescription.TorqueControl.TorqueOn);
            TaskThread.Start();

        }

     

        void TaskThreadProcess()
        {
            while(true)
            {

                Thread.Sleep(2000);
                parent.OnSetPosition(ServoId.Rack1, (ushort)Positions.RackVertical, 50);
                parent.OnSetPosition(ServoId.Rack2, (ushort)Positions.RackVertical, 50);
                //parent.OnPololuSetUs(0, 1790);
                Thread.Sleep(1000);
                parent.OnSetPosition(ServoId.Rack1, (ushort)Positions.RackHorizontal, 50);
                parent.OnSetPosition(ServoId.Rack2, (ushort)Positions.RackHorizontal, 50);

                //parent.OnPololuSetUs(0, 2000);

                parent.OnSetWantedLocation(new Location
                {
                    X = 1,
                    Y = 1
                }, false);

            }


        }
    }
}
