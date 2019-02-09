using System;
using System.Collections;
using System.Collections.Generic;
using NetcodeIO.NET;
using UnityEngine;
namespace Server.Network
{            
    public struct NetworkPacket
    {
        private OP _type;
        private readonly byte[] _data;
        private readonly int _length;

        public NetworkPacket(OP type, byte[] data)
        {
            _type = type;
            _length = data.Length;
            byte[] bType = BitConverter.GetBytes((int) type);
            _data = new byte[_length + 2];
            
            Buffer.BlockCopy(bType, 0, _data, 0, 4 );     
            Buffer.BlockCopy(data, 0, _data, 4, _length );           
        }

        public NetworkPacket(byte[] data)
        {
            _type = (OP)BitConverter.ToInt32(data, 0);            
            _length = data.Length - 4;
            _data = new byte[_length];
            Buffer.BlockCopy(data, 4, _data, 0, _length );           
        }
        
        public NetworkPacket(OP type)
        {
            _type = type;
            _data = BitConverter.GetBytes((int) type);
            _length = _data.Length;
        }

        public OP type => _type;
        
        public int length => _length;

        public byte[] data => _data;
    }
}