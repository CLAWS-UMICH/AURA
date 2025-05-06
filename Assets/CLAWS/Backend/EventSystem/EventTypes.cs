using System;
using System.Collections.Generic;
using UnityEngine;




// Event for letting us know GPS data was received from the server
public class UpdatedGPSEvent
{
    public UpdatedGPSEvent()
    {
        Debug.Log("GPS update event created");
    }

    public override string ToString()
    {
        return "<UpdatedGPSEvent>: new GPS msg";
    }
}

public class UpdatedGPSOriginEvent
{
    public UpdatedGPSOriginEvent()
    {
        Debug.Log("GPS origin updated");
    }

    public override string ToString()
    {
        return "<UpdatedGPSOriginEvent>: new GPS origin";
    }
}


// VITALS EVENTS
public class UpdatedVitalsEvent
{
    public Vitals vitals { get; private set; }

    public UpdatedVitalsEvent(Vitals _v)
    {
        vitals = _v;
    }
}
public class UpdatedFellowAstronautVitalsEvent
{
    public Vitals vitals { get; private set; }

    public UpdatedFellowAstronautVitalsEvent(Vitals _v)
    {
        vitals = _v;
    }
}


// WAYPOINT EVENTS
public class WaypointDeletedEvent
{
    public Waypoint DeletedWaypoint { get; private set; }

    public WaypointDeletedEvent(Waypoint _deletedWaypoint)
    {
        DeletedWaypoint = _deletedWaypoint;
    }
}
public class WaypointAddedEvent
{
    public Waypoint NewAddedWaypoint{ get; private set; }

    public WaypointAddedEvent(Waypoint _waypoint)
    {
        NewAddedWaypoint = _waypoint;
    }
}
public class GPSUpdatedEvent
{
    public loc NewGPS { get; private set; }

    public GPSUpdatedEvent(GPS _gps)
    {
        NewGPS = _gps;
    }
}


public class MessagesAddedEvent
{
    public List<Message> NewAddedMessages { get; private set; }

    public MessagesAddedEvent(List<Message> _newAddedMessages)
    {
        NewAddedMessages = _newAddedMessages;
    }
}


public class MessagesAppendedEvent 
{   public MessagesAppendedEvent() {}   }


public class MessageSentEvent
{
    public Message NewMadeMessage { get; private set; }

    public MessageSentEvent(Message _newMadeMessage)
    {
        NewMadeMessage = _newMadeMessage;
    }
}


public class MessageReactionEvent
{
    public Message NewReactionMessage { get; private set; }
    public MessageReactionEvent(Message _newRactionMessage)
    {
        NewReactionMessage = _newRactionMessage;
    }
}