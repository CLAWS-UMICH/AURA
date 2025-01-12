using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPop : MonoBehaviour
{
    private List<string> prefabTypes; 
    public GameObject[] prefabs;
    public Transform contentParent;

    void Start()
    {
        InitializePrefabTypes();
        PopulateContent();
    }

    void InitializePrefabTypes()
    {
        // Add the prefab types to the list
        prefabTypes = new List<string>
        {
            "TaskPrefab (geo solo)",
            "TaskPrefab (repair solo)",
            "TaskPrefab (emergency solo)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)"
        };
    }

    void PopulateContent()
    {
        foreach (string type in prefabTypes)
        {
            // Find the corresponding prefab for the type
            GameObject prefab = GetPrefabByType(type);

            if (prefab != null)
            {
                // Instantiate the prefab and add it to the Content area
                GameObject newItem = Instantiate(prefab, contentParent);
            }
            else
            {
                Debug.LogWarning($"Prefab for type '{type}' not found!");
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
        return null; // Return null if no matching prefab is found
    }
}
