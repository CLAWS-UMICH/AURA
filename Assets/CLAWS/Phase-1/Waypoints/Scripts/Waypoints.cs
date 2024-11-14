using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{   
    //list of waypoints
    private List<Waypoint> waypoints;

    // Start is called before the first frame update
    void Start()
    {
        waypoints = new List<Waypoint>();
          // Example usage
        CreateWaypoint("Start Point", 0, 0, 0, "A", "test");
        CreateWaypoint("End Point", 10, 10, 0, "A", "test");


    }

     public void CreateWaypoint(string name, float x, float y, float z, string letter, string type)
    {
        GameObject waypointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        waypointObject.transform.position = new Vector3(x, y, z);

        Waypoint newWaypoint = new Waypoint(name, x, y, z, letter, type, waypointObject);
        waypoints.Add(newWaypoint);

        Debug.Log("Created Waypoint: " + name + " letter: " + letter + " type: " + " at (" + x + ", " + y + ")");
    }

    public void DeleteWaypoint(string name)
    {
        Waypoint waypointToRemove = waypoints.Find(wp => wp.Name == name);
        if (waypointToRemove != null)
        {
            if (waypointToRemove.WaypointObject != null)
            {
                Destroy(waypointToRemove.WaypointObject);
            }
            waypoints.Remove(waypointToRemove);
            Debug.Log("Deleted Waypoint: " + name);
        }
        else
        {
            Debug.LogWarning("Waypoint not found: " + name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
