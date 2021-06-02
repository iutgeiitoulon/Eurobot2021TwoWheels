using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace WorldMap
{
    public enum ZeroFormatterMsgType
    {
        LocalWM, GlobalWM, RefBoxMsg
    }

    // UnionAttribute abstract/interface type becomes Union, arguments is union subtypes.
    // It needs single UnionKey to discriminate
    [Union(typeof(LocalWorldMap), typeof(GlobalWorldMap), typeof(RefBoxMessage))]
    public abstract class ZeroFormatterMsg
    {
        [UnionKey]
        public abstract ZeroFormatterMsgType Type { get; }
    }
}
