using System;
using System.Collections.Generic;
using System.Linq;

namespace RotMGStats.RealmShark.NET.packets.data.enums
{
    /// <summary>
    /// Represents new condition bits.
    /// </summary>
    public struct ConditionNewBits
    {
        private static readonly Dictionary<int, ConditionNewBits> _conditionNewBits = new Dictionary<int, ConditionNewBits>();

        public static readonly ConditionNewBits SLOWED_IMMUNE = new ConditionNewBits(0x1);
        public static readonly ConditionNewBits DAZED_IMMUNE = new ConditionNewBits(0x2);
        public static readonly ConditionNewBits PARALYZED_IMMUNE = new ConditionNewBits(0x4);
        public static readonly ConditionNewBits PETRIFIED = new ConditionNewBits(0x8);
        public static readonly ConditionNewBits PETRIFIED_IMMUNE = new ConditionNewBits(0x10);
        public static readonly ConditionNewBits PET_STASIS = new ConditionNewBits(0x20);
        public static readonly ConditionNewBits CURSE = new ConditionNewBits(0x40);
        public static readonly ConditionNewBits CURSE_IMMUNE = new ConditionNewBits(0x80);
        public static readonly ConditionNewBits HEALTH_BOOST = new ConditionNewBits(0x100);
        public static readonly ConditionNewBits MANA_BOOST = new ConditionNewBits(0x200);
        public static readonly ConditionNewBits ATTACK_BOOST = new ConditionNewBits(0x400);
        public static readonly ConditionNewBits DEFENSE_BOOST = new ConditionNewBits(0x800);
        public static readonly ConditionNewBits SPEED_BOOST = new ConditionNewBits(0x1000);
        public static readonly ConditionNewBits VITALITY_BOOST = new ConditionNewBits(0x2000);
        public static readonly ConditionNewBits WISDOM_BOOST = new ConditionNewBits(0x4000);
        public static readonly ConditionNewBits DEXTERITY_BOOST = new ConditionNewBits(0x8000);
        public static readonly ConditionNewBits SILENCED = new ConditionNewBits(0x10000);
        public static readonly ConditionNewBits EXPOSED = new ConditionNewBits(0x20000);
        public static readonly ConditionNewBits ENERGIZED = new ConditionNewBits(0x40000);
        public static readonly ConditionNewBits HEALTH_DEBUFF = new ConditionNewBits(0x80000);
        public static readonly ConditionNewBits MANA_DEBUFF = new ConditionNewBits(0x100000);
        public static readonly ConditionNewBits ATTACK_DEBUFF = new ConditionNewBits(0x200000);
        public static readonly ConditionNewBits DEFENSE_DEBUFF = new ConditionNewBits(0x400000);
        public static readonly ConditionNewBits SPEED_DEBUFF = new ConditionNewBits(0x800000);
        public static readonly ConditionNewBits VITALITY_DEBUFF = new ConditionNewBits(0x1000000);
        public static readonly ConditionNewBits WISDOM_DEBUFF = new ConditionNewBits(0x2000000);
        public static readonly ConditionNewBits DEXTERITY_DEBUFF = new ConditionNewBits(0x4000000);
        public static readonly ConditionNewBits INSPIRED = new ConditionNewBits(0x8000000);

        private readonly int _bitMask;

        private ConditionNewBits(int bitMask)
        {
            _bitMask = bitMask;
            _conditionNewBits[bitMask] = this;
        }

        public int Value()
        {
            return _bitMask;
        }

        public static ConditionNewBits[] GetEffects(int effects)
        {
            return _conditionNewBits.Values.Where(e => (effects & e._bitMask) != 0).ToArray();
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
            if (obj is ConditionNewBits other)
            {
                return _bitMask == other._bitMask;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _bitMask.GetHashCode();
        }

        public static bool operator ==(ConditionNewBits left, ConditionNewBits right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConditionNewBits left, ConditionNewBits right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{nameof(ConditionNewBits)}({_bitMask})";
        }
    }
}