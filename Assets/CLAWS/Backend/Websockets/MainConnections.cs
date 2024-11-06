using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class MainConnections : MonoBehaviour
{
    [SerializeField] private string webSocketUrl;
    [SerializeField] private bool autoConnectWebSocket = false;
    [SerializeField] private string tssUrl;
    [SerializeField] private bool autoConnectTSS = false;
    [SerializeField] private int id;

    private bool websocketConnected;
    private bool TSSConnected;

    void Start()
    {
        websocketConnected = false;
        TSSConnected = false;

        if (autoConnectTSS)
        {
            ConnectTSS(tssUrl);
            AstronautInstance.User.id = id;
        }

        if (autoConnectWebSocket)
        {
            StartCoroutine(_ConnectWebSocket(webSocketUrl));
        }
    }

    private async Task<bool> ConnectWebsocket(string connectionString)
    {
        WebSocketClient webSocketClient = transform.GetComponent<WebSocketClient>();
        return await webSocketClient.ReConnect(connectionString);
    }


    private void ConnectTSS(string url)
    {
        // Uncomment and implement TSS connection if needed.
        // transform.GetComponent<TSScConnection>().TSSConnect(url);
    }

    private IEnumerator _ConnectWebSocket(string connectionString)
    {
        while (!websocketConnected)
        {
            Task<bool> connectTask = ConnectWebsocket(connectionString);
            yield return new WaitUntil(() => connectTask.IsCompleted); // Wait for the async task to complete

            websocketConnected = connectTask.Result;

            if (!websocketConnected)
            {
                Debug.Log("WebSocket: Connection Failed. Trying again in 5 seconds.");
                yield return new WaitForSeconds(5f);
            }
            else
            {
                Debug.Log("WebSocket: Connection Successful");
            }
        }
    }

    public void ConnectToWebsocket(string connectionString)
    {
        if (!websocketConnected)
        {
            StartCoroutine(_ConnectWebSocket(connectionString));
        }
    }

    public void ConnectToTSS()
    {
        ConnectTSS(tssUrl);
    }

    public string getWebsocketURL()
    {
        return webSocketUrl;
    }
}
