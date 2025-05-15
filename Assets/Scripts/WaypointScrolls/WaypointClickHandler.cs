using System.Xml.Serialization;
using UnityEngine;

public class WaypointClickHandler : MonoBehaviour
{
    private int waypointIndex = -1;

    public void SetWaypointIndex(int index)
    {
        waypointIndex = index;
    }


    public void OnClick()
    {
        Debug.Log($"Waypoint clicked for the first time: Index = {waypointIndex}");
        StoreWaypointIndex(waypointIndex);
    }

    private void StoreWaypointIndex(int index)
    {
        Debug.Log($"Storing waypoint index: {index}");
        NavigationFrontend navigationFrontend = FindObjectOfType<NavigationFrontend>();
        Debug.Log("transform name: " + transform.name);
        if (transform.name == "GEO")
        {
            navigationFrontend.openGeoNavigation(index);
        }
        else if (transform.name == "DANGER")
        {
            navigationFrontend.openDangerNavigation(index);
        }
        else if (transform.name == "POI")
        {
            navigationFrontend.openPOINavigation(index);
        }
        else if (transform.name == "STATION")
        {
            navigationFrontend.openStationNavigation(index);
        }
    }
}