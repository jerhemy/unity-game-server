using UnityEngine;

namespace Models
{
    public class Mob
    {
        public GameObject gameObject;
        
        public int id;
        public string name;
        public Vector3 position;
        public Quaternion heading;

        private bool _isDead = false;
        
        private const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
           
        
        // Stats
        public int hp;
        
        public Mob(int id, bool randomize)
        {
            if (!randomize) return;
            hp = 100;
            this.id = id;
            name = randomeName();
            position = new Vector3(Random.Range(-100.0f, 100.0f), 0.0f, Random.Range(-100.0f, 100.0f));
            heading = Quaternion.Euler(0f, Random.Range(0, 90), 0f);
        }
        
        public Mob(int id, string name, float x, float y, float z, float heading)
        {
            this.id = id;
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
    }
}