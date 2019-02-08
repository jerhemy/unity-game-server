namespace Net
{
    public enum OP
    {
        ClientConnect = 0x001,
        ClientDisconnect = 0x002,
        ClientGetPlayer = 0x003,
          
        // 1000
        ServerAddEntity = 0x03E8,
        ServerRemoveEntity = 0x03E9,
        ServerUpdateEntity = 0x03EA
    }
}