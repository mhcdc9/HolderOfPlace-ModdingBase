using ADV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModUtils
{
    public static class CardExt
    {
        public static void AddTrait(this Card target, string cardKey, bool ignoreLimit = true)
        {
            Card source = Library.Main.GetCard(cardKey)?.GetComponent<Card>();
            if (source == null)
            {
                return;
            }

            if (!ignoreLimit)
            {
                for(int i=target.Skills.Count-1; i>=0; i--)
                {
                    if (target.Skills[i].GetKey("Trait") == 1f && target.Skills[i].GetKey("Mod") == 0f && target.Skills[i].GetKey("Implicit") == 0f)
                    {
                        target.RemoveSkill(target.Skills[i]);
                    }
                }
                
                for(int i=target.Status.Count-1; i>=0; i--)
                {
                    if (target.Status[i].GetKey("Trait") == 1f && target.Status[i].GetKey("Mod") == 0f && target.Status[i].GetKey("Implicit") == 0f)
                    {
                        target.RemoveStatus(target.Status[i]);
                    }
                }
            }

            foreach (Mark mark in source.GetComponentsInChildren<Mark>())
            {
                if (mark.GetKey("Trait") == 1f && mark.GetKey("Mod") == 0f)
                {
                    if (mark is Mark_Skill)
                    {
                        target.AddSkill(mark as Mark_Skill).SetKey("Mod",1);
                    }
                    else if (mark is Mark_Status)
                    {
                        target.AddStatus(mark as Mark_Status).SetKey("Mod", 1);
                    }
                }
            }

            string desc = source.GetInfo().Description;

            Signal_DescriptionChange descSignal = source.GetComponentInChildren<Signal_DescriptionChange>();
            if (descSignal != null)
            {
                desc = descSignal.Suffix ?? desc;
            }

            if (!ignoreLimit)
            {
                target.GetInfo().AddDescription = desc;
            }
            else
            {
                target.GetInfo().AddAspectDescription(desc);
            }
        }
    }
}
