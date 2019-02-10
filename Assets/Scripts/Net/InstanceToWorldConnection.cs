using System;
using System.Numerics;
using System.Text;
using UnityEngine;
using UnityNetcodeIO;

namespace Server.Network
{
    public class InstanceToWorldConnection : NetcodeClientBehaviour
    {
        [SerializeField] private bool IsDead;
        [SerializeField] private long SelectedCharacter;
        
        [Header("World Connection Settings")]
        [SerializeField] private string connectToken;
        
        void Awake()
        {
            // World connection should always be present
            DontDestroyOnLoad(this);
        }
        
        void Start()
        {
            var token = Encoding.ASCII.GetBytes(connectToken);
            StartClient(token);
        }


        protected override void OnReceiveMessage(byte[] data, int size)
        {
            byte[] payload = new byte[size];

            Array.Copy(data, payload, size);
            
            //throw new System.NotImplementedException();
        }

        protected override void OnConnect()
        {
            //throw new NotImplementedException();
        }

        public override void OnDisconnect(byte[] data, int size)
        {
            //throw new NotImplementedException();
        }

        protected override void OnNetworkStatus(NetcodeClientStatus status)
        {
            if (status == NetcodeClientStatus.Disconnected)
            {
                Debug.Log("I'VE BEEN DISCONNECTED");
            }
        }
    }
}