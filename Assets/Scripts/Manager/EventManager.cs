using System;
using System.Collections;
using System.Collections.Generic;
using Net;
using UnityEngine;

public class EventManager
{
    private Dictionary<OP_CODE, Action<NetworkPacket>> _events;
    
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
            _events = new Dictionary<OP_CODE, Action<NetworkPacket>>();
        }
    }

    public void Subscribe(OP_CODE eventName, Action<NetworkPacket> listener)
    {
        Action<NetworkPacket> thisEvent;
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

    public void Unsubscribe(OP_CODE eventName, Action<NetworkPacket> listener)
    {
        if (eventManager == null) return;
        Action<NetworkPacket> thisEvent;
        if (instance._events.TryGetValue(eventName, out thisEvent))
        {
            //Remove event from the existing one
            thisEvent -= listener;

            //Update the Dictionary
            instance._events[eventName] = thisEvent;
        }
    }

    public void Publish(OP_CODE eventName, NetworkPacket eventParam)
    {
        Action<NetworkPacket> thisEvent = null;
        if (instance._events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(eventParam);
            // OR USE  instance.eventDictionary[eventName](eventParam);
        }
    }
}