using Unity.VisualScripting;
using UnityEngine;

public class VitalsController : MonoBehaviour
{
    // Serialize the GameObject references to make them visible in the Inspector
    [SerializeField] private GameObject vitalsFirstAstronautScreen;
    [SerializeField] private GameObject vitalsSecondAstronautScreen;
    private WebSocketClient webSocketClient;
    private Subscription<CloseEvent> closeSubscription;
    private Subscription<ScreenChangedEvent> screenChangedSubscription;
    private Subscription<UpdatedVitalsEvent> updateVitalsEvent;
    private Subscription<FellowAstronautVitalsDataChangeEvent> updateFellowVitalsEvent;

    private void Start() 
    {
        InitializeWebSocket();
        closeSubscription = EventBus.Subscribe<CloseEvent>(HandleCloseScreen);
        screenChangedSubscription = EventBus.Subscribe<ScreenChangedEvent>(HandleScreenChange);
        updateVitalsEvent = EventBus.Subscribe<UpdatedVitalsEvent>(onVitalsUpdate);
        updateFellowVitalsEvent = EventBus.Subscribe<FellowAstronautVitalsDataChangeEvent>(onFellowVitalsUpdate);
        vitalsSecondAstronautScreen.SetActive(false);
    }


    private void InitializeWebSocket()
    {
        GameObject controllerObject = GameObject.Find("Controller");
        if (controllerObject != null){
            webSocketClient = controllerObject.GetComponent<WebSocketClient>();
            if (webSocketClient != null){
                Debug.Log("Successfully connected to the existing WebSocketClient from Controller.");
            } else{
                Debug.LogWarning("WebSocketClient component not found on Controller.");
            }
        } else{
            Debug.LogError("Controller object not found in the scene.");
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


    private void onVitalsUpdate(UpdatedVitalsEvent e)
    {
        //Instantiate UI gameObjects with data from TSS (that published event)

        // Send data recieved from TSS to Web (LMCC)
        VitalsData vitalsData = new VitalsData
            {
                type = "VITALS",
                use = "POST",
                data = AstronautInstance.User.VitalsData
            };
        string json = JsonUtility.ToJson(vitalsData);
        webSocketClient.SendJsonData(json, "VITALS");
        Debug.Log(json);
    }


    
    private void onFellowVitalsUpdate(FellowAstronautVitalsDataChangeEvent e)
    {
        //Instantiate UI gameObjects with data from TSS (that published event)

        // Send data recieved from TSS to Web (LMCC)
        VitalsData vitalsData = new VitalsData
            {
                type = "VITALS",
                use = "POST",
                data = AstronautInstance.User.VitalsData
            };
        string json = JsonUtility.ToJson(vitalsData);
        webSocketClient.SendJsonData(json, "VITALS");
        Debug.Log(json);
    }


    private void OnDestroy() 
    {
        EventBus.Unsubscribe(closeSubscription);
        EventBus.Unsubscribe(screenChangedSubscription);
        EventBus.Unsubscribe(updateVitalsEvent);
        EventBus.Unsubscribe(updateFellowVitalsEvent);
    }
}
