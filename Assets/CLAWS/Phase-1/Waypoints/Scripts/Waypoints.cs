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
        
    }

     public void CreateWaypoint(string name, float x, float y, string letter, string type)
    {
        GameObject waypointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        waypointObject.transform.position = new Vector3(x, y, 0);

        Waypoint newWaypoint = new Waypoint(name, x, y, letter, type, waypointObject);
        waypoints.Add(newWaypoint);

        Debug.Log("Created Waypoint: " + name + " at (" + x + ", " + y + ")");
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
