using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Net
{
    public struct BasePacket
    {
        public ulong clientId;
        public byte[] buffer;
        public int size;
    }
}