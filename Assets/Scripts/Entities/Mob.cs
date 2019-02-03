namespace Entities
{
    public class Mob : Entity
    {
        protected int AC;
        
        protected int ATK;
        /// <summary>
        /// Strength
        /// </summary>
        protected int STR;
        /// <summary>
        /// Constitution
        /// Effects:
        /// Melee ATK Bonus
        /// HP Max
        /// HP Regen
        /// Stamina Max
        /// Stamina Regen
        /// Melee Damage Bonus
        /// </summary>
        protected int CON;
        /// <summary>
        /// Dexterity
        /// Effects:
        /// Ranged Damage Bonus
        /// Ranged ATK Bonux
        /// </summary>
        protected int DEX;
        /// <summary>
        /// Agility
        /// Effects:
        /// AC Bonus
        /// Dodge
        /// 
        /// </summary>
        protected int AGI;
        /// <summary>
        /// Intelligence
        /// Effects:
        /// Pure Caster MP Max
        /// Pure Caster
        /// </summary>
        protected int INT;
        protected int WIS;
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
    }
}