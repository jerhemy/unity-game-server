using System;
using System.Collections.Concurrent;
using UnityEngine;
using NetcodeIO.NET;
using ReliableNetcode;
using Utils;

namespace Server.Network
{    
    public class InstanceServer : NetcodeServerBehaviour
    {   
        private ConcurrentDictionary<RemoteClient, long> gameClients;
        private ConcurrentDictionary<long, RemoteClient> gameClientIDs;

        void Start()
        {
            base.Init();
            
            gameClients = new ConcurrentDictionary<RemoteClient, long>();
            gameClientIDs = new ConcurrentDictionary<long, RemoteClient>();
        }
        public void StartServer(ServerConfig config)
        {
            base.StartServer(config.ip, config.port, config.protocolId, config.maxClients, config.GetKey());
            Debug.Log($"{GenerateToken(config)}");
        }
   
        protected override void OnServerReceiveMessage(RemoteClient client, byte[] data, int size)
        {
            if (!gameClients.TryGetValue(client, out var clientID)) return;
            
            var request = new NetworkPacket(clientID, data);
            Debug.Log($"[{DateTime.Now}] [Server] Received: {clientID}:{request.type}");
            
            if (OP.ClientConnect == request.type)
            {                       
                var response = new NetworkPacket(clientID, OP.ClientConnect);
                base.Send(client, response.data, response.length, QosType.Reliable);
            }           
        }

        protected override void OnClientConnected(RemoteClient client)
        {
            
            // Game logic works off entity ID's, use IDs for everything past this point
            var clientID = Identity.GetID();
            gameClients.TryAdd(client, clientID);
            gameClientIDs.TryAdd(clientID, client);

            #if !UNITY_EDITOR
                Debug.Log($"[{DateTime.Now}] [Server] Client Connected");
            #endif
        }

        protected override void OnClientDisconnected(RemoteClient client)
        {            
            gameClients.TryRemove(client, out var clientID);
            gameClientIDs.TryRemove(clientID, out _);
        }
              
    }
}
