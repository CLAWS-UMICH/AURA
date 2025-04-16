using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class MainConnections : MonoBehaviour
{
    [Header("WebSocket LMCC Settings")]
    [SerializeField] private bool autoConnectWebSocket = false;
    public LMCCWebSocketClient LMCCWebSocketClient;
    private bool websocketConnected;
    public Action<bool> OnWebConnectionResult;


    [Header("UDP Connection Controller")]
    public UDPConnectionController udpConnectionController;


    [Header("TSS Settings")]
    [SerializeField] private bool autoConnectTSS = false;
    public TSSConnection tssConnection;


    [Header("UDP PR Team Settings")]
    [SerializeField] private string prTeamIP = ""; // GET FROM START UP
    [SerializeField] private int prTeamPort = 0; // GET FROM START UP
    public PRConnection prConnection;


    void Start()
    {
        websocketConnected = false;
        udpConnectionController.OnPRDataReceived += HandlePRDataReceived;

        if (autoConnectTSS)
            ConnectTSS(AstronautInstance.User.TSSurl);

        if (autoConnectWebSocket)
            StartCoroutine(TryingConnectionToWebSocket(AstronautInstance.User.LMCCurl));
    }


    private void HandlePRDataReceived(string message)
    {
        Debug.Log($"[PR] Message Received: {message}");
        prConnection.UpdatePRMessage(message);
    }


    // called in setup
    public void ConnectTSS(string url)
    {
        Uri uri = new Uri(url);
        string host = uri.Host;
        tssConnection.TSSConnect(host);
    }


    // called in setup
    public void ConnectLMCC(string connectionString)
    {
        if (!websocketConnected)
        {
            Debug.Log("WebSocket: Attempting to connect...");
            StartCoroutine(TryingConnectionToWebSocket(connectionString));
        }
    }


    private IEnumerator TryingConnectionToWebSocket(string connectionString)
    {
        while (!websocketConnected)
        {
            Task<bool> connectTask = ConnectWebsocket(connectionString);
            yield return new WaitUntil(() => connectTask.IsCompleted); // Wait for the async task to complete

            websocketConnected = connectTask.Result;

            if (!websocketConnected)
            {
                OnWebConnectionResult?.Invoke(false);
                Debug.Log("WebSocket: Connection Failed");
            }
            else
            {
                OnWebConnectionResult?.Invoke(true);
                Debug.Log("WebSocket: Connection Successful");
            }
        }
    }


    // called by tryingConnection
    private async Task<bool> ConnectWebsocket(string connectionString)
    {
        if (LMCCWebSocketClient == null)
        {
            Debug.LogWarning("WebSocketClient component not assigned.");
            return false;
        }
        return await LMCCWebSocketClient.ReConnect(connectionString);
    }

    
    void OnDestroy()
    {
        if (udpConnectionController != null)
        {
            udpConnectionController.OnPRDataReceived -= HandlePRDataReceived;
        }
    }
}
