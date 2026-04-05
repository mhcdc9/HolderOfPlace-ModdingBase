using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModUtils
{
    [Flags]
    public enum ApplyToFlags
    {
        None = 0,
        Source = 1,
        Target = 2,
        Friendly = 4,
        Enemies = 8,
        Recruit = 16
    }

    [Flags]
    public enum IgnoreFlags
    {
        None = 0,
        Source = 1,
        Target = 2,
        Dead = 4,
        Alive = 8,
        Untargeted = 16,
        Targeted = 32,
    }
}
