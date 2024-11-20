using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using System;
using System.Threading.Tasks;
using PimDeWitte.UnityMainThreadDispatcher;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
            Debug.Log($"Raw response from WEB: {response.ToString()}");

            JArray jsonArray = JArray.Parse(response.ToString());
            JObject firstObject = (JObject)jsonArray[0];
            JObject data = (JObject)firstObject["data"];
            UnityMainThreadDispatcher.Instance().Enqueue(() => HandleJsonMessage(data.ToString()));
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
        try
        {
            // Log the received data
            Debug.Log($"Received data: {jsonData}");

            // Parse the incoming JSON string into a JObject
            JObject jsonObject = JObject.Parse(jsonData);

            // Check if 'type' exists in the JSON
            if (jsonObject["type"] == null)
            {
                Debug.LogError("Missing 'type' in the received JSON.");
                return; // Exit early if 'type' is missing
            }

            string type = (string)jsonObject["type"];

            // Check if 'use' exists in the JSON
            if (jsonObject["use"] == null)
            {
                Debug.LogError("Missing 'use' in the received JSON.");
                return; // Exit early if 'use' is missing
            }

            string use = (string)jsonObject["use"];

            // Check if 'data' exists in the JSON
            if (jsonObject["data"] == null)
            {
                Debug.LogError("Missing 'data' in the received JSON.");
                return; // Exit early if 'data' is missing
            }

            JObject data = (JObject)jsonObject["data"];

            // Handle different types based on the 'type' field
            switch (type)
            {
                case "TEST":
                    /// Deserialize 'data' to the VitalsData class and publish to event
                    TestWebObj testData = data.ToObject<TestWebObj>();
                    EventBus.Publish(new WebTestEvent(testData, use));
                    break;
                case "TASKLIST":
                    TaskListObj taskData = data.ToObject<TaskListObj>(); 
                    EventBus.Publish(new TaskListEvent(taskData, use)); 
                    break;
                default:
                    // Log if the 'type' is not recognized
                    Debug.LogWarning($"Unhandled 'type': {type}");
                    break;
            }
        }
        catch (JsonException ex)
        {
            // Catch any JSON parsing errors
            Debug.LogError($"Error parsing JSON: {ex.Message}");
        }
    }

    [System.Serializable]
    public class MessageData
    {
        public string room;
        public string message;
    }

    // Message should be a json serialized class 
    // Room is who to Web you want this data to be sent to. Ex. VITALS, TASKLIST, etc.

    public async void SendJsonData(string message, string room)
    {
        if (client != null)
        {
            MessageData data = new MessageData { room = room.ToUpper(), message = message };
            string jsonString = JsonUtility.ToJson(data);
            await client.EmitAsync("send_to_room", jsonString);
        }
    }
}
