using ADV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModUtils
{
    [Obsolete("Signal_ColorEffect from the base game does the exact same thing, use that instead")]
    public class Signal_AnimColor : Signal
    {
        public Color color;

        /*
        public static Signal_AnimColor NewAnimColor(Color color, params string[] keys)
        {
            Signal_AnimColor signal = MarkFactory.NewSignal<Signal_AnimColor>(keys);
            signal.color = color;
            return signal;
        }
        */

        public override void EndEffect()
        {
            if (Target != null && Target.GetAnim() != null)
            {
                Target.GetAnim().ColorEffect(color);
            }
        }
    }
}
