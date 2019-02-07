using System.Numerics;

namespace Entities
{
    public abstract class IEntity
    {
        public abstract bool IsClient();
        public abstract bool IsNPC();
        public abstract bool IsMob();
        public abstract bool IsCorpse();
        public abstract bool IsPlayerCorpse();
        public abstract bool IsObject();
        public abstract bool IsDoor();
        public abstract bool IsTrap();
        public abstract bool Process();
        public abstract bool Save();
    }
}