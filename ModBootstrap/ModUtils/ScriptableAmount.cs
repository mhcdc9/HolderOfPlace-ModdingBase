using ADV;
using ModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModUtils
{
    public abstract class ScriptableAmount : ScriptableObject
    {
        public enum Type
        {
            Source,
            Target,
            Both
        }

        public virtual string nameDefault => "Scriptable";
        public float Get(Signal signal, Type type, float defaultValue = 0)
        {
            switch (type)
            {
                case (ScriptableAmount.Type.Source):
                    if (signal.Source != null)
                        return Get(signal.Source);
                    break;
                case (ScriptableAmount.Type.Target):
                    if (signal.Target != null)
                        return Get(signal.Target);
                    break;
                case (ScriptableAmount.Type.Both):
                    if (signal.Source != null && signal.Target != null)
                        return Get(signal.Source, signal.Target);
                    break;
            }
            return defaultValue;
        }

        public abstract float Get(Card source, Card target);
        public abstract float Get(Card card);

        public override string ToString()
        {
            string s = name == "" ? nameDefault : name;
            return s;
        }
    }

    public class ScriptableAmount_Constant : ScriptableAmount
    {
        public override string nameDefault => value.ToString();
        public float value = 0;

        public override float Get(Card source, Card target)
        {
            return value;
        }

        public override float Get(Card card)
        {
            return value;
        }
    }

    public class ScriptableAmount_Generic : ScriptableAmount
    {
        public override string nameDefault => "Generic";
        public Func<Card,float> sourceFunc;
        public Func<Card, Card, float> sourceTargetFunc;


        public override float Get(Card source, Card target)
        {
            if (sourceTargetFunc == null)
            {
                return Get(source);
            }
            return sourceTargetFunc(source, target);
        }

        public override float Get(Card card)
        {
            if (sourceFunc == null)
            {
                return 0;
            }
            return sourceFunc(card);
        }
    }
}
