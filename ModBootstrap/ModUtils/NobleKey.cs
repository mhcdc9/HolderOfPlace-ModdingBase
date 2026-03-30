using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ADV;

namespace ModUtils
{
    public class NobleKey : MonoBehaviour
    {
        public KeyBase kb;
        public string key;
        public float add;
        public Vector3 values = Vector3.zero;

        public void Ennoble()
        {
            kb.ChangeKey(key, add);
            SignalInfo info = GetComponent<SignalInfo>();
            if (info != null)
            {
                info.Values += values;
            }
        }
    }
}
