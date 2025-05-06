using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using System;
using System.Threading.Tasks;
using PimDeWitte.UnityMainThreadDispatcher;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class PRWebSocketClient : MonoBehaviour
{
    private SocketIO client;
    private string serverUrl;

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

        // Emit "connect_pr" after connecting
        client.OnConnected += async (server, e) =>
        {
            Debug.Log("Connected to server, emitting 'connect_pr'");
            await client.EmitAsync("connect_pr");
        };


        // Listen for 'hololens_data' event to receive data
        client.On("pr_data", response =>
        {
            try
            {
                Debug.Log($"Raw response from WEB: {response}");

                JArray jsonArray = JArray.Parse(response.ToString()); // Now raw is already the correct string
                if (jsonArray.Count > 0)
                {
                    JObject outer = (JObject)jsonArray[0];
                    JObject data = (JObject)outer["data"];

                    Debug.Log($"Parsed data: {data}");

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        HandleJsonMessage(data.ToString())
                    );
                }
                else
                {
                    Debug.LogWarning("Empty response array.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing hololens_data response: {ex.Message}");
            }
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

            if (jsonObject["client"] == null) // client should alwaus be 'pr_client' since youre reeceiving data to the pr_client
            {
                Debug.LogError("Missing 'client' in the received JSON.");
                return; // Exit early if 'client' is missing
            }
            string client = (string)jsonObject["client"];

            if (jsonObject["room"] == null)
            {
                Debug.LogError("Missing 'room' in the received JSON.");
                return; // Exit early if 'room' is missing
            }
            string room = (string)jsonObject["room"];

            if (jsonObject["data"] == null)
            {
                Debug.LogError("Missing 'data' in the received JSON.");
                return; // Exit early if 'data' is missing
            }
            JObject data = (JObject)jsonObject["data"];

            // Handle different types based on the 'room' field
            switch (room)
            {
                
                case "UIA":
                    break;

                case "WAYPOINTS":
                    break;

                case "MESSAGING":
                    break;

                case "ALERTS":
                    break;

                case "SAMPLES":
                    break;

                case "TASKS":
                    break;

                default:
                    // Log if the 'type' is not recognized
                    Debug.LogWarning($"Unhandled 'type': {room}");
                    break;
            }
        }
        catch (JsonException ex)
        {
            // Catch any JSON parsing errors
            Debug.LogError($"Error parsing JSON: {ex.Message}");
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////  SENDING DATA ////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////
    [System.Serializable]
    public class Data
    {
        public string client; // Target client (e.g., "hololens_1", "hololens_2", "pr_client")
        public string room;   // Room name (e.g., "VITALS", "WAYPOINTS", "MESSAGES")
        public string data; // The message to send, dependent on what the room is
    }

    public async void SendJsonData(string message, string room, int clientId)
    {
        if (client != null)
        {
            // Determine the target client based on clientId
            string targetClient = clientId switch
            {
                1 => "hololens_1", // EV1
                2 => "hololens_2", // EV2
                3 => "pr_client",  // PR
                4 => "lmcc",       // LMCC
            };

            if (targetClient == null)
            {
                Debug.LogError("Invalid clientId provided.");
                return;
            }

            // Create the data object
            Data data = new Data
            {
                client = targetClient,
                room = "",
                data = message
            };

            // Serialize the data to JSON
            string jsonString = JsonUtility.ToJson(data);

            // Emit the appropriate event
            string eventName = clientId switch
            {
                3 => "send_to_pr",       // PR client
                4 => "send_to_room",     // LMCC client
                _ => "send_to_hololens"  // Default for hololens clients
            };

            await client.EmitAsync(eventName, jsonString);

            Debug.Log($"Sent message to {targetClient} in room '{room}': {jsonString}");
        }
        else
        {
            Debug.LogError("Socket client is not connected.");
        }
    }
}