using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;
using UnityEditor.Rendering.LookDev;
using MixedReality.Toolkit.UX.Experimental;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using MixedReality.Toolkit.Examples.Demos;

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
    public GameObject ev2Icon_full;
    public GameObject roverIcon_full;
    public GameObject ev2Icon_small;


    [Header("Closed Icon Prefabs")]
    public GameObject dangerClosedPrefab_Icon;
    public GameObject geoClosedPrefab_Icon;
    public GameObject stationClosedPrefab_Icon;
    public GameObject poiClosedPrefab_Icon;


    [Header("Minimaps")]
    public GameObject FullMap;
    public GameObject POIMap;
    public GameObject DangerMap;
    public GameObject StationMap;
    public GameObject GeoMap;


    [Header("Ray Interactors")]
    public XRBaseInteractor leftRayInteractor;
    public XRBaseInteractor rightRayInteractor;


    [Header("Waypoint Scroll Rects")]
    public VirtualizedScrollRectList dangerScrollRectList;
    public VirtualizedScrollRectList geoScrollRectList;
    public VirtualizedScrollRectList stationScrollRectList;
    public VirtualizedScrollRectList poiScrollRectList;


    // Screens Section
    [Header("Screens")]
    public GameObject Controller;
    public ToggleCollection MainMenuToggleCollection;
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

    [Header("Icon Parents")]
    public GameObject dangerIconParent;
    public GameObject geoIconParent;
    public GameObject stationIconParent;
    public GameObject poiIconParent;

    [Header("Icon Closed Parents")]
    public GameObject dangerClosedIconParent;
    public GameObject geoClosedIconParent;
    public GameObject stationClosedIconParent;
    public GameObject poiClosedIconParent;

    [Header("Cameras")]
    public GameObject geoCamera;
    public GameObject stationCamera;
    public GameObject poiCamera;
    public GameObject dangerCamera;
    public GameObject minimapCamera;
    public GameObject companionCamera;

    // add if 3d map added
    // [SerializeField] private GameObject dangerPrefab_3D;
    // [SerializeField] private GameObject geoPrefab_3D;
    // [SerializeField] private GameObject stationPrefab_3D;
    // [SerializeField] private GameObject poiPrefab_3D;
    // [SerializeField] private GameObject companionPrefab_3D;

    private Subscription<WaypointAddedEvent> waypointAddedSubscription;
    private Subscription<WaypointDeletedEvent> waypointRemovedSubscription;
    public List<Waypoint> waypointList = new List<Waypoint>();
    public List<Waypoint> GeoWaypointList = new List<Waypoint>();
    public List<Waypoint> StationWaypointList = new List<Waypoint>();
    public List<Waypoint> POIWaypointList = new List<Waypoint>();
    public List<Waypoint> DangerWaypointList = new List<Waypoint>();


    void Start()
    {
        waypointAddedSubscription = EventBus.Subscribe<WaypointAddedEvent>(OnWaypointAdded);
        waypointRemovedSubscription = EventBus.Subscribe<WaypointDeletedEvent>(OnWaypointRemoved);

        // INSTANTIATE STATION WAYPOINTS
        Waypoint station1 = new Waypoint {
            Use = "ADD",
            Id = StationWaypointList.Count,
            Name = "Station 1",
            IMUposX = -5616f,
            IMUposY = -10005f,
            Type = WaypointType.STATION,
            Author = AstronautInstance.User.id == 1 ? AuthorType.EV1 : AuthorType.EV2,
        };

        Waypoint station2 = new Waypoint {
            Use = "ADD",
            Id = StationWaypointList.Count,
            Name = "Station 2",
            IMUposX = -5643f,
            IMUposY = -9970f,
            Type = WaypointType.STATION,
            Author = AstronautInstance.User.id == 1 ? AuthorType.EV1 : AuthorType.EV2,
        };

        Waypoint station3 = new Waypoint {
            Use = "ADD",
            Id = StationWaypointList.Count,
            Name = "Station 3",
            IMUposX = -5608f,
            IMUposY = -9988f,
            Type = WaypointType.STATION,
            Author = AstronautInstance.User.id == 1 ? AuthorType.EV1 : AuthorType.EV2,
        };
        Debug.Log("Instantiating Station Waypoints...");
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(station1));
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(station2));
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(station3));    

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // INSTANTIATE LTV WAYPOINTS ---- HARD CODED FOR TESTING  ------ WILL SEE IF WE CAN GET THE DATA FROM THE SERVER //
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        Waypoint ltv1 = new Waypoint {
            Use = "ADD",
            Id = StationWaypointList.Count,
            Name = "Waypoint A",
            IMUposX = -5635f,
            IMUposY = -9970f,
            Type = WaypointType.POI,
            Author = AuthorType.PR
        };
        Waypoint ltv2 = new Waypoint {
            Use = "ADD",
            Id = StationWaypointList.Count,
            Name = "Waypoint B",
            IMUposX = -5610f,
            IMUposY = -9971f,
            Type = WaypointType.POI,
            Author = AuthorType.PR
        };
        Waypoint ltv3 = new Waypoint {
            Use = "ADD",
            Id = StationWaypointList.Count,
            Name = "Waypoint C",
            IMUposX = -5615f,
            IMUposY = -9995f,
            Type = WaypointType.POI,
            Author = AuthorType.PR,
        };
        Debug.Log("Instantiating LTV Waypoints...");
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(ltv1));
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(ltv2));
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(ltv3));

        CompanionScreen.SetActive(true);
        POIScreen.SetActive(false);
        StationScreen.SetActive(false);
        GeoScreen.SetActive(false);
        DangerScreen.SetActive(false);
    }


    void OnWaypointAdded(WaypointAddedEvent e)
    {
        Debug.Log("Waypoint added: " + e.NewAddedWaypoint);
        Waypoint newWaypoint = e.NewAddedWaypoint;
        switch(newWaypoint.Type)
        {
            case WaypointType.DANGER:
                Debug.Log("Adding a DANGER waypoint...");
                // ICON WORLD SPACE POSITION
                Vector3 dangerPosition = new Vector3(
                    (float)(newWaypoint.IMUposX - AstronautInstance.User.origin.posX),
                    0,
                    (float)(newWaypoint.IMUposY - AstronautInstance.User.origin.posY)
                );
                Debug.Log($"DANGER waypoint position: {dangerPosition}");
                char firstLetter = newWaypoint.Name[0];
                // get letter from waypoint name
                newWaypoint.Name = newWaypoint.Name.ToUpper();
                if (newWaypoint.Name.Contains("WAYPOINT"))
                {
                    int index = newWaypoint.Name.IndexOf("WAYPOINT") + "WAYPOINT".Length;
                    if (index < newWaypoint.Name.Length)
                    {
                        firstLetter = newWaypoint.Name[index];
                        Debug.Log($"The first letter after 'WAYPOINT' is: {firstLetter}");
                    }
                }
                // Instantiate the danger map icon
                GameObject dangerIcon = Instantiate(dangerPrefab_Icon, dangerPosition, Quaternion.identity, dangerIconParent.transform);
                dangerIcon.transform.Find("TypeText").GetComponent<TextMeshPro>().text = firstLetter.ToString();
                dangerIcon.name = newWaypoint.Name;
                Debug.Log($"DANGER icon instantiated with name: {dangerIcon.name}");
                // Instantiate the danger minimized icon
                GameObject dangerIconClosed = Instantiate(dangerClosedPrefab_Icon, dangerPosition, Quaternion.identity, dangerClosedIconParent.transform);
                dangerIconClosed.name = newWaypoint.Name + "_closed";

                // Instantiate the the danger prefab button in NAV menu
                DangerWaypointList.Add(newWaypoint);
                waypointList.Add(newWaypoint);
                // Set scroll list items ++
                dangerScrollRectList.SetItemCount(DangerWaypointList.Count);
                break;

            case WaypointType.GEO:
                 Debug.Log("Adding a GEO waypoint...");
                Vector3 geoPosition = new Vector3((float)(newWaypoint.IMUposX - AstronautInstance.User.origin.posX), 0, (float)(newWaypoint.IMUposY - AstronautInstance.User.origin.posY));
                 Debug.Log($"GEO waypoint position: {geoPosition}");
                GameObject geoIcon = Instantiate(geoPrefab_Icon, geoPosition, Quaternion.identity, geoIconParent.transform);
                 geoIcon.name = newWaypoint.Name;
                Debug.Log($"GEO icon instantiated with name: {geoIcon.name}");
                GameObject geoIconClosed = Instantiate(geoClosedPrefab_Icon, geoPosition, Quaternion.identity, geoClosedIconParent.transform);
                geoIconClosed.name = newWaypoint.Name + "_closed";
                GeoWaypointList.Add(newWaypoint);
                waypointList.Add(newWaypoint);
                geoScrollRectList.SetItemCount(GeoWaypointList.Count);
                break;

            case WaypointType.STATION:
                Debug.Log("Adding a STATION waypoint...");
                Vector3 stationPosition = new Vector3((float)(newWaypoint.IMUposX - AstronautInstance.User.origin.posX), 0, (float)(newWaypoint.IMUposY - AstronautInstance.User.origin.posY));
                Debug.Log($"STATION waypoint position: {stationPosition}");
                GameObject stationIcon = Instantiate(stationPrefab_Icon, stationPosition, Quaternion.identity, stationIconParent.transform);
                stationIcon.name = newWaypoint.Name;
                Debug.Log($"STATION icon instantiated with name: {stationIcon.name}");
                GameObject stationIconClosed = Instantiate(stationClosedPrefab_Icon, stationPosition, Quaternion.identity, stationClosedIconParent.transform);
                stationIconClosed.name = newWaypoint.Name + "_closed";
                StationWaypointList.Add(newWaypoint);
                waypointList.Add(newWaypoint);
                stationScrollRectList.SetItemCount(StationWaypointList.Count);
                break;

            case WaypointType.POI:
                Debug.Log("Adding a POI waypoint...");
                Vector3 poiPosition = new Vector3((float)(newWaypoint.IMUposX - AstronautInstance.User.origin.posX), 0, (float)(newWaypoint.IMUposY - AstronautInstance.User.origin.posY));
                Debug.Log($"POI waypoint position: {poiPosition}");
                GameObject poiIcon = Instantiate(poiPrefab_Icon, poiPosition, Quaternion.identity, poiIconParent.transform);
                poiIcon.name = newWaypoint.Name;
                Debug.Log($"POI icon instantiated with name: {poiIcon.name}");
                GameObject poiIconClosed = Instantiate(poiClosedPrefab_Icon, poiPosition, Quaternion.identity, poiClosedIconParent.transform);
                poiIconClosed.name = newWaypoint.Name + "_closed";
                POIWaypointList.Add(newWaypoint);
                waypointList.Add(newWaypoint);
                poiScrollRectList.SetItemCount(POIWaypointList.Count);
                break;
        }
    }


    void OnWaypointRemoved(WaypointDeletedEvent e)
    {
        Debug.Log("Waypoint removed: " + e.DeletedWaypoint);
        Waypoint deletedWaypoint = e.DeletedWaypoint;

        // Remove the waypoint from the respective list based on its type
        switch (deletedWaypoint.Type)
        {
            case WaypointType.DANGER:
                Debug.Log("Removing a DANGER waypoint...");
                DangerWaypointList.Remove(deletedWaypoint);
                waypointList.Remove(deletedWaypoint);
                dangerScrollRectList.SetItemCount(DangerWaypointList.Count);
                Debug.Log($"DANGER waypoint removed. Total Danger Waypoints: {DangerWaypointList.Count}");

                DestroyWaypointPrefabInstances(deletedWaypoint.Name, deletedWaypoint.Name + "_closed");
                break;

            case WaypointType.GEO:
                Debug.Log("Removing a GEO waypoint...");
                GeoWaypointList.Remove(deletedWaypoint);
                waypointList.Remove(deletedWaypoint);
                geoScrollRectList.SetItemCount(GeoWaypointList.Count);
                Debug.Log($"GEO waypoint removed. Total Geo Waypoints: {GeoWaypointList.Count}");

                DestroyWaypointPrefabInstances(deletedWaypoint.Name, deletedWaypoint.Name + "_closed");
                break;

            case WaypointType.STATION:
                Debug.Log("Removing a STATION waypoint...");
                StationWaypointList.Remove(deletedWaypoint);
                waypointList.Remove(deletedWaypoint);
                stationScrollRectList.SetItemCount(StationWaypointList.Count);
                Debug.Log($"STATION waypoint removed. Total Station Waypoints: {StationWaypointList.Count}");

                DestroyWaypointPrefabInstances(deletedWaypoint.Name, deletedWaypoint.Name + "_closed");
                break;

            case WaypointType.POI:
                Debug.Log("Removing a POI waypoint...");
                POIWaypointList.Remove(deletedWaypoint);
                waypointList.Remove(deletedWaypoint);
                poiScrollRectList.SetItemCount(POIWaypointList.Count);
                Debug.Log($"POI waypoint removed. Total POI Waypoints: {POIWaypointList.Count}");

                DestroyWaypointPrefabInstances(deletedWaypoint.Name, deletedWaypoint.Name + "_closed");
                break;
        }
    }

    void DestroyWaypointPrefabInstances(string iconName, string closedIconName)
    {
        // Destroy the main icon prefab
        GameObject waypointToRemove = GameObject.Find(iconName);
        if (waypointToRemove != null)
        {
            Debug.Log($"Destroying waypoint prefab: {waypointToRemove.name}");
            Destroy(waypointToRemove);
        }
        else
        {
            Debug.LogWarning($"Waypoint prefab not found: {iconName}");
        }

        // Destroy the closed icon prefab
        GameObject closedWaypointToRemove = GameObject.Find(closedIconName);
        if (closedWaypointToRemove != null)
        {
            Debug.Log($"Destroying closed waypoint prefab: {closedWaypointToRemove.name}");
            Destroy(closedWaypointToRemove);
        }
        else
        {
            Debug.LogWarning($"Closed waypoint prefab not found: {closedIconName}");
        }
    }
}