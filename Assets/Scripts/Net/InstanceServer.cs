﻿using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

using UnityNetcodeIO;
using NetcodeIO.NET;
using ReliableNetcode;

namespace Server.Network
{    
    public class InstanceServer : NetcodeServerBehaviour
    {   
        public override void OnServerReceiveMessage(RemoteClient client, byte[] data, int size)
        {
            // Always read the first 2 bytes to find out what type of message was received.
            var request = new NetworkPacket(data);
            Debug.Log($"[{DateTime.Now}] [Server] Client Message: {request.type}");
            
            if (OP.ClientConnect == request.type)
            {                       
                var response = new NetworkPacket(OP.ClientConnect);
                base.SendPacket(client, response, QosType.Reliable);
            }
//            byte[] buffer = new byte[size - 2];
//            Buffer.BlockCopy(data, 2, buffer, 0, size - 2);
//            var packet = new NetworkPacket(client, buffer);
//            
//            eventManager.Publish(type, packet);
        }

        public override void OnServerReceiveMessageByID(long clientID, byte[] data, int size)
        {
            // Always read the first 2 bytes to find out what type of message was received.
            var request = new NetworkPacket(data);
            Debug.Log($"[{DateTime.Now}] [Server] Client Message: {request.type}");
            
            if (OP.ClientConnect == request.type)
            {                       
                var response = new NetworkPacket(OP.ClientConnect);
                base.SendPacketByID(clientID, response, QosType.Reliable);
            }
//            byte[] buffer = new byte[size - 2];
//            Buffer.BlockCopy(data, 2, buffer, 0, size - 2);
//            var packet = new NetworkPacket(client, buffer);
//            
//            eventManager.Publish(type, packet);
        }

        public override void OnClientConnected(RemoteClient client)
        {
            
            #if !UNITY_EDITOR
            Debug.Log($"[{DateTime.Now}] [Server] Client Connected");
            #endif
        }

        public override void OnClientConnectedByID(long clientID)
        {
            throw new NotImplementedException();
        }

        public override void OnClientDisconnected(RemoteClient client)
        {
            #if !UNITY_EDITOR
            Debug.Log($"[{DateTime.Now}] [Server] Client Disconnected");
            #endif
        }

        public override void OnClientDisconnectedByID(long clientID)
        {
            throw new NotImplementedException();
        }
    }
}
