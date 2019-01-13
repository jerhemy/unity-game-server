using System.Collections.Generic;
using UnityEngine;

namespace MMO.Entity
{
    public class SpellUser : MonoBehaviour
    {
        private int maxMana;
        private int currentMana;
        private float manaRegen;
        
        /// <summary>
        /// List of spell id's player has learned
        /// </summary>
        private List<int> spellList = new List<int>();

        void Start()
        {
           
        }

        void Update()
        {
            
        }
    }
}