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

	    protected abstract void OnReceiveMessage(byte[] data, int size);
	    protected abstract void OnConnect();
	    public abstract void OnDisconnect(byte[] data, int size);

	    protected abstract void OnNetworkStatus(NetcodeClientStatus status);
	    
	    protected void StartClient(byte[] connectToken)
	    {
		    UnityNetcode.QuerySupport(supportStatus =>
		    {
			    switch (supportStatus)
			    {
				    case NetcodeIOSupportStatus.Available:
					    UnityNetcode.CreateClient(NetcodeIOClientProtocol.IPv4, client =>
					    {
						    this.client = client;
						    endpoint = new ReliableEndpoint();
					    
						    client.Connect(connectToken, () =>
						    {
							    OnConnect();
							    client.AddPayloadListener(ReceivePacket);
							    StartCoroutine(StatusUpdate());
						    }, err =>
						    {
							    Debug.Log($"[{DateTime.Now}] [Client] {err}");
						    });
					    });
					    break;
				    case NetcodeIOSupportStatus.Unavailable:
					    //logLine("Netcode.IO not available");
					    break;
				    case NetcodeIOSupportStatus.HelperNotInstalled:
					    //logLine("Netcode.IO is available, but native helper is not installed");
					    break;
				    case NetcodeIOSupportStatus.Unknown:
					    break;
				    default:
					    throw new ArgumentOutOfRangeException(nameof(supportStatus), supportStatus, null);
			    }
		    });		    
	    }
		
	    private void ReceivePacket(NetcodeClient client, NetcodePacket packet)
	    {	  
		    endpoint.ReceiveCallback = OnReceiveMessage;
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
				client.QueryStatus(OnNetworkStatus);
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