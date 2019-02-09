using System;

namespace Utils
{
    public class Identity
    {
        public static long GetID()
        {
            return DateTime.UtcNow.Ticks;
        }
    }
}