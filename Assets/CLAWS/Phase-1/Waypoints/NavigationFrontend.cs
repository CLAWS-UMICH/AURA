using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.UX;
using UnityEngine;

public class NavigationFrontend : MonoBehaviour
{
    NavigationController navigationController;

    void Start()
    {
        navigationController.navBarToggleCollection.OnToggleSelected.AddListener(OnToggleSelected);
        openCompanionScreen();
    }

    void OnToggleSelected(int index)
    {
        // Map the toggled index to the corresponding screen
        switch (index)
        {
            case 0:
                openCompanionScreen();
                break;
            case 1:
                openPOIScreen();
                break;
            case 2:
                openStationScreen();
                break;
            case 3:
                openGeoScreen();
                break;
            case 4:
                openDangerScreen();
                break;
            default:
                Debug.LogWarning("Invalid toggle index: " + index);
                break;
        }
    }


    void openCompanionScreen()
    {
        navigationController.CompanionScreen.SetActive(true);
        navigationController.POIScreen.SetActive(false);
        navigationController.StationScreen.SetActive(false);
        navigationController.GeoScreen.SetActive(false);
        navigationController.DangerScreen.SetActive(false);
    }


    void openPOIScreen()
    {
        navigationController.CompanionScreen.SetActive(false);
        navigationController.POIScreen.SetActive(true);
        navigationController.StationScreen.SetActive(false);
        navigationController.GeoScreen.SetActive(false);
        navigationController.DangerScreen.SetActive(false);
    }


    void openStationScreen()
    {
        navigationController.CompanionScreen.SetActive(false);
        navigationController.POIScreen.SetActive(false);
        navigationController.StationScreen.SetActive(true);
        navigationController.GeoScreen.SetActive(false);
        navigationController.DangerScreen.SetActive(false);
    }


    void openGeoScreen()
    {
        navigationController.CompanionScreen.SetActive(false);
        navigationController.POIScreen.SetActive(false);
        navigationController.StationScreen.SetActive(false);
        navigationController.GeoScreen.SetActive(true);
        navigationController.DangerScreen.SetActive(false);
    }


    void openDangerScreen()
    {
        navigationController.CompanionScreen.SetActive(false);
        navigationController.POIScreen.SetActive(false);
        navigationController.StationScreen.SetActive(false);
        navigationController.GeoScreen.SetActive(false);
        navigationController.DangerScreen.SetActive(true);
    }


    void closeScreens() 
    {
        navigationController.Controller.SetActive(false);
        navigationController.MainMenuToggleCollection.SetSelection(1, false);
    }


}
