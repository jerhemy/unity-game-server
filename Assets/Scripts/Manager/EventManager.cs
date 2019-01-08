﻿using System;
using System.Collections;
using System.Collections.Generic;
using Net;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, Action<BasePacket>> _events;
    
    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        if (_events == null)
        {
            _events = new Dictionary<string, Action<BasePacket>>();
        }
    }

    public static void Subscribe(string eventName, Action<BasePacket> listener)
    {
        Action<BasePacket> thisEvent;
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

    public static void Unsubscribe(string eventName, Action<BasePacket> listener)
    {
        if (eventManager == null) return;
        Action<BasePacket> thisEvent;
        if (instance._events.TryGetValue(eventName, out thisEvent))
        {
            //Remove event from the existing one
            thisEvent -= listener;

            //Update the Dictionary
            instance._events[eventName] = thisEvent;
        }
    }

    public static void Publish(string eventName, BasePacket eventParam)
    {
        Action<BasePacket> thisEvent = null;
        if (instance._events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(eventParam);
            // OR USE  instance.eventDictionary[eventName](eventParam);
        }
    }
}

//Re-usable structure/ Can be a class to. Add all parameters you need inside it
public struct EventParam
{
    public string param1;
    public int param2;
    public float param3;
    public bool param4;
}