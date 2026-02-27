using ADV;
using ModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModdingCore
{
    public abstract class Command
    {
        public string id = "command";
        public List<string> defaults = new List<string>();
        public abstract void Run(List<string> messages);
        public virtual void OnValueChanged(string[] messages)
        {

        }

        public int SafeParseInt(string message, int defaultValue)
        {
            if (int.TryParse(message, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        public float SafeParseFloat(string message, float defaultValue)
        {
            if (float.TryParse(message, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        public class CommandRecruit : Command
        {
            public override void Run(List<string> messages)
            {
                if (Library.Main == null)
                {
                    return;
                }

                IEnumerable<string> keys = messages.Select(s => LibraryExt.FindBestKey(s)).Where(k => k != null);
                foreach (string key in keys)
                {
                    RecruitPanel.Main.DirectRecruit(key, true);
                }
            }
        }

        public class CommandMap : Command
        {
            public override void Run(List<string> messages)
            {
                if (Library.Main == null)
                {
                    return;
                }

                IEnumerable<string> keys = messages.Select(s => LibraryExt.FindBestKey(s)).Where(k => k != null);
                foreach (string key in keys)
                {
                    LibraryExt.MapCard(key, true);
                }
            }
        }

        public class CommandPray : Command
        {
            public override void Run(List<string> messages)
            {
                if (messages[0].ToLower() == "locus")
                {
                    CombatControl.Main.ChangeMana(SafeParseFloat(messages[1], 1));
                }
                else
                {
                    CombatControl.Main.ChangeMana(SafeParseFloat(messages[0], 1));
                }
            }
        }
    }
}
