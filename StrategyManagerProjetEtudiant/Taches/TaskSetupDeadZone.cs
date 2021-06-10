using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;
using Utilities;
using EventArgsLibrary;
using System.Threading;

namespace StrategyManagerProjetEtudiantNS
{
    class TaskSetupDeadZone
    {
        Thread TaskThread;
        StrategyEurobot parent;

        public TaskSetupDeadZone(StrategyEurobot parent)
        {
            this.parent = parent;
            TaskThread = new Thread(TaskThreadProcess);
            TaskThread.IsBackground = true;
            TaskThread.Start();
        }

        public void UpdateDeadZone()
        {
            if (parent.localWorldMap.Team == null)
                return;

            List<RectangleOriented> list_of_deadzones = new List<RectangleOriented>();

            list_of_deadzones.AddRange(CommonDeadZone());
            switch (parent.localWorldMap.Team)
            {
                case Equipe.Bleue:
                    list_of_deadzones.AddRange(BlueDeadZone());
                    break;
                case Equipe.Jaune:
                    list_of_deadzones.AddRange(YellowDeadZone());
                    break;
            }

            parent.OnNewDeadZones(list_of_deadzones);
        }

        public List<RectangleOriented> CommonDeadZone()
        {
            List<RectangleOriented> list_of_common_deadzones = new List<RectangleOriented>();

            list_of_common_deadzones.Add(new RectangleOriented(new PointD(0, -0.820), 0.1, 0.40, 0)); // Middle Stop
            list_of_common_deadzones.Add(new RectangleOriented(new PointD(- 0.600, -0.820), 0.1, 0.40, 0)); // Left Stop
            list_of_common_deadzones.Add(new RectangleOriented(new PointD(  0.600, -0.820), 0.1, 0.40, 0)); // Right Stop

            return list_of_common_deadzones;
        }
        public List<RectangleOriented> BlueDeadZone()
        {
            List<RectangleOriented> list_of_blue_deadzones = new List<RectangleOriented>();

            list_of_blue_deadzones.Add(new RectangleOriented(new PointD(1.275, 0.2), 0.45, 1.4, 0));    // Big Harbor
            list_of_blue_deadzones.Add(new RectangleOriented(new PointD(- 0.3, -0.820), 0.5, 0.4, 0));  // Small Harbor

            return list_of_blue_deadzones;
        }

        public List<RectangleOriented> YellowDeadZone()
        {
            List<RectangleOriented> list_of_yellow_deadzones = new List<RectangleOriented>();

            list_of_yellow_deadzones.Add(new RectangleOriented(new PointD(- 1.275, 0.2), 0.45, 1.4, 0)); // Big Harbor
            list_of_yellow_deadzones.Add(new RectangleOriented(new PointD(0.3, -0.820), 0.5, 0.4, 0));   // Small Harbor
            return list_of_yellow_deadzones;
        }

        void TaskThreadProcess()
        {
            while (true)
            {
                UpdateDeadZone();
                Thread.Sleep(100);
            }
        }
    }
}
