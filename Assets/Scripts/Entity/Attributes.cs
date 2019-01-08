using Models;
using UnityEngine;

namespace MMO.Entity
{
    public class Attributes : MonoBehaviour
    {
        public int ac;
        
        public int hp;
        public int hp_regen;
        
        public int mana;
        public int mana_regen;

        public int str;
        public int con;
        public int dex;
        public int agi;
        public int _int;
        public int wis;
        public int chr;


        void FixedUpdate()
        {
            
        }
        
        void Update()
        {
            
        }

        void LoadAttributes(Mob mob)
        {
            hp = mob.hp;
        }

    }
}