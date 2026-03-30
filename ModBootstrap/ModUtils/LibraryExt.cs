using ADV;
using HarmonyLib;
using ModdingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Console = System.Console;

namespace ModUtils
{
    [HarmonyPatch()]
    public static class LibraryExt
    {
        internal static GameObject modAssets;
        //internal static List<string> leaderPool = new List<string>();
        //@Recruit_Sea_Real
        //internal static List<string> seaPool = new List<string>();
        //Sandbat's Signal_Recruit keys
        //internal static List<string> hollowPool = new List<string>();

        public static List<GameObject> animPrefabs = new List<GameObject>();

        internal static Dictionary<string, List<string>> virtPools = new Dictionary<string, List<string>>();

        public static void StartVirtualPools()
        {
            virtPools["Origin"] = new List<string>();
            virtPools["Hollow"] = new List<string>();
            virtPools["Sea"] = new List<string>();
            virtPools["Trinket_Early"] = new List<string>();
            virtPools["Trinket_Middle"] = new List<string>();
            virtPools["Trinket_Late"] = new List<string>();
        }

        public static void ClearAllPools()
        {
            foreach(string key in virtPools.Keys)
            {
                virtPools[key].Clear();
            }
        }

        public static void ScrapeCards()
        {
            string PrintCardData(string key, Card card)
            {
                string s = key + "::";
                s += card.GetName() + "::";
                s += card.GetRealName() + "::";
                s += card.Life + "::";
                s += card.BaseDamage + "::";
                s += card.Tier + "::";
                foreach (GameObject ini in card.IniSkills)
                {
                    Mark_Skill skill = ini.GetComponent<Mark_Skill>();
                    if (skill != null)
                        s += skill.name + ", ";
                }
                s += "::";
                foreach (GameObject ini in card.IniStatus)
                {
                    Mark_Status skill = ini.GetComponent<Mark_Status>();
                    if (skill != null)
                        s += skill.name + ", ";
                }
                s += "::";
                foreach (GameObject ini in card.IniDefaults)
                {
                    Mark_Skill defSkill = ini.GetComponent<Mark_Skill>();
                    if (defSkill != null)
                        s += defSkill.name + ", ";
                }
                s += "::";
                return s;
            }

            ScrapeCards(PrintCardData);
        }

        public static void ScrapeSignalInfo()
        {
            string PrintSignalData(string key, SignalInfo signal)
            {
                string s = key + "::";
                s += signal.Message;
                return s;
            }

            ScrapeSignalInfo(PrintSignalData);
        }

        public static void ScrapeEvents()
        {
            ADV.Event[] events = ThreadControl.Main.GetComponentsInChildren<ADV.Event>();
            Console.WriteLine("====Scraping Event Data====");
            foreach (ADV.Event e in events)
            {
                if (e == null)
                {
                    continue;
                }
                string s = (e?.Key ?? "Null?") + "::";
                s += e.GetType().FullName + "::";
                if (e is Event_Encounter enc)
                {
                    enc?.Keys.Do((k) => s += k + ", ");
                    s += "::";
                }
                if (e is Event_Random ran)
                {
                    ran?.RandomEvents.Do(e2 => s += (e2?.Key ?? "Null?" + ", "));
                    s += "::";
                }
                if (e is Event_FR_Recruit rec)
                {
                    rec?.Keys.Do((k) => s += k + ",");
                    s += ":";
                }
                if (e is Event_FR_RecruitII rec2)
                {
                        rec2?.Keys.Do((k) => s += k + ", ");
                    s += "::";
                        rec2?.KeysII.Do((k) => s += k + ", ");
                    s += "::";
                }
                Console.WriteLine(s);
            }
            Console.WriteLine("====End of Event Scrape====");

        }

        public static void ScrapeCards(Func<string, Card, string> func)
        {
            Console.WriteLine("====Scraping Card Data====");
            for(int i =0; i<Library.Main.Keys.Count; i++)
            {
                string key = Library.Main.Keys[i];
                Console.WriteLine(func(key, Library.Main.GetCard(key).GetComponent<Card>()));
            }
            Console.WriteLine("====End of Card Scrape====");
        }

