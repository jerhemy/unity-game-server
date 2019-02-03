using System.Collections.Generic;
using System.Linq;
using Entities;

namespace UnityMMO.Entities
{
    public class Encounter
    {
        /// <summary>
        /// Collection to hold the different spawnable entities
        /// </summary>
        private Dictionary<int, Spawn> Spawns;


        public Encounter()
        {
            Spawns = new Dictionary<int, Spawn>();
        }
        public Spawn GetSpawn()
        {
            return Spawns.FirstOrDefault().Value;
        }
    }
}