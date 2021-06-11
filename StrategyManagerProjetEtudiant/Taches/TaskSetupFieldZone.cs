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
    class TaskSetupFieldZone
    {
        Thread TaskThread;
        StrategyEurobot parent;

        public TaskSetupFieldZone(StrategyEurobot parent)
        {
            this.parent = parent;
            TaskThread = new Thread(TaskThreadProcess);
            TaskThread.IsBackground = true;
            TaskThread.Start();
        }

        public void UpdateFields()
        {
            if (parent.localWorldMap.Team == null)
                return;

            List<Field> list_of_fields = new List<Field>();

            list_of_fields.AddRange(CommonFields());
            switch (parent.localWorldMap.Team)
            {
                case Equipe.Bleue:
                    list_of_fields.AddRange(BlueFields());
                    break;
                case Equipe.Jaune:
                    list_of_fields.AddRange(YellowFields());
                    break;
            }

            parent.OnNewFields(list_of_fields);
        }

        public List<Field> CommonFields()
        {
            List<Field> list_of_common_field = new List<Field>();

            /// Dead Zones
            list_of_common_field.Add(new Field (new RectangleOriented(new PointD(0, -0.820), 0.1, 0.40, 0), FieldType.DeadZone)); /// Middle Stop
            list_of_common_field.Add(new Field (new RectangleOriented(new PointD(- 0.600, -0.820), 0.1, 0.40, 0), FieldType.DeadZone)); /// Left Stop
            list_of_common_field.Add(new Field (new RectangleOriented(new PointD(  0.600, -0.820), 0.1, 0.40, 0), FieldType.DeadZone)); /// Right Stop

            return list_of_common_field;
        }
        public List<Field> BlueFields()
        {
            List<Field> list_of_blue_field = new List<Field>();

            /// Dead Zones
            list_of_blue_field.Add(new Field (new RectangleOriented(new PointD(1.275, 0.2), 0.45, 1.4, 0), FieldType.DeadZone));    /// Big Harbor
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(- 0.3, -0.820), 0.5, 0.4, 0), FieldType.DeadZone));  /// Small Harbor

            /// 

            return list_of_blue_field;
        }

        public List<Field> YellowFields()
        {
            List<Field> list_of_yellow_field = new List<Field>();

            /// Dead Zones
            list_of_yellow_field.Add(new Field (new RectangleOriented(new PointD(- 1.275, 0.2), 0.45, 1.4, 0), FieldType.DeadZone)); /// Big Harbor
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(0.3, -0.820), 0.5, 0.4, 0), FieldType.DeadZone));   /// Small Harbor

            /// Big Harbor
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, 0.2), 0.4, 0.6, 0), FieldType.StartZone));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, 0.2), 0.4, 0.6, 0), FieldType.Harbor));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, 0.485), 0.4, 0.03, 0), FieldType.RedHarbor));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, -0.085), 0.4, 0.03, 0), FieldType.GreenHarbor));

            /// Little Harbor
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(-0.3, -0.87), 0.32, 0.3, 0), FieldType.Harbor));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(-0.4, -0.87), 0.1, 0.29, 0), FieldType.RedHarbor));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(-0.2, -0.87), 0.1, 0.29, 0), FieldType.GreenHarbor));

            /// North South
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, 0.7), 0.4, 0.4, 0), FieldType.NorthField));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, -0.3), 0.4, 0.4, 0), FieldType.SouthField));

            return list_of_yellow_field;
        }

        void TaskThreadProcess()
        {
            while (true)
            {
                UpdateFields();
                Thread.Sleep(100);
            }
        }
    }
}
