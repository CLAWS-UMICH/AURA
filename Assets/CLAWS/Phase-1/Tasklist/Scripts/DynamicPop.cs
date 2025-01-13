using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPop : MonoBehaviour
{
    private List<string> prefabTypes;
    private List<string> prefabTypes1;
    public GameObject[] prefabs;
    public Transform contentParent;

    void Awake()
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
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)"
        };

        prefabTypes1 = new List<string>
        {
            "TaskPrefab (geo solo)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
            "SubaskPrefab (emergency)",
        };
    }

    void PopulateContent()
    {
        foreach (string type in prefabTypes1)
        {
            // Find the corresponding prefab for the type
            GameObject prefab = GetPrefabByType(type);

            if (prefab != null)
            {
                // Instantiate the prefab and add it to the Content area
                GameObject newItem = Instantiate(prefab, contentParent);
                newItem.transform.localPosition = new Vector3(contentParent.localPosition.x, contentParent.localPosition.y, -0.01f);
                newItem.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
            }
            else
            {
                Debug.LogWarning($"Prefab for type '{type}' not found!");
            }    
        }

        EventBus.Publish(new InitPopFinishedEvent("i just published" + prefabTypes[0]));
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
