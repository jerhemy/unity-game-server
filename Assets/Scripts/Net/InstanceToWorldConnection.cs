using System;
using System.Text;
using Common;
using Common.Net.Core;
using Net;
using ReliableNetcode;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        void Start()
        {
            var token = Encoding.ASCII.GetBytes(connectToken);
            StartClient(token);
        }
        

        public override void OnClientReceiveMessage(byte[] data, int size)
        {
            byte[] payload = new byte[size];

            Array.Copy(data, payload, size);
            
            //throw new System.NotImplementedException();
        }

        public override void OnClientConnect()
        {
            //throw new NotImplementedException();
        }

        public override void OnClientDisconnect(byte[] data, int size)
        {
            //throw new NotImplementedException();
        }

        public override void OnClientNetworkStatus(NetcodeClientStatus status)
        {
            throw new NotImplementedException();
        }

        public override void OnClientNetworkStatus(NetcodeClientStatus status)
        {
            //throw new System.NotImplementedException();
        }

        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            // We prefix non-connected scenes with '_'
            if (scene.name.StartsWith("_")) {
                Destroy(gameObject);
            }
        }
    }
}