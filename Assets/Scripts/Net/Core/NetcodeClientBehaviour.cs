using System;
using System.Collections;
using ReliableNetcode;
using UnityEngine;
using UnityNetcodeIO;

namespace Server.Network
{
    public abstract class NetcodeClientBehaviour : MonoBehaviour
    {
		private NetcodeClient client;
	    private ReliableEndpoint endpoint;
	       
	    protected long GetID()
	    {
		    return DateTime.UtcNow.Ticks;
	    }
	    
	    public abstract void OnClientReceiveMessage(byte[] data, int size);
	    public abstract void OnClientConnect();
	    public abstract void OnClientDisconnect(byte[] data, int size);
	    
	    public abstract void OnClientNetworkStatus(NetcodeClientStatus status);
	    
	    protected void StartClient(byte[] connectToken)
	    {
		    UnityNetcode.QuerySupport(supportStatus =>
		    {
			    if (NetcodeIOSupportStatus.Available == supportStatus)
			    {
				    UnityNetcode.CreateClient(NetcodeIOClientProtocol.IPv4, client =>
				    {
					    this.client = client;
					    endpoint = new ReliableEndpoint();
					    
					    client.Connect(connectToken, () =>
					    {
						    OnClientConnect();
						    // add listener for network messages
						    client.AddPayloadListener(ReceivePacket);

						    // do stuff
						    StartCoroutine(StatusUpdate());
					    }, err =>
					    {
							Debug.Log($"[{DateTime.Now}] [Client] {err}");
					    });
				    });
			    }
			    else if (NetcodeIOSupportStatus.Unavailable == supportStatus)
			    {
				    //logLine("Netcode.IO not available");
			    }
			    else if (NetcodeIOSupportStatus.HelperNotInstalled == supportStatus)
			    {
				    //logLine("Netcode.IO is available, but native helper is not installed");
			    }
		    });		    
	    }
		
	    private void ReceivePacket(NetcodeClient client, NetcodePacket packet)
	    {	  
		    endpoint.ReceiveCallback = (data, size) =>
		    {		
			    #if UNITY_EDITOR
			    Debug.Log($"[{DateTime.Now}] [Client] Received Server Message: {(OP)BitConverter.ToInt32(data, 0)}");
			    #endif
			    OnClientReceiveMessage(data, size);
		    };
		    endpoint.ReceivePacket(packet.PacketBuffer.InternalBuffer, packet.PacketBuffer.Length);
	    }

	    /// <summary>
	    /// Sends data to the Game Server
	    /// </summary>
	    protected void Send(byte[] payload, int size, QosType type)
	    {
		    endpoint.TransmitCallback = ( data, length ) =>
		    {
			    client.Send( data, length );
		    };

		    endpoint.SendMessage(payload, size, type);
	    }
	    
		IEnumerator StatusUpdate()
		{
			while (true)
			{
				client.QueryStatus(OnClientNetworkStatus);
				endpoint.Update();
				yield return new WaitForSeconds(0.5f);
			}
		}

		public virtual void OnDestroy()
		{
			DestroyConnection();
		}

	    private void DestroyConnection()
	    {
		    if (client != null)
			    UnityNetcode.DestroyClient(client);
	    }

    }
}