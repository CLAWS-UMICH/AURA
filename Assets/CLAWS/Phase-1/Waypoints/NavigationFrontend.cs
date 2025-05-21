using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.UX;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class NavigationFrontend : MonoBehaviour
{
    [SerializeField] private NavigationController navigationController;
    public double UNITYposX = 0;
    public double UNITYposY = 0;

    private bool geoButtonPressed = false;
    private bool dangerButtonPressed = false;
    private bool poiButtonPressed = false;
    private GameObject activeScreen = null;

    private GameObject dangerMarker;
    private GameObject geoMarker;
    private GameObject poiMarker;
    private TextMeshPro nameField;

    [Header("Pathfinding System")]
    public Pathfinding pathfindingSystem;
    bool isCompanionLayer = false;
    // private const string COMPANION_LAYER = "Companion";
    // private const string FULLMAP_LAYER = "FULL_Map";

    private string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    void Start()
    {
        if (navigationController == null)
        {
            Debug.LogError("NavigationController is not assigned.");
            return;
        }

        Debug.Log("NavigationFrontend initialized.");
        navigationController.WaypointMenuScreen.SetActive(true);
        openCompanionScreen();
    }


    // for creating waypoint to set 
    // EDIT TESTTING
    // public void addingWaypoint()
    // {
    //     Debug.Log("Adding waypoint...");
    //     navigationController.CreateWaypointScreen.SetActive(false);
    //     UnityEngine.Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 2f;
    //     spawnPosition.y -= 1f;

    //     Debug.Log($"Waypoint spawn position: {spawnPosition}");

    //     GameObject newWaypointMarker = Instantiate(
    //         navigationController.WSPACE_IconpPrefab,
    //         spawnPosition,
    //         UnityEngine.Quaternion.identity,
    //         navigationController.Controller.transform
    //     );

    //     Debug.Log("Waypoint marker instantiated.");
    //     UpdateActiveMarker(newWaypointMarker);

    //     newWaypointMarker.GetComponent<SolverHandler>().LeftInteractor = navigationController.leftRayInteractor;
    //     newWaypointMarker.GetComponent<SolverHandler>().RightInteractor = navigationController.rightRayInteractor;
    //     newWaypointMarker.GetComponent<TapToPlace>().StartPlacement();

    //     Debug.Log("Waypoint placement started.");

    //     newWaypointMarker.GetComponent<TapToPlace>().OnPlacingStopped
    //     .AddListener(() =>
    //     {
    //         Debug.Log("Waypoint placement stopped.");
    //         Debug.Log($"Waypoint marker position: {newWaypointMarker.transform.position}");
    //         Waypoint newWaypoint = new Waypoint
    //         {
    //             Use = "ADD",
    //             Id = navigationController.waypointList.Count + 1,
    //             Name = navigationController.CreateWaypointScreen.transform.GetChild(4).GetChild(3).GetComponent<TextMeshPro>().text,
    //             IMUposX = newWaypointMarker.transform.position.x + AstronautInstance.User.origin.posX,
    //             IMUposY = newWaypointMarker.transform.position.z + AstronautInstance.User.origin.posY,
    //             Type = dangerButtonPressed ? WaypointType.DANGER : geoButtonPressed ? WaypointType.GEO : WaypointType.POI,
    //             Author = AstronautInstance.User.id == 1 ? AuthorType.EV1 : AuthorType.EV2,
    //         };

    //         Debug.Log($"New waypoint created: {newWaypoint.Name}, Type: {newWaypoint.Type}, IMUposX: {newWaypoint.IMUposX}, IMUposY: {newWaypoint.IMUposY}");
    //         EventBus.Publish(new WaypointAddedEvent(newWaypoint));

    //         // After creating the waypoint, trigger pathfinding
    //         if (navigationController.pathfindingSystem != null)
    //         {
    //             // Convert waypoint IMU to world coordinates
    //             UnityEngine.Vector3 targetPosition = new UnityEngine.Vector3(
    //                 (float)(newWaypoint.IMUposX - AstronautInstance.User.origin.posX),
    //                 0,
    //                 (float)(newWaypoint.IMUposY - AstronautInstance.User.origin.posZ) // Use posZ for Z-axis
    //             );
                
    //             navigationController.pathfindingSystem.CalculatePath(targetPosition);
    //         }
    //         else
    //         {
    //             Debug.LogError("Pathfinding system reference not set!");
    //         }
    //     });

    // }

    public void addingWaypoint()
    {
        Debug.Log("Adding waypoint at astronaut's position...");
        navigationController.CreateWaypointScreen.SetActive(false);

        // Get astronaut's current position in IMU coordinates
        Location astronautLoc = AstronautInstance.User.current;

        // Create waypoint marker at astronaut's position
        UnityEngine.Vector3 astronautPosition = new UnityEngine.Vector3(
            (float)astronautLoc.posX,
            (float)astronautLoc.posY,
            (float)astronautLoc.posZ
        );
        GameObject newWaypointMarker = Instantiate(
            navigationController.WSPACE_IconpPrefab,
            astronautPosition,
            UnityEngine.Quaternion.identity,
            navigationController.Controller.transform
        );

        // Remove placement components 
        Destroy(newWaypointMarker.GetComponent<TapToPlace>());
        Destroy(newWaypointMarker.GetComponent<SolverHandler>());

        // Create waypoint data
        Waypoint newWaypoint = new Waypoint
        {
            Use = "ADD",
            Id = navigationController.waypointList.Count + 1,
            Name = navigationController.CreateWaypointScreen.transform.GetChild(3).GetComponent<TextMeshPro>().text,
            UNITYposX = astronautLoc.posX, // Direct unity coords
            UNITYposZ = astronautLoc.posZ,
            Type = dangerButtonPressed ? WaypointType.DANGER : 
                geoButtonPressed ? WaypointType.GEO : 
                WaypointType.POI,
            Author = AstronautInstance.User.id == 1 ? AuthorType.EV1 : AuthorType.EV2,
        };

        Debug.Log($"New waypoint created at astronaut's position: {newWaypoint.UNITYposX}, {newWaypoint.UNITYposZ}");
        EventBus.Publish(new WaypointAddedEvent(newWaypoint));

        // Immediately trigger pathfinding
        if (navigationController.pathfindingSystem != null)
        {
            // Convert to pathfinding target position
            UnityEngine.Vector3 targetPosition = new UnityEngine.Vector3(
                (float)newWaypoint.UNITYposX,
                0,
                (float)newWaypoint.UNITYposZ
            );
            
            navigationController.pathfindingSystem.CalculatePath(targetPosition);
        }
        else
        {
            Debug.LogError("Pathfinding system reference not set!");
        }
    }


    public void UpdateActiveMarker(GameObject newMarker)
    {
        Debug.Log("Updating active marker...");
        geoMarker = newMarker.transform.GetChild(3).GetChild(0).gameObject;
        dangerMarker = newMarker.transform.GetChild(3).GetChild(2).gameObject;
        poiMarker = newMarker.transform.GetChild(3).GetChild(1).gameObject;
        //nameField = navigationController.CreateWaypointScreen.transform.GetChild(4).GetChild(3).GetComponent<TextMeshPro>();
        nameField = navigationController.CreateWaypointScreen.transform.GetChild(3).GetComponent<TextMeshPro>();
        Debug.Log($"geoButtonPressed: {geoButtonPressed}, dangerButtonPressed: {dangerButtonPressed}, poiButtonPressed: {poiButtonPressed}");


        if (geoButtonPressed)
        {
            geoMarker.SetActive(true);
            dangerMarker.SetActive(false);
            poiMarker.SetActive(false);
            if (nameField.text == "Waypoint Name")
            {
                int waypointIndex = navigationController.GeoWaypointList.Count;
                char waypointLetter = waypointIndex < alphabet.Length ? alphabet[waypointIndex] : '*'; // Fallback to '*' if out of range
                nameField.text = "Waypoint " + waypointLetter;
            }
            Debug.Log("Geo marker activated.");
        }
        else if (dangerButtonPressed)
        {
            geoMarker.SetActive(false);
            dangerMarker.SetActive(true);
            poiMarker.SetActive(false);
            if (nameField.text == "Waypoint Name")
            {
                int waypointIndex = navigationController.DangerWaypointList.Count;
                char waypointLetter = waypointIndex < alphabet.Length ? alphabet[waypointIndex] : '*'; 
                nameField.text = "Waypoint " + waypointLetter;
            }
            Debug.Log("Danger marker activated.");
        }
        else if (poiButtonPressed)
        {
            geoMarker.SetActive(false);
            dangerMarker.SetActive(false);
            poiMarker.SetActive(true);
            if (nameField.text == "Waypoint Name")
            {
                int waypointIndex = navigationController.POIWaypointList.Count;
                char waypointLetter = waypointIndex < alphabet.Length ? alphabet[waypointIndex] : '*';
                nameField.text = "Waypoint " + waypointLetter;
            }
            Debug.Log("POI marker activated.");
        }
    }

    public void openCompanionScreen()
    {
        //maps
        navigationController.GeoMap.SetActive(false);
        navigationController.FullMap.SetActive(true);
        navigationController.DangerMap.SetActive(false);
        navigationController.POIMap.SetActive(false);
        navigationController.StationMap.SetActive(false);

        //cameras
        navigationController.geoCamera.SetActive(false);
        navigationController.companionCamera.SetActive(true);
        navigationController.dangerCamera.SetActive(false);
        navigationController.poiCamera.SetActive(false);
        navigationController.stationCamera.SetActive(false);

        //screens
        navigationController.CompanionScreen.SetActive(true);
        navigationController.POIScreen.SetActive(false);
        navigationController.StationScreen.SetActive(false);
        navigationController.GeoScreen.SetActive(false);
        navigationController.DangerScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(true);
        activeScreen = navigationController.CompanionScreen;
    }

    
    public void openPOIScreen()
    {
        //maps
        navigationController.GeoMap.SetActive(false);
        navigationController.FullMap.SetActive(false);
        navigationController.DangerMap.SetActive(false);
        navigationController.POIMap.SetActive(true);
        navigationController.StationMap.SetActive(false);

        //cameras
        navigationController.geoCamera.SetActive(false);
        navigationController.companionCamera.SetActive(false);
        navigationController.dangerCamera.SetActive(false);
        navigationController.poiCamera.SetActive(true);
        navigationController.stationCamera.SetActive(false);

        //screens
        navigationController.CompanionScreen.SetActive(false);
        navigationController.POIScreen.SetActive(true);
        navigationController.StationScreen.SetActive(false);
        navigationController.GeoScreen.SetActive(false);
        navigationController.DangerScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(true);
        

        // close closed icon
        navigationController.poiClosedIconParent.SetActive(false);
        navigationController.geoClosedIconParent.SetActive(true);
        navigationController.dangerClosedIconParent.SetActive(true);
        navigationController.stationClosedIconParent.SetActive(true);

        activeScreen = navigationController.POIScreen;
        dangerButtonPressed = false;
        geoButtonPressed = false;
        poiButtonPressed = true;
    }


    public void openStationScreen()
    {
        //maps
        navigationController.GeoMap.SetActive(false);
        navigationController.FullMap.SetActive(false);
        navigationController.DangerMap.SetActive(false);
        navigationController.POIMap.SetActive(false);
        navigationController.StationMap.SetActive(true);

        //cameras
        navigationController.geoCamera.SetActive(false);
        navigationController.companionCamera.SetActive(false);
        navigationController.dangerCamera.SetActive(false);
        navigationController.poiCamera.SetActive(false);
        navigationController.stationCamera.SetActive(true);

        //screens
        navigationController.CompanionScreen.SetActive(false);
        navigationController.POIScreen.SetActive(false);
        navigationController.StationScreen.SetActive(true);
        navigationController.GeoScreen.SetActive(false);
        navigationController.DangerScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(true);

        // close closed icon
        navigationController.stationClosedIconParent.SetActive(false);
        navigationController.geoClosedIconParent.SetActive(true);
        navigationController.dangerClosedIconParent.SetActive(true);
        navigationController.poiClosedIconParent.SetActive(true);

        activeScreen = navigationController.StationScreen;
    }

    public void openGeoScreen()
    {
        //maps
        navigationController.GeoMap.SetActive(true);
        navigationController.FullMap.SetActive(false);
        navigationController.DangerMap.SetActive(false);
        navigationController.POIMap.SetActive(false);
        navigationController.StationMap.SetActive(false);

        //cameras
        navigationController.geoCamera.SetActive(true);
        navigationController.companionCamera.SetActive(false);
        navigationController.dangerCamera.SetActive(false);
        navigationController.poiCamera.SetActive(false);
        navigationController.stationCamera.SetActive(false);

        //screens
        navigationController.CompanionScreen.SetActive(false);
        navigationController.POIScreen.SetActive(false);
        navigationController.StationScreen.SetActive(false);
        navigationController.GeoScreen.SetActive(true);
        navigationController.DangerScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(true);

        // close closed icon
        navigationController.geoClosedIconParent.SetActive(false);
        navigationController.poiClosedIconParent.SetActive(true);
        navigationController.dangerClosedIconParent.SetActive(true);
        navigationController.stationClosedIconParent.SetActive(true);

        activeScreen = navigationController.GeoScreen;
        dangerButtonPressed = false;
        geoButtonPressed = true;
        poiButtonPressed = false;
    }

    public void openDangerScreen()
    {
        //maps
        navigationController.GeoMap.SetActive(false);
        navigationController.FullMap.SetActive(false);
        navigationController.DangerMap.SetActive(true);
        navigationController.POIMap.SetActive(false);
        navigationController.StationMap.SetActive(false);

        //cameras
        navigationController.geoCamera.SetActive(false);
        navigationController.companionCamera.SetActive(false);
        navigationController.dangerCamera.SetActive(true);
        navigationController.poiCamera.SetActive(false);
        navigationController.stationCamera.SetActive(false);

        //screens
        navigationController.CompanionScreen.SetActive(false);
        navigationController.POIScreen.SetActive(false);
        navigationController.StationScreen.SetActive(false);
        navigationController.GeoScreen.SetActive(false);
        navigationController.DangerScreen.SetActive(true);
        navigationController.addWaypointButton.SetActive(true);

        // close closed icon
        navigationController.dangerClosedIconParent.SetActive(false);
        navigationController.geoClosedIconParent.SetActive(true);
        navigationController.poiClosedIconParent.SetActive(true);
        navigationController.stationClosedIconParent.SetActive(true);

        activeScreen = navigationController.DangerScreen;
        dangerButtonPressed = true;
        geoButtonPressed = false;
        poiButtonPressed = false;
    }


    public void openWaypointScreen()
    {
        navigationController.CreateWaypointScreen.SetActive(true);
        navigationController.verticalButtonScreen.SetActive(false);
        navigationController.WaypointMenuScreen.SetActive(false);
        navigationController.NavigationScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(false);
    }


    public void openDangerNavigation(int waypointIndex)
    {
        Debug.Log($"Opening danger navigation for waypoint index: {waypointIndex}");
        navigationController.WaypointMenuScreen.SetActive(false);
        navigationController.NavigationScreen.SetActive(true);
        navigationController.verticalButtonScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(false);

        Waypoint waypoint = navigationController.StationWaypointList[waypointIndex];
        
        UnityEngine.Vector3 targetPosition = new UnityEngine.Vector3(
            (float)waypoint.UNITYposX,
            0,
            (float)waypoint.UNITYposZ
        );

        NavigateToPosition(targetPosition);

        Debug.Log($"Waypoint details: {waypoint.Name}, Type: {waypoint.Type}, IMUposX: {waypoint.UNITYposX}, IMUposY: {waypoint.UNITYposZ}");
    }


    public void openGeoNavigation(int waypointIndex)
    {
        Debug.Log($"Opening geo navigation for waypoint index: {waypointIndex}");
        navigationController.WaypointMenuScreen.SetActive(false);
        navigationController.NavigationScreen.SetActive(true);
        navigationController.verticalButtonScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(false);

        Waypoint waypoint = navigationController.StationWaypointList[waypointIndex];
        
        UnityEngine.Vector3 targetPosition = new UnityEngine.Vector3(
            (float)waypoint.UNITYposX,
            0,
            (float)waypoint.UNITYposZ
        );

        NavigateToPosition(targetPosition);

        Debug.Log($"Waypoint details: {waypoint.Name}, Type: {waypoint.Type}, IMUposX: {waypoint.UNITYposX}, IMUposY: {waypoint.UNITYposZ}");
    }


    public void openPOINavigation(int waypointIndex)
    {
        Debug.Log($"Opening POI navigation for waypoint index: {waypointIndex}");
        navigationController.WaypointMenuScreen.SetActive(false);
        navigationController.NavigationScreen.SetActive(true);
        navigationController.verticalButtonScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(false);

        Waypoint waypoint = navigationController.StationWaypointList[waypointIndex];
        
        UnityEngine.Vector3 targetPosition = new UnityEngine.Vector3(
            (float)waypoint.UNITYposX,
            0,
            (float)waypoint.UNITYposZ
        );

        NavigateToPosition(targetPosition);

        Debug.Log($"Waypoint details: {waypoint.Name}, Type: {waypoint.Type}, IMUposX: {waypoint.UNITYposX}, IMUposY: {waypoint.UNITYposZ}");
    }


    public void openStationNavigation(int waypointIndex)
    {
        Debug.Log($"Opening station navigation for waypoint index: {waypointIndex}");
        navigationController.WaypointMenuScreen.SetActive(false);
        navigationController.NavigationScreen.SetActive(true);
        navigationController.verticalButtonScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(false);

        Waypoint waypoint = navigationController.StationWaypointList[waypointIndex];
        
        UnityEngine.Vector3 targetPosition = new UnityEngine.Vector3(
            (float)waypoint.UNITYposX,
            0,
            (float)waypoint.UNITYposZ
        );

        NavigateToPosition(targetPosition);

        Debug.Log($"Waypoint details: {waypoint.Name}, Type: {waypoint.Type}, IMUposX: {waypoint.UNITYposX}, IMUposY: {waypoint.UNITYposZ}");
    }


    public void geoButton()
    {
        Debug.Log("Geo button pressed.");
        geoButtonPressed = true;
        poiButtonPressed = false;
        dangerButtonPressed = false;
    }

    public void poiButton()
    {
        Debug.Log("POI button pressed.");
        poiButtonPressed = true;
        geoButtonPressed = false;
        dangerButtonPressed = false;
    }

    public void dangerButton()
    {
        Debug.Log("Danger button pressed.");
        dangerButtonPressed = true;
        geoButtonPressed = false;
        poiButtonPressed = false;
    }

    

    public void openFeatureScreen()
    {
        navigationController.WaypointMenuScreen.SetActive(true);
        navigationController.verticalButtonScreen.SetActive(true);
        navigationController.addWaypointButton.SetActive(true);
        // Check which screen is currently active
        if (activeScreen == navigationController.CompanionScreen)
        {
            openCompanionScreen();
        }
        else if (activeScreen == navigationController.POIScreen)
        {
            openPOIScreen();
        }
        else if (activeScreen == navigationController.StationScreen)
        {
            openStationScreen();
        }
        else if (activeScreen == navigationController.GeoScreen)
        {
            openGeoScreen();
        }
        else if (activeScreen == navigationController.DangerScreen)
        {
            openDangerScreen();
        }
        else
        {
            openCompanionScreen();
        }
    }

    void SetPathToCompanionLayer()
    {
        SetLineRendererLayer("Companion");
    }

    void SetPathToFullMapLayer()
    {
        SetLineRendererLayer("Default");
    }

    void SetLineRendererLayer(string layerName)
    {
        LineRenderer lr = pathfindingSystem.GetComponent<LineRenderer>();
        int layer = LayerMask.NameToLayer(layerName);
        
        if (layer == -1)
        {
            Debug.LogError($"Layer {layerName} does not exist!");
            return;
        }
        
        lr.gameObject.layer = layer;
        
        // Update all child objects if needed
        foreach(Transform child in lr.transform)
        {
            child.gameObject.layer = layer;
        }
    }

    public void navigateToEV(int index)
    {
        GameObject ev2Object = GameObject.Find("EV2_PlayerIcon");
        if (ev2Object == null)
        {
            Debug.LogError("EV2 (PlayerIcon2) not found in scene!");
            return;
        }

        UnityEngine.Vector3 targetPosition = ev2Object.transform.position;
        targetPosition.y = 0;

        if (pathfindingSystem != null)
        {
            // Toggle layer on each click
            SetPathToFullMapLayer();

            // Toggle the state for next click
            isCompanionLayer = !isCompanionLayer;

            pathfindingSystem.CalculatePath(targetPosition);
            Debug.Log($"Pathfinding to EV2 on {(isCompanionLayer ? "Default" : "COMPANION")} layer");

            // Close navigation screen after switching to FULL_Map (i.e., after the second click)
            if (!isCompanionLayer)
            {
                // Assuming you have a reference to the navigation screen
                closeScreens();
                Debug.Log("Navigation screen closed after second click.");
            }
        }
    }

    public void navigateToPR(int index)
    {
        // pull up pr coords
        // UnityEngine.Vector3 targetPosition = pathfindingSystem.target.position;

        // AstronautInstance.User.fellowAstronaut.location.posX = targetPosition.x;
        // AstronautInstance.User.fellowAstronaut.location.posY = targetPosition.y;
        // AstronautInstance.User.fellowAstronaut.location.posZ = targetPosition.z;
    }


    public void closeScreens()
    {
        Debug.Log("Closing nav current screen...");
        navigationController.Controller.SetActive(true);
        foreach (Transform screen in navigationController.transform)
        {
            screen.gameObject.SetActive(false);
        }
    }

    public void NavigateToPosition(UnityEngine.Vector3 targetPosition)
    {
        if (pathfindingSystem != null)
        {
            pathfindingSystem.SetTarget(targetPosition);
        }
    }
}