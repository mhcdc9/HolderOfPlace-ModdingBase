using ADV;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static ModUtils.KeyLib;

namespace ModUtils
{
    public static class MarkFactory
    {
        public static T NewMark<T>(string name, params string[] keys) where T : Mark
        {
            GameObject obj = new GameObject(name);
            T mark = obj.AddComponent<T>();
            KeyBase keyBase = obj.AddComponent<KeyBase>();
            keyBase.Keys = keys.ToList();
            mark.KB = keyBase;
            if (mark is Mark_Skill skill)
            {
                skill.Targetings = new List<GameObject>();
                skill.Conditions = new List<GameObject>();
                skill.MainSignals = new List<GameObject>();
            }
            return mark;
        }
        public static T NewSignal<T>(params string[] keys) where T : Signal
        {
            GameObject obj = new GameObject("signal");
            T signal = obj.AddComponent<T>();
            obj.name = typeof(T).Name;
            KeyBase keyBase = obj.AddComponent<KeyBase>();
            keyBase.Keys = keys.ToList();
            signal.KB = keyBase;
            return signal;
        }

        public static Signal NewSignalInfo(this Signal s, string message, string copyFrom = "")
        {
            return NewSignalInfo(s, message, Vector3.zero, copyFrom);
        }

        public static Signal NewSignalInfo(this Signal s, string message, Vector3 values, string copyFrom = "")
        {
            SignalInfo info = s.gameObject.AddComponent<SignalInfo>();
            info.CopyFrom = copyFrom;
            info.Message = message;
            info.TargetText = "";
            info.SourceText = "";
            info.Values = values;
            return s;
        }
        public static T NewTargeting<T>(params string[] keys) where T : Targeting
        {
            GameObject obj = new GameObject("targeting");
            T targeting = obj.AddComponent<T>();
            obj.name = typeof(T).Name;
            KeyBase keyBase = obj.AddComponent<KeyBase>();
            keyBase.Keys = keys.ToList();
            targeting.KB = keyBase;
            return targeting;
        }
        public static Signal_CreateMedium NewMedium<T>(string name, out T medium, params string[] keys) where T : Medium
        {
            var signal = NewSignal<Signal_CreateMedium>(keys);
            signal.name = name;
            medium = signal.gameObject.AddComponent<T>();
            KeyBase keybase2 = signal.gameObject.AddComponent<KeyBase>();
            medium.KB = keybase2;
            keybase2.Keys = new List<string>();
            medium.Signals = new List<GameObject>();
            if (medium is Medium_Explosion explosion)
            {
                explosion.SubSignals = new List<GameObject>();
            }
            signal.MediumPrefab = medium.gameObject;
            return signal;
        }
        
        public static Signal_CreateMedium_Inverse NewMediumInverse<T>(string name, out T medium, params string[] keys) where T : Medium
        {
            var signal = NewSignal<Signal_CreateMedium_Inverse>(keys);
            signal.name = name;
            medium = signal.gameObject.AddComponent<T>();
            KeyBase keybase2 = signal.gameObject.AddComponent<KeyBase>();
            medium.KB = keybase2;
            keybase2.Keys = new List<string>();
            medium.Signals = new List<GameObject>();
            if (medium is Medium_Explosion explosion)
            {
                explosion.SubSignals = new List<GameObject>();
            }
            signal.MediumPrefab = medium.gameObject;
            return signal;
        }

        public static Signal_CreateMediums_Count NewMediumsCount<T>(string name, out T medium, params string[] keys) where T : Medium
        {
            var signal = NewSignal<Signal_CreateMediums_Count>(keys);
            signal.name = name;
            medium = signal.gameObject.AddComponent<T>();
            KeyBase keybase2 = signal.gameObject.AddComponent<KeyBase>();
            medium.KB = keybase2;
            keybase2.Keys = new List<string>();
            medium.Signals = new List<GameObject>();
            if (medium is Medium_Explosion explosion)
            {
                explosion.SubSignals = new List<GameObject>();
            }
            signal.Medium = medium.gameObject;
            return signal;
        }

        public static Signal_CreateMedium_Explosion NewMediumExplosion<T>(string name, out T medium, params string[] keys) where T : Medium
        {
            var signal = NewSignal<Signal_CreateMedium_Explosion>(keys);
            signal.name = name;
            medium = signal.gameObject.AddComponent<T>();
            KeyBase keybase2 = signal.gameObject.AddComponent<KeyBase>();
            medium.KB = keybase2;
            keybase2.Keys = new List<string>();
            medium.Signals = new List<GameObject>();
            if (medium is Medium_Explosion explosion)
            {
                explosion.SubSignals = new List<GameObject>();
            }
            signal.MediumPrefab = medium.gameObject;
            return signal;
        }

