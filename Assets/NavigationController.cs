using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;
using UnityEditor.Rendering.LookDev;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class NavigationController : MonoBehaviour
{
    // Prefabs Section
    [Header("Waypoint Prefabs")]
    public GameObject dangerPrefab;
    public GameObject geoPrefab;
    public GameObject stationPrefab;
    public GameObject poiPrefab;

    [Header("Companion Prefabs")]
    public GameObject Ev2;
    public GameObject Rover;

    [Header("Icon Prefabs")]
    public GameObject dangerPrefab_Icon;
    public GameObject geoPrefab_Icon;
    public GameObject stationPrefab_Icon;
    public GameObject poiPrefab_Icon;
    public GameObject WSPACE_IconpPrefab;

    [Header("Closed Icon Prefabs")]
    public GameObject dangerClosedPrefab_Icon;
    public GameObject geoClosedPrefab_Icon;

    public GameObject stationClosedPrefab_Icon;
    public GameObject poiClosedPrefab_Icon;


    [Header("Minimaps")]
    public GameObject FullMap;
    public GameObject EVmap;

    [Header("Ray Interactors")]
    public XRBaseInteractor leftRayInteractor;
    public XRBaseInteractor rightRayInteractor;

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
    public GameObject CreateWaypointScreen;
    public GameObject NavigationScreen;
    public GameObject NotifcationScreen;
    public GameObject WaypointMenuScreen;

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
    public dynamicWaypointPop dynamicWaypointPop;
    public List<Waypoint> waypointList = new List<Waypoint>();
    public List<Waypoint> GeoWaypointList = new List<Waypoint>();
    public List<Waypoint> StationWaypointList = new List<Waypoint>();
    public List<Waypoint> POIWaypointList = new List<Waypoint>();
    public List<Waypoint> DangerWaypointList = new List<Waypoint>();


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
                // ICON WORLD SPACE POSITION
                Vector3 position = new Vector3((float)(newWaypoint.IMUposX - AstronautInstance.User.origin.posX), 0, (float)(newWaypoint.IMUposY - AstronautInstance.User.origin.posY));
                
                // Instantiate the danger map icon
                GameObject dangerIcon = Instantiate(dangerPrefab_Icon, position, Quaternion.identity);
                dangerIcon.name = newWaypoint.Name;
                // Instantiate the danger minimized icon
                GameObject dangerIconClosed = Instantiate(dangerClosedPrefab_Icon, position, Quaternion.identity);
                dangerIconClosed.name = newWaypoint.Name + "_closed";

                // Instantiate the the danger prefab button in NAV menu
                DangerWaypointList.Add(newWaypoint);
                waypointList.Add(newWaypoint);
                dynamicWaypointPop.AddWaypointToMenu(newWaypoint);
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
