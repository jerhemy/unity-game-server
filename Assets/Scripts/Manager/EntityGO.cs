﻿using System;
using Server.Entities;
using Entities.AI;
using JetBrains.Annotations;
using Server.Interface;
using UnityEngine;
using UnityEngine.AI;

namespace Server.Manager
{
    public class EntityGO : MonoBehaviour, IEntityGO
    {
        public Transform activeContainer;

        [SerializeField] private long id;

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
            // List of Entities that are within range of the Client
            var hitColliders = Physics.OverlapSphere(transform.position, 5f, ServerEntityManager.SPAWNED_ENTITY_LAYER );

            
            foreach (var t in hitColliders)
            {
                var x = t.GetComponent<EntityGO>().id;
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

        public bool SpawnOrUpdateEntity()
        {
            return true;
        }
    }
}
