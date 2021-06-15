using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Constants;
using EventArgsLibrary;
using WorldMap;

namespace FieldSetterNs
{
    public class FieldSetter
    {
        private int robotID;
        private TeamColor Team;


        public FieldSetter(int robotId)
        {
            robotID = robotId;
            Team = TeamColor.Blue;
        }

        #region Functions
        public void UpdateFields()
        {
            if (Team == null)
                return;

            List<Field> list_of_fields = new List<Field>();

            list_of_fields.AddRange(CommonFields());
            switch (Team)
            {
                case TeamColor.Blue:
                    list_of_fields.AddRange(BlueFields());
                    break;
                case TeamColor.Yellow:
                    list_of_fields.AddRange(YellowFields());
                    break;
            }

            OnSetupFields(list_of_fields);
        }

        public List<Field> CommonFields()
        {
            List<Field> list_of_common_field = new List<Field>();

            /// Dead Zones
            list_of_common_field.Add(new Field(new RectangleOriented(new PointD(0, -0.820), 0.1, 0.40, 0), FieldType.DeadZone)); /// Middle Stop
            list_of_common_field.Add(new Field(new RectangleOriented(new PointD(-0.600, -0.820), 0.1, 0.40, 0), FieldType.DeadZone)); /// Left Stop
            list_of_common_field.Add(new Field(new RectangleOriented(new PointD(0.600, -0.820), 0.1, 0.40, 0), FieldType.DeadZone)); /// Right Stop

            return list_of_common_field;
        }
        public List<Field> BlueFields()
        {
            List<Field> list_of_blue_field = new List<Field>();

            /// Dead Zones
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(1.275, 0.2), 0.45, 1.4, 0), FieldType.DeadZone));    /// Big Harbor
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(-0.3, -0.820), 0.5, 0.4, 0), FieldType.DeadZone));  /// Small Harbor

            /// Big Harbor
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(-1.3, 0.2), 0.4, 0.6, 0), FieldType.StartZone));
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(-1.3, 0.2), 0.4, 0.6, 0), FieldType.Harbor));
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(-1.3, 0.485), 0.4, 0.03, 0), FieldType.GreenHarbor));
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(-1.3, -0.085), 0.4, 0.03, 0), FieldType.RedHarbor));

            /// Little Harbor
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(0.3, -0.87), 0.32, 0.3, 0), FieldType.Harbor));
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(0.4, -0.87), 0.1, 0.29, 0), FieldType.RedHarbor));
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(0.2, -0.87), 0.1, 0.29, 0), FieldType.GreenHarbor));

            /// North South
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(-1.3, 0.7), 0.4, 0.4, 0), FieldType.NorthField));
            list_of_blue_field.Add(new Field(new RectangleOriented(new PointD(-1.3, -0.3), 0.4, 0.4, 0), FieldType.SouthField));

            return list_of_blue_field;
        }

        public List<Field> YellowFields()
        {
            List<Field> list_of_yellow_field = new List<Field>();

            /// Dead Zones
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(-1.275, 0.2), 0.45, 1.4, 0), FieldType.DeadZone)); /// Big Harbor
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(0.3, -0.820), 0.5, 0.4, 0), FieldType.DeadZone));   /// Small Harbor

            /// Big Harbor
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, 0.2), 0.4, 0.6, 0), FieldType.StartZone));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, 0.2), 0.4, 0.6, 0), FieldType.Harbor));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, 0.485), 0.4, 0.03, 0), FieldType.RedHarbor));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, -0.085), 0.4, 0.03, 0), FieldType.GreenHarbor));

            /// Little Harbor
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(-0.3, -0.87), 0.32, 0.3, 0), FieldType.Harbor));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(-0.4, -0.87), 0.1, 0.29, 0), FieldType.GreenHarbor));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(-0.2, -0.87), 0.1, 0.29, 0), FieldType.RedHarbor));

            /// North South
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, 0.7), 0.4, 0.4, 0), FieldType.NorthField));
            list_of_yellow_field.Add(new Field(new RectangleOriented(new PointD(1.3, -0.3), 0.4, 0.4, 0), FieldType.SouthField));

            return list_of_yellow_field;
        }

        public void UpdateTeam(TeamColor team)
        {
            Team = team;
            UpdateFields();
        }
        #endregion

        #region Inputs Callback
        public void OnTeamChangeReceived(object sender, TeamColor team)
        {
            UpdateTeam(team);
        }

        public void OnLocalWorldMapChangeReceived(object sender, LocalWorldMap localWorld)
        {
            UpdateTeam(localWorld.Team);
        }
        #endregion

        #region Events
        public event EventHandler<List<Field>> OnSetupFieldsEvent;

        public virtual void OnSetupFields(List<Field> list_of_fields)
        {
            OnSetupFieldsEvent?.Invoke(this, list_of_fields);
        }

        #endregion
    }
}
