using System;
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
	    private ConcurrentDictionary<long, RemoteClient> _clientLookup;
	    private ConcurrentDictionary<RemoteClient, long> _clientIDLookup;
	    private NetcodeServer _server;
	    
	    public abstract void OnServerReceiveMessage(RemoteClient client, byte[] data, int size);
	    public abstract void OnServerReceiveMessageByID(long clientID, byte[] data, int size);
	    public abstract void OnClientConnected(RemoteClient client);
	    public abstract void OnClientConnectedByID(long clientID);
	    public abstract void OnClientDisconnected(RemoteClient client);
	    public abstract void OnClientDisconnectedByID(long clientID);
	 	    
	    void Start()
	    {
		    _clients = new ConcurrentDictionary<RemoteClient, ReliableEndpoint>();
		    _clientLookup = new ConcurrentDictionary<long, RemoteClient>();
		    _clientIDLookup = new ConcurrentDictionary<RemoteClient, long>();
	    }

	    public void StartServer(ServerConfig config)
	    {
		    StartServer(config.ip, config.port, config.protocolId, config.maxClients, config.GetKey());
		    Debug.Log($"{GenerateToken(config)}");
	    }
	    
	    public void StartServer(string ipAddress, int port, ulong protocolID, int maxClients, byte[] privateKey)
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

		    var id = Identity.GetID();
			_clientLookup.TryAdd(id, client);
			_clientIDLookup.TryAdd(client, id);
			
			var endpoint = new ReliableEndpoint();			
			_clients.TryAdd(client, endpoint);

		    OnClientConnectedByID(id);
	    }

	    private void ClientDisconnected(RemoteClient client)
	    {
		    OnClientDisconnected(client);
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

			    if (GetClientID(client, out var clientID))
			    {
				    OnServerReceiveMessageByID(clientID, data, size);
			    }
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
	    
	    protected void SendPacket(RemoteClient client, NetworkPacket packet, QosType type)
	    {
		    if (!_clients.TryGetValue(client, out var endpoint)) return;
		    endpoint.Update();
		    
		    endpoint.TransmitCallback = ( data, size ) =>
		    {			    		
			    ByteBuffer buffer = BufferPool.GetBuffer(size);
			    buffer.BufferCopy(data, 0, 0, size);
			    _server.SendPayload(client, buffer );
			    
			    //_server.SendPayload(client, data, size );
		    };
		    
		    #if UNITY_EDITOR		    
		    Debug.Log($"{DateTime.Now} [Server] Sent {packet.type} to client {client.ClientID}");
		    #endif
		    
		    endpoint.SendMessage(packet.data, packet.length, type);
	    }

	    protected void SendPacketByID(long clientID, NetworkPacket packet, QosType type)
	    {
		    if (GetClientByID(clientID, out var client))
		    {
			    SendPacket(client, packet, type);
		    }
		    else
		    {
			    Debug.LogError($"{DateTime.Now} [Server] Could not find client for ID {clientID}");
		    }
			
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

	    protected bool GetClientByID(long id, out RemoteClient client)
	    {
		    return _clientLookup.TryGetValue(id, out client);
	    }
	    
	    protected bool GetClientID(RemoteClient client, out long clientID)
	    {
		    return _clientIDLookup.TryGetValue(client, out clientID);
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
