using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    // Public list of waypoints, accessible from other scripts
    public List<Waypoint> WaypointsList { get; private set; }

    void Start()
    {
        WaypointsList = new List<Waypoint>();

        // Example usage
        CreateWaypoint("Start Point", 0, 0, 0, "A", "test");
        CreateWaypoint("End Point", 10, 10, 0, "B", "test");

        // Edit the first waypoint by its ID
        EditWaypoint(0, 5, 5, 5, "C", "example");
        /*
            JSON DATA FORMAT:
        {
            "type": "WAYPOINT", 
            "use": "<GET/POST/PUT/DELETE>",
            "data": {
                "id": <number>,
                "name": <string>,
                "location": <Location>,
                "type": <string>,
                "author": <string>
            }
        }
        */
        string json = @"{
            ""type"": ""WAYPOINT"",
            ""use"": ""PUT"",
            ""data"": {
                ""id"": 0,
                ""name"": ""Start Point"",
                ""location"": {
                    ""x"": 5,
                    ""y"": 5, 
                    ""z"": 5
                },
                ""type"": ""example"",
                ""author"": ""system""
            }
        }";
        Debug.Log(json);
        // Get reference to WebSocketClient instance
        WebSocketClient client = FindObjectOfType<WebSocketClient>();
        if (client != null) {
            client.SendJsonData(json, "WAYPOINTS");
        } else {
            Debug.LogError("WebSocketClient not found in scene");
        }

        // Delete the second waypoint by its ID
        DeleteWaypoint(1);
    }

    public void CreateWaypoint(string name, float x, float y, float z, string letter, string type)
    {
        int newId = WaypointsList.Count;

        GameObject waypointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        waypointObject.transform.position = new Vector3(x, y, z);

        Waypoint newWaypoint = new Waypoint(newId, name, x, y, z, letter, type, waypointObject);
        WaypointsList.Add(newWaypoint);

        Debug.Log($"Created Waypoint ID: {newId}, Name: {name}, Letter: {letter}, Type: {type}, Position: ({x}, {y}, {z})");
    }

    public void DeleteWaypoint(int id)
    {
        Waypoint waypointToRemove = WaypointsList.Find(wp => wp.Id == id);
        if (waypointToRemove != null)
        {
            if (waypointToRemove.WaypointObject != null)
            {
                Destroy(waypointToRemove.WaypointObject);
            }
            WaypointsList.Remove(waypointToRemove);

            Debug.Log($"Deleted Waypoint ID: {id}");
            UpdateWaypointIds();
        }
        else
        {
            Debug.LogWarning($"Waypoint with ID {id} not found.");
        }
    }

    public void EditWaypoint(int id, float newX, float newY, float newZ, string newLetter, string newType)
    {
        Waypoint waypointToEdit = WaypointsList.Find(wp => wp.Id == id);
        if (waypointToEdit != null)
        {
            waypointToEdit.X = newX;
            waypointToEdit.Y = newY;
            waypointToEdit.Z = newZ;
            waypointToEdit.Letter = newLetter;
            waypointToEdit.Type = newType;

            if (waypointToEdit.WaypointObject != null)
            {
                waypointToEdit.WaypointObject.transform.position = new Vector3(newX, newY, newZ);
            }

            Debug.Log($"Edited Waypoint ID: {id}, New Position: ({newX}, {newY}, {newZ}), Letter: {newLetter}, Type: {newType}");
        }
        else
        {
            Debug.LogWarning($"Waypoint with ID {id} not found.");
        }
    }

    private void UpdateWaypointIds()
    {
        for (int i = 0; i < WaypointsList.Count; i++)
        {
            WaypointsList[i].Id = i;
            Debug.Log($"Updated Waypoint ID to: {i}, Name: {WaypointsList[i].Name}");
        }
    }
}