        public static T EditMark<T>(this T mark, Action<T> action) where T : Mark
        {
            action(mark);
            return mark;
        }

        public static T EditSignal<T>(this T signal, Action<T> action) where T : Signal
        {
            action(signal);
            return signal;
        }
        public static Signal_CreateMedium EditMedium<T>(this Signal_CreateMedium signal, Action<T> action) where T:Medium
        {
            T medium = signal.GetComponent<T>();
            if (medium != null)
            {
                action(medium);
            }
            return signal;
        }

        public static NobleKey AddNobleKey(this KeyBase kb, string key, float add)
        {
            return AddNobleKey(kb, key, add, Vector3.zero);
        }

        public static NobleKey AddNobleKey(this KeyBase kb, string key, float add, Vector3 v)
        {
            NobleKey nk = kb.AddComponent<NobleKey>();
            nk.kb = kb;
            nk.key = key;
            nk.add = add;
            nk.values = v;
            return nk;
        }

        public static void AddKeys(this Medium medium, params string[] keys)
        {
            KeyBase kb = medium.GKB();
            foreach(string key in keys)
            {
                kb.Keys.Add(key);
            }
        }

        public static void AddTo_Generic(Transform parent, List<GameObject> mainList, IEnumerable<GameObject> newObj)
        {
            foreach (GameObject g in newObj)
            {
                g.transform.SetParent(parent);
                mainList.Add(g.gameObject);
            }
        }
        public static Mark_Skill AddSignal(this Mark_Skill skill, params Signal[] signals)
        {
            AddTo_Generic(skill.transform, skill.MainSignals, signals.Select(s => s.gameObject));
            return skill;
        }
        public static Mark_Trigger AddSignal(this Mark_Trigger trigger, params Signal[] signals)
        {
            if (trigger.MainSignals == null)
            {
                trigger.MainSignals = new List<GameObject>();
            }
            AddTo_Generic(trigger.transform, trigger.MainSignals, signals.Select(s => s.gameObject));
            return trigger;
        }
        public static Medium AddSignal(this Medium medium, params Signal[] signals)
        {
            if (medium is Medium_Explosion explosion)
            {
                return AddSignal_Explosion(explosion, false, signals);
            }
            AddTo_Generic(medium.transform, medium.Signals, signals.Select(s => s.gameObject));
            return medium;
        }
        public static Medium_Explosion AddSignal_Explosion(this Medium_Explosion medium, bool addToSubSignalsOnly, params Signal[] signals)
        {
            AddTo_Generic(medium.transform, medium.SubSignals, signals.Select(s => s.gameObject));
            if (!addToSubSignalsOnly)
            {
                AddTo_Generic(medium.transform, medium.Signals, signals.Select(s => s.gameObject));
            }
            return medium;
        }
        public static Mark_Skill AddTargeting(this Mark_Skill skill, params Targeting[] targetings)
        {
            AddTo_Generic(skill.transform, skill.Targetings, targetings.Select(t => t.gameObject));
            return skill;
        }

        public static Signal_Invoke NewInvoke(string skillKey, params string[] keys)
        {
            var invoke = NewSignal<Signal_Invoke>(keys);
            invoke.SkillKey = "Auto";
            return invoke;
        }

        public static Signal_AddSkill NewAddSkill(Mark_Skill skill, params string[] keys)
        {
            var addSkill = NewSignal<Signal_AddSkill>(keys);
            addSkill.SkillPrefab = skill.gameObject;
            skill.transform.SetParent(addSkill.transform);
            return addSkill;
        }

        public static Signal_AddStatus NewAddStatus(Mark_Status status, params string[] keys)
        {
            var addStatus = NewSignal<Signal_AddStatus>(keys);
            addStatus.StatusPrefabs = new List<GameObject> { status.gameObject };
            status.transform.SetParent(addStatus.transform);
            return addStatus;
        }

        public static Signal_AnimEffect NewAnimEffect(string animKey, params string[] keys)
        {
            Signal_AnimEffect anim = NewSignal<Signal_AnimEffect>(keys);
            GameObject animPrefab = LibraryExt.animPrefabs.FirstOrDefault(a => a.name.ToLower() == animKey?.ToLower());
            anim.Prefab = animPrefab;
            return anim;
        }

        public static Signal_AnimTrigger NewAnimTrigger(string triggerKey, params string[] keys)
        {
            Signal_AnimTrigger anim = NewSignal<Signal_AnimTrigger>(keys);
            anim.AnimKey = triggerKey;
            return anim;
        }

        public static Signal_SoundEvent NewSound(string soundKey, params string[] keys)
        {
            Signal_SoundEvent sound = NewSignal<Signal_SoundEvent>(keys);
            sound.Key = soundKey;
            return sound;
        }

        public static T InstantiateEditName<T> (this T original, string name) where T:MonoBehaviour
        {
            T newObj = GameObject.Instantiate<T>(original);
            newObj.name = name;
            return newObj;
        }

