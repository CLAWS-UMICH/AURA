using System.Collections;
using System.Collections.Generic;
using SocketIOClient.Messages;
using UnityEngine;

public class SetUpScreenController : MonoBehaviour
{
    [SerializeField] private GameObject TSSscreen;
    [SerializeField] private GameObject LMCCscreen;
    [SerializeField] private GameObject PRScreen;
    [SerializeField] private GameObject GreetingScreen;
    [SerializeField] private GameObject SetUpScreen;
    [SerializeField] private GameObject ConnectionScreen;
    [SerializeField] private GameObject Controller;
    private GameObject Backplate;
    private GameObject LoadingBox;
    private GameObject connected;
    private GameObject disconnected;
    private bool connectedToTSS = false;
    private bool connectedToWEB = false;
    private bool connectedToPR = false;
    private bool isConnectingTSS = false;
    private bool isConnectingLMCC = false;
    private bool isConnectingPR = false;



    public void openSetUpScreen()
    {
        TSSscreen.SetActive(false);
        LMCCscreen.SetActive(false);
        PRScreen.SetActive(false);
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
        TSSscreen.SetActive(false);
        LMCCscreen.SetActive(false);
        PRScreen.SetActive(false);
        ConnectionScreen.SetActive(true);
    }


    public void openAURA() {
        GameObject main = transform.parent.GetChild(2).gameObject;
        transform.gameObject.SetActive(false);
        main.SetActive(true);
    }


    public void openConnectionScreen()
    {
        TSSscreen.SetActive(false);
        LMCCscreen.SetActive(false);
        PRScreen.SetActive(false);
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
        PRScreen.SetActive(false);
        SetUpScreen.SetActive(false);
        ConnectionScreen.SetActive(false);
        GreetingScreen.SetActive(false);
        
        StartCoroutine(ShowTSSLoadingBoxAndConnect());
    }

    private IEnumerator ShowTSSLoadingBoxAndConnect()
    {
        LoadingBox.SetActive(true);
        yield return new WaitForSeconds(3);
        LoadingBox.SetActive(false);
        if (tssConnectionCheck()) 
        {
            connected.SetActive(true);
            disconnected.SetActive(false);
            // new backplate dimensions
            Vector3 newPosition = Backplate.transform.localPosition;
            newPosition.y = -0.0027f;
            Backplate.transform.localPosition = newPosition;
            Vector3 newScale = Backplate.transform.localScale;
            newScale.y = 0.1334668f;
            Backplate.transform.localScale = newScale;
            yield break;
        }

        // Subscribe to the connection result event
        var mainConnections = Controller.GetComponent<MainConnections>();
        mainConnections.tssConnection.OnTSSConnectionResult += HandleTSSConnectionResult;

        Controller.GetComponent<MainConnections>().ConnectTSS(AstronautInstance.User.TSSurl);
    }

    private void HandleTSSConnectionResult(bool success)
    {
        // unsubscribe now that result is determined
        Controller.GetComponent<MainConnections>().tssConnection.OnTSSConnectionResult -= HandleTSSConnectionResult;
        Debug.Log("TSS connection result received: " + success);
        if (success)
        {
            Debug.Log("entered");
            connectedToTSS = true;
            connected.SetActive(true);
            disconnected.SetActive(false);
            // new backplate dimensions
            Vector3 newPosition = Backplate.transform.localPosition;
            newPosition.y = -0.0027f;
            Backplate.transform.localPosition = newPosition;
            Vector3 newScale = Backplate.transform.localScale;
            newScale.y = 0.1334668f;
            Backplate.transform.localScale = newScale;
            Debug.Log("TSS connection successful.");
        }
        else
        {
            connectedToTSS = false;
            connected.SetActive(false);
            disconnected.SetActive(true);
            // new backplate dimensions
            Vector3 newPosition = Backplate.transform.localPosition;
            newPosition.y = -0.0234f;
            Backplate.transform.localPosition = newPosition;
            Vector3 newScale = Backplate.transform.localScale;
            newScale.y = 0.174935f;
            Backplate.transform.localScale = newScale;
            Debug.LogWarning("TSS connection failed.");
        }
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
        PRScreen.SetActive(false);
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
        yield return new WaitForSeconds(3);
        LoadingBox.SetActive(false);

        if (lmccConnectionCheck())
        {
            connected.SetActive(true);
            disconnected.SetActive(false);

            // Update Backplate dimensions
            Vector3 newPosition = Backplate.transform.localPosition;
            newPosition.y = -0.0027f;
            Backplate.transform.localPosition = newPosition;

            Vector3 newScale = Backplate.transform.localScale;
            newScale.y = 0.1334668f;
            Backplate.transform.localScale = newScale;

            isConnectingLMCC = false;
            yield break;
        }

        // Subscribe to the connection result event
        var mainConnections = Controller.GetComponent<MainConnections>();
        mainConnections.OnWebConnectionResult += HandleLMCCconnectionResult;

        Debug.Log("Attempting to connect to LMCC...");
        Controller.GetComponent<MainConnections>().ConnectLMCC(AstronautInstance.User.LMCCurl);

        // Wait until the connection attempt is complete
        yield return new WaitUntil(() => !isConnectingLMCC);

        isConnectingLMCC = false;
    }

    private void HandleLMCCconnectionResult(bool success)
    {
        // unsubscribe now that result is determined
        Controller.GetComponent<MainConnections>().OnWebConnectionResult -= HandleLMCCconnectionResult;
        Debug.Log("LMCC connection result received: " + success);
        if (success)
        {
            connectedToWEB = true;
            connected.SetActive(true);
            disconnected.SetActive(false);
            // new backplate dimensions
            Vector3 newPosition = Backplate.transform.localPosition;
            newPosition.y = -0.0027f;
            Backplate.transform.localPosition = newPosition;
            Vector3 newScale = Backplate.transform.localScale;
            newScale.y = 0.1334668f;
            Backplate.transform.localScale = newScale;
            Debug.Log("LMCC connection successful.");
        }
        else
        {
            connectedToWEB = false;
            connected.SetActive(false);
            disconnected.SetActive(true);
            // new backplate dimensions
            Vector3 newPosition = Backplate.transform.localPosition;
            newPosition.y = -0.02365f;
            Backplate.transform.localPosition = newPosition;
            Vector3 newScale = Backplate.transform.localScale;
            newScale.y = 0.1754949f;
            Backplate.transform.localScale = newScale;
            Debug.Log("LMCC connection failed.");
        }
    }

    public bool lmccConnectionCheck()
    {
        return connectedToWEB;
    }

// TODO: PR 

}
