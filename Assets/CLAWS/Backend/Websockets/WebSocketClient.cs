using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using System;
using System.Threading.Tasks;
using PimDeWitte.UnityMainThreadDispatcher;
using Newtonsoft.Json.Linq;  // Import Newtonsoft.Json for JSON handling

public class WebSocketClient : MonoBehaviour
{
    private SocketIO client;
    private string serverUrl;
    private string assignedId; // Store the unique ID assigned by the server as a string

    private void Start()
    {
        // Initialize other components if needed
    }

    public async Task<bool> ReConnect(string connectionString)
    {
        try
        {
            serverUrl = connectionString;
            await InitializeSocket();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error occurred in ReConnect: " + ex.Message);
            return false;
        }
    }

    private async Task InitializeSocket()
    {
        if (client != null)
        {
            await client.DisconnectAsync();
        }

        client = new SocketIO(serverUrl);

        // Emit "connect_hololens" after connecting
        client.OnConnected += async (server, e) =>
        {
            Debug.Log("Connected to server, emitting 'connect_hololens'");
            await client.EmitAsync("connect_hololens");
        };

        // Listen for 'assign_id' event to receive the unique ID
        client.On("assign_id", response =>
        {

            // Parse the response (which is an array in this case)
            JArray jsonResponseArray = JArray.Parse(response.ToString());

            // Extract the 'id' value from the first item in the array
            if (jsonResponseArray.Count > 0)
            {
                JObject jsonResponse = (JObject)jsonResponseArray[0];
                assignedId = jsonResponse["id"]?.ToString();  // Get the "id" value
                Debug.Log($"Received ID from server: {assignedId}");
            }
            else
            {
                Debug.LogError("No valid 'id' received in the response");
            }
        });

        // Listen for 'hololens_data' event to receive data
        client.On("hololens_data", response =>
        {
            Debug.Log("test");
            Debug.Log($"Raw response from hololens_data: {response.ToString()}");
            JObject dataResponse = JObject.Parse(response.ToString());
            string data = dataResponse["data"]?.ToString(); // Extract the data payload
            UnityMainThreadDispatcher.Instance().Enqueue(() => HandleJsonMessage(data));
        });

        await client.ConnectAsync();
    }

    private void OnDestroy()
    {
        if (client != null)
        {
            client.DisconnectAsync();
        }
    }

    public void HandleJsonMessage(string jsonData)
    {
        Debug.Log($"Received data for {assignedId}: {jsonData}");
        // Handle the JSON data here as needed
    }

    public async void SendJsonData(string jsonData)
    {
        if (client != null)
        {
            await client.EmitAsync("message", jsonData);
        }
    }
}
