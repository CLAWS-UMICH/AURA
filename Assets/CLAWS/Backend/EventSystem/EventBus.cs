using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus
{
    // Change this to "true" and all events will be logged to console automatically
    public const bool DEBUG_MODE = false;

    // Dictionary to hold subscribers for each event type
    private static Dictionary<Type, IList<ISubscription>> _topics = new Dictionary<Type, IList<ISubscription>>();

    // Publish an event to its subscribers
    public static void Publish<T>(T publishedEvent)
    {
        Type eventType = typeof(T);

        if (DEBUG_MODE)
            Debug.Log($"[Publish] Event of type {eventType} with contents ({publishedEvent.ToString()})");

        if (_topics.TryGetValue(eventType, out IList<ISubscription> subscriberList))
        {
            // Create a copy of the subscriber list to avoid issues during iteration
            var subscribers = new List<ISubscription>(subscriberList);

            // Iterate through subscribers and execute callbacks
            foreach (var subscription in subscribers)
            {
                // Check for null or destroyed target objects
                if (subscription.IsOrphaned)
                {
                    EventBus.Unsubscribe(subscription);
                }
                else
                {
                    subscription.Invoke(publishedEvent);
                }
            }
        }
        else
        {
            if (DEBUG_MODE)
                Debug.Log("...but no one is subscribed to this event right now.");
        }
    }

    // Subscribe to an event and return a subscription handle
    public static Subscription<T> Subscribe<T>(Action<T> callback)
    {
        Type eventType = typeof(T);
        Subscription<T> newSubscription = new Subscription<T>(callback);

        if (!_topics.TryGetValue(eventType, out IList<ISubscription> subscriberList))
        {
            // Create a new subscriber list for the event type
            subscriberList = new List<ISubscription>();
            _topics[eventType] = subscriberList;
        }

        subscriberList.Add(newSubscription);

        if (DEBUG_MODE)
            Debug.Log($"[Subscribe] Subscribed to event type {eventType}. There are now {subscriberList.Count} subscriptions to this type.");

        return newSubscription;
    }

    // Unsubscribe from an event
    public static void Unsubscribe(ISubscription subscription)
    {
        Type eventType = subscription.EventType;

        if (_topics.TryGetValue(eventType, out IList<ISubscription> subscriberList) && subscriberList.Count > 0)
        {
            subscriberList.Remove(subscription);

            if (DEBUG_MODE)
                Debug.Log($"[Unsubscribe] Unsubscribed from event type {eventType}. There are now {subscriberList.Count} subscriptions to this type.");
        }
        else
        {
            if (DEBUG_MODE)
                Debug.Log("...but this subscription is not currently valid (perhaps you already unsubscribed?)");
        }
    }
}

// The ISubscription interface defines the contract for a subscription
public interface ISubscription
{
    Type EventType { get; }
    bool IsOrphaned { get; }
    void Invoke(object eventObj);
}

// A handle type for subscriptions, automatically unsubscribes on destruction
public class Subscription<T> : ISubscription
{
    public Action<T> Callback { get; private set; }
    public bool IsOrphaned => Callback.Target == null || Callback.Target.Equals(null);

    public Type EventType => typeof(T);

    public Subscription(Action<T> callback)
    {
        Callback = callback;
    }

    // Automatically unsubscribe when the subscription is destroyed
    ~Subscription()
    {
        EventBus.Unsubscribe(this);
    }

    public void Invoke(object eventObj)
    {
        if (eventObj is T typedEvent)
        {
            Callback(typedEvent);
        }
        else
        {
            Debug.LogError($"Event object type mismatch for subscription of type {EventType}. Expected: {typeof(T)}, Received: {eventObj.GetType()}");
        }
    }
}
