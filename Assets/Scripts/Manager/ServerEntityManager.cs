using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Net;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Manages all entities in the scene
/// </summary>
public class ServerEntityManager : MonoBehaviour
{
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

        
        if (_entity == null)
        {
            _entity = new ConcurrentDictionary<long, EntityGO>();
            
            activeContainer = new GameObject("Active");
            activeContainer.transform.position = new Vector3(0,0,0);
            activeContainer.transform.parent = transform;                     
        } 
        
        objectPool = new List<GameObject>();
        for (var count = 0; count < objectPoolSize; count++)
        {
            var newGO = new GameObject();
            var entityGO = newGO.AddComponent<EntityGO>();
            
            #if UNITY_EDITOR
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.position = new Vector3(0f, 0.5f, 0f);
            box.transform.parent = newGO.transform;
            #endif
            
            objectPool.Add(newGO);
        }
        
        //StartCoroutine(Process());
        //EventManager.Subscribe("CreatePlayer", CreatePlayer);
        //EventManager.Subscribe("UpdatePlayer", CreatePlayer);
        //EventManager.Subscribe("RemovePlayer", CreatePlayer);
    }
    
    private IEnumerator Process()
    {
        while (true)
        {
            // Get all EntityGO objects active in scene
            var entities = _entity.Where(entity => entity.Value.gameObject.activeInHierarchy).Select( go => go.Value);
            
            // Run Update
            foreach (var go in entities)
            {
                go.GetComponent<EntityGO>().Process();
            }
            
            yield return new WaitForSeconds(0.1f);
        }

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
        // On creation we use the Mob object as this is the initial state
        // For Updates we use the GameObject as that will maintain current state
        foreach (var e in entity)
        {
            // Find available object from pool
            var go = GetFreeObject();
            
            var id = GetID();
            go.SetID(id);
            go.AddEntity(e);

            _entity.TryAdd(id, go);
            
            go.Spawn();
        }
    }

    private long GetID()
    {
        return DateTime.UtcNow.Ticks;
    }
    
    public void CreatePlayer(long id, ClientPlayer client)
    {   
        
        var go = GetFreeObject();
        go.AddEntity(client);

        _entity.TryAdd(id, go);
            
        go.Spawn();
    }
    
    public void RemovePlayer(INetworkPacket packet)
    {
        // Load Player Details from Persistant Storage
        //_entity.TryRemove(packet.clientId, out _);  
        
        // Send Packet that user is finished loading
        //EventManager.Publish("");
    }
    
    public void UpdatePlayer(INetworkPacket packet)
    {
        // Load Player Details from Persistant Storage
            
        // Send Packet that user is finished loading
        //EventManager.Publish("");
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

