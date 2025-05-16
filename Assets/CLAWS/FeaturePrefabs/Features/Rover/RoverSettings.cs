using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverSettings : MonoBehaviour
{
    public GameObject roverSettingsScreen;
    [Header("Toggle Icons")]
    public GameObject messagingToggle;
    public GameObject waypointToggle;
    public GameObject samplingToggle;
    [Header("Map")]
    public GameObject map;


    void Start()
    {


    }

    public void openRoverSettingsScreen()
    {
        roverSettingsScreen.SetActive(true);
        foreach(Transform child in roverSettingsScreen.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    public void closeRoverSettingsScreen()
    {
        roverSettingsScreen.SetActive(false);
        foreach(Transform child in roverSettingsScreen.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
