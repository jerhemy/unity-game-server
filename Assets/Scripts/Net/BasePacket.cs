using System;
using System.Collections;
using System.Collections.Generic;
using NetcodeIO.NET;
using UnityEngine;
namespace Net
{
    public class NetworkPacket : INetworkPacket
    {
        private RemoteClient _client;
        private readonly short _type;
        private byte[] buffer;

        public NetworkPacket(RemoteClient client)
        {
            _client = client;
        }
        
        public NetworkPacket(RemoteClient client, byte[] data)
        {
            _client = client;
            _type = BitConverter.ToInt16(data, 0);
            buffer = new byte[data.Length];
            Buffer.BlockCopy(data, 2, buffer, 0, data.Length);
        }
        
        public OP_CODE GetPacketType()
        {
            return (OP_CODE) _type;
        }
    }
    
    public enum OP_CODE
    {
        CLIENT_CONNECT = 0x001,
        CLIENT_DISCONNECT = 0x002,
        ENITTY_UPDATE = 0x003
    }
    
    public interface INetworkPacket
    {
        OP_CODE GetPacketType();   
    }    
}