namespace Net
{
    public interface INetworkMessage
    {
        NetworkMessage GetType();
    }
    
    public enum NetworkMessage
    {
        ENITTY_UPDATE = 0x001
    }
}