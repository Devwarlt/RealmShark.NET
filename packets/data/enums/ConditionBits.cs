using System;
using System.Collections.Generic;
using System.Linq;

namespace RotMGStats.RealmShark.NET.packets.data.enums
{
    /// <summary>
    /// Represents condition bits.
    /// </summary>
    public struct ConditionBits
    {
        private static readonly Dictionary<int, ConditionBits> _conditionBits = new Dictionary<int, ConditionBits>();

        public static readonly ConditionBits DEAD = new ConditionBits(0x1);
        public static readonly ConditionBits QUIET = new ConditionBits(0x2);
        public static readonly ConditionBits WEAK = new ConditionBits(0x4);
        public static readonly ConditionBits SLOWED = new ConditionBits(0x8);
        public static readonly ConditionBits SICK = new ConditionBits(0x10);
        public static readonly ConditionBits DAZED = new ConditionBits(0x20);
        public static readonly ConditionBits STUNNED = new ConditionBits(0x40);
        public static readonly ConditionBits BLIND = new ConditionBits(0x80);
        public static readonly ConditionBits HALLUCINATING = new ConditionBits(0x100);
        public static readonly ConditionBits DRUNK = new ConditionBits(0x200);
        public static readonly ConditionBits CONFUSED = new ConditionBits(0x400);
        public static readonly ConditionBits STUNIMMUME = new ConditionBits(0x800);
        public static readonly ConditionBits INVISIBLE = new ConditionBits(0x1000);
        public static readonly ConditionBits PARALYZED = new ConditionBits(0x2000);
        public static readonly ConditionBits SPEEDY = new ConditionBits(0x4000);
        public static readonly ConditionBits BLEEDING = new ConditionBits(0x8000);
        public static readonly ConditionBits ARMORBREAKIMMUNE = new ConditionBits(0x10000);
        public static readonly ConditionBits HEALING = new ConditionBits(0x20000);
        public static readonly ConditionBits DAMAGING = new ConditionBits(0x40000);
        public static readonly ConditionBits BERSERK = new ConditionBits(0x80000);
        public static readonly ConditionBits INCOMBAT = new ConditionBits(0x100000);
        public static readonly ConditionBits STASIS = new ConditionBits(0x200000);
        public static readonly ConditionBits STASISIMMUNE = new ConditionBits(0x400000);
        public static readonly ConditionBits INVINCIBLE = new ConditionBits(0x800000);
        public static readonly ConditionBits INVULNERABLE = new ConditionBits(0x1000000);
        public static readonly ConditionBits ARMORED = new ConditionBits(0x2000000);
        public static readonly ConditionBits ARMORBROKEN = new ConditionBits(0x4000000);
        public static readonly ConditionBits HEXED = new ConditionBits(0x8000000);
        public static readonly ConditionBits NINJASPEEDY = new ConditionBits(0x10000000);
        public static readonly ConditionBits UNSTABLE = new ConditionBits(0x20000000);

        private readonly int _bitMask;

        private ConditionBits(int bitMask)
        {
            _bitMask = bitMask;
            _conditionBits[bitMask] = this;
        }

        public int Value()
        {
            return _bitMask;
        }

        public static ConditionBits[] GetEffects(int effects)
        {
            return _conditionBits.Values.Where(e => (effects & e._bitMask) != 0).ToArray();
        }

        public static string EffectsToString(int effects)
        {
            return string.Join(", ", GetEffects(effects).Select(e => e.ToString()));
        }

        public bool Effect(int mask)
        {
            return (_bitMask & mask) != 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is ConditionBits other)
            {
                return _bitMask == other._bitMask;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _bitMask.GetHashCode();
        }

        public static bool operator ==(ConditionBits left, ConditionBits right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConditionBits left, ConditionBits right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{nameof(ConditionBits)}({_bitMask})";
        }
    }
}