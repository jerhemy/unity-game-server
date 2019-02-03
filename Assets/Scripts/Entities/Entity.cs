using NetcodeIO.NET;

namespace Entities
{
    public class Entity
    {
        public virtual bool IsClient() { return false; }
        public virtual bool IsNPC() { return false; }
        public virtual bool IsMob() { return false; }
        public virtual bool IsCorpse() { return false; }
        public virtual bool IsPlayerCorpse() { return false; }
        public virtual bool IsObject() { return false; }
        public virtual bool IsDoor() { return false; }
        public virtual bool IsTrap() { return false; }
        
        public virtual bool Process() { return false; }
        public virtual bool Save() { return false; }
    }
}