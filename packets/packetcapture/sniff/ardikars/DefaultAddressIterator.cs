using System.Collections;
using SharpPcap.LibPcap;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.ardikars
{
    /// <summary>
    /// Directly extracted out of ardikars library to make edits possible.
    /// https://github.com/ardikars/pcap
    /// </summary>
    public class DefaultAddressIterator : IEnumerator<PcapAddress>
    {
        private readonly IList<PcapAddress> addresses;
        private int currentIndex;

        public DefaultAddressIterator(IList<PcapAddress> addresses)
        {
            this.addresses = addresses ?? throw new ArgumentNullException(nameof(addresses));
            this.currentIndex = -1;
        }

        public bool MoveNext()
        {
            if (currentIndex + 1 < addresses.Count)
            {
                currentIndex++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public PcapAddress Current
        {
            get
            {
                if (currentIndex < 0 || currentIndex >= addresses.Count)
                {
                    throw new InvalidOperationException();
                }
                return addresses[currentIndex];
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // No resources to dispose
        }
    }
}