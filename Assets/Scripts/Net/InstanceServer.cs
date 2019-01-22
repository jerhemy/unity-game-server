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
        public override void OnServerReceiveMessage(RemoteClient client, short type, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void OnClientConnected(RemoteClient client)
        {
            throw new NotImplementedException();
        }

        public override void OnClientDisconnected(RemoteClient client)
        {
            throw new NotImplementedException();
        }

        void Start()
        {

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

    }
}
