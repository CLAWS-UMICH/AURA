using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.UX;
using TMPro;
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

    void Start()
    {
        if (navigationController == null)
        {
            Debug.LogError("NavigationController is not assigned.");
            return;
        }

        Debug.Log("NavigationFrontend initialized.");
        navigationController.navBarToggleCollection.OnToggleSelected.AddListener(OnToggleSelected);
        openCompanionScreen();
    }

    void OnToggleSelected(int index)
    {
        Debug.Log($"Toggle selected: {index}");
        switch (index)
        {
            case 0: openCompanionScreen(); break;
            case 1: openPOIScreen(); break;
            case 2: openStationScreen(); break;
            case 3: openGeoScreen(); break;
            case 4: openDangerScreen(); break;
        }
    }

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
            Waypoint newWaypoint = new Waypoint
            {
                Use = "ADD",
                Id = navigationController.waypointList.Count + 1,
                Name = newWaypointMarker.name,
                IMUposX = newWaypointMarker.transform.position.x + AstronautInstance.User.origin.posX,
                IMUposY = newWaypointMarker.transform.position.z + AstronautInstance.User.origin.posY,
                Type = dangerButtonPressed ? WaypointType.DANGER : geoButtonPressed ? WaypointType.GEO : WaypointType.POI,
                Author = AstronautInstance.User.id == 1 ? AuthorType.EV1 : AuthorType.EV2,
            };

            Debug.Log($"New waypoint created: {newWaypoint.Name}, Type: {newWaypoint.Type}, IMUposX: {newWaypoint.IMUposX}, IMUposY: {newWaypoint.IMUposY}");
            EventBus.Publish(new WaypointAddedEvent(newWaypoint));
        });
    }

    public void UpdateActiveMarker(GameObject newMarker)
    {
        Debug.Log("Updating active marker...");
        geoMarker = newMarker.transform.GetChild(2).GetChild(1).GetChild(2).gameObject;
        dangerMarker = newMarker.transform.GetChild(2).GetChild(1).GetChild(3).gameObject;
        poiMarker = newMarker.transform.GetChild(2).GetChild(1).GetChild(4).gameObject;

        Debug.Log($"geoButtonPressed: {geoButtonPressed}, dangerButtonPressed: {dangerButtonPressed}, poiButtonPressed: {poiButtonPressed}");

        if (geoButtonPressed)
        {
            geoMarker.SetActive(true);
            dangerMarker.SetActive(false);
            poiMarker.SetActive(false);
            Debug.Log("Geo marker activated.");
        }
        else if (dangerButtonPressed)
        {
            geoMarker.SetActive(false);
            dangerMarker.SetActive(true);
            poiMarker.SetActive(false);
            Debug.Log("Danger marker activated.");
        }
        else if (poiButtonPressed)
        {
            geoMarker.SetActive(false);
            dangerMarker.SetActive(false);
            poiMarker.SetActive(true);
            Debug.Log("POI marker activated.");
        }
    }

    public void openCompanionScreen()
    {
        navigationController.poiIconParent.SetActive(false);
        navigationController.dangerIconParent.SetActive(false);
        navigationController.geoIconParent.SetActive(false);
        navigationController.stationIconParent.SetActive(false);
        navigationController.poiClosedIconParent.SetActive(true);
        navigationController.dangerClosedIconParent.SetActive(true);
        navigationController.geoClosedIconParent.SetActive(true);
        navigationController.stationClosedIconParent.SetActive(true);

        navigationController.ev2Icon_full.SetActive(true);
        navigationController.ev2Icon_small.SetActive(false);
        navigationController.roverIcon_full.SetActive(true);

        navigationController.EVmap.SetActive(false);
        navigationController.FullMap.SetActive(true);
        navigationController.EVmapDangerZones.SetActive(false);
        SetScreen(navigationController.CompanionScreen);
    }

    // helper for opening screens -- not frontend 
    private void openEvMapScreens()
    {
        navigationController.ev2Icon_full.SetActive(false);
        navigationController.ev2Icon_small.SetActive(true);
        navigationController.roverIcon_full.SetActive(false);
    }

    
    public void openPOIScreen()
    {
        navigationController.poiIconParent.SetActive(true);
        navigationController.dangerIconParent.SetActive(false);
        navigationController.geoIconParent.SetActive(false);
        navigationController.stationIconParent.SetActive(false);
        navigationController.poiClosedIconParent.SetActive(false);
        navigationController.dangerClosedIconParent.SetActive(true);
        navigationController.geoClosedIconParent.SetActive(true);
        navigationController.stationClosedIconParent.SetActive(true);
       
        openEvMapScreens();

        navigationController.EVmap.SetActive(true);
        navigationController.FullMap.SetActive(false);
        navigationController.EVmapDangerZones.SetActive(false);
        SetScreen(navigationController.POIScreen);
    }


    public void openStationScreen()
    {
        navigationController.poiIconParent.SetActive(false);
        navigationController.dangerIconParent.SetActive(false);
        navigationController.geoIconParent.SetActive(false);
        navigationController.stationIconParent.SetActive(true);
        navigationController.poiClosedIconParent.SetActive(true);
        navigationController.dangerClosedIconParent.SetActive(true);
        navigationController.geoClosedIconParent.SetActive(true);
        navigationController.stationClosedIconParent.SetActive(false);
       
        openEvMapScreens();

        navigationController.EVmap.SetActive(true);
        navigationController.FullMap.SetActive(false);
        navigationController.EVmapDangerZones.SetActive(false);
        SetScreen(navigationController.StationScreen);
    }

    public void openGeoScreen()
    {
        navigationController.poiIconParent.SetActive(false);
        navigationController.dangerIconParent.SetActive(false);
        navigationController.geoIconParent.SetActive(true);
        navigationController.stationIconParent.SetActive(false);
        navigationController.poiClosedIconParent.SetActive(true);
        navigationController.dangerClosedIconParent.SetActive(true);
        navigationController.geoClosedIconParent.SetActive(false);
        navigationController.stationClosedIconParent.SetActive(true);
        
        openEvMapScreens();

        navigationController.EVmap.SetActive(true);
        navigationController.FullMap.SetActive(false);
        navigationController.EVmapDangerZones.SetActive(false);
        SetScreen(navigationController.GeoScreen);
        geoButtonPressed = true;
        poiButtonPressed = false;
        dangerButtonPressed = false;
    }

    public void openDangerScreen()
    {
        navigationController.poiIconParent.SetActive(false);
        navigationController.dangerIconParent.SetActive(true);
        navigationController.geoIconParent.SetActive(false);
        navigationController.stationIconParent.SetActive(false);
        navigationController.poiClosedIconParent.SetActive(true);
        navigationController.dangerClosedIconParent.SetActive(false);
        navigationController.geoClosedIconParent.SetActive(true);
        navigationController.stationClosedIconParent.SetActive(true);
       
        openEvMapScreens();

        navigationController.EVmap.SetActive(false);
        navigationController.FullMap.SetActive(false);
        navigationController.EVmapDangerZones.SetActive(true);
        SetScreen(navigationController.DangerScreen);
        dangerButtonPressed = true;
        geoButtonPressed = false;
        poiButtonPressed = false;
    }

    void SetScreen(GameObject screenToShow)
    {
        navigationController.CompanionScreen.SetActive(screenToShow == navigationController.CompanionScreen);
        navigationController.POIScreen.SetActive(screenToShow == navigationController.POIScreen);
        navigationController.StationScreen.SetActive(screenToShow == navigationController.StationScreen);
        navigationController.GeoScreen.SetActive(screenToShow == navigationController.GeoScreen);
        navigationController.DangerScreen.SetActive(screenToShow == navigationController.DangerScreen);

        activeScreen = screenToShow;
    }


    public void openWaypointScreen()
    {
        navigationController.CreateWaypointScreen.SetActive(true);
        navigationController.CompanionScreen.transform.parent.gameObject.SetActive(false);
        navigationController.NavigationScreen.SetActive(false);
    }


    public void openNavigationScreen()
    {
        navigationController.NavigationScreen.SetActive(true);
        navigationController.CreateWaypointScreen.transform.parent.gameObject.SetActive(false);
        navigationController.CompanionScreen.SetActive(false);
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
        navigationController.Controller.SetActive(true);
        navigationController.CreateWaypointScreen.SetActive(false);
        navigationController.WaypointMenuScreen.SetActive(true);
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


    public void navigateToEV()
    {
        // pull up ev2 coords
        // AstronautInstance.User.fellowAstronaut.location.posX
        // AstronautInstance.User.fellowAstronaut.location.posY
        // AstronautInstance.User.fellowAstronaut.location.posZ
        openNavigationScreen();
    }


    public void navigateToPR()
    {
        // pull up pr coords
        // AstronautInstance.User.fellowAstronaut.location.posX
        // AstronautInstance.User.fellowAstronaut.location.posY
        // AstronautInstance.User.fellowAstronaut.location.posZ
        openNavigationScreen();
    }


    public void closeScreens()
    {
        navigationController.Controller.SetActive(false);
        navigationController.MainMenuToggleCollection.SetSelection(1, false);
    }
}
