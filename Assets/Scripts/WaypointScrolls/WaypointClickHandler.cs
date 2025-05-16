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
        GeoSampleFrontend geoSampleFrontend = FindObjectOfType<GeoSampleFrontend>();
        Debug.Log("transform name: " + transform.name);
        if (transform.name == "GEO(Clone)")
        {
            navigationFrontend.openGeoNavigation(index);
        }
        else if (transform.name == "DANGER(Clone)")
        {
            navigationFrontend.openDangerNavigation(index);
        }
        else if (transform.name == "POI(Clone)")
        {
            navigationFrontend.openPOINavigation(index);
        }
        else if (transform.name == "STATION(Clone)")
        {
            navigationFrontend.openStationNavigation(index);
        }
        else if (transform.name == "EV2")
        {
            navigationFrontend.navigateToEV(index);
        }
        else if (transform.name == "ROVER")
        {
            navigationFrontend.navigateToPR(index);
        }
        else if (transform.name == "ZONEA(Clone)")
        {
            geoSampleFrontend.openNavigationToZoneA(index);
        }
        else if (transform.name == "ZONEB(Clone)")
        {
            geoSampleFrontend.openNavigationToZoneB(index);
        }
        else if (transform.name == "ZONEC(Clone)")
        {
            geoSampleFrontend.openNavigationToZoneC(index);
        }
    }
}