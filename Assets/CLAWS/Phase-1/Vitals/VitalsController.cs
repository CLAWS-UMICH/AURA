using UnityEngine;

public class VitalsController : MonoBehaviour
{
    // Serialize the GameObject references to make them visible in the Inspector
    [SerializeField] private GameObject vitalsFirstAstronautScreen;
    [SerializeField] private GameObject vitalsSecondAstronautScreen;
    private Subscription<CloseEvent> closeSubscription;
    private Subscription<ScreenChangedEvent> screenChangedSubscription;

    private void Start() 
    {
        closeSubscription = EventBus.Subscribe<CloseEvent>(HandleCloseScreen);
        screenChangedSubscription = EventBus.Subscribe<ScreenChangedEvent>(HandleScreenChange);
        vitalsSecondAstronautScreen.SetActive(false);
    }

    public void ToggleVitalsScreen()
    {
        // Check the current screen, then switch accordingly
        if (StateMachine.Instance.CurrScreen == Screens.VitalsFirstAstronaut)
        {
            EventBus.Publish(new CloseEvent(Screens.VitalsFirstAstronaut));
            EventBus.Publish(new ScreenChangedEvent(Screens.VitalsSecondAstronaut));  // Switch to the second screen
        }
        else if (StateMachine.Instance.CurrScreen == Screens.VitalsSecondAstronaut)
        {
            EventBus.Publish(new CloseEvent(Screens.VitalsSecondAstronaut));
            EventBus.Publish(new ScreenChangedEvent(Screens.VitalsFirstAstronaut));  // Switch back to the first screen
        }
    }

    private void HandleScreenChange(ScreenChangedEvent e)
    {
        if (e.Screen == Screens.VitalsFirstAstronaut)
        {
            // Deactivate the screen
            vitalsFirstAstronautScreen.SetActive(true);
            vitalsSecondAstronautScreen.SetActive(false);
            Debug.Log("VitalsSecondAstronaut screen changed to VitalsFirstAstronaut.");
        }
        else if (e.Screen == Screens.VitalsSecondAstronaut)
        {
            vitalsSecondAstronautScreen.SetActive(true);
            vitalsFirstAstronautScreen.SetActive(false);
            Debug.Log("VitalFirstAstronaut screen changed to VitalsSecondAstronaut.");
        }
    }

    private void HandleCloseScreen(CloseEvent e)
    {
        // Check if the event's screen matches this screen's screen type
        if (e.Screen == Screens.VitalsFirstAstronaut)
        {
            // Deactivate the screen
            vitalsFirstAstronautScreen.SetActive(false);
            Debug.Log("VitalsFirstAstronaut screen closed.");
        }
        else if (e.Screen == Screens.VitalsSecondAstronaut)
        {
            vitalsSecondAstronautScreen.SetActive(false);
            Debug.Log("VitalsSecondAstronaut screen closed.");
        }
    }

    // Closes both Vitals screens
    public void CloseVitalScreen()
    {
        if (StateMachine.Instance.CurrScreen == Screens.VitalsFirstAstronaut)
        {
            // Deactivate the screen
            EventBus.Publish(new CloseEvent(Screens.VitalsFirstAstronaut));
            Debug.Log("VitalsFirstAstronaut screen closed.");
        }
        else if (StateMachine.Instance.CurrScreen == Screens.VitalsSecondAstronaut)
        {
            EventBus.Publish(new CloseEvent(Screens.VitalsSecondAstronaut));
            Debug.Log("VitalsSecondAstronaut screen closed.");
        }
    }

    private void OnDestroy() 
    {
        EventBus.Unsubscribe(closeSubscription);
        EventBus.Unsubscribe(screenChangedSubscription);
    }
}
