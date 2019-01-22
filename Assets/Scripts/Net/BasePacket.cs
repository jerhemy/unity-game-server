using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Net
{
    public class NetworkPacket : INetworkPacket
    {
        private readonly short _type;
        private byte[] buffer;
        
        public NetworkPacket(byte[] data)
        {
            _type = BitConverter.ToInt16(data, 0);
            Buffer.BlockCopy(data, 2, buffer, 0, data.Length);
        }
        
        public OP_CODE GetType()
        {
            return (OP_CODE) _type;
        }
    }
    
    public enum OP_CODE
    {
        ENITTY_UPDATE = 0x001
    }
    
    public interface INetworkPacket
    {
        OP_CODE GetType();   
    }    
}