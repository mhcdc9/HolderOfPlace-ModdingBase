using ADV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModUtils
{
    public abstract class SScript : MonoBehaviour
    {
        public int priority = 0;
        public abstract void Run(Signal signal);

        public override string ToString()
        {
            return "Unknown " + this.GetType().Name;
        }
    }

    public class SScript_KeyChange : SScript
    {
        public KeyBase kb;
        public string key;
        public ScriptableAmount amount;
        public ScriptableAmount.Type type;

        public bool set = true;

        public override void Run(Signal signal)
        {
            if (kb == null)
            {
                Debug.Log("[SScript_KeyChange] Warning: No KeyBase!!");
                return;
            }

            float f = amount.Get(signal, type);

            if (set)
            {
                kb.SetKey(key, f);
            }
            else
            {
                kb.ChangeKey(key, f);
            }
        }

        public override string ToString()
        {
            return "Change " + key + " based on [" + amount.ToString() + "]";
        }
    }

    public class SScript_Repeat : SScript
    {
        public int offset = 0;
        public float delay = 0.1f;
        public ScriptableAmount amount;
        public ScriptableAmount.Type type;

        public override void Run(Signal signal)
        {
            float f = amount != null ? amount.Get(signal, type) + offset : offset;
            enabled = false;

            if (signal.Source == null)
            {
                return;
            }

            for(int i=0; i<f; i++)
            {
                signal.Source.SendSignal(signal.gameObject, new List<string>() { "Delay[" + ((i+1)*delay).ToString() }, signal.Target);
            }
        }

        public override string ToString()
        {
            return (amount != null) ? "Repeat this effect based on [" + amount.ToString() + " + " + offset + "]" : "Repeat this effect [" + offset + "] times";
        }
    }
}
