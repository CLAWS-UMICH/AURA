using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaypointSelectionScript : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject InterestButton;
    GameObject SamplesButton;
    GameObject DangerButton;
    GameObject StationButton;
    GameObject HighlightedInterestButton;
    GameObject HighlightedSamplesButton;
    GameObject HighlightedDangerButton;
    GameObject HighlightedStationButton;
    TextMeshPro waypointSubText;

    string gameObjectName;


    public static GameObject FindGameObjectEvenIfInactive(string name)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name && obj.scene.IsValid())
            {
                return obj;
            }
        }
        return null; // Object not found
    }


    void Start()
    {
        InterestButton = FindGameObjectEvenIfInactive("InterestButton");
        DangerButton = FindGameObjectEvenIfInactive("DangerButton");
        SamplesButton = FindGameObjectEvenIfInactive("SamplesButton");
        StationButton = FindGameObjectEvenIfInactive("StationButton");
        HighlightedInterestButton = FindGameObjectEvenIfInactive("HighlightedInterestButton");
        HighlightedSamplesButton = FindGameObjectEvenIfInactive("HighlightedSamplesButton");
        HighlightedDangerButton = FindGameObjectEvenIfInactive("HighlightedDangerButton");
        HighlightedStationButton = FindGameObjectEvenIfInactive("HighlightedStationButton");

        Debug.Log(InterestButton);
        Debug.Log(DangerButton);
        Debug.Log(SamplesButton);
        Debug.Log(StationButton);
        Debug.Log(HighlightedInterestButton);
        Debug.Log(HighlightedDangerButton);
        Debug.Log(HighlightedSamplesButton);
        Debug.Log(HighlightedStationButton);

        waypointSubText = GameObject.Find("WaypointSubText").GetComponent<TextMeshPro>();

        gameObjectName = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        Debug.Log(gameObjectName);
        if(gameObjectName == "SamplesButton" || gameObjectName == "HighlightedSamplesButton")
        {
            InterestButton.SetActive(true);
            DangerButton.SetActive(true);
            SamplesButton.SetActive(false);
            StationButton.SetActive(true);
            HighlightedInterestButton.SetActive(false);
            HighlightedSamplesButton.SetActive(true);
            HighlightedDangerButton.SetActive(false);
            HighlightedStationButton.SetActive(false);
            waypointSubText.text = "Geosamples";
        }
        else if(gameObjectName == "StationButton" || gameObjectName == "HighlightedStationButton")
        {
            InterestButton.SetActive(true);
            DangerButton.SetActive(true);
            SamplesButton.SetActive(true);
            StationButton.SetActive(false);
            HighlightedInterestButton.SetActive(false);
            HighlightedSamplesButton.SetActive(false);
            HighlightedDangerButton.SetActive(false);
            HighlightedStationButton.SetActive(true);
            waypointSubText.text = "Stations";
        }
        else if(gameObjectName == "DangerButton" || gameObjectName == "HighlightedDangerButton")
        {
            InterestButton.SetActive(true);
            DangerButton.SetActive(false);
            SamplesButton.SetActive(true);
            StationButton.SetActive(true);
            HighlightedInterestButton.SetActive(false);
            HighlightedSamplesButton.SetActive(false);
            HighlightedDangerButton.SetActive(true);
            HighlightedStationButton.SetActive(false);
            waypointSubText.text = "Danger";
        }
        else if(gameObjectName == "InterestButton" || gameObjectName == "HighlightedInterestButton")
        {
            InterestButton.SetActive(false);
            DangerButton.SetActive(true);
            SamplesButton.SetActive(true);
            StationButton.SetActive(true);
            HighlightedInterestButton.SetActive(true);
            HighlightedSamplesButton.SetActive(false);
            HighlightedDangerButton.SetActive(false);
            HighlightedStationButton.SetActive(false);
            waypointSubText.text = "Points of Interest";
        }
    }
}