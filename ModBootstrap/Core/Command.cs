using ADV;
using ModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ModdingCore
{
    public abstract class Command
    {
        public string id = "command";
        public string args = "";
        public static void LoadInitCommands()
        {
            AddCommand(new CommandUnits()
            {
                id = "recruit",
                args = "<card(s)>",
                action = CommandUnits.Recruit
            });
            AddCommand(new CommandUnits()
            {
                id = "map",
                args = "<card(s)>",
                action = CommandUnits.Map
            });
            AddCommand(new CommandUnits()
            {
                id = "reroll",
                args = "[card(s)]",
                action = CommandUnits.Reroll
            });
            AddCommand(new CommandUnits()
            {
                id = "trait",
                args = "<card(s)>",
                action = CommandUnits.AddTrait
            });
            AddCommand(new CommandNumber()
            {
                id = "faith",
                args = "<amount>",
                action = CommandNumber.Faith
            });
            AddCommand(new CommandNumber()
            {
                id = "locus",
                args = "<amount>",
                action = CommandNumber.Locus
            });
            AddCommand(new CommandNumber()
            {
                id = "core",
                args = "<amount>",
                action = CommandNumber.Core
            });
            AddCommand(new CommandCard()
            {
                id = "life",
                args = "<amount>",
                action = CommandCard.Life
            });
            AddCommand(new CommandCard()
            {
                id = "damage",
                args = "<amount>",
                action = CommandCard.Damage
            });
            AddCommand(new CommandCard()
            {
                id = "destroy",
                action = CommandCard.Destroy
            });
            AddCommand(new CommandGeneric() 
            {
                id = "skip",
                action = CommandGeneric.SkipEvent //BUGGY
            });
            AddCommand(new CommandEvent()
            {
                id = "forceevent",
                args = "<event> [options]",
                action = CommandEvent.ForceNext
            });
            AddCommand(new CommandPredict()
            {
                id = "predict",
            });
        }

        public static void AddCommand(Command c) => CommandLine.commands[c.id] = c;

        public abstract void Run(List<string> messages);
        public virtual List<string> OnValueChanged(string[] messages)
        {
            return null;
        }

        public static int SafeParseInt(string message, int defaultValue)
        {
            if (int.TryParse(message, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        public static float SafeParseFloat(string message, float defaultValue)
        {
            if (float.TryParse(message, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        public static void Fail(string message)
        {
            (CommandLine.inputField.placeholder as TextMeshPro).SetText("ERROR: " + message);
        }
        public static void Success()
        {
            (CommandLine.inputField.placeholder as TextMeshPro).SetText("Command successfully executed!");
        }

        public class CommandEvent : Command
        {
            public Action<string, List<string>> action;
            public override void Run(List<string> messages)
            {
                if (ThreadControl.Main == null || messages.Count == 0)
                {
                    Fail("Run has not started");
                    return;
                }
                string eventId = messages.Count > 0 ? messages[0] : "";
                string key = ThreadControl.Main.GetComponentsInChildren<ADV.Event>().Select(e => e.Key).FirstOrDefault(k => k.ToLower() == eventId.ToLower() || k.ToLower() == "@" + eventId.ToLower());
                if (key == null)
                {
                    Fail("Event not found");
                    return;
                }
                action(key, messages);
            }

            public override List<string> OnValueChanged(string[] messages)
            {
                if (messages.Length == 0 || ThreadControl.Main == null)
                {
                    return null;
                }
                if (messages.Length == 1)
                {
                    string keyFrag = messages[0];
                    IEnumerable<string> keys = ThreadControl.Main.GetComponentsInChildren<ADV.Event>().Select(e => e.Key).Where(k => k.ToLower().Contains(keyFrag.ToLower()));
                    return keys.ToList();
                }
                return null;
            }

            public static void ForceNext(string key, List<string> messages)
            {
                int index = (ThreadControl.Main.GetCurrentEvent() == ThreadControl.Main.Thread[0]) ? 1 : 0;
                if (messages.Count > 1 && messages[1].ToLower() == "add")
                {
                    ThreadControl.Main.Thread.Insert(index, ThreadControl.Main.FindEvent(key));
                }
                else
                {
                    ThreadControl.Main.Thread[index] = ThreadControl.Main.FindEvent(key);
                }
            }
        }

        public class CommandUnits : Command
        {
            public Action<IEnumerable<string>> action;
            public override void Run(List<string> messages)
            {
                if (Library.Main == null)
                {
                    Fail("Command can only be used in a run or in the codex");
                    return;
                }

                IEnumerable<string> keys = messages.Select(s => LibraryExt.FindBestKey(s)).Where(k => k != null);
                action(keys);
            }

            public override List<string> OnValueChanged(string[] messages)
            {
                if (messages.Length == 0 || messages[messages.Length - 1] == "" || Library.Main == null)
                {
                    return null;
                }

                List<string> keys = LibraryExt.FindPartialKey(messages[messages.Length - 1]);
                keys = keys.GetRange(0, Math.Min(5, keys.Count));
                return keys;
            }

            public static void Recruit(IEnumerable<string> keys)
            {
                foreach (string key in keys)
                {
                    RecruitPanel.Main.DirectRecruit(key, true);
                }
                Success();
            }

            public static void Map(IEnumerable<string> keys)
            {
                foreach (string key in keys)
                {
                    ObjMapper.MapCard(key, true);
                }
                Success();
            }

            public static void Reroll(IEnumerable<string> keys)
            {
                List<string> validKeys = keys.ToList();
                RecruitPanel.Main.ResetSlot(IgnoreLock: false);
                if (validKeys.Count > 0)
                {
                    RecruitPanel.Main.ResetSlot(IgnoreLock: false);
                    RecruitPanel.Main.RecruitOverride = validKeys;
                    RecruitPanel.Main.NewRecruitProcess(CanSkip: true);
                }
                else
                {
                    ThreadControl.Main.CurrentEvent.SpecialAction("ForceRefresh");
                }
                //RecruitPanel.Main.NewRecruitProcess(CanSkip: true);
                Success();
            }

            public static void AddTrait(IEnumerable<string> keys)
            {
                if (UIControl.Main?.SelectingCard == null)
                {
                    Fail("Must be hovering over a card to use command");
                    return;
                }
                foreach (string key in keys)
                {
                    UIControl.Main.SelectingCard.AddTrait(key);
                }
                Success();
            }
        }

        public class CommandGeneric : Command
        {
            public Action<List<string>> action;

            public override void Run(List<string> messages)
            {
                action(messages);
            }

            public static void SkipEvent(List<string> messages)
            {
                if (ThreadControl.Main == null)
                {
                    Fail("Run has not started");
                }

                ThreadControl.Main.NextEvent();
            }
        }

        public class CommandNumber : Command
        {
            public Action<float> action;
            public override void Run(List<string> messages)
            {
                if (messages.Count == 0)
                {
                    messages.Add("1");
                }

                action(SafeParseFloat(messages[0], 1));
            }

            public static void Faith(float amount)
            {
                CombatControl.Main.ChangeFate(amount);
                Success();
            }

            public static void Locus(float amount)
            {
                CombatControl.Main.ChangeMana(amount);
                Success();
            }

            public static void Core(float amount)
            {
                CombatControl.Main.SetCoreLife(amount);
                Success();
            }
        }

        public class CommandCard : Command
        {
            public Action<List<string>, Card> action;
            public override void Run(List<string> messages)
            {
                if (UIControl.Main?.SelectingCard == null)
                {
                    Fail("Must be hovering over a card to use command");
                    return;
                }
                action(messages, UIControl.Main.SelectingCard);
            }

            public static void Life(List<string> messages, Card card)
            {
                if (messages.Count == 0)
                {
                    messages.Add("1");
                }

                float amount = SafeParseFloat(messages[0], 1);
                card.SetMaxLife(amount);
                card.SetLife(amount);
                Success();
            }

            public static void Damage(List<string> messages, Card card)
            {
                if (messages.Count == 0)
                {
                    messages.Add("1");
                }

                float amount = SafeParseFloat(messages[0], 1);
                card.SetBaseDamage(amount);
                Success();
            }

            public static void Destroy(List<string> messages, Card card)
            {
                card.Destroy();
            }
        }

        public class CommandComposite : Command
        {
            public List<Command> subCommands;
            public override void Run(List<string> messages)
            {
                if (messages.Count == 0)
                {
                    Fail("No subcommand provided");
                    return;
                }
                string subCommandId = messages[0];
                Command subCommand = subCommands.FirstOrDefault(c => c.id.ToLower() == subCommandId.ToLower());
                if (subCommand == null)
                {
                    Fail("Subcommand not found");
                    return;
                }
                subCommand.Run(messages.Skip(1).ToList());
            }

            public override List<string> OnValueChanged(string[] messages)
            {
                if (messages.Length == 0)
                {
                    return null;
                }
                List<string> subCommandIds = subCommands.Select(c => c.id).ToList();
                Command currentCommand = subCommands.FirstOrDefault(c => c.id.ToLower() == messages[0].ToLower());
                if (currentCommand != null && messages.Length > 1)
                {
                    return currentCommand.OnValueChanged(messages.Skip(1).ToArray());
                }
                subCommandIds = subCommandIds.Where(id => id.ToLower().StartsWith(messages[0].ToLower())).ToList();
                return subCommandIds.Count > 0 ? subCommandIds : null;

            }
        }

        public class CommandPredict : Command
        {
            public Func<List<string>, List<string>> action;
            public override void Run(List<string> messages)
            {

            }

            public override List<string> OnValueChanged(string[] messages)
            {
                if (ThreadControl.Main == null)
                {
                    return null;
                }

                List<string> events = ThreadControl.Main.Thread.Select(e => (e.Key ?? "Null?")).ToList();
                if (events.Count > 0)
                {
                    events[0] = "Next: " + (events[0] ?? "Null?");
                }
                return events;
            }
        }
    }
}
