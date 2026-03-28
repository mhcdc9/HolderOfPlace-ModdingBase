using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModUtils
{
    public static class KeyLib
    {
        public static string TARGET(int selfOrTarget) => "Target[" + selfOrTarget;
        public static string TARGET(bool self) => TARGET(self ? 0 : 1);
        public static string TARGET_SELF => TARGET(0);
        public static string TARGET_OTHER => TARGET(1);
        public static string ICHANNEL(float f) => "InvokeChannel[" + f;
        public static string ON_RECRUIT => ICHANNEL(0);
        public static string ON_SUMMON => ICHANNEL(0.2f);
        public static string ON_ATTACK => ICHANNEL(1);
        public static string AFTER_ATTACK => ICHANNEL(1.5f);
        public static string PERMANENT => "Permanent[1";
        public static string PERM(bool b) => b ? "Permanent[1" : "Permanent[0";
        public static string TRAIT => "Trait[1";

        public static string DELAY_SCALE(float f) => "DelayScale[" + f;
        public static string DELAY(float f) => "Delay[" + f;
        public static string DELAY_II(float f) => "AddDelayII[" + f;
        public static string VALUE(float f) => "Value[" + f;

        public static string BUFF => "Buff[1";
        public static string ADD_LIFE(float f) => "AddLife[" + f;
        public static string AUTO_HEAL => "AutoHeal[1";
        public static string ADD_DAMAGE(float f) => "AddDamage[" + f;

        public static string SPELL => "Spell[1";

        public static string PRECOMBAT_EARLY => "PreCombat_Early[1";
        public static string PRECOMBAT => "PreCombat[1";
        public static string POSTCOMBAT => "PostCombat[1";
    }
}
