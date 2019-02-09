using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Server.Entities;
using Server.Network;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Server.Manager
{
    public class StartupManager : MonoBehaviour
    {
        public static AssetBundle BaseSceneBundle;
        public static List<string> BaseScenePaths;

        private static ServerConfig serverConfig;

        public static StartupManager instance;

        void Awake()
        {
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {

            // Load JSON Config
            LoadConfig();

            #if UNITY_STANDALONE_OSX
                BaseSceneBundle = AssetBundle.LoadFromFile($"AssetBundles/StandaloneOSXUniversal/{serverConfig.zone}");
            #elif UNITY_STANDALONE
                BaseSceneBundle = AssetBundle.LoadFromFile($"AssetBundles/StandaloneWindows/{serverConfig.zone}");
            #endif

            BaseScenePaths = BaseSceneBundle.GetAllScenePaths().ToList();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(BaseScenePaths[0]);
        }

        private static void LoadConfig()
        {
            var m_Path = Application.dataPath;
            //Debug.Log($"Path: {m_Path}");
            try
            {
                using (StreamReader r = new StreamReader($"{m_Path}/config.json"))
                {
                    string json = r.ReadToEnd();
                    serverConfig = JsonUtility.FromJson<ServerConfig>(json);
                    Debug.Log($"ZoneID: {serverConfig.zone}");
                    Debug.Log($"IP: {serverConfig.ip}");
                    Debug.Log($"Port: {serverConfig.port}");
                    Debug.Log($"PrivateKey: {serverConfig.privateKey}");

                    var keyLength = serverConfig.privateKey.Length == 16;
                    //Debug.Assert(keyLength, "Invalid privateKey length");             
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        private void LoadEntities()
        {
            List<Entity> mobs = new List<Entity>();
            for (int x = 0; x < 100; x++)
            {
                mobs.Add(new Mob());
            }

            ServerEntityManager.instance.AddEntity(mobs);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == serverConfig.zone)
            {
                LoadEntities();
                //baseServer.StartServer(serverConfig);
                var instanceServer = gameObject.AddComponent<InstanceServer>();
                instanceServer.StartServer(serverConfig);               
            }
        }
    }
}
