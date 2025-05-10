using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.GraphicsTools.Editor;
using MixedReality.Toolkit.Examples.Demos;
using MixedReality.Toolkit.UX;
using SocketIOClient.Messages;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SetUpScreenController : MonoBehaviour
{
    [SerializeField] private GameObject TSSscreen;
    [SerializeField] private GameObject LMCCscreen;
    [SerializeField] private GameObject GreetingScreen;
    [SerializeField] private GameObject SetUpScreen;
    [SerializeField] private GameObject ConnectionScreen;
    [SerializeField] private GameObject LTVScreen;
    [SerializeField] private GameObject GPSScreen;
    [SerializeField] private GameObject Controller;
    [SerializeField] private GameObject connectionButton;
    [SerializeField] private GameObject doneButton;
    private GameObject Backplate;
    private GameObject LoadingBox;
    private GameObject connected;
    private GameObject disconnected;
    private bool connectedToTSS = false;
    private bool connectedToWEB = false;
    private bool isConnectingTSS = false;
    private bool isConnectingLMCC = false;
    private int connectionCount = 0;
    private bool hasCountedTSSConnection = false;
    private bool hasCountedLMCCConnection = false;
    private Subscription<RoverUpdatedEvent> roverUpdatedSubscription;
    private Subscription<RoverStatusUpdatedEvent> roverStatusUpdatedSubscription;


    void Start()
    {
        roverUpdatedSubscription = EventBus.Subscribe<RoverUpdatedEvent>(OnRoverUpdated);
        roverStatusUpdatedSubscription = EventBus.Subscribe<RoverStatusUpdatedEvent>(OnRoverStatusUpdated);
    }


    public void openSetUpScreen()
    {
        transform.gameObject.SetActive(true);
        TSSscreen.SetActive(false);
        LMCCscreen.SetActive(false);
        SetUpScreen.SetActive(true);
        ConnectionScreen.SetActive(false);
        GreetingScreen.SetActive(false);
    }

    
    public void EV1()
    {
        AstronautInstance.User.id = 1;
    }

    
    public void EV2()
    {
        AstronautInstance.User.id = 2;
    }


    public void red() {
        AstronautInstance.User.avatarColor = "red";
        // send this to other astronaut
    }
    public void blue() {
        AstronautInstance.User.avatarColor = "blue";
    }
    public void green() {
        AstronautInstance.User.avatarColor = "green";
    }
    public void yellow() {
        AstronautInstance.User.avatarColor = "yellow";
    }
    public void pink() {
        AstronautInstance.User.avatarColor = "pink";
    }
    public void orange() {
        AstronautInstance.User.avatarColor = "orange";
    }


    public void returnToConnectionScreen() {
        if (connectionCount == 2)
        {
            connectionButton.SetActive(false);
            doneButton.SetActive(true);
        }
        else
        {
            connectionButton.SetActive(true);
            doneButton.SetActive(false);
        }
        TSSscreen.SetActive(false);
        LMCCscreen.SetActive(false);
        ConnectionScreen.SetActive(true);
    }



 /////////////////////////////////////////////  LTV  ///////////////////////////////////////
    public void openLTV()
    {
        LTVScreen.SetActive(true);;
        TSSscreen.SetActive(false);
        LMCCscreen.SetActive(false);
        SetUpScreen.SetActive(false);
        ConnectionScreen.SetActive(false);
        GreetingScreen.SetActive(false);

        LoadingBox = LTVScreen.transform.Find("LoadingBox").gameObject;
        GameObject POIs = LTVScreen.transform.Find("POIs").gameObject;

        // Set initial states
        POIs.SetActive(false);
        LoadingBox.SetActive(true);
        Debug.Log("LTV screen opened");
         StartCoroutine(CheckLTVPing(POIs, LoadingBox));
    }


    private IEnumerator CheckLTVPing(GameObject POIs, GameObject LoadingBox)
    {
        while (true)
        {
            if (AstronautInstance.User.rover.rover.ping)
            {
                // If ping is true, show POIs and hide the loading box
                POIs.SetActive(true);
                LoadingBox.SetActive(false);
                Debug.Log("Ping successful, showing POIs.");
                yield break; // Exit the coroutine once the ping is successful
            }
            else
            {
                // If ping is false, keep showing the loading box
                POIs.SetActive(false);
                LoadingBox.SetActive(true);
                Debug.Log("Waiting for ping...");
            }
            yield return new WaitForSeconds(1f);
        }
    }


    private void OnRoverUpdated(RoverUpdatedEvent e)
    {
        GameObject POIs = LTVScreen.transform.Find("POIs").gameObject;
        POIs.transform.Find("POI_1").Find("Coords").GetComponent<TextMesh>().text = "[" + e.data.poi_1_x.ToString() + ", " + e.data.poi_1_y.ToString() + "]";
        POIs.transform.Find("POI_2").Find("Coords").GetComponent<TextMesh>().text = "[" + e.data.poi_2_x.ToString() + ", " + e.data.poi_2_y.ToString() + "]";
        POIs.transform.Find("POI_3").Find("Coords").GetComponent<TextMesh>().text = "[" + e.data.poi_3_x.ToString() + ", " + e.data.poi_3_y.ToString() + "]";
    }


    private void OnRoverStatusUpdated(RoverStatusUpdatedEvent e)
    {
        if (e.data)
        {
            LTVScreen.transform.Find("Confirmed").gameObject.SetActive(true);
            LTVScreen.transform.Find("NotConfirmed").gameObject.SetActive(false);
            Debug.Log("Rover is connected, and POIs are correct.");
        }
        else
        {
            LTVScreen.transform.Find("Confirmed").gameObject.SetActive(false);
            LTVScreen.transform.Find("NotConfirmed").gameObject.SetActive(true);
            Debug.Log("Rover is not connected, or POIs are incorrect.");
        }
    }


    public void sendToPR()
    {
        var data = new 
        {
            client = "pr_client",
            room = "LTV_POI",
            data = new
            {
                from = AstronautInstance.User.id,
                rover = new
                {
                    ping = true,
                    poi_1_x = AstronautInstance.User.rover.rover.poi_1_x,
                    poi_1_y = AstronautInstance.User.rover.rover.poi_1_y,
                    poi_2_x = AstronautInstance.User.rover.rover.poi_2_x,
                    poi_2_y = AstronautInstance.User.rover.rover.poi_2_y,
                    poi_3_x = AstronautInstance.User.rover.rover.poi_3_x,
                    poi_3_y = AstronautInstance.User.rover.rover.poi_3_y,
                }
            }
        };

        // Send the data to the server
        string jsonString = JsonUtility.ToJson(data);
        LMCCWebSocketClient webSocketClient = Controller.GetComponent<LMCCWebSocketClient>();
        if (webSocketClient != null)
        {
            webSocketClient.SendJsonData(jsonString, "LTV_POI", AstronautInstance.User.id);
            Debug.Log($"Sent JSON: {jsonString}");
        }
        else
        {
            Debug.LogError("LMCCWebSocketClient is not assigned to the Controller.");
        }

        // hide POIs and show loading box
        LTVScreen.transform.Find("POIs").gameObject.SetActive(false);
    }


     /////////////////////////////////////////////  GPS  ///////////////////////////////////////
     public void openGPS()
     {
        GPSScreen.SetActive(true);
        TSSscreen.SetActive(false);
        LMCCscreen.SetActive(false);
        SetUpScreen.SetActive(false);
        ConnectionScreen.SetActive(false);
        GreetingScreen.SetActive(false);
        LoadingBox = GPSScreen.transform.Find("LoadingBox").gameObject;
        GameObject mapCheck = GPSScreen.transform.Find("MapCheck").gameObject;
        LTVScreen.transform.Find("OnFail").gameObject.SetActive(false);
        mapCheck.SetActive(false);
        LoadingBox.SetActive(true);

        StartCoroutine(ShowMapCheckAfterDelay(mapCheck, LoadingBox));
     }
    private IEnumerator ShowMapCheckAfterDelay(GameObject mapCheck, GameObject loadingBox)
    {
        yield return new WaitForSeconds(3);
        loadingBox.SetActive(false);
        mapCheck.SetActive(true);
    }


    public void badGPS()
    {
        GPSScreen.transform.Find("OnFail").gameObject.SetActive(true);
        openAURA();
        StartCoroutine(ShowNotificationAfterDelay(GPSScreen.transform.Find("OnFail").gameObject));
    }
    private IEnumerator ShowNotificationAfterDelay(GameObject notification)
    {
        yield return new WaitForSeconds(3);
        notification.SetActive(false);
    }

[System.Serializable]
 private class JsonData
{
    public string client;
    public string room;
    public JsonDataDetails data;
}
[System.Serializable]
private class JsonDataDetails
{
    public string use;
    public string name;
    public string color;
}

    public void openAURA() {
        GameObject main = transform.parent.GetChild(2).gameObject;
        transform.gameObject.SetActive(false);
        main.SetActive(true);

        // check if user put in a name or color or id
        if (AstronautInstance.User.name == null)
        {
            AstronautInstance.User.name = "Neil Armstrong";
        }
        if (AstronautInstance.User.id == 0)
        {
            AstronautInstance.User.id = 1;
        }
        
        if (AstronautInstance.User.avatarColor == null)
        {
            AstronautInstance.User.avatarColor = "red";
        }
        
        JsonData jsonData = new JsonData
        {
            client = $"hololens_{AstronautInstance.User.id}",
            room = "EV",
            data = new JsonDataDetails
            {
                use = "INIT",
                name = AstronautInstance.User.name,
                color = AstronautInstance.User.avatarColor
            }
        };
        // Serialize the JSON object to a string
        string jsonString = JsonUtility.ToJson(jsonData);
        LMCCWebSocketClient webSocketClient = Controller.GetComponent<LMCCWebSocketClient>();
        if (webSocketClient != null)
        {
            webSocketClient.SendJsonData(jsonString, "EV", AstronautInstance.User.id);
            Debug.Log($"Sent JSON: {jsonString}");
        }
        else
        {
            Debug.LogError("LMCCWebSocketClient is not assigned to the Controller.");
        }
    }


    public void openConnectionScreen()
    {
        TSSscreen.SetActive(false);
        LMCCscreen.SetActive(false);
        SetUpScreen.SetActive(false);
        ConnectionScreen.SetActive(true);
        GreetingScreen.SetActive(false);
    }


    ////////////////////////////////  TSS  ///////////////////////////////////////
    public void openTSSscreen() 
    {
        TSSscreen.SetActive(true);
        Backplate = TSSscreen.transform.Find("UIBackplate").Find("UX.Slate.ContentBackplate").gameObject;
        Backplate.transform.localPosition = new Vector3(0.0313699991f, 0.0131000001f, 0);
        Backplate.transform.localScale = new Vector3(0.190743789f, 0.10200458f, 0.0199999996f);
        LoadingBox = TSSscreen.transform.Find("LoadingBox").gameObject;
        connected = TSSscreen.transform.Find("Connected").gameObject;
        disconnected = TSSscreen.transform.Find("Disconnected").gameObject;
        connected.SetActive(false);
        disconnected.SetActive(false);
        LMCCscreen.SetActive(false);
        SetUpScreen.SetActive(false);
        ConnectionScreen.SetActive(false);
        GreetingScreen.SetActive(false);
        
        StartCoroutine(ShowTSSLoadingBoxAndConnect());
    }


    private IEnumerator ShowTSSLoadingBoxAndConnect()
    {
        if (isConnectingTSS) yield break; // Prevent multiple instances
        isConnectingTSS = true;
        LoadingBox.SetActive(true);
        
        // Subscribe to the connection result event
        var mainConnections = Controller.GetComponent<MainConnections>();
        var tssConnection = mainConnections.tssConnection;
       tssConnection.OnTSSConnectionResult += HandleTSSConnectionResult;
        Debug.Log("Attempting to connect to TSS...");
        mainConnections.ConnectTSS(AstronautInstance.User.TSSurl);
        
        // Wait for either a connection result or a timeout of 10 seconds
        float timeout = 10f;
        float elapsedTime = 0f;
        while (isConnectingTSS && elapsedTime < timeout)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        
        if (isConnectingTSS)
        {
            Debug.LogWarning("TSS connection timed out.");
            HandleFailedTSSConnection();
        }
        LoadingBox.SetActive(false);
    }


    private void HandleTSSConnectionResult(bool success)
    {
        // unsubscribe now that result is determined
        Controller.GetComponent<MainConnections>().tssConnection.OnTSSConnectionResult -= HandleTSSConnectionResult;
        Debug.Log("TSS connection result received: " + success);
        
        if (success)
        {
            StartCoroutine(HandleSuccessfulTSSConnection());
        }
        else
        {
            HandleFailedTSSConnection();
        }
    }


    private IEnumerator HandleSuccessfulTSSConnection()
    {
        Debug.Log("TSS connection successful.");
        connectedToTSS = true;
        // Wait for 3 seconds before proceeding to simulate load
        yield return new WaitForSeconds(3);

        // Update Backplate dimensions
        Vector3 newPosition = Backplate.transform.localPosition;
        newPosition.y = -0.0027f;
        Backplate.transform.localPosition = newPosition;

        Vector3 newScale = Backplate.transform.localScale;
        newScale.y = 0.1334668f;
        Backplate.transform.localScale = newScale;

        connected.SetActive(true);
        disconnected.SetActive(false);
        if (!hasCountedTSSConnection)
        {
            connectionCount++;
            hasCountedTSSConnection = true;
            Debug.Log($"TSS connection counted. Total connections: {connectionCount}");
        }
        isConnectingTSS = false;
        ConnectionScreen.transform.Find("Menu/VisualRoot/ScaleRoot/Window/Buttons/ConnectToTSS").gameObject.GetComponent<PressableButton>().ForceSetToggled(true);;
    }


    private void HandleFailedTSSConnection()
    {
        Debug.LogWarning("TSS connection failed.");
        connectedToTSS = false;
        connected.SetActive(false);
        disconnected.SetActive(true);

        // Update Backplate dimensions
        Vector3 newPosition = Backplate.transform.localPosition;
        newPosition.y = -0.0234f;
        Backplate.transform.localPosition = newPosition;

        Vector3 newScale = Backplate.transform.localScale;
        newScale.y = 0.174935f;
        Backplate.transform.localScale = newScale;

        isConnectingTSS = false;
        ConnectionScreen.transform.Find("Menu/VisualRoot/ScaleRoot/Window/Buttons/ConnectToTSS").gameObject.GetComponent<PressableButton>().ForceSetToggled(false);
    }


    public bool tssConnectionCheck()
    {
        return connectedToTSS;
    }


    ////////////////////////////////  LMCC  ///////////////////////////////////////
    public void openLMCCscreen() 
    {
        LMCCscreen.SetActive(true);
        Backplate = LMCCscreen.transform.Find("UIBackplate").Find("UX.Slate.ContentBackplate").gameObject;
        Backplate.transform.localPosition = new Vector3(0.0313699991f, 0.0131000001f, 0);
        Backplate.transform.localScale = new Vector3(0.190743789f, 0.10200458f, 0.0199999996f);
        LoadingBox = LMCCscreen.transform.Find("LoadingBox").gameObject;
        connected = LMCCscreen.transform.Find("Connected").gameObject;
        disconnected = LMCCscreen.transform.Find("Disconnected").gameObject;
        connected.SetActive(false);
        disconnected.SetActive(false);
        TSSscreen.SetActive(false);
        SetUpScreen.SetActive(false);
        ConnectionScreen.SetActive(false);
        GreetingScreen.SetActive(false);
        Debug.Log("LMCC screen opened");
        StartCoroutine(ShowLMCCLoadingBoxAndConnect());
    }
    

    private IEnumerator ShowLMCCLoadingBoxAndConnect()
    {
        if (isConnectingLMCC) yield break; // Prevent multiple instances
        isConnectingLMCC = true;
        LoadingBox.SetActive(true);

        var mainConnections = Controller.GetComponent<MainConnections>();
        mainConnections.OnWebConnectionResult += HandleLMCCconnectionResult;
        Debug.Log("Attempting to connect to LMCC...");
        Controller.GetComponent<MainConnections>().ConnectLMCC(AstronautInstance.User.LMCCurl);
        
        float timeout = 10f;
        float elapsedTime = 0f;
        while (isConnectingLMCC && elapsedTime < timeout)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (isConnectingLMCC)
        {
            Debug.LogWarning("LMCC connection timed out.");
            HandleFailedLMCCConnection();
        }
        LoadingBox.SetActive(false);
    }


    private void HandleLMCCconnectionResult(bool success)
    {
        // unsubscribe now that result is determined
        Controller.GetComponent<MainConnections>().OnWebConnectionResult -= HandleLMCCconnectionResult;
        Debug.Log("LMCC connection result received: " + success);
        if (success)
        {
            StartCoroutine(HandleSuccessfulLMCCConnection());
        }
        else
        {
            HandleFailedLMCCConnection();
        }
    }


    private IEnumerator HandleSuccessfulLMCCConnection()
    {
        Debug.Log("LMCC connection successful.");
        connectedToWEB = true;
        // Wait for 3 seconds before proceeding to simulate load
        yield return new WaitForSeconds(3);

        // Update Backplate dimensions
        Vector3 newPosition = Backplate.transform.localPosition;
        newPosition.y = -0.0027f;
        Backplate.transform.localPosition = newPosition;

        Vector3 newScale = Backplate.transform.localScale;
        newScale.y = 0.1334668f;
        Backplate.transform.localScale = newScale;

        connected.SetActive(true);
        disconnected.SetActive(false);
        if (!hasCountedLMCCConnection)
        {
            connectionCount++;
            hasCountedLMCCConnection = true;
            Debug.Log($"LMCC connection counted. Total connections: {connectionCount}");
        }
        isConnectingLMCC = false;
        ConnectionScreen.transform.Find("Menu/VisualRoot/ScaleRoot/Window/Buttons/ConnectToLMCC").gameObject.GetComponent<PressableButton>().ForceSetToggled(true);
    }


    private void HandleFailedLMCCConnection()
    {
        Debug.LogWarning("LMCC connection failed.");
        connectedToWEB = false;
        connected.SetActive(false);
        disconnected.SetActive(true);

        // Update Backplate dimensions
        Vector3 newPosition = Backplate.transform.localPosition;
        newPosition.y = -0.02365f;
        Backplate.transform.localPosition = newPosition;

        Vector3 newScale = Backplate.transform.localScale;
        newScale.y = 0.1754949f;
        Backplate.transform.localScale = newScale;

        isConnectingLMCC = false;
        ConnectionScreen.transform.Find("Menu/VisualRoot/ScaleRoot/Window/Buttons/ConnectToLMCC").gameObject.GetComponent<PressableButton>().ForceSetToggled(false);
    }


    public bool lmccConnectionCheck()
    {
        return connectedToWEB;
    }

}
