using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    // List of waypoints
    private List<Waypoint> waypoints;

    void Start()
    {
        waypoints = new List<Waypoint>();

        // Example usage
        CreateWaypoint("Start Point", 0, 0, 0, "A", "test");
        CreateWaypoint("End Point", 10, 10, 0, "B", "test");

        // Edit the first waypoint by its ID
        EditWaypoint(0, 5, 5, 5, "C", "example");

        // Delete the second waypoint by its ID
        DeleteWaypoint(1);
    }

    public void CreateWaypoint(string name, float x, float y, float z, string letter, string type)
    {
        // Generate the new ID as the current size of the list
        int newId = waypoints.Count;

        // Create the waypoint's GameObject
        GameObject waypointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        waypointObject.transform.position = new Vector3(x, y, z);

        // Create the new waypoint
        Waypoint newWaypoint = new Waypoint(newId, name, x, y, z, letter, type, waypointObject);
        waypoints.Add(newWaypoint);

        Debug.Log($"Created Waypoint ID: {newId}, Name: {name}, Letter: {letter}, Type: {type}, Position: ({x}, {y}, {z})");
    }

    public void DeleteWaypoint(int id)
    {
        Waypoint waypointToRemove = waypoints.Find(wp => wp.Id == id);
        if (waypointToRemove != null)
        {
            // Destroy the associated GameObject
            if (waypointToRemove.WaypointObject != null)
            {
                Destroy(waypointToRemove.WaypointObject);
            }

            // Remove the waypoint from the list
            waypoints.Remove(waypointToRemove);

            Debug.Log($"Deleted Waypoint ID: {id}");

            // Update IDs of the remaining waypoints to match their new positions
            UpdateWaypointIds();
        }
        else
        {
            Debug.LogWarning($"Waypoint with ID {id} not found.");
        }
    }

    public void EditWaypoint(int id, float newX, float newY, float newZ, string newLetter, string newType)
    {
        Waypoint waypointToEdit = waypoints.Find(wp => wp.Id == id);
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
        }
        else
        {
            Debug.LogWarning($"Waypoint with ID {id} not found.");
        }
    }

    private void UpdateWaypointIds()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            waypoints[i].Id = i;
            Debug.Log($"Updated Waypoint ID to: {i}, Name: {waypoints[i].Name}");
        }
    }

    void Update()
    {

    }
}