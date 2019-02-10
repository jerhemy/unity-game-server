using System.Collections.Generic;
using NetcodeIO.NET;

namespace Server.Entities
{
    public class ClientPlayer : Mob
    {
        private RemoteClient _client;
        private List<long> EntityList;
        
        public ClientPlayer(RemoteClient client)
        {
            EntityList = new List<long>();
            _client = client;
        }
        
        public override bool IsClient()
        {
            return true;
        }
    }
}