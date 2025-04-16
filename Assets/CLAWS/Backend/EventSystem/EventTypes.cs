using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenChangedEvent
{
    public Screens Screen;

    public ScreenChangedEvent(Screens screen)
    {
        Screen = screen;
    }
}

public class ModeChangedEvent
{
    public Modes Mode;

    public ModeChangedEvent(Modes mode)
    {
        Mode = mode;
    }
}

public class CloseEvent
{
    public Screens Screen;

    public CloseEvent(Screens screen)
    {
        Screen = screen;
    }
}

public enum Direction { up, down }

public class ScrollEvent
{
    public Screens screen;
    public Direction direction;

    public ScrollEvent(Screens _screen, Direction _dir)
    {
        screen = _screen;
        direction = _dir;
        Debug.Log("Scrolling " + _screen.ToString() + " " + _dir.ToString());
    }

    public override string ToString()
    {
        return "<ScrollEvent>: " + screen.ToString() + " " + direction.ToString();
    }
}

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



public class TasklistEvent
{
    public TasklistObj taskdata { get; set; }
    public string use { get; set; }

    public TasklistEvent(TasklistObj _taskData, string _use)
    {
        taskdata = _taskData;
        use = _use;
    }
}

public class InitPopFinishedEvent
{
    public List<TaskObj> tl;
    public InitPopFinishedEvent(List<TaskObj> _tl)
    {
        tl = _tl;
    }
}

public class TaskFinishedEvent
{
    public int id;
    public int pid;
    public TaskFinishedEvent(int _id, int _pid)
    {
        id = _id;
        pid = _pid;
    }
}

public class ProgressBarUpdateEvent
{
    public int comp;
    public int total;
    public ProgressBarUpdateEvent(int _comp, int _total)
    {
        comp = _comp;
        total = _total;
    }
}


public class TaskDeletedEvent
{
    public int id;
    public TaskDeletedEvent(int _id)
    {
        id = _id;
    }
}

public class TaskEditedEvent
{
    public int id;
    public TaskObj data;
    public TaskEditedEvent(int _id, TaskObj _data)
    {
        id = _id;
        data = _data;
    }
}


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
public class WaypointsEditedEvent
{
    public Waypoint EditedWaypoint { get; private set; }

    public WaypointsEditedEvent(Waypoint _editedWaypoint)
    {
        EditedWaypoint = _editedWaypoint;
    }
}


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