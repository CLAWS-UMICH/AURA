using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add new Screens to the Enum
[System.Serializable]
public enum Screens
{
    Menu,


}

public enum Modes
{
    Normal,
}

public class StateMachine : MonoBehaviour
{

    public static Screens CurrScreen = Screens.Menu;
    public static Modes CurrMode = Modes.Normal;
    private Subscription<ScreenChangedEvent> screenChangedSubscription; // Store the subscription
    private Subscription<ModeChangedEvent> modeChangedSubscription;

    private void Start()
    {
        screenChangedSubscription = EventBus.Subscribe<ScreenChangedEvent>(SwitchScreen);
        modeChangedSubscription = EventBus.Subscribe<ModeChangedEvent>(SwitchMode);
    }

    private void OnDestroy()
    {
        // Unsubscribe when the script is destroyed
        if (screenChangedSubscription != null)
        {
            EventBus.Unsubscribe(screenChangedSubscription);
        }
    }

    public void SwitchScreen(ScreenChangedEvent e)
    {
        Debug.Log(CurrScreen.ToString() + " -> " + e.Screen.ToString());
        CurrScreen = e.Screen;
    }

    public void SwitchMode(ModeChangedEvent e)
    {
        Debug.Log(CurrMode.ToString() + " -> " + e.Mode.ToString());
        CurrMode = e.Mode;
    }

    [ContextMenu("CloseScreen")]
    public void CloseScreen()
    {
        EventBus.Publish(new CloseEvent(CurrScreen));
    }

    [ContextMenu("CloseAll")]
    public void CloseAll()
    {
        /*EventBus.Publish(new CloseEvent(Screens.CreatingWaypoint));
        EventBus.Publish(new CloseEvent(Screens.Vitals_1));
        EventBus.Publish(new CloseEvent(Screens.Vitals_2));
        EventBus.Publish(new CloseEvent(Screens.Map_3D));
        EventBus.Publish(new CloseEvent(Screens.Map_2D));
        EventBus.Publish(new CloseEvent(Screens.Navigation));
        EventBus.Publish(new CloseEvent(Screens.Tasklist));
        CurrMode = Modes.Normal;
        CurrScreen = Screens.Menu;*/
    }
}

