using System.Collections.Generic;
using MixedReality.Toolkit.SpatialManipulation;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class dynamicWaypointPop : MonoBehaviour
{
    NavigationController navigationController;
    public List<Transform> DangerClones = new List<Transform>();
    public List<Transform> GeoClones = new List<Transform>();
    public List<Transform> StationClones = new List<Transform>();
    public List<Transform> POIClones = new List<Transform>();
    private float initialYPosition = 0.0f; // Initial Y position for the first WAYPOINT
    private float yOffset = 0.02f; // Offset for the Y position of each subsequent WAYPOINT
    private float maxYPosition = 0.0f; // Maximum Y position for the WAYPOINTS


    public void AddWaypointToMenu(Waypoint waypoint)
    {
        switch(waypoint.Type)
        {
            case WaypointType.DANGER:
                
                // Instantiate the danger waypoint prefab
                //GameObject dangerWaypoint = Instantiate(navigationController.dangerButton, new Vector3(0, lastY + yOffset, 0), Quaternion.identity);
                break;
            case WaypointType.GEO:

                break;
            case WaypointType.STATION:
                break;

            case WaypointType.POI:
                break;
        }
    }
}