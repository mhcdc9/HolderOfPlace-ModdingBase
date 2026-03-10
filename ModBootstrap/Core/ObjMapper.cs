using ADV;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModdingCore
{
    public static class ObjMapper
    {
        public static void MapAllCards()
        {
            StreamWriter writer = new StreamWriter(BootstrapMain.corePath + "/CardMaps.txt");
            foreach(GameObject obj in Library.Main.CardPrefabs)
            {
                writer.WriteLine(MapCard(obj.GetComponent<Card>(), false));

            }
        }

        public static string MapCard(string key, bool hideAutoAttack)
        {
            Card card = Library.Main.GetCard(key)?.GetComponent<Card>();
            return MapCard(card, hideAutoAttack);
        }

        public static string MapCard(Card card, bool hideAutoAttack = true)
        {
            string output = "";
            if (card == null)
            {
                output = "[ModUtils] Could not find card";
                System.Console.WriteLine(output);
                return output;
            }
            output += "============" + "\n";
            output += card.GetID() + " (" + card.Info.RenderName + ")"+ "\n";
            output += card.GetDescription() + "\n\n";
            output += "Keys: " + MapKeys(card.KeyMark.GKB()) + "\n";

            Targeting t = card.IniTargeting?.GetComponent<Targeting>();
            if (t != null)
            {
                output += MapTargetting(t, "|");
            }
            card.AddConditions.Select(c => c.GetComponent<Condition>()).Where(c => c != null).Do(c => output += MapConditions(c, "|"));
            card.IniSkills.Select(g => g.GetComponent<Mark_Skill>()).Where(sk => sk != null && (!hideAutoAttack || sk.name != "AutoAttack")).Do(sk => output += MapSkill(sk, "-"));
            card.IniStatus.Select(g => g.GetComponent<Mark_Status>()).Where(st => st != null).Do(st => output += MapStatus(st, "-"));
            System.Console.WriteLine(output);
            return output;
        }

        public static string MapSkill(Mark_Skill skill, string prefix)
        {
            if (skill == null)
            {
                return "[NoSkill?]";
            }

            string output = prefix + "[SK]" + skill.GetType().Name + "(" + skill.name + "): ";
            output += MapKeys(skill.GKB()) + "\n";

            skill.Targetings.Select(t => t.GetComponent<Targeting>()).Where(t => t != null).Do(t => output += MapTargetting(t, prefix + "|"));
            skill.Conditions.Select(c => c.GetComponent<Condition>()).Where(c => c != null).Do(c => output += MapConditions(c, prefix + "|"));

            IEnumerable<Signal> signals = skill.MainSignals.Select(m => m.GetComponent<Signal>()).Where(s => s != null);
            foreach (Signal sig in signals)
            {
                output += MapSignal(sig.GetComponent<Signal>(), prefix + "-");
            }
            return output;
        }

        public static string MapTargetting(Targeting t, string prefix)
        {
            string output = prefix + "[T]" + t.GetType().Name + ": ";
            output += MapKeys(t.GKB()) + "\n";
            return output;
        }

        public static string MapConditions(Condition c, string prefix)
        {
            string output = prefix + "[C]" + c.GetType().Name + ": ";
            output += MapKeys(c.GKB()) + "\n";
            return output;
        }

        public static string MapStatus(Mark_Status status, string prefix)
        {
            if (status == null)
            {
                return "[NoStatus?]";
            }
            string output = prefix + "[ST]" + status.GetType().Name + "(" + status.name + "): ";
            output += MapKeys(status.GKB()) + "\n";
            if (status.HasKey("AddCondition"))
            {
                status.GetComponentsInChildren<Condition>().Select(c => c.GetComponent<Condition>()).Where(s => s != null).Do(c => output += MapConditions(c, prefix + "|"));
            }
            if (status is Mark_Trigger trigger)
            {
                IEnumerable<Signal> signals = trigger.MainSignals.Select(m => m.GetComponent<Signal>()).Where(s => s != null);
                foreach (Signal sig in signals)
                {
                    output += MapSignal(sig.GetComponent<Signal>(), prefix + "-");
             
                }
            }
            return output;
        }

        public static string MapSignal(Signal signal, string prefix)
        {
            string output = prefix + "[SG]";
            if (signal is Signal_AnimEffect animEffect)
            {
                output +="AnimEffect(" + (animEffect?.Prefab?.name ?? "Clear") + "):";
            }
            else if (signal is Signal_AnimTrigger animTrigger)
            {
                output +="AnimTrigger(" + (animTrigger?.AnimKey ?? "Null?") + "):";
            }
            else if (signal is Signal_SoundEvent sound)
            {
                output +="SoundFX(" + (sound?.Key ?? "Null?") + "):";
            }
            else
            {
                output +=signal.GetType().Name + ": ";
            }
            output += MapKeys(signal.GKB()) + "\n";
            List<Medium> mediums = new List<Medium>();
            if (signal is Signal_CreateMedium sigMedium)
            {
                mediums.Add(sigMedium.MediumPrefab.GetComponent<Medium>());
            }
            if (signal is Signal_CreateMediums sigMediums)
            {
                sigMediums.MediumPrefabs.Do(g => mediums.Add(g.GetComponent<Medium>()));
            }
            if (signal is Signal_CreateMedium_Inverse sigMediumInv)
            {
                mediums.Add(sigMediumInv.MediumPrefab.GetComponent<Medium>());
            }
            if (signal is Signal_CreateMediums_Count sigMedCount)
            {
                mediums.Add(sigMedCount.Medium.GetComponent<Medium>());
            }
            if (signal is Signal_CreateMedium_Explosion sigMedExp)
            {
                mediums.Add(sigMedExp.MediumPrefab.GetComponent<Medium>());
            }
            foreach (Medium medium in mediums)
            {
                output +=prefix + "|[M]" + medium.GetType().Name + ":";
                output += MapKeys(medium.GKB()) + "\n";
                IEnumerable<Signal> signals = medium.Signals.Select(m => m.GetComponent<Signal>()).Where(s => s != null);
                foreach (Signal sig in signals)
                {
                    output += MapSignal(sig.GetComponent<Signal>(), prefix + "-");
                }
            }
            if (signal is Signal_AddStatus sigStatus)
            {
                sigStatus.StatusPrefabs?.Do(st => output += MapStatus(st.GetComponent<Mark_Status>(), prefix + "-"));
            }
            if (signal is Signal_AddSkill sigSkill)
            {
                output += MapSkill(sigSkill.SkillPrefab?.GetComponent<Mark_Skill>(), prefix + "-");
            }
            return output;
        }

        public static string MapKeys(KeyBase kb)
        {
            string output = "";
            foreach(string s in kb.Keys)
            {
                output += s + ", ";
            }
            if (output.Length > 1)
            {
                output = output.Substring(0, output.Length - 1);
            }
            return output;
        }
    }
}
