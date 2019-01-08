using System.IO;
using UnityEngine.Networking;
using Net;

namespace Models
{
    public struct Movement
    {
        public float x;
        public float y;
        public float z;
        public float direction;

        public byte[] GetBytes()
        {
            using(MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(x);
                writer.Write(y);
                writer.Write(z);
                writer.Write(direction);

                return ms.ToArray();
            }
        }

        public static Movement fromBytes(BasePacket packet)
        {
            using(MemoryStream ms = new MemoryStream(packet.buffer))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                var movement = new Movement();
                movement.x = reader.ReadSingle();
                movement.y = reader.ReadSingle();
                movement.z = reader.ReadSingle();
                movement.direction = reader.ReadSingle();
                
                return movement;
            }
        }
    }
    
    
}