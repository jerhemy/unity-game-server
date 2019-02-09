using System;
using System.Collections.Generic;

namespace Server.Network
{
    public class EventManager
    {
        private static Dictionary<OP, Action<long, NetworkPacket>> _events = new Dictionary<OP, Action<long, NetworkPacket>>();
        
        public static void Subscribe(OP eventName, Action<long, NetworkPacket> listener)
        {
            Action<long, NetworkPacket> thisEvent;
            if (_events.TryGetValue(eventName, out thisEvent))
            {
                //Add more event to the existing one
                thisEvent += listener;

                //Update the Dictionary
                _events[eventName] = thisEvent;
            }
            else
            {
                //Add event to the Dictionary for the first time
                thisEvent += listener;
                _events.Add(eventName, thisEvent);
            }
        }

        public static void Unsubscribe(OP eventName, Action<long, NetworkPacket> listener)
        {
            Action<long, NetworkPacket> thisEvent;
            if (_events.TryGetValue(eventName, out thisEvent))
            {
                //Remove event from the existing one
                thisEvent -= listener;

                //Update the Dictionary
                _events[eventName] = thisEvent;
            }
        }

        public static void Publish(OP eventName, long id, NetworkPacket eventParam)
        {
            Action<long, NetworkPacket> thisEvent = null;
            if (_events.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(id, eventParam);
                // OR USE  instance.eventDictionary[eventName](eventParam);
            }
        }
    }
}