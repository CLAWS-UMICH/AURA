using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    // Public list of waypoints, accessible from other scripts
    public List<Waypoint> WaypointsList { get; private set; }
    private WebSocketClient webSocketClient; //TODO: make a function to initalize this webSocket

    void Start()
    {
        WaypointsList = new List<Waypoint>();
        passInSocket();
        // Example usage
        Waypoint w1= CreateWaypoint("Start Point", 0, 0, 0, "A", "test", "testauthor");
        Waypoint w2 = CreateWaypoint("End Point", 10, 10, 0, "B", "test", "testauthor");

        // Edit the first waypoint by its ID
        EditWaypoint(0, 5, 5, 5, "C", "example"); 

        // Delete the second waypoint by its ID
        DeleteWaypoint(w2);

        // Subscribe to events
        EventBus.Subscribe<WaypointToAdd>(OnWaypointToAdd);
        EventBus.Subscribe<WaypointToDelete>(OnWaypointToDelete);
        EventBus.Subscribe<WaypointsEditedEvent>(OnWaypointsEditedEvent);
    }

    private void passInSocket(){
        //controller object
        GameObject controllerObject = GameObject.Find("Controller");
        if (controllerObject != null){
            webSocketClient = controllerObject.GetComponent<WebSocketClient>();
            if (webSocketClient != null){
                Debug.Log("Successfully connected to the existing WebSocketClient from Controller.");
            } else{
                Debug.LogWarning("WebSocketClient component not found on Controller.");
            }
        } else{
            Debug.LogError("Controller object not found in the scene.");
        }
      
   
    }

    private void OnWaypointToAdd(WaypointToAdd eventData){
    Waypoint waypointToAdd = eventData.waypointToAdd;
    WaypointsList.Add(waypointToAdd);
    Debug.Log($"Added Waypoint ID: {waypointToAdd.Id}, Name: {waypointToAdd.Name}");
    }

    private void OnWaypointToDelete(WaypointToDelete eventData)
    {
        Waypoint waypointToDelete = WaypointsList.Find(wp => wp.Id == eventData.Id);
        if (waypointToDelete != null)
        {
            DeleteWaypoint(waypointToDelete);
        }
        else
        {
            Debug.LogWarning($"Waypoint to delete with ID {eventData.Id} not found.");
        }
    }

    private void OnWaypointsEditedEvent(WaypointsEditedEvent eventData)
    {
        Waypoint editedWaypoint = eventData.EditedWaypoint;
        EditWaypoint(editedWaypoint.Id, editedWaypoint.X, editedWaypoint.Y, editedWaypoint.Z, editedWaypoint.Letter, editedWaypoint.Type);
    }

    public Waypoint CreateWaypoint(string name, float x, float y, float z, string letter, string type, string author)
    {
        // Generate the new ID as the current size of the list
        int newId = WaypointsList.Count;

        // Create the waypoint's GameObject
        GameObject waypointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        waypointObject.transform.position = new Vector3(x, y, z);

        // Create the new waypoint
        Waypoint newWaypoint = new Waypoint(newId, name, x, y, z, letter, type, author, waypointObject);
        WaypointsList.Add(newWaypoint);

        //send new Waypoint to EventBus
        string json = newWaypoint.getJsonString("POST");

        if (webSocketClient != null){
                webSocketClient.SendJsonData(json, "WAYPOINTS");
        }
        Debug.Log($"Created Waypoint ID: {newId}, Name: {name}, Letter: {letter}, Type: {type}, Position: ({x}, {y}, {z})");
        return newWaypoint;
    }

    public void DeleteWaypoint(Waypoint waypointToDelete)
    {
        int id = waypointToDelete.Id;
        Waypoint waypointToRemove = WaypointsList.Find(wp => wp.Id == id);
        if (waypointToRemove != null)
        {
            // Destroy the associated GameObject
            if (waypointToRemove.WaypointObject != null)
            {
                Destroy(waypointToRemove.WaypointObject);
            }

            // Remove the waypoint from the list
            WaypointsList.Remove(waypointToRemove);

            Debug.Log($"Deleted Waypoint ID: {id}");

            string json = waypointToRemove.getJsonString("DELETE");
            if(webSocketClient!= null){
                webSocketClient.SendJsonData(json, "WAYPOINTS");
            }

            // Update IDs of the remaining waypoints to match their new positions
            UpdateWaypointIds();
        }
        else
        {
            Debug.LogWarning($"Waypoint with ID {id} not found.");
        }
    }

    public Waypoint EditWaypoint(int id, float newX, float newY, float newZ, string newLetter, string newType)
    {
        Waypoint waypointToEdit = WaypointsList.Find(wp => wp.Id == id);
        if (waypointToEdit != null)
        {
            // Update properties of the waypoint
            waypointToEdit.X = newX;
            waypointToEdit.Y = newY;
            waypointToEdit.Z = newZ;
            waypointToEdit.Letter = newLetter;
            waypointToEdit.Type = newType;

            // Update the position of the associated GameObject
            if (waypointToEdit.WaypointObject != null)
            {
                waypointToEdit.WaypointObject.transform.position = new Vector3(newX, newY, newZ);
            }

            Debug.Log($"Edited Waypoint ID: {id}, New Position: ({newX}, {newY}, {newZ}), Letter: {newLetter}, Type: {newType}");

            string json = waypointToEdit.getJsonString("EDIT");
            if (webSocketClient!=null){
                webSocketClient.SendJsonData(json, "WAYPOINTS");
            }
        }
        else
        {
            Debug.LogWarning($"Waypoint with ID {id} not found.");
        }
        return waypointToEdit;
    }



    //who wrote this and does this work??
    private void UpdateWaypointIds()
    {
        for (int i = 0; i < WaypointsList.Count; i++)
        {
            WaypointsList[i].Id = i;
            Debug.Log($"Updated Waypoint ID to: {i}, Name: {WaypointsList[i].Name}");
        }
    }

    void Update()
    {

    }
}