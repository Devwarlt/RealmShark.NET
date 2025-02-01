using RotMGStats.RealmShark.NET.packets.data.enums;
using RotMGStats.RealmShark.NET.packets.reader;
using RotMGStats.RealmShark.NET.assets;

namespace RotMGStats.RealmShark.NET.packets.data
{
    /// <summary>
    /// Represents the data of a stat.
    /// </summary>
    [Serializable]
    public class StatData
    {
        /// <summary>
        /// The type of stat.
        /// </summary>
        public int StatTypeNum { get; set; }

        public StatType StatType { get; set; }

        /// <summary>
        /// The number value of this stat, if this is not a string stat.
        /// </summary>
        public int StatValue { get; set; }

        /// <summary>
        /// The string value of this stat, if this is a string stat.
        /// </summary>
        public string StringStatValue { get; set; }

        /// <summary>
        /// The secondary stat value.
        /// </summary>
        public int StatValueTwo { get; set; }

        /// <summary>
        /// Deserializer method to extract data from the buffer.
        /// </summary>
        /// <param name="buffer">Data that needs deserializing.</param>
        /// <returns>Returns this object after deserializing.</returns>
        public StatData Deserialize(BufferReader buffer)
        {
            StatTypeNum = buffer.ReadUnsignedByte();
            StatType = StatType.ByOrdinal(StatTypeNum);

            if (IsStringStat())
            {
                StringStatValue = buffer.ReadString();
            }
            else
            {
                StatValue = buffer.ReadCompressedInt();
            }
            StatValueTwo = buffer.ReadCompressedInt();

            return this;
        }

        private bool IsStringStat()
        {
            return StatType == StatType.EXP_STAT ||
                   StatType == StatType.NAME_STAT ||
                   StatType == StatType.ACCOUNT_ID_STAT ||
                   StatType == StatType.OWNER_ACCOUNT_ID_STAT ||
                   StatType == StatType.GUILD_NAME_STAT ||
                   StatType == StatType.MATERIAL_STAT ||
                   StatType == StatType.MATERIAL_CAP_STAT ||
                   StatType == StatType.UNIQUE_DATA_STRING ||
                   StatType == StatType.GRAVE_ACCOUNT_ID ||
                   StatType == StatType.MODIFIERS_STAT ||
                   StatType == StatType.DUST_STAT ||
                   StatType == StatType.CRUCIBLE_STAT ||
                   StatType == StatType.DUST_AMOUNT_STAT ||
                   StatType == StatType.PET_NAME_STAT;
        }

        public override string ToString()
        {
            string secondValue = StatValueTwo == -1 ? "" : $" [{StatValueTwo}]";
            string stringValue = StringStatValue == null ? "" : $" {StringStatValue}";
            string stringExtra = "";
            if (StatTypeNum == 29)
            {
                stringExtra += $" {ConditionBits.EffectsToString(StatValue)}";
            }
            else if (StatTypeNum == 96)
            {
                stringExtra += $" {ConditionNewBits.EffectsToString(StatValue)}";
            }
            else if (StatTypeNum >= 8 && StatTypeNum <= 19)
            {
                string name = IdToAsset.ObjectName(StatValue);
                stringExtra += $" {name}";
            }
            return $"      {StatType}({StatTypeNum}) = {StatValue}{secondValue}{stringValue}{stringExtra}";
        }
    }
}