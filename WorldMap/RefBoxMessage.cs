using Constants;
using System;
using ZeroFormatter;

namespace WorldMap
{
    [ZeroFormattable]
    public class RefBoxMessage: ZeroFormatterMsg
    {
        public override ZeroFormatterMsgType Type
        {
            get
            {
                return ZeroFormatterMsgType.RefBoxMsg;
            }
        }
        [Index(0)]
        public virtual RefBoxCommand command { get; set; }
        [Index(1)]
        public virtual string targetTeam { get; set; }
        [Index(2)]
        public virtual int robotID { get; set; }
        [Index(3)]
        public virtual double posX { get; set; }
        [Index(4)]
        public virtual double posY { get; set; }
        [Index(5)]
        public virtual double posTheta { get; set; }
    }

    public class RefBoxMessageArgs : EventArgs
    {
        public RefBoxMessage refBoxMsg { get; set; }
    }
}
