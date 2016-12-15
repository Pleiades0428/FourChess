using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    [Flags]
    public enum AIStyle
    {
        PreferRandom,

        PreferShrink,

        PreferFar,

        ThinkImmediately,

        ThinkFast,

        ThinkSlow,

        Professional
    }
}
