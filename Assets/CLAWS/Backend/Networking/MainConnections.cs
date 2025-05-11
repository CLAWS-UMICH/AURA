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
    // public PRWebSocketClient prWebSocketClient;
    private bool websocketConnected;
    public Action<bool> OnWebConnectionResult;


    [Header("TSS Settings")]
    [SerializeField] private bool autoConnectTSS = false;
    public TSSConnection tssConnection;

    void Start()
    {
        websocketConnected = false;

        if (autoConnectTSS)
            ConnectTSS(AstronautInstance.User.TSSurl);

        if (autoConnectWebSocket)
            StartCoroutine(TryingConnectionToWebSocket(AstronautInstance.User.LMCCurl));
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

}
