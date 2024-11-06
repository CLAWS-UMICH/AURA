using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSStartScreenHandler : MonoBehaviour
{
    GameObject optionScreen;
    ResetGPSSpawn resetGPSSpawn;

    // Start is called before the first frame update
    void Start()
    {
        resetGPSSpawn = GameObject.Find("Controller").gameObject.GetComponent<ResetGPSSpawn>();
        optionScreen = gameObject.transform.Find("ResetOptionScreen").gameObject;
        optionScreen.SetActive(false);
    }

    public void OpenOptionScreen()
    {
        optionScreen.SetActive(true);
    }

    public void CloseOptionScreen()
    {
        optionScreen.SetActive(false);
    }

    public void TSSLocation()
    {

    }

    public void TruckLocation()
    {
        Location loc = new Location(29.56459834, -95.08144150); // Location of norht of the truck

        resetGPSSpawn.Reset(loc);
    }
}
