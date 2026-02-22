using ModdingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModUtils
{
    public abstract class HopMod
    {
        public string modPath;
        public bool enabled;

        public abstract string Guid {get; }
        public abstract string Title { get; }
        public abstract string[] Depends { get; }

        public abstract string Description { get; }

        public Sprite icon;

        public HopMod(string path)
        {
            modPath = path;
            icon = BootstrapMain.GetSprite(modPath + "/icon.png");
            BootstrapMain.mainHarmony.PatchAll(this.GetType().Assembly);
        }

        protected internal virtual void CreateAssets()
        {

        }

        protected internal virtual void PostCreateAssets()
        {

        }

        protected internal virtual void SendData(string guid, List<object> data)
        {

        }

        protected internal virtual void ReceiveData(string guid, List<object> data)
        {

        }

        protected internal virtual void OnEnable()
        {
            enabled = true;
        }

        protected internal virtual void OnDisable()
        {
            enabled = false;
        }
    }
}
