using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaypointsDynamicPop : MonoBehaviour
{
    private Dictionary<string, string> prefabNames;
    //private List<TaskObj> localTL;
    public GameObject[] prefabs;
    public Transform contentParent;
    [SerializeField] private List<GameObject> currentWaypoints = new List<GameObject>();

    // additions for new TL
    [SerializeField] private static List<Waypoint> waypointsList = Waypoints.WaypointsList;
    [SerializeField] private WaypointsMenuController menuController;

    void Start()
    {
        menuController = transform.parent.parent.parent.GetComponent<WaypointsMenuController>();
        InitializePrefabTypes();
        PopulateContent("Companion");
    }

    private void Update()
    {
        // FIXME: Optimize so the list is not constantly updating (only update when waypoint is added, edited, or deleted)
        waypointsList = Waypoints.WaypointsList;
    }

    void InitializePrefabTypes()
    {
        // mapping types and shared status to names of prefabs for generation
        prefabNames = new Dictionary<string, string>();
        prefabNames["Companion"] = "CompanionWaypointListIcon";
        prefabNames["Station"] = "StationWaypointListIcon";
        prefabNames["Sample"] = "SampleWaypointListIcon";
        prefabNames["Danger"] = "DangerWaypointListIcon";
        prefabNames["Interest"] = "InterestWaypointListIcon";
    }

    public void PopulateContent(string waypointType)
    {
        // Erase all previous waypoints
        foreach (GameObject waypoint in currentWaypoints)
        {
            Destroy(waypoint);
        }
        currentWaypoints.Clear();

        // Populate new waypoints
        if (waypointsList == null)
        {
            return;
        }

        float offset = 0f;
        foreach (Waypoint waypoint in waypointsList)
        {
            if (waypoint == null || waypointType != waypoint.Type)
            {
                continue;
            }

            // Find the corresponding prefab for the type
            GameObject prefab = GetPrefabByType(prefabNames[waypoint.Type]);
            if (prefab != null)
            {
                // Instantiate the prefab and add it to the Content area
                GameObject newWaypoint = Instantiate(prefab, contentParent);
                newWaypoint.transform.localPosition = new Vector3(0.527f, -0.115f + offset, -0.5f);
                offset += -0.26f;
                //newWaypoint.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
                newWaypoint.transform.Find("CompressableButtonVisuals").Find("IconAndText").Find("WaypointName").GetComponent<TextMeshPro>().text = waypoint.Name;
                newWaypoint.transform.Find("CompressableButtonVisuals").Find("IconAndText").Find("WaypointLabel").GetComponent<TextMeshPro>().text = waypoint.Letter;
                newWaypoint.GetComponent<MixedReality.Toolkit.UX.PressableButton>().OnClicked.AddListener(() => menuController.onClickWaypoint(waypoint));
                currentWaypoints.Add(newWaypoint);
                Debug.Log("waypoint button instantiated");
            }
            else
            {
                Debug.LogWarning($"Prefab not found!");
            }
        }
    }

    GameObject GetPrefabByType(string type)
    {
        // Find the prefab that matches the type
        foreach (GameObject prefab in prefabs)
        {
            if (prefab.name == type)
            {
                return prefab;
            }
        }
        // Return null if no matching prefab is found
        return null;
    }
}
