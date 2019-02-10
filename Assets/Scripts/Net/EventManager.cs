using System;
using System.Collections.Generic;
using Server.Interface;

namespace Server.Network
{
    public static class EventManager
    {
        private static Dictionary<OP, Action> _event = new Dictionary<OP, Action>();
        private static Dictionary<OP, Action<long, IEntity>> _entityEvent = new Dictionary<OP, Action<long, IEntity>>();
        private static Dictionary<OP, Action<NetworkPacket>> _networkEvent = new Dictionary<OP, Action<NetworkPacket>>();
        
        #region Network Events
        public static void Subscribe(OP eventName, Action<NetworkPacket> listener)
        {
            Action<NetworkPacket> thisEvent;
            if (_networkEvent.TryGetValue(eventName, out thisEvent))
            {
                //Add more event to the existing one
                thisEvent += listener;

                //Update the Dictionary
                _networkEvent[eventName] = thisEvent;
            }
            else
            {
                //Add event to the Dictionary for the first time
                thisEvent += listener;
                _networkEvent.Add(eventName, thisEvent);
            }
        }

        public static void Unsubscribe(OP eventName, Action<NetworkPacket> listener)
        {
            Action<NetworkPacket> thisEvent;
            if (_networkEvent.TryGetValue(eventName, out thisEvent))
            {
                //Remove event from the existing one
                thisEvent -= listener;

                //Update the Dictionary
                _networkEvent[eventName] = thisEvent;
            }
        }

        public static void Publish(OP eventName, NetworkPacket eventParam)
        {
            Action<NetworkPacket> thisEvent = null;
            if (_networkEvent.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(eventParam);
                // OR USE  instance.eventDictionary[eventName](eventParam);
            }
        }
        #endregion
        
        #region Entity Events
        public static void Subscribe(OP eventName, Action<long, IEntity> listener)
        {
            Action<long, IEntity> thisEvent;
            if (_entityEvent.TryGetValue(eventName, out thisEvent))
            {
                //Add more event to the existing one
                thisEvent += listener;

                //Update the Dictionary
                _entityEvent[eventName] = thisEvent;
            }
            else
            {
                //Add event to the Dictionary for the first time
                thisEvent += listener;
                _entityEvent.Add(eventName, thisEvent);
            }
        }

        public static void Unsubscribe(OP eventName, Action<long, IEntity> listener)
        {
            Action<long, IEntity> thisEvent;
            if (_entityEvent.TryGetValue(eventName, out thisEvent))
            {
                //Remove event from the existing one
                thisEvent -= listener;

                //Update the Dictionary
                _entityEvent[eventName] = thisEvent;
            }
        }

        public static void Publish(OP eventName, long id, IEntity entity)
        {
            Action<long, IEntity> thisEvent = null;
            if (_entityEvent.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(id, entity);
                // OR USE  instance.eventDictionary[eventName](eventParam);
            }
        }
        #endregion
        
    }
}