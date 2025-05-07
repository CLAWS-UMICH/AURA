using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;

public class NavigationController : MonoBehaviour
{
    // Prefabs Section
    [Header("Waypoint Prefabs")]
    public GameObject dangerPrefab;
    public GameObject geoPrefab;
    public GameObject stationPrefab;
    public GameObject poiPrefab;
    public GameObject Ev2;
    public GameObject Rover;
    public GameObject dangerPrefab_Icon;
    public GameObject geoPrefab_Icon;
    public GameObject stationPrefab_Icon;
    public GameObject poiPrefab_Icon;
    public GameObject companionPrefab_Icon;

    // Screens Section
    [Header("Screens")]
    public GameObject Controller;
    public ToggleCollection MainMenuToggleCollection;
    public ToggleCollection navBarToggleCollection;
    public GameObject CompanionScreen;
    public GameObject POIScreen;
    public GameObject StationScreen;
    public GameObject GeoScreen;
    public GameObject DangerScreen;

    // Buttons Section
    [Header("Buttons")]
    public GameObject companionButton;
    public GameObject poiButton;
    public GameObject stationButton;
    public GameObject geoButton;
    public GameObject dangerButton;

    [Header("Notifications")]
    public GameObject waypointsUpdate;

    // add if 3d map added
    // [SerializeField] private GameObject dangerPrefab_3D;
    // [SerializeField] private GameObject geoPrefab_3D;
    // [SerializeField] private GameObject stationPrefab_3D;
    // [SerializeField] private GameObject poiPrefab_3D;
    // [SerializeField] private GameObject companionPrefab_3D;

    private Subscription<WaypointAddedEvent> waypointAddedSubscription;
    private Subscription<WaypointDeletedEvent> waypointRemovedSubscription;


    void Start()
    {
        waypointAddedSubscription = EventBus.Subscribe<WaypointAddedEvent>(OnWaypointAdded);
        waypointRemovedSubscription = EventBus.Subscribe<WaypointDeletedEvent>(OnWaypointRemoved);

        CompanionScreen.SetActive(true);
        POIScreen.SetActive(false);
        StationScreen.SetActive(false);
        GeoScreen.SetActive(false);
        DangerScreen.SetActive(false);
    }


    void OnWaypointAdded(WaypointAddedEvent e)
    {
        // Handle waypoint added event
        Debug.Log("Waypoint added: " + e.NewAddedWaypoint);
        Waypoint newWaypoint = e.NewAddedWaypoint;
        switch(newWaypoint.Type)
        {
            case WaypointType.DANGER:
                Vector3 position = new Vector3((float)(newWaypoint.IMUposX - AstronautInstance.User.origin.posX), 0, (float)(newWaypoint.IMUposY - AstronautInstance.User.origin.posY));
                // Instantiate the danger map icon in world space
                GameObject dangerIcon = Instantiate(dangerPrefab, position, Quaternion.identity);
                dangerIcon.name = newWaypoint.Name;
                break;
            case WaypointType.GEO:
                
                break;
            case WaypointType.STATION:
                
                break;
            case WaypointType.POI:
                
                break;
        }
    }

    void OnWaypointRemoved(WaypointDeletedEvent e)
    {
        // Handle waypoint removed event
        Debug.Log("Waypoint removed: " + e.DeletedWaypoint);
        // Destroy the waypoint prefab here
        GameObject waypointToRemove = GameObject.Find(e.DeletedWaypoint.Name);
        if (waypointToRemove != null)
        {
            Destroy(waypointToRemove);
        }
    }

}
