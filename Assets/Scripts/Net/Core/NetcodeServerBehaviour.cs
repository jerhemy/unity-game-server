﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using NetcodeIO.NET;
using ReliableNetcode;
using UnityEngine;
using UnityNetcodeIO;
using Utils;

namespace Server.Network
{
	public class ServerConfig
	{
		public string zone;
		public string ip;
		public int port;
		public ulong protocolId;
		public int maxClients;
		public string privateKey;

		public byte[] GetKey()
		{
			var pkey = privateKey.Substring(0, 16);
			return Encoding.Unicode.GetBytes(pkey);
		}

	}
	
    public abstract class NetcodeServerBehaviour : MonoBehaviour
    {	    
	    private ConcurrentDictionary<RemoteClient, ReliableEndpoint> _clients;
	    private NetcodeServer _server;
	    
	    protected abstract void OnServerReceiveMessage(RemoteClient client, byte[] data, int size);
	    protected abstract void OnClientConnected(RemoteClient client);
	    protected abstract void OnClientDisconnected(RemoteClient client);
	    
	    public void Start()
	    {
		    Init();
	    }

	    protected void Init()
	    {
		    _clients = new ConcurrentDictionary<RemoteClient, ReliableEndpoint>();
	    }

	    protected void StartServer(string ipAddress, int port, ulong protocolID, int maxClients, byte[] privateKey)
	    {
		    try
		    {
			    _server = UnityNetcode.CreateServer(ipAddress, port, protocolID, maxClients, privateKey);
			    
			    _server.ClientConnectedEvent.AddListener( ClientConnected );	// void( RemoteClient client );
			    _server.ClientDisconnectedEvent.AddListener( ClientDisconnected );	// void( RemoteClient client );
			    _server.ClientMessageEvent.AddListener( ReceivePacket );	// void( RemoteClient sender, ByteBuffer payload );
			    
			    _server.StartServer();
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
			var endpoint = new ReliableEndpoint();			
			_clients.TryAdd(client, endpoint);

		    OnClientConnected(client);
	    }

	    private void ClientDisconnected(RemoteClient client)
	    {	   
		    OnClientDisconnected(client);
		    
		    //Remote Remote Client from Collection
		    _clients.TryRemove(client, out _);		    
	    }

	    protected void DisconnectClient(RemoteClient client)
	    {
		    _server.Disconnect(client);
		    _clients.TryRemove(client, out _);
	    }
	    
	    private void ReceivePacket(RemoteClient client, ByteBuffer packet)
	    {
		    // Get ID from Packet
		    
		    // Verify Client for ID matches Client for ReliableEndpoint
		    
		    if (!_clients.TryGetValue(client, out var endpoint)) return;
		    endpoint.Update();
		    
		    endpoint.ReceiveCallback = (data, size) =>
		    {		
				#if UNITY_EDITOR		    
				Debug.Log($"{DateTime.Now} [Server] Sent {(OP)BitConverter.ToInt32(data, 0)} to client {client.ClientID}");
				#endif
			    
			    OnServerReceiveMessage(client, data, size);
		    };
		    

		    endpoint.ReceivePacket(packet.InternalBuffer, packet.Length);
	    }

	    /// <summary>
	    /// Sends data to the Game Server
	    /// </summary>
	    protected void Send(RemoteClient client, byte[] payload, int payloadSize, QosType type)
	    {
		    if (!_clients.TryGetValue(client, out var endpoint)) return;
		    endpoint.Update();
		    endpoint.TransmitCallback = ( data, size ) =>
		    {
			    ByteBuffer buffer = BufferPool.GetBuffer(size);
			    buffer.BufferCopy(data, 0, 0, size);
			    _server.SendPayload(client, buffer );
		    };
		    
			#if UNITY_EDITOR		    
			Debug.Log($"{DateTime.Now} [Server] Sent {(OP)BitConverter.ToInt32(payload, 0)} to client {client.ClientID}");
			#endif
		    
		    endpoint.SendMessage(payload, payloadSize, type);
	    }
	    	    
		public void OnDestroy()
		{
			DestroyServer();
		}

	    private void DestroyServer()
	    {
		    if (_server != null)
			    _server.Dispose();
	    }
	    
	    public string GenerateToken(ServerConfig config)
	    {
		    var token = GenerateToken(config.protocolId, config.privateKey, config.ip, config.port);
		    return Convert.ToBase64String(token);
	    }
	    
	    public byte[] GenerateToken(ulong protocolID, string serverKey, string ipAddress, int port)
	    {
		    var sequenceNumber = ulong.Parse(DateTime.Now.ToString("hhmmssffffff"));
		    var privateKey = Encoding.Unicode.GetBytes(serverKey);

			//var privateKey = new byte[]
			//{
			//	0x60, 0x6a, 0xbe, 0x6e, 0xc9, 0x19, 0x10, 0xea,
			//	0x9a, 0x65, 0x62, 0xf6, 0x6f, 0x2b, 0x30, 0xe4,
			//	0x43, 0x71, 0xd6, 0x2c, 0xd1, 0x99, 0x27, 0x26,
			//	0x6b, 0x3c, 0x60, 0xf4, 0xb7, 0x15, 0xab, 0xa1
			//};
	    
		    var worldIP = new IPEndPoint(IPAddress.Parse(ipAddress), port);	    
		    IPEndPoint[] addressList = {worldIP}; 
		    
		    TokenFactory tokenFactory = new TokenFactory(
			    protocolID,		// must be the same protocol ID as passed to both client and server constructors
			    privateKey		// byte[32], must be the same as the private key passed to the Server constructor
		    );

		    var cID = (ulong)DateTime.UtcNow.Ticks;
		    var userData = new byte[256];
		    
		    // ClientID will be AccountID as only clients will be connecting to the World Server
		    return tokenFactory.GenerateConnectToken(
			    addressList,		// IPEndPoint[] list of addresses the client can connect to. Must have at least one and no more than 32.
			    60,		// in how many seconds will the token expire
			    60,		// how long it takes until a connection attempt times out and the client tries the next server.
			    1UL,		// ulong token sequence number used to uniquely identify a connect token.
			    cID,		// ulong ID used to uniquely identify this client
			    userData		// byte[], up to 256 bytes of arbitrary user data (available to the server as RemoteClient.UserData)
		    );

	    }
    }
}
