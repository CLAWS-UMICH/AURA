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
                // get last danger clone y position
                float lastY = DangerClones.Count > 0 ? DangerClones[DangerClones.Count - 1].localPosition.y : initialYPosition;
                if (lastY > (maxYPosition))
                {
                    // move the clones up by offset
                    for (int i = 0; i < DangerClones.Count; i++)
                    {
                        Vector3 newPos = DangerClones[i].localPosition;
                        newPos.y += yOffset;
                        DangerClones[i].localPosition = newPos;
                    }
                }
                // Instantiate the danger waypoint prefab
                GameObject dangerWaypoint = Instantiate(navigationController.dangerButton, new Vector3(0, lastY + yOffset, 0), Quaternion.identity);
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