using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Screens
{
    Menu,
    VitalsFirstAstronaut,
    VitalsSecondAstronaut,
}

public enum Modes
{
    Normal,
}

public class StateMachine : MonoBehaviour
{
    // Singleton instance
    public static StateMachine Instance { get; private set; }

    public Screens CurrScreen = Screens.Menu;
    public Modes CurrMode = Modes.Normal;

    private Subscription<ScreenChangedEvent> screenChangedSubscription;
    private Subscription<ModeChangedEvent> modeChangedSubscription;

    private void Awake()
    {
        // Singleton pattern ensures there's only one instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Destroy if an instance already exists
        }

        // Ensure EventBus is initialized before subscribing
        InitializeEventSubscriptions();
    }

    private void InitializeEventSubscriptions()
    {
        // Subscribe to events
        screenChangedSubscription = EventBus.Subscribe<ScreenChangedEvent>(SwitchScreen);
        modeChangedSubscription = EventBus.Subscribe<ModeChangedEvent>(SwitchMode);
    }

    private void OnDestroy()
    {
        // Clean up subscriptions when the state machine is destroyed
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        // Unsubscribe to prevent memory leaks
        if (screenChangedSubscription != null)
        {
            EventBus.Unsubscribe(screenChangedSubscription);
        }
        if (modeChangedSubscription != null)
        {
            EventBus.Unsubscribe(modeChangedSubscription);
        }
    }

    // Switch screen when ScreenChangedEvent is published
    public void SwitchScreen(ScreenChangedEvent e)
    {
        Debug.Log($"{CurrScreen} -> {e.Screen}");
        CurrScreen = e.Screen;
    }

    // Switch mode when ModeChangedEvent is published
    public void SwitchMode(ModeChangedEvent e)
    {
        Debug.Log($"{CurrMode} -> {e.Mode}");
        CurrMode = e.Mode;
    }

    // Public method to close the current screen
    [ContextMenu("CloseScreen")]
    public void CloseScreen(CloseEvent e)
    {
        // Debug log for closing the screen
        Debug.Log("Closing screen: " + e.Screen.ToString());
    }

    // Public method to close all screens (you can extend this with more screens)
    [ContextMenu("CloseAll")]
    public void CloseAll()
    {
        // Publish CloseEvent for multiple screens
        EventBus.Publish(new CloseEvent(Screens.VitalsFirstAstronaut));
        EventBus.Publish(new CloseEvent(Screens.VitalsSecondAstronaut));

        // Reset to menu or default screen
        CurrScreen = Screens.Menu;
        EventBus.Publish(new ScreenChangedEvent(Screens.Menu));

        // Reset the mode if necessary
        CurrMode = Modes.Normal;
        EventBus.Publish(new ModeChangedEvent(Modes.Normal));
    }
}
