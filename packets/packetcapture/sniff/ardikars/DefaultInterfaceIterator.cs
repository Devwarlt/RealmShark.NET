using System;
using System.Collections;
using System.Collections.Generic;
using SharpPcap;

namespace RotMGStats.RealmShark.NET.packets.packetcapture.sniff.ardikars
{
    /// <summary>
    /// Directly extracted out of ardikars library to make edits possible.
    /// https://github.com/ardikars/pcap
    /// </summary>
    public class DefaultInterfaceIterator : IEnumerator<ICaptureDevice>
    {
        private readonly IList<ICaptureDevice> devices;
        private int currentIndex;

        public DefaultInterfaceIterator(IList<ICaptureDevice> devices)
        {
            this.devices = devices ?? throw new ArgumentNullException(nameof(devices));
            this.currentIndex = -1;
        }

        public bool MoveNext()
        {
            if (currentIndex + 1 < devices.Count)
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

        public ICaptureDevice Current
        {
            get
            {
                if (currentIndex < 0 || currentIndex >= devices.Count)
                {
                    throw new InvalidOperationException();
                }
                return devices[currentIndex];
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // No resources to dispose
        }
    }
}