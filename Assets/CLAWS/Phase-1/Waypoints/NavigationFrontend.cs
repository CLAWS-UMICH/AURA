using System.Collections;
using System.Collections.Generic;
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
    public WaypointWorldSpace waypointWorldSpace;

    void Start()
    {
        if (navigationController == null)
        {
            Debug.LogError("NavigationController is not assigned.");
            return;
        }

        navigationController.navBarToggleCollection.OnToggleSelected.AddListener(OnToggleSelected);
        openCompanionScreen();
    }

    void OnToggleSelected(int index)
    {
        switch (index)
        {
            case 0: openCompanionScreen(); break;
            case 1: openPOIScreen(); break;
            case 2: openStationScreen(); break;
            case 3: openGeoScreen(); break;
            case 4: openDangerScreen(); break;
            default: Debug.LogWarning("Invalid toggle index: " + index); break;
        }
    }


    public void addingWaypoint()
    {
        // waypointWorldSpace.At();
    }


    public void openCompanionScreen()
    {
        navigationController.EVmap.SetActive(false);
        navigationController.FullMap.SetActive(true);
        SetScreen(navigationController.CompanionScreen);
    }


    public void openPOIScreen()
    {
        navigationController.EVmap.SetActive(true);
        navigationController.FullMap.SetActive(false);
        SetScreen(navigationController.POIScreen);
    }


    public void openStationScreen()
    {
        navigationController.EVmap.SetActive(true);
        navigationController.FullMap.SetActive(false);
        SetScreen(navigationController.StationScreen);
    }

    public void openGeoScreen()
    {
        navigationController.EVmap.SetActive(true);
        navigationController.FullMap.SetActive(false);
        SetScreen(navigationController.GeoScreen);
        geoButtonPressed = true;
        poiButtonPressed = false;
        dangerButtonPressed = false;
    }

    public void openDangerScreen()
    {
        navigationController.EVmap.SetActive(true);
        navigationController.FullMap.SetActive(false);
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
        geoButtonPressed = true;
        poiButtonPressed = false;
        dangerButtonPressed = false;
    }

    public void poiButton()
    {
        poiButtonPressed = true;
        geoButtonPressed = false;
        dangerButtonPressed = false;
    }

    public void dangerButton()
    {
        dangerButtonPressed = true;
        geoButtonPressed = false;
        poiButtonPressed = false;
    }


    public void openFeatureScreen()
    {
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
        navigationController.Controller.SetActive(true);
    }


    public void addWaypoint()
    {
        if ((dangerButtonPressed || geoButtonPressed || poiButtonPressed) && (IMUposX != 0 && IMUposY != 0))
        {
            var nameField = navigationController.CreateWaypointScreen.transform.GetChild(4).GetChild(3).GetComponent<TMP_Text>();
            Waypoint newWaypoint = new Waypoint
            {
                Use = "ADD",
                Id = navigationController.waypointList.Count + 1,
                Name = nameField.text,
                IMUposX = IMUposX,
                IMUposY = IMUposY,
                Type = dangerButtonPressed ? WaypointType.DANGER : geoButtonPressed ? WaypointType.GEO : WaypointType.POI,
                Author = AstronautInstance.User.id == 1 ? AuthorType.EV1 : AuthorType.EV2,
            };
            EventBus.Publish(new WaypointAddedEvent(newWaypoint));
        }
        else
        {
            Debug.Log("Waypoint not added. Please fill in all fields.");
        }
    }

    public void closeScreens()
    {
        navigationController.Controller.SetActive(false);
        navigationController.MainMenuToggleCollection.SetSelection(1, false);
    }
}
