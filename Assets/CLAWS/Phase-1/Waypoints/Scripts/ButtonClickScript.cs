using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonClickScript : MonoBehaviour
{
    GameObject GeosampleButton;
    GameObject DangerButton;
    GameObject POIButton;
    GameObject HighlightedGeosampleButton;
    GameObject HighlightedDangerButton;
    GameObject HighlightedPOIButton;
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
        GeosampleButton = FindGameObjectEvenIfInactive("GeosampleButton");
        DangerButton = FindGameObjectEvenIfInactive("DangerButton");
        POIButton = FindGameObjectEvenIfInactive("POIButton");
        HighlightedGeosampleButton = FindGameObjectEvenIfInactive("HighlightedGeosampleButton");
        HighlightedDangerButton = FindGameObjectEvenIfInactive("HighlightedDangerButton");
        HighlightedPOIButton = FindGameObjectEvenIfInactive("HighlightedPOIButton");

        gameObjectName = gameObject.name;

        Debug.Log(gameObjectName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {

        if(gameObjectName == "GeosampleButton")
        {
            gameObject.SetActive(false);
            DangerButton.SetActive(true);
            POIButton.SetActive(true);
            HighlightedGeosampleButton.SetActive(true);
            HighlightedDangerButton.SetActive(false);
            HighlightedPOIButton.SetActive(false);
        }
        else if(gameObjectName == "DangerButton")
        {
            gameObject.SetActive(false);
            GeosampleButton.SetActive(true);
            POIButton.SetActive(true);
            HighlightedGeosampleButton.SetActive(false);
            HighlightedDangerButton.SetActive(true);
            HighlightedPOIButton.SetActive(false);
        }
        else if(gameObjectName == "POIButton")
        {
            gameObject.SetActive(false);
            GeosampleButton.SetActive(true);
            DangerButton.SetActive(true);
            HighlightedGeosampleButton.SetActive(false);
            HighlightedDangerButton.SetActive(false);
            HighlightedPOIButton.SetActive(true);
        }
        else if(gameObjectName == "HighlightedGeosampleButton" || gameObjectName == "HighlightedDangerButton" || gameObjectName == "HighlightedPOIButton")
        {
            GeosampleButton.SetActive(true);
            DangerButton.SetActive(true);
            POIButton.SetActive(true);  
            HighlightedGeosampleButton.SetActive(false);
            HighlightedDangerButton.SetActive(false);
            HighlightedPOIButton.SetActive(false);
        }
    }
}