        public static Mark_Trigger_Signal NewTriggerSignal(string triggerKey, params string[] keys)
        {
            var triggerSignal = NewMark<Mark_Trigger_Signal>(triggerKey,keys);
            triggerSignal.TriggerKey = triggerKey;
            triggerSignal.MainSignals = new List<GameObject>();
            return triggerSignal;
        }

        public static Signal_ColorEffect NewColorEffect(Color color, params string[] keys)
        {
            Signal_ColorEffect signal = NewSignal<Signal_ColorEffect>(keys);
            signal.color = color;
            return signal;
        }

        public static T AddMarkInfo<T>(this T mark, string name, string desc) where T:Mark
        {
            MarkInfo markInfo = mark.AddComponent<MarkInfo>();
            markInfo.Name = name;
            markInfo.Description = desc;
            return mark;
        }

        public static T AddAspectTrigger<T>(this T skill) where T : Mark_Skill
        {
            skill.AddSignal(NewMedium<Medium_Instant>("AspectTrigger", out var instant, TARGET_OTHER));
            var mediumExplosion = NewMediumInverse<Medium_Explosion>("SendToAllFriendly", out var explosion, TARGET_OTHER);
            explosion.AddKeys("TargetAllFriendly[1", "TargetDeath[1", "TargetUntargeted[1", "IgnoreSource[0");
            explosion.AddSignal_Explosion(false, NewSignal<Signal>(TARGET_OTHER, "OnAspect[1", "OnTraitChange[1"));
            instant.AddSignal(mediumExplosion,
                NewSignal<Signal>(TARGET_OTHER, "OnAdditionalTrait[1")
                );
            return skill;
        }

        public static T AddTrinketTrigger<T>(this T skill) where T : Mark_Skill
        {
            skill.AddSignal(NewMedium<Medium_Instant>("Trinket Trigger", out var medInstant, TARGET_OTHER));

            medInstant.AddSignal(NewMediumInverse<Medium_Explosion>("Trinket Trigger II", out var explosion, TARGET_OTHER),
                NewSignal<Signal>(TARGET_OTHER,"OnTrinket[1"));

            explosion.AddKeys("TargetAllFriendly[1", "TargetDeath[1", "TargetUntargeted[1", "IgnoreSource[0");
            explosion.AddSignal_Explosion(false, NewSignal<Signal>(TARGET_OTHER, "OnTrinket[1"));
            return skill;
        }

        public static Signal[] Effect_AddFaith(float value=1, float bonusDelay = 0)
        {
            Signal[] signals = new Signal[]
            {
                NewSignal<Signal_FateChange>(TARGET_OTHER, VALUE(value), "NumberEffect[1"),
                NewAnimEffect("AE_Faith", TARGET_SELF),
                NewSignal<Signal_AddActingDelay>(TARGET_SELF, DELAY_SCALE(0.5f)),
                NewSound("Faith",TARGET_SELF)
            };
            signals[0].GKB().AddNobleKey("Value", 1, Vector3.one);
            signals[0].NewSignalInfo("[Source] gains *Pink*+[Value1] Faith*CE*", new Vector3(value,0,0));
            if (bonusDelay != 0)
            {
                signals[0].AddKey(DELAY(bonusDelay));
                signals[2].AddKey(DELAY_II(bonusDelay));
            }
            return signals;
        }

        public static Signal[] Effect_BuffStats(float damage, float life, bool permanent, bool self = true, float bonusDelay = 0) //Need to test with familiar
        {
            string colorKey = "";
            string message = "";
            string target = self ? "[SOURCE]" : "[TARGET]";
            Mark_Status_StatMod statMod = NewMark<Mark_Status_StatMod>("Mod", PERM(permanent), BUFF);
            if (life > 0)
            {
                statMod.SetKey("AddLife",life);
                statMod.SetKey("AutoHeal",1);
                statMod.GKB().AddNobleKey("AddLife", 1, new Vector3(0,1,0));
                colorKey = "AE_GreenBuff";
                if (damage > 0)
                {
                    statMod.SetKey("AddDamage",damage);
                    statMod.GKB().AddNobleKey("AddDamage", 1, new Vector3(1,0,0));
                    colorKey = "AE_YellowBuff";
                    message = target + " gains *Red*[Value1]*CE* / *Green*+[Value2]*CE*";
                }
                else
                {
                    message = target + " gains *Green*+[Value2] health*CE*";
                }
            }
            else if (damage > 0)
            {
                statMod.SetKey("AddDamage", damage);
                statMod.GKB().AddNobleKey("AddDamage", 1, new Vector3(1, 0, 0));
                colorKey = "AE_RedBuff";
                message = target + " gains *Red*+[Value1] attack damage*CE*";
            }

            List<Signal> signals = new List<Signal>
            {
                NewAddStatus(statMod, TARGET(self)),
                NewAnimEffect(colorKey, TARGET_OTHER)
            };
            signals[0].NewSignalInfo(message, new Vector3(damage, life));
            if (!self)
            {
                signals.Add(NewAnimEffect(colorKey + "_Empty", TARGET_SELF, "Priority[-1"));
            }
            signals.Add(NewSignal<Signal_AddActingDelay>(TARGET_SELF, DELAY_SCALE(0.5f)));
            signals.Add(NewSound("Buff", TARGET_SELF));
            return signals.ToArray();
        }

