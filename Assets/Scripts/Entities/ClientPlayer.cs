using NetcodeIO.NET;

namespace Entities
{
    public class ClientPlayer : Mob
    {
        private RemoteClient _client;
        
        public ClientPlayer(RemoteClient client)
        {
            _client = client;
        }
        
        public override bool IsClient()
        {
            return true;
        }
    }
}