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

public class SelectButton
{
    public string letter { get; private set; }

    public SelectButton(string l)
    {
        letter = l;
    }
}

// Highlight Button
public class HighlightButton
{
    public GameObject button { get; private set; }

    public HighlightButton(GameObject _button)
    {
        button = _button;
    }
}

public class UnHighlight
{
    public List<string> levelnames { get; private set; }

    public UnHighlight(List<string> _levelnames)
    {
        levelnames = _levelnames;
    }
}

public class WebTestEvent
{
    public TestWebObj testData { get; set; }
    public string use { get; set; }

    public WebTestEvent(TestWebObj _testData, string _use)
    {
        testData = _testData;
        use = _use;
    }
}

public class TaskListEvent
{
    public TaskListObj testData { get; set; }
    public string use { get; set; }

    public TaskListEvent(TaskListObj _testData, string _use)
    {
        testData = _testData;
        use = _use;
    }
}

public class UpdatedVitalsEvent
{
    public Vitals vitals { get; private set; }

    public UpdatedVitalsEvent(Vitals v)
    {
        vitals = v;
    }
    public override string ToString()
    {
        return "<VitalsUpdatedEvent>: vitals were updated";
    }
}

public class FellowAstronautVitalsDataChangeEvent
{
    public Vitals vitals { get; private set; }

    public FellowAstronautVitalsDataChangeEvent(Vitals v)
    {
        vitals = v;
    }
}