        public static void ScrapeSignalInfo(Func<string, SignalInfo, string> func)
        {
            Console.WriteLine("====Scraping Signal Data====");
            for (int i = 0; i < Library.Main.SignalInfoKeys.Count; i++)
            {
                string key = Library.Main.SignalInfoKeys[i];
                Console.WriteLine(func(key, Library.Main.GetSignalInfo(key)));
            }
            Console.WriteLine("====End of Signal Scrape====");
        }

        public static void GatherAnimEffects()
        {
            List<string> animNames = new List<string>();

            foreach(string key in Library.Main.Keys)
            {
                Signal_AnimEffect[] effects = Library.Main.GetCard(key).GetComponentsInChildren<Signal_AnimEffect>();
                foreach(Signal_AnimEffect effect in effects)
                {
                    if (effect?.Prefab != null && !animNames.Contains(effect.Prefab.name))
                    {
                        //System.Console.WriteLine("[Anim] " + effect.Prefab.name + " (" + key + ")");
                        animPrefabs.Add(effect.Prefab);
                        animNames.Add(effect.Prefab.name);
                    }
                }
            }
        }

        public static void GatherAnimTriggers()
        {
            List<string> animNames = new List<string>();

            foreach (string key in Library.Main.Keys)
            {
                Signal_AnimTrigger[] effects = Library.Main.GetCard(key).GetComponentsInChildren<Signal_AnimTrigger>();
                foreach (Signal_AnimTrigger effect in effects)
                {
                    if (effect?.AnimKey != null && !animNames.Contains(effect.AnimKey))
                    {
                        System.Console.WriteLine("[Anim] " + effect.AnimKey + " (" + key + ")");
                        animNames.Add(effect.AnimKey);
                    }
                }
            }
        }

        public static void AddKey(this Library l, string key, GameObject obj)
        {
            l.Keys.Add(key);
            l.CardPrefabs.Add(obj);
        }

        public static string FindBestKey(string s)
        {
            string key = null;
            if (s.Length > 1 && s[0] == '@')
            {
                for(int i=0; i<Library.Main.CardPrefabs.Count; i++)
                {
                    Card card = Library.Main.CardPrefabs[i].GetComponent<Card>();

                    if (card.Info.RealName.ToLower().Replace(" ", "") == s.Substring(1).ToLower())
                    {
                        key = Library.Main.Keys[i];
                        break;
                    }
                }
            }
            else
            {
                key = Library.Main.Keys.FirstOrDefault(k => k.ToLower() == s.ToLower());
            }
            return key;
        }

        public static List<string> FindPartialKey(string s)
        {
            s = s.ToLower();
            IEnumerable<string> list = Library.Main.Keys;
            if (s.Length >= 2 && s[0] == '@')
            {
                list = Library.Main.CardPrefabs.Select(c => c.GetComponent<CardInfo>()?.RealName).Where(title => title!=null).Select(title => "@" + title.Replace(" ",""));
            }
            List<string> keys = new List<string>();
            List<string> subkeys = new List<string>();
            foreach(string key in list)
            {
                string k = key.ToLower();
                if (k.StartsWith(s))
                {
                    keys.Add(k);
                }
                else if (k.Contains(s))
                {
                    subkeys.Add(k);
                }
            }
            keys.AddRange(subkeys);
            return keys;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Library), nameof(Library.Awake))]
        static void LibraryInit()
        {
            if (modAssets == null)
            {
                modAssets = new GameObject("Mod Assets");
                GameObject.DontDestroyOnLoad(modAssets);
                modAssets.SetActive(false);
            }
                System.Console.WriteLine("Starting Timer...");
                GatherAnimEffects();
                var watch = System.Diagnostics.Stopwatch.StartNew();
                foreach (string guid in BootstrapMain.activeMods)
                {
                    BootstrapMain.modDictionary[guid].CreateAssets();
                }
                foreach (string guid in BootstrapMain.activeMods)
                {
                    BootstrapMain.modDictionary[guid].PostCreateAssets();
                }
                watch.Stop();
                System.Console.WriteLine("Time Elapsed: " + watch.ElapsedMilliseconds + "ms");

            ModEvents.InvokeLibraryIni();

            
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Card), nameof(Card.Awake))]
        static bool Prefix()
        {
            return !ModdedCard.freezeAwake;
        }
    }
}
