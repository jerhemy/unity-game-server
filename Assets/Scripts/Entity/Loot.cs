using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MMO.Entity
{
    public class EntityLoot : MonoBehaviour
    {
        private IEnumerable<long> items;

        void Start()
        {
            items = new List<long>();
        }
        
        public bool HasItems()
        {
            return items.Any();
        }
    }
}