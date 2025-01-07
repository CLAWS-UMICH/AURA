using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCreateWaypointMenu : MonoBehaviour
{
    // Start is called before the first frame update

   List<string> CreateWaypointMenuGameObjectNames = new List<string>
    {
        "MenuBackplate",
        "CloseButton",
        "GeosampleButton",
        "DangerButton",
        "POIButton",
        "HighlightedGeosampleButton",
        "HighlightedDangerButton",
        "HighlightedPOIButton",
        "WaypointNameBar"
    };

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
        return null;
    }

    public void CloseMenu()
    {

        foreach (String name in CreateWaypointMenuGameObjectNames)
        {
            GameObject obj = FindGameObjectEvenIfInactive(name);

            if(name == "Menu")
            {
                Debug.Log(obj);
            }

            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        
    }
}
