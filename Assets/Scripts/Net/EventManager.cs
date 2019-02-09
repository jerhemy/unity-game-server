using System;
using System.Collections.Generic;

namespace Server.Network
{
    public class EventManager
    {
        private Dictionary<OP, Action<long, NetworkPacket>> _events;

        private static EventManager eventManager;

        public static EventManager instance
        {
            get
            {
                if (eventManager != null)
                {
                    eventManager = new EventManager();
                }

                return eventManager;
            }
        }

        private EventManager()
        {
            if (_events == null)
            {
                _events = new Dictionary<OP, Action<long, NetworkPacket>>();
            }
        }

        public void Subscribe(OP eventName, Action<long, NetworkPacket> listener)
        {
            Action<long, NetworkPacket> thisEvent;
            if (instance._events.TryGetValue(eventName, out thisEvent))
            {
                //Add more event to the existing one
                thisEvent += listener;

                //Update the Dictionary
                instance._events[eventName] = thisEvent;
            }
            else
            {
                //Add event to the Dictionary for the first time
                thisEvent += listener;
                instance._events.Add(eventName, thisEvent);
            }
        }

        public void Unsubscribe(OP eventName, Action<long, NetworkPacket> listener)
        {
            if (eventManager == null) return;
            Action<long, NetworkPacket> thisEvent;
            if (instance._events.TryGetValue(eventName, out thisEvent))
            {
                //Remove event from the existing one
                thisEvent -= listener;

                //Update the Dictionary
                instance._events[eventName] = thisEvent;
            }
        }

        public void Publish(OP eventName, long id, NetworkPacket eventParam)
        {
            Action<long, NetworkPacket> thisEvent = null;
            if (instance._events.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(id, eventParam);
                // OR USE  instance.eventDictionary[eventName](eventParam);
            }
        }
    }
}