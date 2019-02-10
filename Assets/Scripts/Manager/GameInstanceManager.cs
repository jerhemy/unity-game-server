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
    /// <summary>
    /// 1. If Config file is present, skip to 10.
    /// 2. Read Command Line Arguments
    /// 3. Create InstanceToWorldConnection
    /// 4. Connect to World Server
    /// 5. Wait for World Server to Specify Zone Instance
    /// 
    /// 10. Read Config Options
    /// 10. Load Zone Asset
    /// 10. Request Zone Entities
    /// 10. Create Instance Server
    /// 10. Start Server
    /// </summary>
    
    
    public class GameInstanceManager : MonoBehaviour
    {
        private bool hasConfigFile = false;
        
        private AssetBundle BaseSceneBundle;
        private List<string> BaseScenePaths;
        
        private static ServerConfig config;

        public static GameInstanceManager instance;

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
        private void Start()
        {
            Init();
        }

        private void Init()
        {
            hasConfigFile = File.Exists($"{Application.dataPath}/config.json");

            if (hasConfigFile)
            {
                LoadConfigFile();
                LoadScene();
            }
            else
            {
                LoadCommandLineArguments();
            }
        }
        
        /// <summary>
        /// Load instance server configuration from config.json file
        /// </summary>
        /// <returns>Returns true if file loaded</returns>
        private bool LoadConfigFile()
        {
            try
            {
                using (StreamReader r = new StreamReader($"{Application.dataPath}/config.json"))
                {
                    string json = r.ReadToEnd();
                    config = JsonUtility.FromJson<ServerConfig>(json);
                    Debug.LogFormat("PrivateKey: {0}", config.privateKey);       
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return false;
            }
        }

        private void LoadCommandLineArguments()
        {
            
        }
        
        private void LoadScene()
        {

#if UNITY_STANDALONE_OSX
            BaseSceneBundle = AssetBundle.LoadFromFile($"AssetBundles/StandaloneOSXUniversal/{serverConfig.zone}");
#elif UNITY_STANDALONE
            BaseSceneBundle = AssetBundle.LoadFromFile($"AssetBundles/StandaloneWindows/{config.zone}");
#endif

            Debug.Log($"{DateTime.Now} [Instance Server] Loading Instance for {config.zone}");
            BaseScenePaths = BaseSceneBundle.GetAllScenePaths().ToList();
            SceneManager.sceneLoaded += OnSceneLoadedHandler;
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
                    config = JsonUtility.FromJson<ServerConfig>(json);
                    Debug.LogFormat("PrivateKey: {0}", config.privateKey);       
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

        #region Event Handlers

        void OnSceneLoadedHandler(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"{DateTime.Now} [Instance Server] Loaded Scene {scene.name}");
            
            if (scene.name == config.zone)
            {
                LoadEntities();
                //baseServer.StartServer(serverConfig);
                var instanceServer = gameObject.AddComponent<ClientToInstanceServer>();
                instanceServer.StartServer(config);               
            }
        }


        #endregion
    }
}
