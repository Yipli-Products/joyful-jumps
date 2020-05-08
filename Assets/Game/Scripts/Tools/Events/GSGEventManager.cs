//#define EVENTROUTER_THROWEXCEPTIONS 
#if EVENTROUTER_THROWEXCEPTIONS
//#define EVENTROUTER_REQUIRELISTENER // Uncomment this if you want listeners to be required for sending events.
#endif

using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace GodSpeedGames.Tools
{
    public struct GSGSfxEvent
    {
        public AudioClip ClipToPlay;
        public GSGSfxEvent(AudioClip clipToPlay)
        {
            ClipToPlay = clipToPlay;
        }
        static GSGSfxEvent e;
        public static void Trigger(AudioClip clipToPlay)
        {
            e.ClipToPlay = clipToPlay;
            GSGEventManager.TriggerEvent(e);
        }
    }

    /// <summary>
    /// This class handles event management, and can be used to broadcast events throughout the game, to tell one class (or many) that something's happened.
    /// Events are structs, you can define any kind of events you want. This manager comes with GSGGameEvents, which are 
    /// basically just made of a string, but you can work with more complex ones if you want.
    /// 
    /// To trigger a new event, from anywhere, do YOUR_EVENT.Trigger(YOUR_PARAMETERS)
    /// So GSGGameEvent.Trigger("Save"); for example will trigger a Save GSGGameEvent
    /// 
    /// you can also call GSGEventManager.TriggerEvent(YOUR_EVENT);
    /// For example : GSGEventManager.TriggerEvent(new GSGGameEvent("GameStart")); will broadcast an GSGGameEvent named GameStart to all listeners.
    ///
    /// To start listening to an event from any class, there are 3 things you must do : 
    ///
    /// 1 - tell that your class implements the GSGEventListener interface for that kind of event.
    /// For example: public class GUIManager : Singleton<GUIManager>, GSGEventListener<GSGGameEvent>
    /// You can have more than one of these (one per event type).
    ///
    /// 2 - On Enable and Disable, respectively start and stop listening to the event :
    /// void OnEnable()
    /// {
    /// 	this.GSGEventStartListening<GSGGameEvent>();
    /// }
    /// void OnDisable()
    /// {
    /// 	this.GSGEventStopListening<GSGGameEvent>();
    /// }
    /// 
    /// 3 - Implement the GSGEventListener interface for that event. For example :
    /// public void OnGSGEvent(GSGGameEvent gameEvent)
    /// {
    /// 	if (gameEvent.eventName == "GameOver")
    ///		{
    ///			// DO SOMETHING
    ///		}
    /// } 
    /// will catch all events of type GSGGameEvent emitted from anywhere in the game, and do something if it's named GameOver
    /// </summary>
    [ExecuteInEditMode]
    public static class GSGEventManager
    {
        private static Dictionary<Type, List<GSGEventListenerBase>> _subscribersList;

        static GSGEventManager()
        {
            _subscribersList = new Dictionary<Type, List<GSGEventListenerBase>>();
        }

        /// <summary>
        /// Adds a new subscriber to a certain event.
        /// </summary>
        /// <param name="listener">listener.</param>
        /// <typeparam name="GSGEvent">The event type.</typeparam>
        public static void AddListener<GSGEvent>(GSGEventListener<GSGEvent> listener) where GSGEvent : struct
        {
            Type eventType = typeof(GSGEvent);

            if (!_subscribersList.ContainsKey(eventType))
                _subscribersList[eventType] = new List<GSGEventListenerBase>();

            if (!SubscriptionExists(eventType, listener))
                _subscribersList[eventType].Add(listener);
        }

        /// <summary>
        /// Removes a subscriber from a certain event.
        /// </summary>
        /// <param name="listener">listener.</param>
        /// <typeparam name="GSGEvent">The event type.</typeparam>
        public static void RemoveListener<GSGEvent>(GSGEventListener<GSGEvent> listener) where GSGEvent : struct
        {
            Type eventType = typeof(GSGEvent);

            if (!_subscribersList.ContainsKey(eventType))
            {
#if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
#else
                return;
#endif
            }

            List<GSGEventListenerBase> subscriberList = _subscribersList[eventType];
            bool listenerFound;
            listenerFound = false;

            if (listenerFound)
            {

            }

            for (int i = 0; i < subscriberList.Count; i++)
            {
                if (subscriberList[i] == listener)
                {
                    subscriberList.Remove(subscriberList[i]);
                    listenerFound = true;

                    if (subscriberList.Count == 0)
                        _subscribersList.Remove(eventType);

                    return;
                }
            }

#if EVENTROUTER_THROWEXCEPTIONS
		        if( !listenerFound )
		        {
					throw new ArgumentException( string.Format( "Removing listener, but the supplied receiver isn't subscribed to event type \"{0}\".", eventType.ToString() ) );
		        }
#endif
        }

        public static void TriggerEvent<GSGEvent>(GSGEvent newEvent) where GSGEvent : struct
        {
            List<GSGEventListenerBase> list;
            if (!_subscribersList.TryGetValue(typeof(GSGEvent), out list))
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( GSGEvent ).ToString() ) );
#else
                return;
#endif

            for (int i = 0; i < list.Count; i++)
            {
                (list[i] as GSGEventListener<GSGEvent>).OnGSGEvent(newEvent);
            }
        }

        /// <summary>
        /// Checks if there are subscribers for a certain type of events
        /// </summary>
        /// <returns><c>true</c>, if exists was subscriptioned, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <param name="receiver">Receiver.</param>
        private static bool SubscriptionExists(Type type, GSGEventListenerBase receiver)
        {
            List<GSGEventListenerBase> receivers;

            if (!_subscribersList.TryGetValue(type, out receivers)) return false;

            bool exists = false;

            for (int i = 0; i < receivers.Count; i++)
            {
                if (receivers[i] == receiver)
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }
    }

    /// <summary>
    /// Static class that allows any class to start or stop listening to events
    /// </summary>
    public static class EventRegister
    {
        public delegate void Delegate<T>(T eventType);

        public static void GSGEventStartListening<EventType>(this GSGEventListener<EventType> caller) where EventType : struct
        {
            GSGEventManager.AddListener<EventType>(caller);
        }

        public static void GSGEventStopListening<EventType>(this GSGEventListener<EventType> caller) where EventType : struct
        {
            GSGEventManager.RemoveListener<EventType>(caller);
        }
    }

    /// <summary>
    /// Event listener basic interface
    /// </summary>
    public interface GSGEventListenerBase { };

    /// <summary>
    /// A public interface you'll need to implement for each type of event you want to listen to.
    /// </summary>
    public interface GSGEventListener<T> : GSGEventListenerBase
    {
        void OnGSGEvent(T eventType);
    }
}