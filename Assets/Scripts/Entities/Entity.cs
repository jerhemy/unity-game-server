namespace Server.Entities
{
    public abstract class Entity
    {
        public string name;
        public float posX;
        public float posY;
        public float posZ;
        public float direction;
        
        public virtual bool IsClient() { return false; }
        public virtual bool IsNPC() { return false; }
        public virtual bool IsMob() { return false; }
        public virtual bool IsCorpse() { return false; }
        public virtual bool IsPlayerCorpse() { return false; }
        public virtual bool IsObject() { return false; }
        public virtual bool IsDoor() { return false; }
        public virtual bool IsTrap() { return false; }
            
        public abstract bool Process();
        public abstract bool Save();
    }
}