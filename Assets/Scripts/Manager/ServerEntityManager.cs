using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Server.Entities;
using Server.Interface;
using Server.Network;
using UnityEngine;
using Utils;
using Vector3 = UnityEngine.Vector3;

namespace Server.Manager
{
    /// <summary>
    /// Manages all entities in the scene
    /// </summary>
    public class ServerEntityManager : MonoBehaviour
    {
        public static int STATIC_ENTITY_LAYTER = 9;
        public static int SPAWNED_ENTITY_LAYER = 10;
        
        /// <summary>
        /// Collection holds all NPC entities in scene
        /// </summary>
        private ConcurrentDictionary<long, EntityGO> _entity;
            
        public GameObject activeContainer;
        
        [SerializeField]
        private int objectPoolSize = 2000;
    
        private List<GameObject> objectPool;
        
        public static ServerEntityManager instance;
    
        
        void Awake()
        {
            activeContainer = GameObject.Find("Active");
            
            if (_entity == null)
            {
                _entity = new ConcurrentDictionary<long, EntityGO>();                                 
            } 
            
            objectPool = new List<GameObject>();

            for (var count = 0; count < objectPoolSize; count++)
            {
                var newGO = new GameObject();
                newGO.transform.parent = transform.root;
                newGO.layer = SPAWNED_ENTITY_LAYER;
                var entityGO = newGO.AddComponent<EntityGO>();
                
                var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                box.transform.position = new Vector3(0f, 0.5f, 0f);
                box.transform.parent = newGO.transform;
                
                objectPool.Add(newGO);
            }
            
            //Check if instance already exists
            if (instance == null)
            {
                //if not, set instance to this
                instance = this;
    
            }
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
            //StartCoroutine(Process());
            EventManager.Subscribe(OP.ClientGetPlayer, CreatePlayer);
            EventManager.Subscribe(OP.EntityUpdate, UpdateEntityHandler);
            //EventManager.Subscribe("RemovePlayer", CreatePlayer);
            StartCoroutine(Process());
        }
        
        private IEnumerator Process()
        {
            while (true)
            {
                // Get all EntityGO objects active in scene
                var entities = _entity.Where(entity => entity.Value.activeInHierarchy).Select( go => go.Value);

                var length = entities.Count();

                // Run Update
                for (var idx = 0; idx < length; idx++)
                {
                    entities.ElementAt(idx).Process();
                }
                 
                yield return new WaitForSeconds(0.1f);
            }
    
        }


        public void UpdateEntityHandler(long id, IEntity entity)
        {
            
        }
        
        public void LoadEntities(IEnumerable<Mob> mobs)
        {
    
        }
        
        /// <summary>
        /// Spawn all entities that can be spawned at server startup
        /// </summary>
        /// <param name="entities"></param>
        /// <typeparam name="T"></typeparam>
        private void SpawnEntities<T>(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                //SpawnEntity();
            }
        }
        public void AddEntity(IEnumerable<Entity> entity)
        {
            Debug.Log($"{DateTime.Now} [Instance Server] Add Entities Called");
            // On creation we use the Mob object as this is the initial state
            // For Updates we use the GameObject as that will maintain current state
            foreach (var e in entity)
            {
                // Find available object from pool
                var go = GetFreeObject();
                
                var id = Identity.GetID();
                go.SetID(id);
                go.AddEntity(e);
    
                _entity.TryAdd(id, go);
                
                go.Spawn();
            }
        }
            
        public void CreatePlayer(NetworkPacket client)
        {
            var x = new Mob();
            var go = GetFreeObject();
            go.AddEntity(x);
            
            //_entity.TryAdd(id, go);
                
            go.Spawn();
            
        }
           
        private EntityGO GetFreeObject()
        {
            var go = objectPool.FirstOrDefault(g => g.gameObject.activeInHierarchy == false);
            if (go) return go.GetComponent<EntityGO>();
            
            var newGO = new GameObject();
            var entityGO = newGO.AddComponent<EntityGO>();
            objectPool.Add(newGO);
            return entityGO;
        }
    
        public EntityGO GetTargetByID(long id)
        {
            _entity.TryGetValue(id, out var entity);
            return entity;
        }
    }
}

