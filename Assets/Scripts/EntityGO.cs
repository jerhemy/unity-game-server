using System;
using Entities;
using JetBrains.Annotations;
using MMO.Entity.AI;
using UnityEngine;
using UnityEngine.AI;

public class EntityGO : MonoBehaviour
{
    public Transform activeContainer;
    
    [SerializeField]
    private long id;
  
    private ServerEntityManager entityManager = ServerEntityManager.instance;
    
    [CanBeNull] private Entity _entity;

    void Awake()
    {       
        gameObject.SetActive(false);
        activeContainer = entityManager.activeContainer.transform;
        gameObject.transform.parent = activeContainer;
    }

    public void SetID(long id)
    {
        this.id = id;
    }

    
    void OnEnable()
    {
        gameObject.transform.parent = activeContainer;
        gameObject.name = _entity.name;
        if (_entity.IsMob())
        {
            gameObject.AddComponent<NavMeshAgent>();
            gameObject.AddComponent<WanderingAI>();
        }
    }

    public void AddEntity(Entity entity)
    {
        _entity = entity;
        gameObject.transform.position = EntityPositon;

    }

    public Entity GetEntity()
    {
        return _entity;
    }
      
    private void OnDisable()
    {       
        //Move gameObject to inactive container too keep heirarchy clean
        _entity = null;
        id = 0;
        gameObject.name = String.Empty;
    }
    
    public void Process()
    {
        //Get Entities around Object
        var hitColliders = Physics.OverlapSphere(transform.position, 5f);
        
        foreach (var t in hitColliders)
        {
            var x = t.GetComponent<EntityGO>();
            
        }

        
        
        _entity?.Process();
        
    }

    public void Spawn()
    {
        gameObject.SetActive(true);
    }

    public Vector3 EntityPositon
    {
        get => new Vector3(_entity.posX, _entity.posY, _entity.posZ);
        set
        {
            transform.position = value;
            _entity.posX = value.x;
            _entity.posY = value.y;
            _entity.posZ = value.z;          
        }
    }
}
