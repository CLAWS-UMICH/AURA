using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.UX;
using UnityEngine;

public class RoverSettings : MonoBehaviour
{
    public GameObject roverSettingsScreen;
    public GameObject imu_pos;
    public ToggleCollection menuToggleCollection;
    [Header("Toggle Icons")]
    public GameObject messagingToggle;
    public GameObject waypointToggle;
    public GameObject samplingToggle;
    [Header("Map")]
    public GameObject map;



    public void openFeatureScreen()
    {
        roverSettingsScreen.SetActive(true);
        foreach (Transform child in roverSettingsScreen.transform)
        {
            child.gameObject.SetActive(true);
        }
        menuToggleCollection.SetSelection(5);
    }


    public void closeRoverSettingsScreen()
    {
        roverSettingsScreen.SetActive(false);
        foreach (Transform child in roverSettingsScreen.transform)
        {
            child.gameObject.SetActive(false);
        }
        menuToggleCollection.SetSelection(6);
    }
}
