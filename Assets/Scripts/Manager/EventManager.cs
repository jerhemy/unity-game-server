using System;
using System.Collections.Generic;
using Server.Interface;

namespace Server.Network
{
    public class EntityEventManager
    {
        private static Dictionary<string, Action<IEntity>> entityEvent = new Dictionary<string, Action<IEntity>>();
        
        public static void Subscribe(string eventName, Action<IEntity> listener)
        {
            Action<IEntity> thisEvent;
            if (entityEvent.TryGetValue(eventName, out thisEvent))
            {
                //Add more event to the existing one
                thisEvent += listener;

                //Update the Dictionary
                entityEvent[eventName] = thisEvent;
            }
            else
            {
                //Add event to the Dictionary for the first time
                thisEvent += listener;
                entityEvent.Add(eventName, thisEvent);
            }
        }

        public static void Unsubscribe(string eventName, Action<IEntity> listener)
        {
            Action<IEntity> thisEvent;
            if (entityEvent.TryGetValue(eventName, out thisEvent))
            {
                //Remove event from the existing one
                thisEvent -= listener;

                //Update the Dictionary
                entityEvent[eventName] = thisEvent;
            }
        }

        public static void Publish(string eventName, IEntity entity)
        {
            Action<IEntity> thisEvent = null;
            if (entityEvent.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(entity);
                // OR USE  instance.eventDictionary[eventName](eventParam);
            }
        }
    }
}