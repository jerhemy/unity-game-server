using System.Collections.Generic;
using MMO.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace MMO.Entity
{
    public class Entity : MonoBehaviour
    {
        public long id;
        
        private bool _isDead;
        public SortedDictionary<long, int> hateList;
        public EntityLoot loot;
        
        public NavMeshAgent agent;
        public IEntityAI entityAI;
        public Attributes attributes;
        
        void Start()
        {
            _isDead = false;
            hateList = new SortedDictionary<long, int>();
            
        }


        public void AddHate(long playerId, int val)
        {
            
        }
        
        public bool Loot(long playerId)
        {
            return false;
        }

        public int GetCurrentHP()
        {
            return attributes.hp;
        }

        public bool IsDead()
        {
            if (attributes)
            {
                Destroy(agent);
                Destroy(attributes);             
                return attributes.hp <= 0;
            }

            return false;
        }
    }
}