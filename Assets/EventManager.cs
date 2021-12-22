using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public static class EventManager
{
    [Serializable]
    public delegate void CallBackMethod();

    [Serializable]
    public class Event
    {
        public int uniqueID;
        public string eventCode;
        public bool hasTriggered;
        public bool hasListeners;

        public CallBackMethod listeners;

        public Event(int uniqueID, string eventCode, CallBackMethod listener)
        {

            this.uniqueID = uniqueID;

            //Null-Checking
            if (string.IsNullOrEmpty(eventCode))
            {
                //There is no
                this.eventCode = "Unassigned";
            }
            else
            {
                this.eventCode = eventCode;
            }

            hasTriggered = false;
        }

        /// <summary>
        /// Return the uniqueId given to this event
        /// </summary>
        /// <returns></returns>
        public int GetUniqueID() => uniqueID;

        /// <summary>
        /// Return the eventCode given to this event
        /// </summary>
        /// <returns></returns>
        public string GetEventCode() => eventCode;

        public void AddNewListener(CallBackMethod listener, bool multicast = false)
        {
            if (listener == null)
            {
                listeners = new CallBackMethod(listener);
            }

            if (multicast)
                listeners += listener;
            else
                listeners = listener;

            HasListerners();
        }

        public void RemoveListener(CallBackMethod listener)
        {
            if (listeners != null)
                listeners -= listener;
        }

        /// <summary>
        /// Trigger this event, executing all listeners assigned to it.
        /// </summary>
        public void Trigger()
        {
            if (listeners != null)
            {
                hasTriggered = true;
                listeners.Invoke();
                return;
            }

            Debug.LogError("There are no listeners in this event...");
            return;
        }

        public void Trigger(bool discardAfter = false)
        {
            Trigger();
            if(discardAfter) RemoveEvent(this);
        }

        /// <summary>
        /// Set HasTriggered to false, as if it hasn't been triggered
        /// </summary>
        public void Reset()
        {
            if (hasTriggered)
                hasTriggered = false;
        }

        /// <summary>
        /// Returns if this even has been triggered
        /// </summary>
        public bool HasTriggered()
        {
            return hasTriggered;
        }


        public bool HasListerners()
        {
            hasListeners = (listeners.GetInvocationList().Length != 0);
            return hasListeners;
        }

        ~Event()
        {
            RemoveEvent(this);
        }
    }

    internal static void Empty()
    {
        Events.Clear();
    }

    //This associated an event with
    static List<Event> Events = new List<Event>();

    /// <summary>
    /// Add a new event with a uniqueID, name, and defined listeners
    /// </summary>
    /// <param name="uniqueID"></param>
    /// <param name="name"></param>
    /// <param name="listeners"></param>
    public static Event AddEvent(int uniqueID, string name, params CallBackMethod[] listeners)
    {
        
        Event newEvent = new Event(uniqueID, name, null);

        if (listeners.Length <= 1)
        {
            newEvent.AddNewListener(listeners[0]);
            Events.Add(newEvent);
            return newEvent;
        }
        else
        {

            foreach (CallBackMethod listener in listeners)
            {
                newEvent.AddNewListener(listener, true);
            }
            Events.Add(newEvent);
            return newEvent;
        }
    }

    /// <summary>
    /// Add a new event with a uniqueID, name, and defined listeners
    /// </summary>
    /// <param name="uniqueID"></param>
    /// <param name="name"></param>
    /// <param name="listeners"></param>
    public static Event AddEvent(Event newEvent)
    {
        return AddEvent(newEvent.uniqueID, newEvent.eventCode, newEvent.listeners);
    }

    /// <summary>
    /// Remove an event based on it's eventCode
    /// </summary>
    /// <param name="eventCode"></param>
    public static void RemoveEvent(string eventCode)
    {
        for (int idIndex = 0; idIndex < Events.Count; idIndex++)
        {
            //If we found the event with this eventCode, remove it
            if (eventCode == Events[idIndex].GetEventCode())
            {
                //Now delete the event itself
                Events.Remove(Events[idIndex]);
                return;
            }
        }
    }

    /// <summary>
    /// Remove an event based on it's uniqueID
    /// </summary>
    /// <param name="eventCode"></param>
    public static void RemoveEvent(int uniqueId)
    {
        for (int idIndex = 0; idIndex < Events.Count; idIndex++)
        {
            //If we found the event with this eventCode, remove it
            if (uniqueId.Equals(Events[idIndex].GetUniqueID()))
            {
                //Now delete the event itself
                Events.Remove(Events[idIndex]);
            }
        }
    }

    /// <summary>
    /// Remove an event 
    /// </summary>
    /// <param name="eventCode"></param>
    public static void RemoveEvent(Event @event)
    {
        try
        {
            for (int idIndex = 0; idIndex < Events.Count; idIndex++)
            {
                //If we found the event with this eventCode, remove it
                if (@event != null && @event.Equals(Events[idIndex]))
                {
                    //Now delete the event itself
                    Events.Remove(Events[idIndex]);
                }
            }
        }
        catch
        {
            return;
        }
    }

    /// <summary>
    /// Retuns all events of this event code
    /// </summary>
    /// <param name="eventCode"></param>
    /// <returns></returns>
    public static Event[] FindEventsOfEventCode(string eventCode)
    {
        List<Event> foundEvents = new List<Event>();
        for (int idIndex = 0; idIndex < Events.Count; idIndex++)
        {
            //If we found the event with this eventCode, remove it
            if (eventCode.Equals(Events[idIndex].GetEventCode()))
            {
                //Add it to our discorvered events
                foundEvents.Add(Events[idIndex]);
            }
        }

        //Return the foundEvents
        return foundEvents.ToArray();
    }

    /// <summary>
    /// Check if all events of this kind have been triggered
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    public static bool HaveAllTriggered(this Event[] events)
    {
        foreach (Event @event in events)
        {
            if (!@event.HasTriggered()) return false;
        }

        return true;
    }

    public static void TriggerEvent(int uniqueId)
    {
        for (int idIndex = 0; idIndex < Events.Count; idIndex++)
        {
            //If we found the event with this eventCode, remove it
            if (uniqueId.Equals(Events[idIndex].GetUniqueID()))
            {
                //Trigger events of this uniqueID
                Events[idIndex].Trigger();
            }
            else
            {
                Events[idIndex].hasTriggered = false;
            }
        }
    }

    public static void TriggerEvent(string eventCode)
    {
        for (int idIndex = 0; idIndex < Events.Count; idIndex++)
        {
            //If we found the event with this eventCode, remove it
            if (eventCode.Equals(Events[idIndex].GetEventCode()))
            {
                //Trigger events of this eventCode
                Events[idIndex].Trigger();
            }
            else
            {
                Events[idIndex].hasTriggered = false;
            }
        }
    }

    /// <summary>
    /// Returns all events of different IDs and EventCodes
    /// </summary>
    /// <returns></returns>
    public static Event[] GetAllEvents() => Events.ToArray();

    /// <summary>
    /// Will watch for a certain condition to be met before executing
    /// an event
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="if"></param>
    /// <param name="else"></param>
    /// <returns></returns>
    public static bool Watch(bool condition, Action @if, Action @else)
    {
        try
        {
            if (condition) @if.Invoke();
            else @else.Invoke();

            return condition;
        }
        catch { return false; }
    }

    /// <summary>
    /// Will watch for a certain condition to be met before executing
    /// an event
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="if"></param>
    /// <param name="results"></param>
    public static bool Watch(bool condition, Action @if, out bool result)
    {
        result = Watch(condition, @if, null);
        return result;
    }

    /// <summary>
    /// Will watch for a certain condition to be met before executing
    /// an event
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="if"></param>
    /// <param name="else"></param>
    /// <returns></returns>
    public static bool Watch(bool condition, Event @if, Event @else)
    {
        try
        {
            if (condition) @if.Trigger();
            else @else.Trigger();

            return condition;
        }
        catch { return false; }
    }

    /// <summary>
    /// Will watch for a certain condition to be met before executing
    /// an event
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="if"></param>
    /// <param name="results"></param>
    public static bool Watch(bool condition, Event @if, out bool results)
    {
        results = Watch(condition, @if, null);
        return results;
    }

    public static void DebugEventList()
    {
        foreach (Event @event in GetAllEvents())
        {
            Debug.Log($"Event{@event.uniqueID}: \"{@event.eventCode}\"; Listeners: {@event.listeners.GetInvocationList().Length}");
        }
    }

    public static int FreeID => GetAnyFreeID();
    static int GetAnyFreeID()
    {
        const int MIN_RANGE = 100;
        const int MAX_RANGE = 999;
        int _randomNum = Random.Range(MIN_RANGE, MAX_RANGE);
        try
        {
            foreach (Event evt in Events)
            {
                if (_randomNum == evt.uniqueID)
                {
                    return GetAnyFreeID();
                }
            }
        }
        catch
        {
            return _randomNum;
        }
        return 0;
    }
}
