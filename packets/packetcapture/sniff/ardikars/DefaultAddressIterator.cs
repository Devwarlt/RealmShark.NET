using System;
using System.Collections;
using System.Collections.Generic;
using PcapDotNet.Core;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.ardikars
{
    /// <summary>
    /// Directly extracted out of ardikars library to make edits possible.
    /// https://github.com/ardikars/pcap
    /// </summary>
    public class DefaultAddressIterator : IEnumerator<DeviceAddress>
    {
        private readonly IList<DeviceAddress> addresses;
        private int currentIndex;

        public DefaultAddressIterator(IList<DeviceAddress> addresses)
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
            currentIndex = -1;
        }

        public DeviceAddress Current
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