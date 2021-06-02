using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace LogRecorderNs
{
    public enum ZeroFormatterLoggingType
    {
        IMUDataEventArgs, PolarSpeedEventArgs, RawLidarArgs
    }

    // UnionAttribute abstract/interface type becomes Union, arguments is union subtypes.
    // It needs single UnionKey to discriminate
    [Union(typeof(RawLidarArgsLog), typeof(PolarSpeedEventArgsLog))]
    public abstract class ZeroFormatterLogging
    {
        [UnionKey]
        public abstract ZeroFormatterLoggingType Type { get; }
    }

}
