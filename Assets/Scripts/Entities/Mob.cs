using UnityEngine;

namespace Entities
{
    public class Mob : Entity
    {
        public Mob()
        {
            hp = 100;
            name = randomeName();
            position = new Vector3(Random.Range(-100.0f, 100.0f), 0.0f, Random.Range(-100.0f, 100.0f));
            heading = Quaternion.Euler(0f, Random.Range(0, 90), 0f);
        }
        
        public Mob(string name, float x, float y, float z, float heading)
        {
            this.name = name;
            position = new Vector3(x, y, z);
            this.heading = Quaternion.Euler (0f, heading, 0f);
        }

        private string randomeName()
        {
            int charAmount = Random.Range(3, 16); //set those to the minimum and maximum length of your string
            var name = "";
            for(int i=0; i<charAmount; i++)
            {
                name += glyphs[Random.Range(0, glyphs.Length)];
            }

            return name;
        }
        
        public int id;        
        private const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
           
        
        // Stats
        public int hp;
        
        protected string orig_name;
        protected string clean_name;
        protected string lastname;

        protected Vector3 position;
        protected Quaternion heading;
        
        int  ExtraHaste; // for the #haste command
        bool mezzed;
        bool stunned;
        bool charmed; //this isnt fully implemented yet
        bool rooted;
        bool silenced;
        bool amnesiad;
        bool inWater; // Set to true or false by Water Detection code if enabled by rules
        bool has_virus; // whether this mob has a viral spell on them

        bool offhand;
        bool has_shieldequiped;
        bool has_twohandbluntequiped;
        bool has_twohanderequipped;
        bool can_facestab;
        bool has_numhits;
        bool has_MGB;
        bool has_ProjectIllusion;
        
        bool last_los_check;
        bool pseudo_rooted;
        bool endur_upkeep;
        bool degenerating_effects; // true if we have a buff that needs to be recalced every tick
        bool spawned_in_water;
        
        protected int AC;
        
        protected int ATK;
        
        /// <summary>
        /// Strength - Melee ATK Bonus, Carrying Capacity
        /// </summary>
        protected int STR;
        
        /// <summary>
        /// Constitution - HP Max/Regen, Stamina Max/Regen
        /// </summary>
        protected int CON;
        
        /// <summary>
        /// Dexterity - Ranged ATK Bonus, Damange Bonus
        /// </summary>
        protected int DEX;
        
        /// <summary>
        /// Agility - AC Bonus, Dodge Chance
        /// </summary>
        protected int AGI;
        
        /// <summary>
        /// Intelligence Pure Caster MP Max, Spell Power
        /// </summary>
        protected int INT;
        
        /// <summary>
        /// Wisdom - Hybrid Caster MP Max, Spell Power
        /// </summary>
        protected int WIS;
        
        /// <summary>
        /// Charisma - Merchant Buy/Sell Mob, Charm/Crowd Control Power
        /// </summary>
        protected int CHR;
        
        /// <summary>
        /// Magic Resist
        /// </summary>
        protected int MR;
        
        /// <summary>
        /// Cold Resist
        /// </summary>
        protected int CR;
        
        /// <summary>
        /// Fire Resist
        /// </summary>
        protected int FR;
        
        /// <summary>
        /// Disease Resist
        /// </summary>
        protected int DR;
        
        /// <summary>
        /// Poison Resist
        /// </summary>
        protected int PR;

        protected bool moving;
        protected float base_size;
        protected float size;
        protected float runspeed;
        protected float walkspeed;
        protected float fearspeed;
        
        public override bool IsMob()
        {
            return true;
        }

        public override bool Process()
        {
            throw new System.NotImplementedException();
        }

        public override bool Save()
        {
            throw new System.NotImplementedException();
        }
    }
}