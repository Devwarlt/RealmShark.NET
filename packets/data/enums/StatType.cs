namespace RotMGStats.RealmShark.NET.packets.data.enums
{
    /// <summary>
    /// Ordinal of stats.
    /// </summary>
    public struct StatType
    {
        private static readonly Dictionary<int, StatType> _statTypes = new Dictionary<int, StatType>();

        public static readonly StatType MAX_HP_STAT = new StatType(0);
        public static readonly StatType HP_STAT = new StatType(1);
        public static readonly StatType SIZE_STAT = new StatType(2);
        public static readonly StatType MAX_MP_STAT = new StatType(3);
        public static readonly StatType MP_STAT = new StatType(4);
        public static readonly StatType NEXT_LEVEL_EXP_STAT = new StatType(5);
        public static readonly StatType EXP_STAT = new StatType(6);
        public static readonly StatType LEVEL_STAT = new StatType(7);
        public static readonly StatType INVENTORY_0_STAT = new StatType(8);
        public static readonly StatType INVENTORY_1_STAT = new StatType(9);
        public static readonly StatType INVENTORY_2_STAT = new StatType(10);
        public static readonly StatType INVENTORY_3_STAT = new StatType(11);
        public static readonly StatType INVENTORY_4_STAT = new StatType(12);
        public static readonly StatType INVENTORY_5_STAT = new StatType(13);
        public static readonly StatType INVENTORY_6_STAT = new StatType(14);
        public static readonly StatType INVENTORY_7_STAT = new StatType(15);
        public static readonly StatType INVENTORY_8_STAT = new StatType(16);
        public static readonly StatType INVENTORY_9_STAT = new StatType(17);
        public static readonly StatType INVENTORY_10_STAT = new StatType(18);
        public static readonly StatType INVENTORY_11_STAT = new StatType(19);
        public static readonly StatType ATTACK_STAT = new StatType(20);
        public static readonly StatType DEFENSE_STAT = new StatType(21);
        public static readonly StatType SPEED_STAT = new StatType(22);
        public static readonly StatType DISCOVERABLE = new StatType(23);
        public static readonly StatType SEASONAL = new StatType(24);
        public static readonly StatType SKIN_ID = new StatType(25);
        public static readonly StatType VITALITY_STAT = new StatType(26);
        public static readonly StatType WISDOM_STAT = new StatType(27);
        public static readonly StatType DEXTERITY_STAT = new StatType(28);
        public static readonly StatType CONDITION_STAT = new StatType(29);
        public static readonly StatType NUM_STARS_STAT = new StatType(30);
        public static readonly StatType NAME_STAT = new StatType(31);
        public static readonly StatType TEX1_STAT = new StatType(32);
        public static readonly StatType TEX2_STAT = new StatType(33);
        public static readonly StatType MERCHANDISE_TYPE_STAT = new StatType(34);
        public static readonly StatType CREDITS_STAT = new StatType(35);
        public static readonly StatType MERCHANDISE_PRICE_STAT = new StatType(36);
        public static readonly StatType ACTIVE_STAT = new StatType(37);
        public static readonly StatType ACCOUNT_ID_STAT = new StatType(38);
        public static readonly StatType FAME_STAT = new StatType(39);
        public static readonly StatType MERCHANDISE_CURRENCY_STAT = new StatType(40);
        public static readonly StatType CONNECT_STAT = new StatType(41);
        public static readonly StatType MERCHANDISE_COUNT_STAT = new StatType(42);
        public static readonly StatType MERCHANDISE_MINS_LEFT_STAT = new StatType(43);
        public static readonly StatType MERCHANDISE_DISCOUNT_STAT = new StatType(44);
        public static readonly StatType MERCHANDISE_RANK_REQ_STAT = new StatType(45);
        public static readonly StatType MAX_HP_BOOST_STAT = new StatType(46);
        public static readonly StatType MAX_MP_BOOST_STAT = new StatType(47);
        public static readonly StatType ATTACK_BOOST_STAT = new StatType(48);
        public static readonly StatType DEFENSE_BOOST_STAT = new StatType(49);
        public static readonly StatType SPEED_BOOST_STAT = new StatType(50);
        public static readonly StatType VITALITY_BOOST_STAT = new StatType(51);
        public static readonly StatType WISDOM_BOOST_STAT = new StatType(52);
        public static readonly StatType DEXTERITY_BOOST_STAT = new StatType(53);
        public static readonly StatType OWNER_ACCOUNT_ID_STAT = new StatType(54);
        public static readonly StatType RANK_REQUIRED_STAT = new StatType(55);
        public static readonly StatType NAME_CHOSEN_STAT = new StatType(56);
        public static readonly StatType CURR_FAME_STAT = new StatType(57);
        public static readonly StatType NEXT_CLASS_QUEST_FAME_STAT = new StatType(58);
        public static readonly StatType LEGENDARY_RANK_STAT = new StatType(59);
        public static readonly StatType SINK_LEVEL_STAT = new StatType(60);
        public static readonly StatType BXP_STAT = new StatType(61);
        public static readonly StatType GUILD_NAME_STAT = new StatType(62);
        public static readonly StatType GUILD_RANK_STAT = new StatType(63);
        public static readonly StatType BREATH_STAT = new StatType(64);
        public static readonly StatType XP_BOOSTED_STAT = new StatType(65);
        public static readonly StatType XP_TIMER_STAT = new StatType(66);
        public static readonly StatType LD_TIMER_STAT = new StatType(67);
        public static readonly StatType LT_TIMER_STAT = new StatType(68);
        public static readonly StatType MATERIAL_STAT = new StatType(71);
        public static readonly StatType MATERIAL_CAP_STAT = new StatType(72);
        public static readonly StatType UNIQUE_DATA_STRING = new StatType(80);
        public static readonly StatType PET_INSTANCE_ID_STAT = new StatType(81);
        public static readonly StatType PET_NAME_STAT = new StatType(82);
        public static readonly StatType PET_TYPE_STAT = new StatType(83);
        public static readonly StatType PET_RARITY_STAT = new StatType(84);
        public static readonly StatType PET_MAX_ABILITY_POWER_STAT = new StatType(85);
        public static readonly StatType PET_FAMILY_STAT = new StatType(86);
        public static readonly StatType PET_FIRST_ABILITY_POINT_STAT = new StatType(87);
        public static readonly StatType PET_SECOND_ABILITY_POINT_STAT = new StatType(88);
        public static readonly StatType PET_THIRD_ABILITY_POINT_STAT = new StatType(89);
        public static readonly StatType PET_FIRST_ABILITY_POWER_STAT = new StatType(90);
        public static readonly StatType PET_SECOND_ABILITY_POWER_STAT = new StatType(91);
        public static readonly StatType PET_THIRD_ABILITY_POWER_STAT = new StatType(92);
        public static readonly StatType PET_FIRST_ABILITY_TYPE_STAT = new StatType(93);
        public static readonly StatType PET_SECOND_ABILITY_TYPE_STAT = new StatType(94);
        public static readonly StatType PET_THIRD_ABILITY_TYPE_STAT = new StatType(95);
        public static readonly StatType NEW_CON_STAT = new StatType(96);
        public static readonly StatType FORTUNE_TOKEN_STAT = new StatType(97);
        public static readonly StatType SUPPORTER_POINTS_STAT = new StatType(98);
        public static readonly StatType SUPPORTER_STAT = new StatType(99);
        public static readonly StatType TIME_JOINED_STAT = new StatType(101);
        public static readonly StatType PROJECTILE_SPEED_MULT = new StatType(102);
        public static readonly StatType PROJECTILE_LIFE_MULT = new StatType(103);
        public static readonly StatType OPENED_AT_TIMESTAMP = new StatType(104);
        public static readonly StatType EXALTED_ATK = new StatType(105);
        public static readonly StatType EXALTED_DEF = new StatType(106);
        public static readonly StatType EXALTED_SPD = new StatType(107);
        public static readonly StatType EXALTED_VIT = new StatType(108);
        public static readonly StatType EXALTED_WIS = new StatType(109);
        public static readonly StatType EXALTED_DEX = new StatType(110);
        public static readonly StatType EXALTED_HP = new StatType(111);
        public static readonly StatType EXALTED_MP = new StatType(112);
        public static readonly StatType EXALTATION_BONUS_DAMAGE = new StatType(113);
        public static readonly StatType EXALTATIONICREDUCTION = new StatType(114);
        public static readonly StatType GRAVE_ACCOUNT_ID = new StatType(115);
        public static readonly StatType POTION_ONE_TYPE = new StatType(116);
        public static readonly StatType POTION_TWO_TYPE = new StatType(117);
        public static readonly StatType POTION_THREE_TYPE = new StatType(118);
        public static readonly StatType POTION_BELT = new StatType(119);
        public static readonly StatType FORGE_FIRE = new StatType(120);
        public static readonly StatType MODIFIERS_STAT = new StatType(121);
        public static readonly StatType DIFFICULTY_STAT = new StatType(122);
        public static readonly StatType OBJECT_IS_ACTIVE_STAT = new StatType(123);
        public static readonly StatType POWER_LEVEL_STAT = new StatType(124);
        public static readonly StatType ANIMATION_STAT = new StatType(125);
        public static readonly StatType ANIMATION_TIMESTAMP_STAT = new StatType(126);
        public static readonly StatType DUST_STAT = new StatType(127);
        public static readonly StatType CRUCIBLE_STAT = new StatType(128);
        public static readonly StatType SHADER_STAT = new StatType(129);
        public static readonly StatType BACKPACK_SLOTS_STAT = new StatType(130);
        public static readonly StatType BACKPACK_0_STAT = new StatType(131);
        public static readonly StatType BACKPACK_1_STAT = new StatType(132);
        public static readonly StatType BACKPACK_2_STAT = new StatType(133);
        public static readonly StatType BACKPACK_3_STAT = new StatType(134);
        public static readonly StatType BACKPACK_4_STAT = new StatType(135);
        public static readonly StatType BACKPACK_5_STAT = new StatType(136);
        public static readonly StatType BACKPACK_6_STAT = new StatType(137);
        public static readonly StatType BACKPACK_7_STAT = new StatType(138);
        public static readonly StatType BACKPACK_8_STAT = new StatType(139);
        public static readonly StatType BACKPACK_9_STAT = new StatType(140);
        public static readonly StatType BACKPACK_10_STAT = new StatType(141);
        public static readonly StatType BACKPACK_11_STAT = new StatType(142);
        public static readonly StatType BACKPACK_12_STAT = new StatType(143);
        public static readonly StatType BACKPACK_13_STAT = new StatType(144);
        public static readonly StatType BACKPACK_14_STAT = new StatType(145);
        public static readonly StatType BACKPACK_15_STAT = new StatType(146);
        public static readonly StatType DUST_AMOUNT_STAT = new StatType(147);

        private readonly int _index;

        private StatType(int index)
        {
            _index = index;
            _statTypes[index] = this;
        }

        public int Get()
        {
            return _index;
        }

        public static StatType ByOrdinal(int ord)
        {
            return _statTypes.TryGetValue(ord, out var statType) ? statType : default;
        }

        public override bool Equals(object obj)
        {
            if (obj is StatType other)
            {
                return _index == other._index;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _index.GetHashCode();
        }

        public static bool operator ==(StatType left, StatType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StatType left, StatType right)
        {
            return !(left == right);
        }
    }
}