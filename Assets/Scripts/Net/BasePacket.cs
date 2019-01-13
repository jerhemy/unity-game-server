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
    
    public interface INetworkPacket
    {
        byte[] GetBytes();
        
    }

    public struct OP_EntityMove : INetworkPacket
    {
        float x;
        float y;
        float z;
        float direction;
        
        public byte[] GetBytes()
        {
            throw new System.NotImplementedException();
        }
    }
}