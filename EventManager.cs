using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

// From https://unity3d.com/learn/tutorials/topics/scripting/events-creating-simple-messaging-system
public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> myEventDictionary;

    private static EventManager myEventManager;

    public static EventManager myInstance
    {
        get
        {
            if (!myEventManager)
            {
                myEventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!myEventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    myEventManager.Init();
                }
            }

            return myEventManager;
        }
    }

    void Init()
    {
        if (myEventDictionary == null)
        {
            myEventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (myInstance.myEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            myInstance.myEventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (myEventManager == null)
        {
            return;
        }

        UnityEvent thisEvent = null;
        if (myInstance.myEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (myInstance.myEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}