using HarmonyLib;
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
        private Harmony harmony;

        public string modPath;
        public bool enabled;

        public abstract string Guid {get; }
        public abstract string Title { get; }
        public abstract string[] Depends { get; }

        public abstract string Description { get; }

        public virtual bool AppearAsRelic => false;
        public virtual float CorruptionLevel => 1f;

        public Sprite icon;

        private static int channelID = 1000;

        public HopMod(string path)
        {
            modPath = path;
            icon = BootstrapMain.GetSprite(modPath + "/icon.png");
            ConfigAttribute.LoadConfigs(GetType(), this, modPath + "/config.cfg");
            
        }

        public Sprite GetImage(string path)
        {
            return BootstrapMain.GetSprite(modPath + "/Images/" + path);
        }

        public virtual void DebugLog(string prefix, string message)
        {
            ModdingCore.Console.Log("[" + prefix + "] " + message);
        }

        protected internal virtual void CreateAssets()
        {

        }

        protected internal virtual void PostCreateAssets()
        {

        }

        protected internal virtual void SendData(string guid, List<object> data)
        {
            if (BootstrapMain.modDictionary.ContainsKey(guid))
            {
                BootstrapMain.modDictionary[guid].ReceiveData(Guid, data);
            }
        }

        protected internal virtual void ReceiveData(string guid, List<object> data)
        {

        }

        protected internal virtual void OnEnable()
        {
            if (harmony == null)
            {
                harmony = Harmony.CreateAndPatchAll(this.GetType().Assembly, Guid);
            }
            else
            {
                harmony.PatchAll();
            }
            enabled = true;
        }

        protected internal virtual void OnDisable()
        {
            harmony.UnpatchSelf();
            enabled = false;
        }

        public static int NewChannelID()
        {
            return channelID++;
        }
    }
}
