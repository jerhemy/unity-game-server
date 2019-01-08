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
    public class Player
    {
        private ReliableEndpoint Endpoint { get; }

        public Player()
        {
            Endpoint = new ReliableEndpoint();
        }
    }
    
    public class InstanceServer : MonoBehaviour
    {
        private ConcurrentDictionary<RemoteClient, ReliableEndpoint> _clientList;

        public delegate void ClientConnected(RemoteClient client);

        public delegate void ClientDisconnected(RemoteClient client);

        public delegate void ClientMessage(RemoteClient client, ByteBuffer payload);

        static readonly byte[] privateKey = new byte[]
        {
            0x60, 0x6a, 0xbe, 0x6e, 0xc9, 0x19, 0x10, 0xea,
            0x9a, 0x65, 0x62, 0xf6, 0x6f, 0x2b, 0x30, 0xe4,
            0x43, 0x71, 0xd6, 0x2c, 0xd1, 0x99, 0x27, 0x26,
            0x6b, 0x3c, 0x60, 0xf4, 0xb7, 0x15, 0xab, 0xa1,
        };

        public Text outputText;
        public Text NumClientsText;

        public string PublicIP = "127.0.0.1";
        public int Port = 4000;
        public int MaxClients = 256;

        private NetcodeServer server;
        private int clients = 0;

        public ClientConnected OnClientConnect;
        public ClientDisconnected OnClientDisconnect;
        public ClientMessage OnClientMessage;
                
        void Start()
        {
            _clientList = new ConcurrentDictionary<RemoteClient, ReliableEndpoint>();
            
            server = UnityNetcode.CreateServer(PublicIP, Port, 1UL, MaxClients, privateKey);

            server.ClientConnectedEvent.AddListener(Server_OnClientConnected);
            server.ClientDisconnectedEvent.AddListener(Server_OnClientDisconnected);
            server.ClientMessageEvent.AddListener(Server_ReceiveMessage);

            try
            {
                server.StartServer();
                Debug.Log($"{DateTime.Now} [Instance Server] Server Started");

            }
            catch (Exception ex)
            {
                Debug.LogAssertion($"{DateTime.Now} [Instance Server] {ex.Message}"); 
                Application.Quit();
            } 
        }

        void OnDestroy()
        {
            if (!server) return;
            server.StopServer();
            server.Dispose();
            Debug.Log($"{DateTime.Now} [Instance Server] Server Shutdown");
        }

        
        private void NetworkRange(Vector3 center, float radius)
        {
            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            int i = 0;
            while (i < hitColliders.Length)
            {
                //hitColliders[i].SendMessage("AddDamage");
                i++;
            }
        }

        private void Server_SendMessage(RemoteClient client, ByteBuffer payload, QosType type = QosType.Reliable)
        {
            ReliableEndpoint reliableEndpoint;
            if (!_clientList.TryGetValue(client, out reliableEndpoint)) return;

            reliableEndpoint.TransmitCallback = (data, size) => { client.SendPayload(data, size); };

            reliableEndpoint.SendMessage(payload.InternalBuffer, payload.InternalBuffer.Length, type);
        }

        private void Server_ReceiveMessage(RemoteClient client, ByteBuffer payload)
        {
            ReliableEndpoint reliableEndpoint;
            if (!_clientList.TryGetValue(client, out reliableEndpoint)) return;
            reliableEndpoint.ReceiveCallback = (message, messageSize) =>
            {
                // this will be called with individual messages, so do whatever message receive processing you want here.
                //OnClientMessage?.Invoke(client, payload);
                EventManager.Publish("UpdatePlayer", new BasePacket {clientId = client.ClientID} );
            };

            reliableEndpoint.ReceivePacket(payload.InternalBuffer, payload.InternalBuffer.Length);
        }

        private void Server_OnClientConnected(RemoteClient client)
        {
            Debug.Log($"{DateTime.Now} [Instance] Client Connected");
            
            if(!_clientList.TryAdd(client, new ReliableEndpoint()))
            {
                // Couldnt Add Client, Disconnect
                server.Disconnect(client);
            }
            
            // Create Player in Scene
            
            // Register Client with GameObject
            
            
            //EventManager.Publish("CreatePlayer", new BasePacket {clientId = client.ClientID} );
        }

        private void Server_OnClientDisconnected(RemoteClient client)
        {
            Debug.Log($"{DateTime.Now} [Instance] Client Disconnected");
            EventManager.Publish("RemovePlayer", new BasePacket {clientId = client.ClientID} );
            //OnClientDisconnect?.Invoke(client);
            //_clientList.TryRemove(client, out);
        }
    }
}
