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
    public double IMUposX = 0;
    public double IMUposY = 0;

    private bool geoButtonPressed = false;
    private bool dangerButtonPressed = false;
    private bool poiButtonPressed = false;
    private GameObject activeScreen = null;

    private GameObject dangerMarker;
    private GameObject geoMarker;
    private GameObject poiMarker;
    private TextMeshPro nameField;
    [SerializeField]private GameObject waypointNotification;

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
    public void addingWaypoint()
    {
        Debug.Log("Adding waypoint...");
        navigationController.CreateWaypointScreen.SetActive(false);
        UnityEngine.Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 2f;
        spawnPosition.y -= 1f;

        Debug.Log($"Waypoint spawn position: {spawnPosition}");

        GameObject newWaypointMarker = Instantiate(
            navigationController.WSPACE_IconpPrefab,
            spawnPosition,
            UnityEngine.Quaternion.identity,
            navigationController.Controller.transform
        );

        Debug.Log("Waypoint marker instantiated.");
        UpdateActiveMarker(newWaypointMarker);

        newWaypointMarker.GetComponent<SolverHandler>().LeftInteractor = navigationController.leftRayInteractor;
        newWaypointMarker.GetComponent<SolverHandler>().RightInteractor = navigationController.rightRayInteractor;
        newWaypointMarker.GetComponent<TapToPlace>().StartPlacement();

        Debug.Log("Waypoint placement started.");

        newWaypointMarker.GetComponent<TapToPlace>().OnPlacingStopped
        .AddListener(() =>
        {
            Debug.Log("Waypoint placement stopped.");
            Debug.Log($"Waypoint marker position: {newWaypointMarker.transform.position}");
            Waypoint newWaypoint = new Waypoint
            {
                Use = "ADD",
                Id = navigationController.waypointList.Count + 1,
                Name = navigationController.CreateWaypointScreen.transform.GetChild(4).GetChild(3).GetComponent<TextMeshPro>().text,
                IMUposX = newWaypointMarker.transform.position.x + AstronautInstance.User.origin.posX,
                IMUposY = newWaypointMarker.transform.position.z + AstronautInstance.User.origin.posY,
                Type = dangerButtonPressed ? WaypointType.DANGER : geoButtonPressed ? WaypointType.GEO : WaypointType.POI,
                Author = AstronautInstance.User.id == 1 ? AuthorType.EV1 : AuthorType.EV2,
            };

            Debug.Log($"New waypoint created: {newWaypoint.Name}, Type: {newWaypoint.Type}, IMUposX: {newWaypoint.IMUposX}, IMUposY: {newWaypoint.IMUposY}");
            EventBus.Publish(new WaypointAddedEvent(newWaypoint));
            // notification for PR
            if (RoverSettings.isWaypointManualMode)
            {
                if (waypointNotification.activeSelf) {
                    waypointNotification.SetActive(false);
                }
                waypointNotification.SetActive(true);
            }
        });
    }

    public void UpdateActiveMarker(GameObject newMarker)
    {
        Debug.Log("Updating active marker...");
        geoMarker = newMarker.transform.GetChild(3).GetChild(0).gameObject;
        dangerMarker = newMarker.transform.GetChild(3).GetChild(2).gameObject;
        poiMarker = newMarker.transform.GetChild(3).GetChild(1).gameObject;
        nameField = navigationController.CreateWaypointScreen.transform.GetChild(4).GetChild(3).GetComponent<TextMeshPro>();
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

        Waypoint waypoint = navigationController.DangerWaypointList[waypointIndex];
        Debug.Log($"Waypoint details: {waypoint.Name}, Type: {waypoint.Type}, IMUposX: {waypoint.IMUposX}, IMUposY: {waypoint.IMUposY}");
    }


    public void openGeoNavigation(int waypointIndex)
    {
        Debug.Log($"Opening geo navigation for waypoint index: {waypointIndex}");
        navigationController.WaypointMenuScreen.SetActive(false);
        navigationController.NavigationScreen.SetActive(true);
        navigationController.verticalButtonScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(false);

        Waypoint waypoint = navigationController.GeoWaypointList[waypointIndex];
        Debug.Log($"Waypoint details: {waypoint.Name}, Type: {waypoint.Type}, IMUposX: {waypoint.IMUposX}, IMUposY: {waypoint.IMUposY}");
    }


    public void openPOINavigation(int waypointIndex)
    {
        Debug.Log($"Opening POI navigation for waypoint index: {waypointIndex}");
        navigationController.WaypointMenuScreen.SetActive(false);
        navigationController.NavigationScreen.SetActive(true);
        navigationController.verticalButtonScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(false);

        Waypoint waypoint = navigationController.POIWaypointList[waypointIndex];
        Debug.Log($"Waypoint details: {waypoint.Name}, Type: {waypoint.Type}, IMUposX: {waypoint.IMUposX}, IMUposY: {waypoint.IMUposY}");
    }


    public void openStationNavigation(int waypointIndex)
    {
        Debug.Log($"Opening station navigation for waypoint index: {waypointIndex}");
        navigationController.WaypointMenuScreen.SetActive(false);
        navigationController.NavigationScreen.SetActive(true);
        navigationController.verticalButtonScreen.SetActive(false);
        navigationController.addWaypointButton.SetActive(false);

        Waypoint waypoint = navigationController.StationWaypointList[waypointIndex];
        Debug.Log($"Waypoint details: {waypoint.Name}, Type: {waypoint.Type}, IMUposX: {waypoint.IMUposX}, IMUposY: {waypoint.IMUposY}");
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


    public void navigateToEV(int index)
    {
        // pull up ev2 coords
        // AstronautInstance.User.fellowAstronaut.location.posX
        // AstronautInstance.User.fellowAstronaut.location.posY
        // AstronautInstance.User.fellowAstronaut.location.posZ
        
    }


    public void navigateToPR(int index)
    {
        // pull up pr coords
        // AstronautInstance.User.fellowAstronaut.location.posX
        // AstronautInstance.User.fellowAstronaut.location.posY
        // AstronautInstance.User.fellowAstronaut.location.posZ
;
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
}
