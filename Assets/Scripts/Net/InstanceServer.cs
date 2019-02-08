using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Net;
using UnityEngine;
using UnityEngine.UI;

using UnityNetcodeIO;
using NetcodeIO.NET;
using ReliableNetcode;

namespace Server.Net
{    
    public class InstanceServer : NetcodeServerBehaviour
    {
        private readonly EventManager eventManager = EventManager.instance;
        
        public override void OnServerReceiveMessage(RemoteClient client, byte[] data, int size)
        {
            // Always read the first 2 bytes to find out what type of message was received.
            var request = new NetworkPacket(data);
            Debug.Log($"[{DateTime.Now}] [Server] Client Message: {request.type}");
            
            if (OP.ClientConnect == request.type)
            {               
                var response = new NetworkPacket(OP.ServerAddEntity);
                base.SendPacket(client, response, QosType.Unreliable);
            }
//            byte[] buffer = new byte[size - 2];
//            Buffer.BlockCopy(data, 2, buffer, 0, size - 2);
//            var packet = new NetworkPacket(client, buffer);
//            
//            eventManager.Publish(type, packet);
        }

        public override void OnClientConnected(RemoteClient client)
        {
            Debug.Log($"[{DateTime.Now}] [Server] Client Connected");
        }

        public override void OnClientDisconnected(RemoteClient client)
        {
            Debug.Log($"[{DateTime.Now}] [Server] Client Disconnected");
        }
        
    }
}
