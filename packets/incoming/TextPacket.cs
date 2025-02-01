using RotMGStats.RealmShark.NET.packets.reader;

namespace RotMGStats.RealmShark.NET.packets.incoming
{
    /// <summary>
    /// Received when a chat message is sent by another player or NPC
    /// </summary>
    public class TextPacket : Packet
    {
        /// <summary>
        /// The sender of the message
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The object id of the sender
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// The int of stars of the sender
        /// </summary>
        public short NumStars { get; set; }

        /// <summary>
        /// The length of time to display the chat bubble for
        /// </summary>
        public int BubbleTime { get; set; }

        /// <summary>
        /// The recipient of the message
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// The content of the message
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// > Unknown.
        /// </summary>
        public string CleanText { get; set; }

        /// <summary>
        /// Whether or not the sender of the message is a supporter
        /// </summary>
        public bool IsSupporter { get; set; }

        /// <summary>
        /// The star background of the player
        /// </summary>
        public int StarBackground { get; set; }

        public override void Deserialize(BufferReader buffer)
        {
            Name = buffer.ReadString();
            ObjectId = buffer.ReadInt();
            NumStars = buffer.ReadShort();
            BubbleTime = buffer.ReadUnsignedByte();
            Recipient = buffer.ReadString();
            Text = buffer.ReadString();
            CleanText = buffer.ReadString();
            IsSupporter = buffer.ReadBoolean();
            StarBackground = buffer.ReadInt();
        }

        public override string ToString()
        {
            return $"TextPacket{{\n   name={Name}\n   objectId={ObjectId}\n   numStars={NumStars}\n   bubbleTime={BubbleTime}\n   recipient={Recipient}\n   text={Text}\n   cleanText={CleanText}\n   isSupporter={IsSupporter}\n   starBackground={StarBackground}}}";
        }
    }
}