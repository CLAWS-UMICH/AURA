using System;
using System.Collections.Generic;
using UnityEngine;



// UIA EVENTS
public class UIAUpdatedEvent
{
    public UIA data { get; private set; }

    public UIAUpdatedEvent(UIA _data)
    {
        data = _data;
    }
}

[System.Serializable]
public class PR_Vitals
{
    public string oxygen_tank;
    public string battery_level;
}

public class prUpdatedVitalsEvent
{
    public PR_Vitals data { get; private set; }

    public prUpdatedVitalsEvent(PR_Vitals _data)
    {
        data = _data;
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

public class DCUChangedEvent
{
    public EvaDetails eva { get; private set; }
    public int id { get; private set; }

    public DCUChangedEvent(EvaDetails _e, int _id)
    {
        eva = _e;
        id = _id;
    }
}

public class DCUErrorEvent
{
    public ErrorMsg err { get; private set; }

    public DCUErrorEvent(ErrorMsg _e)
    {
        err = _e;
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


public class EV1_LocationUpdatedEvent
{
    public Location data { get; private set; }

    public EV1_LocationUpdatedEvent(Location _gps)
    {
        data = _gps;
    }
}


public class EV2_LocationUpdatedEvent
{
    public Location data { get; private set; }

    public EV2_LocationUpdatedEvent(Location _gps)
    {
        data = _gps;
    }
}


public class PR_LocationUpdatedEvent
{
    public Location data { get; private set; }

    public PR_LocationUpdatedEvent(Location _gps)
    {
        data = _gps;
    }
}


// GEOSAMPLE EVENTS
public class GeoSampleZoneAddedEvent
{
    public GeosampleZone newLTVLocation { get; private set; }

    public GeoSampleZoneAddedEvent(GeosampleZone _newLTVLocation)
    {
        newLTVLocation = _newLTVLocation;
    }
}


public class GeoSampleAddedToZoneEvent
{
    public GeoSample newGeoSample { get; private set; }

    public GeoSampleAddedToZoneEvent(GeoSample _newGeoSample)
    {
        newGeoSample = _newGeoSample;
    }
}


// MESSAGES EVENTS
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


public class RoverUpdatedEvent
{
    public RoverDetails data { get; private set; }

    public RoverUpdatedEvent(RoverDetails d)
    {
        data = d;
    }
}

public class RoverStatusUpdatedEvent
{
    public bool data { get; private set; }

    public RoverStatusUpdatedEvent(bool d)
    {
        data = d;
    }
}



