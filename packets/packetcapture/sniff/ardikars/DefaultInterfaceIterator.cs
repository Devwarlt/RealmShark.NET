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
    public class DefaultInterfaceIterator : IEnumerator<LivePacketDevice>
    {
        private LivePacketDevice next;

        public DefaultInterfaceIterator(LivePacketDevice next)
        {
            this.next = next;
        }

        public bool MoveNext()
        {
            return next != null;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public LivePacketDevice Current
        {
            get
            {
                if (next == null)
                {
                    throw new InvalidOperationException();
                }
                LivePacketDevice previous = next;
                next = GetNextDevice(next);
                return previous;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // No resources to dispose
        }

        private LivePacketDevice GetNextDevice(LivePacketDevice currentDevice)
        {
            // This method should return the next device in the list.
            // Since PcapDotNet does not provide a direct way to get the next device,
            // you need to implement this method based on your specific requirements.
            // For example, you might maintain a list of devices and return the next one in the list.
            return null;
        }
    }
}