namespace Entities
{
    /// <summary>
    /// Definition for each specific spawn for for a mob in an instance.
    /// This separates multiple copies of the same mob spawning in different areas
    /// of the instance with different names. Provides NPC reusability without overlaping names/ids.
    /// </summary>
    public class Spawn
    {
        public string name;
        /// <summary>
        /// Ability to scale spawns difficulty up or down.
        /// </summary>
        public int level;
        
        private NPC npc;
    }
}