        public static Signal[] Effect_RandomDamage(float damage, float count, params string[] keys) //Need to test with turret
        {
            var randomDamage = NewMediumsCount<Medium_Instant_SubTarget>("RandomDamage", out var mediumDamage, TARGET_SELF, "RandomDamage[1", COUNT(count));
            if (count > 0)
            {
                randomDamage.GKB().AddNobleKey("Count", 1, new Vector3(1,0,0));
            }
            randomDamage.NewSignalInfo("[Source] deals *Orange*[Value1] Random Damage*CE*", new Vector3(count,0,0));
            KeyBase kb = randomDamage.GKB();
            kb.Keys.AddRange(keys);
            mediumDamage.SetKey("StepDelay", 0.1f);
            mediumDamage.SetKey("Delay", 0);
            mediumDamage.Targeting = NewTargeting<Targeting_RandomEnemy>().gameObject;
            Signal_RandomDamageLine damageLine = GameObject.Instantiate(Library.Main.GetCard("Zealot").GetComponentInChildren<Signal_RandomDamageLine>());
            damageLine.name = "Damage Line";
            mediumDamage.AddSignal(
                NewSignal<Signal_Damage>(TARGET_OTHER, DAMAGE(damage), "SubHit[1", "RandomDamage[1", "NumberEffect[1", "FastEffect[1"),
                NewAnimEffect("", TARGET_OTHER),
                NewAnimTrigger("OnHitII", TARGET_OTHER),
                damageLine,
                NewSignal<Signal_AddActingDelay>(TARGET_SELF, DELAY_SCALE(0.5f)),
                NewSound("RandomDamage", TARGET_SELF)
                );
            var triggers = NewMediumExplosion<Medium_Explosion>("EventTrigger", out var mediumTrigger, TARGET_SELF);
            mediumTrigger.SetKey("TargetFriendly", 1);
            mediumTrigger.SetKey("TargetDeath", 1);
            mediumTrigger.SetKey("TargetUntargeted", 1);
            mediumTrigger.SetKey("IgnoreSource", 1);
            mediumTrigger.AddSignal_Explosion(false, NewSignal<Signal>(TARGET_OTHER, "OnFriendlyRandomDamage[1"));
            return new Signal[]
            {
                randomDamage,
                triggers,
                NewAnimEffect("AE_RandomDamage",TARGET_SELF, "Priority[-1"),
                NewSignal<Signal_AddActingDelay>(TARGET_SELF,DELAY_SCALE(0.5f))
            };
        }

        public static Signal[] Effect_Summon(string summonKey, int numberOfCopies, bool behind = true, float bonusDelay = 0)
        {
            return Effect_SummonWithStats(summonKey, -99, -99, numberOfCopies, behind, bonusDelay);
        }

        public static Signal[] Effect_SummonWithStats(string summonKey, float damage, float life, int numberOfCopies, bool behind = true, float bonusDelay = 0) //Need to test summon interaction
        {
            List<Signal> signals = new List<Signal>()
            {
                NewAnimEffect("AE_Summon",TARGET_SELF, DELAY(bonusDelay)),
                NewSignal<Signal_AddActingDelay>(DELAY_SCALE(0.5f), DELAY_II(bonusDelay)),
                NewSound("Summon", TARGET_SELF, DELAY(bonusDelay))
            };
            float increment = behind ? -0.01f : 0.01f;
            for (int i = 1; i <= numberOfCopies; i++)
            {
                Signal_Summon summon = NewSignal<Signal_Summon>(TARGET_SELF, "SummonAggro[" + increment * i);
                summon.SummonKey = summonKey;
                if (life >= 0)
                {
                    summon.SetKey("SummonLife", life);
                    summon.GKB().AddNobleKey("SummonLife", 1);
                }
                if (damage >= 0)
                {
                    summon.SetKey("SummonDamage", damage);
                    summon.GKB().AddNobleKey("SummonDamage", 1);
                }
                signals.Insert(i-1,summon);
                if (i ==1)
                {
                    summon.NewSignalInfo("An *BlueGreen*ally*CE* joins the fray");
                }
            }
            return signals.ToArray();
        }
    }
    
}
