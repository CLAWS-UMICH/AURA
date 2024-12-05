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
        if (StateMachine.Instance.CurrScreen == Screens.VitalsFirstAstronaut)
        {
            EventBus.Publish(new CloseEvent(Screens.VitalsFirstAstronaut));
            EventBus.Publish(new ScreenChangedEvent(Screens.VitalsSecondAstronaut)); 
        }
        else if (StateMachine.Instance.CurrScreen == Screens.VitalsSecondAstronaut)
        {
            EventBus.Publish(new CloseEvent(Screens.VitalsSecondAstronaut));
            EventBus.Publish(new ScreenChangedEvent(Screens.VitalsFirstAstronaut));
        }
    }

    private void HandleScreenChange(ScreenChangedEvent e)
    {
        if (e.Screen == Screens.VitalsFirstAstronaut)
        {
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
        if (e.Screen == Screens.VitalsFirstAstronaut)
        {
            vitalsFirstAstronautScreen.SetActive(false);
            Debug.Log("VitalsFirstAstronaut screen closed.");
        }
        else if (e.Screen == Screens.VitalsSecondAstronaut)
        {
            vitalsSecondAstronautScreen.SetActive(false);
            Debug.Log("VitalsSecondAstronaut screen closed.");
        }
    }

    public void CloseVitalScreen()
    {
        if (StateMachine.Instance.CurrScreen == Screens.VitalsFirstAstronaut)
        {
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