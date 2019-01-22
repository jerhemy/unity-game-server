using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using MMO.Entity;
using MMO.Entity.AI;
using Models;
using Net;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages all entities in the scene
/// </summary>
public class EntityManager : MonoBehaviour
{
    /// <summary>
    /// Collection holds all NPC entities in scene
    /// </summary>
    private ConcurrentDictionary<long, Entity> _entity;
    private ConcurrentDictionary<long, Entity> _enitityGraveyard;
        
    private GameObject entityContainer;
    private GameObject entityGraveyardContainer; 

    
    public static EntityManager instance;

    private long _playerCounter = 0;
    private long _npcCounter = 0;
    
    private long GetNextID()
    {
        return Interlocked.Increment(ref _playerCounter);
    }
    
    private long GetNextNPCID()
    {
        return Interlocked.Decrement(ref _npcCounter);
    }
    
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
        if (_entity == null)
        {
            _entity = new ConcurrentDictionary<long, Entity>();
            
            entityContainer = new GameObject("Active");
            entityContainer.transform.position = new Vector3(0,0,0);
            entityContainer.transform.parent = transform;                     
        } 
        
        if(_enitityGraveyard == null)
        {
            _enitityGraveyard = new ConcurrentDictionary<long, Entity>();
            entityGraveyardContainer = new GameObject("Graveyard");
            entityGraveyardContainer.transform.parent = transform;
        }
        
        StartCoroutine(Process());
        //EventManager.Subscribe("CreatePlayer", CreatePlayer);
        //EventManager.Subscribe("UpdatePlayer", CreatePlayer);
        //EventManager.Subscribe("RemovePlayer", CreatePlayer);
    }
    
    private IEnumerator Process()
    {
        while (true)
        {
            foreach (var kvp in _entity)
            {
                var entity = kvp.Value;
                
                if (entity.IsDead())
                {               
                    entity.gameObject.SetActive(false);
                    
                    MoveToGraveyard(kvp.Value.gameObject);
                    _enitityGraveyard.TryAdd(kvp.Key, entity);
                }
                
                
                
                
            }
            
            // Graveyard Cleanup 

            foreach (var kvp in _enitityGraveyard)
            {
                if (kvp.Value.GetComponent<EntityLoot>().HasItems())
                {
                    
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

    }

    
    private void MoveToActive(GameObject go)
    {
        go.transform.parent = entityContainer.transform;
    }

    private void MoveToGraveyard(GameObject go)
    {
        go.transform.parent = entityGraveyardContainer.transform;
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
    public void SpawnEntity(IEnumerable<Mob> mobs)
    {
        // On creation we use the Mob object as this is the initial state
        // For Updates we use the GameObject as that will maintain current state
        foreach (var mob in mobs)
        {
            var id = GetNextNPCID();
            
            //Create Entity
            var go = new GameObject($"{mob.name}_{Math.Abs(id)}");
            //var attributes = new Attributes();
            //attributes.hp = mob.hp;
            
            var entityBase = go.AddComponent<Entity>();
            entityBase.id = id;
            //entityBase.entityAttributes = attributes;
                        
            GameObject.CreatePrimitive(PrimitiveType.Cube).transform.parent = go.transform;
            
            //go.AddComponent<CharacterController>();
            go.AddComponent<NavMeshAgent>();
            entityBase.loot = go.AddComponent<EntityLoot>();
            
            var wandering = go.AddComponent<WanderingAI>();
            wandering.wanderTimer = 0;
            wandering.wanderRadius = 100f;
            
            MoveToActive(go);
            
            go.transform.position = mob.position;
            go.transform.rotation = mob.heading;

            mob.gameObject = go;
            //var CC = go.GetComponent<CharacterController>();
            //CC.transform.TransformDirection()     
            _entity.TryAdd(id, entityBase);
        }
    }

    public void CreatePlayer(Mob mob)
    {
        var id = GetNextID();
        // Load Player Details from Persistant Storage

        //Create Entity
        var go = new GameObject($"{mob.name}_{Math.Abs(id)}");

        go.AddComponent<ClientUpdate>();
        
        GameObject.CreatePrimitive(PrimitiveType.Capsule).transform.parent = go.transform;
        
        go.AddComponent<NavMeshAgent>();
        go.transform.parent = entityContainer.transform;
            
        //go.transform.position = m.position;
        //go.transform.rotation = m.heading;
            
        //var CC = go.GetComponent<CharacterController>();
        //CC.transform.TransformDirection()      
        //_entity.TryAdd(id, go);
        
        // Send Packet that user is finished loading
        //EventManager.Publish("");
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
}
