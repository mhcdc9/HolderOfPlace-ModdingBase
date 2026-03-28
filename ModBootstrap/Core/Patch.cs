using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ADV;
using ModUtils;

namespace ModdingCore
{
    [HarmonyPatch]
    static class PatchInEvents
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TitleScreenControl), "Start")]
        static void AddModMenu()
        {
            System.Console.WriteLine("[Mod] Added Mod Menu");
            ModMenu.CreateModMenu();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ThreadControl), nameof(ThreadControl.LoadEventList), new Type[]
        {
            typeof(EventList)
        })]
        static void ThreadGenerated(ThreadControl __instance)
        {
            ModEvents.InvokeThreadGenerated(__instance.Thread);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Card), nameof(Card.Destroy), new Type[]
        {
            typeof(string)
        })]
        static void CardRemoved(Card __instance)
        {
            ModEvents.InvokeCardRemoved(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Event), nameof(Event.PreActivate))]
        static void EventPreActivated(Event __instance)
        {
            ModEvents.InvokeEventPreActivated(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Event), nameof(Event.Activate))]
        static void EventActivated(Event __instance)
        {
            ModEvents.InvokeEventActivated(__instance);
        }

        /*
        [HarmonyPostfix]
        [HarmonyPatch(typeof(RecruitPanel), nameof(RecruitPanel.DirectRecruit), new Type[]
        {
            typeof(string),
            typeof(bool)
        })]
        static void CardRecruited(RecruitPanel __instance, Card __result)
        {
            if (__result != null)
            {
                ModEvents.InvokeCardRecruited(__result);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RecruitPanel), nameof(RecruitPanel.DirectRecruit), new Type[]
        {
            typeof(string),
            typeof(Card),
            typeof(bool)
        })]
        static void CardRecruited2(RecruitPanel __instance, Card __result)
        {
            if (__result != null)
            {
                ModEvents.InvokeCardRecruited(__result);
            }
        }
        */

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Card), nameof(Card.InvokeSkill), new Type[]
        {
            typeof(float),
            typeof(bool),
            typeof(bool)
        })]
        static void CardRecruited(Card __instance, float Channel)
        {
            if (__instance == null)
            {
                return;
            }
            if (Channel == 0)
            {
                ModEvents.InvokeCardRecruited(__instance);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Card), nameof(Card.UseSkill), new Type[]
        {
            typeof(Mark_Skill),
            typeof(Card)
        })]
        static void SkillUsed(Card __instance, Mark_Skill Skill, Card Target)
        {
            if (Skill != null && Skill.GetKey("Spell") == 1)
            {
                ModEvents.InvokeSpellInvoked(__instance, Target, Skill);
            }
        }

    }

    [HarmonyPatch(typeof(Event_FR_RecruitII), nameof(Event_FR_RecruitII.IniRecruit), new Type[]
    {
        typeof(List<string>),
        typeof(List<string>)
    })]
    static class TestPatch2
    {
        static void Postfix(Event_FR_RecruitII __instance)
        {
            if (__instance.FirstRecruit)
            {
                foreach(string key in LibraryExt.virtPools["Origin"])
                {
                    RecruitPanel.Main.RecruitOverride.Add(key);
                }
            }
        }
    }

    [HarmonyPatch(typeof(UIButton_Settings), nameof(UIButton_Settings.Update))]
    static class IgnoreSettingErrors
    {
        static Exception Finalizer(Exception __exception, UIButton_Settings __instance)
        {
            if (__exception != null)
            {
                System.Console.WriteLine(__instance.name);
            }
            
            return null;
        }
    }
}
