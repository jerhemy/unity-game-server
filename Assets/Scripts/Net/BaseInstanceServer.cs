using System;
using System.Collections.Concurrent;
using NetcodeIO.NET;
using ReliableNetcode;
using Server.Net;
using UnityEngine;
using Server = NetcodeIO.NET.Server;

namespace Net
{
    public class BaseInstanceServer : IDisposable
    {
        private ConcurrentDictionary<RemoteClient, ReliableEndpoint> _clients;
        
        private NetcodeIO.NET.Server server;

        private int maxClients;
        private string publicAddress;
        private int port;
        private ulong protocolID;
        private byte[] privateKeyBytes;
        
        EventManager eventManager = EventManager.instance;
        
        public BaseInstanceServer()
        {
            _clients = new ConcurrentDictionary<RemoteClient, ReliableEndpoint>();
        }
        
        public void StartServer(ServerConfig config)
        {
			Debug.Log($"PrivateKey: ${config.GetKey()}");
            StartServer(config.ip, config.port, config.protocolId, config.maxClients, config.GetKey());
        }

        public void StartServer(string publicAddress, int port, ulong protocolID, int maxClients, byte[] privateKeyBytes)
        {
            try
            {
                server = new NetcodeIO.NET.Server (
                    maxClients,		// int maximum number of clients which can connect to this server at one time
                    publicAddress, port,	// string public address and int port clients will connect to
                    protocolID,		// ulong protocol ID shared between clients and server
                    privateKeyBytes		// byte[32] private crypto key shared between backend servers
                );
			    
                // Called when a client has connected
                server.OnClientConnected += ClientConnected;		// void( RemoteClient client )

                // Called when a client disconnects
                server.OnClientDisconnected += ClientDisconnected;	// void( RemoteClient client )

                // Called when a payload has been received from a client
                // Note that you should not keep a reference to the payload, as it will be returned to a pool after this call completes.
                server.OnClientMessageReceived += ReceivePacket;	// void( RemoteClient client, byte[] payload, int payloadSize )
            
                server.Start();			// start the server running
                Debug.Log($"{DateTime.Now} [Instance Server] Server Started");
            }
            catch (Exception ex)
            {
                Debug.LogAssertion($"{DateTime.Now} [Instance Server] {ex.Message}"); 
                Application.Quit();
            }
	    
        }
        
        private void ClientConnected(RemoteClient client)
        {
            Debug.Log($"{DateTime.Now} [Instance Server] Client Connected");
            var packet = new NetworkPacket(client);
            eventManager.Publish(OP_CODE.CLIENT_CONNECT, packet);
        }

        private void ClientDisconnected(RemoteClient client)
        {
            Debug.Log($"{DateTime.Now} [Instance Server] Client Disconnected");
            var packet = new NetworkPacket(client);
            eventManager.Publish(OP_CODE.CLIENT_DISCONNECT, packet);
            //OnClientDisconnected(client);
        }
	    
        private void ReceivePacket(RemoteClient client, byte[] payload, int payloadSize)
        {
            ReliableEndpoint endpoint;
            if (!_clients.TryGetValue(client, out endpoint)) return;
            endpoint.ReceiveCallback = (data, size) =>
            {			    
                //OnServerReceiveMessage( client, data, size );
            };
            endpoint.ReceivePacket(payload, payloadSize);
        }
        
        public void Dispose()
        {
            server.Stop();
        }
    }
}