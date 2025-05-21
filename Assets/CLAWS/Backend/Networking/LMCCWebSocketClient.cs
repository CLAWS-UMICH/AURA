using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using System;
using System.Threading.Tasks;
using PimDeWitte.UnityMainThreadDispatcher;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEditor.Compilation;

public class LMCCWebSocketClient : MonoBehaviour
{
    private SocketIO client;
    private string serverUrl;
    private string assignedId; // Store the unique ID assigned by the server as a string

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
            try
            {
                Debug.Log($"Raw response from WEB: {response}");
                try
                {
                    JObject jsonObject = null;
                    try
                    {
                        jsonObject = JObject.Parse(response.ToString());
                        JObject data = (JObject)"{data}";
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        HandleJsonMessage(data.ToString()));
                    }
                    catch (JsonReaderException)
                    {
                        try
                        {
                            JArray jsonArray = JArray.Parse(response.ToString());
                            if (jsonArray.Count > 0)
                            {
                                JObject outer = (JObject)jsonArray[0];
                                JObject data = (JObject)outer["data"];
                                Debug.Log($"Parsed data: {data}");
                                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                                HandleJsonMessage(data.ToString()));
                            }
                            else
                            {
                                Debug.LogError("Received empty JSON array.");
                                return;
                            }
                        }
                        catch (JsonReaderException ex)
                        {
                            Debug.LogError($"Failed to parse JSON as JObject or JArray: {ex.Message}");
                            return;
                        }
                    }
                }
                catch (JsonException ex)
                {
                    Debug.LogError($"Error parsing JSON: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing hololens_data response: {ex.Message}");
            }
        });
        await client.ConnectAsync();
        Debug.Log("connected! " + client);
        Debug.Log("This instance: " + this);
    }

    private void OnDestroy()
    {
        Debug.Log("destroyed!");
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

            if (jsonObject["client"] == null)
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
            JToken dataToken = jsonObject["data"];
            JObject data;
            if (dataToken.Type == JTokenType.String)
            {
                // If data is a string, parse it again
                data = JObject.Parse((string)dataToken);
            }
            else
            {
                data = (JObject)dataToken;
            }
            // Handle different types based on the 'room' field
            switch (room)
            {
                case "VITALS":
                    Vitals vitalsData = data.ToObject<Vitals>();
                    if (assignedId == "1")
                    {
                        EventBus.Publish(new UpdatedVitalsEvent(vitalsData));
                    }
                    if (assignedId == "2")
                    {
                        EventBus.Publish(new UpdatedFellowAstronautVitalsEvent(vitalsData));
                    }
                    break;
                case "WAYPOINTS":
                    Waypoint waypointsData = data.ToObject<Waypoint>();
                    if ((string)data["use"] == "DELETE")
                    {
                        EventBus.Publish(new WaypointDeletedEvent(waypointsData));
                    }
                    else if ((string)data["use"] == "ADD")
                    {
                        EventBus.Publish(new WaypointAddedEvent(waypointsData));
                    }
                    break;
                case "MESSAGING":
                    Message newMessage = data.ToObject<Message>();
                    EventBus.Publish(new MessageSentEvent(newMessage));
                    EventBus.Publish(new MessagesAddedEvent(new List<Message> { newMessage }));
                    Debug.Log(newMessage + "SLAY");
                    break;
                case "EV":
                    if ((string)data["use"] == "INIT")
                    {
                        AstronautInstance.User.fellowAstronaut.name = (string)data["name"];
                        AstronautInstance.User.fellowAstronaut.color = (string)data["color"];
                    }
                    break;
                case "PR_VITALS":
                    PR_Vitals prVitalsData = data.ToObject<PR_Vitals>();
                    EventBus.Publish(new prUpdatedVitalsEvent(prVitalsData));
                    break;
                case "PR_LOCATION":
                    double Unity_posX = (float)data["posX"] - AstronautInstance.User.origin.posX;
                    double Unity_posZ = (float)data["posY"] - AstronautInstance.User.origin.posY;

                    Location currentPosition = new Location
                    {
                        posX = Unity_posX,
                        posY = 0.02,
                        posZ = Unity_posZ
                    };
                    EventBus.Publish(new PR_LocationUpdatedEvent(currentPosition));
                    break;
                case "LTV_POI":
                    Debug.Log(data["confirm"]);
                    if (data["confirm"]?.Value<bool>() == true)
                    {
                        Debug.Log("Confirmed LTV POI");
                        EventBus.Publish(new RoverStatusUpdatedEvent(true));
                    }
                    else
                    {
                        Debug.Log("Unconfirmed LTV POI");
                        EventBus.Publish(new RoverStatusUpdatedEvent(false));
                    }

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

    public async void SendJsonData(Dictionary <string, object> message, string room, int clientId)
    {
        Debug.Log("This instance: " + this);
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
                room = room,
                data = message
            };

            // Serialize the data to JSON
            string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
           
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