namespace Server.Network
{
    public enum OP
    {
        ClientConnect = 0x001,
        ClientDisconnect = 0x002,
        ClientGetPlayer = 0x003,
        
        EntityUpdate = 0x004,
        
        ServerChangeScene = 0x03E7,
        // 1000
        ServerSpawnEntity = 0x03E8,
        ServerRemoveEntity = 0x03E9,
        ServerUpdateEntity = 0x03EA
    }
}