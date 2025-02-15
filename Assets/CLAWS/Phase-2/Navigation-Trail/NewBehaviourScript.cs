using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json; // Requires Newtonsoft.Json package

public class CoordinateLoader : MonoBehaviour
{
    // Struct to store lat/long values
    [System.Serializable]
    public struct Coordinate
    {
        public float lat;
        public float @long; // "long" is a reserved keyword, so use "@long"
    }

    // Dictionary to store coordinates by name
    private Dictionary<string, Coordinate> coordinates;

    public GameObject targetObject; // Assign in Inspector
    public string coordinateKey = "topLeft"; // Choose which coordinate to use

    void Start()
    {
        // Hardcoded JSON data (for testing)
        string json = @"
        {
            ""topLeft"": { ""lat"": 29.565369133556835, ""long"": -95.0819529674787 },
            ""bottomRight"": { ""lat"": 29.56440830845782, ""long"": -95.08071056957434 },
            ""bottomLeftSquare"": { ""lat"": 29.564939230058076, ""long"": -95.08120752873609 },
            ""topRightSquare"": { ""lat"": 29.565157705835315, ""long"": -95.08070786870931 }
        }";

        // Parse JSON into dictionary
        coordinates = JsonConvert.DeserializeObject<Dictionary<string, Coordinate>>(json);

        // Set the GameObject position using a specific coordinate key
        if (coordinates.ContainsKey(coordinateKey))
        {
            SetGameObjectPosition(coordinates[coordinateKey]);
        }
        else
        {
            Debug.LogError($"Coordinate key '{coordinateKey}' not found in JSON!");
        }
    }

    void SetGameObjectPosition(Coordinate coord)
    {
        if (targetObject == null)
        {
            Debug.LogError("Target GameObject not assigned!");
            return;
        }

        // Convert lat/long to Unity world coordinates (for now, using lat as X and long as Z)
        float x = coord.lat;
        float z = coord.@long;
        float y = targetObject.transform.position.y; // Keep existing Y position

        targetObject.transform.position = new Vector3(x, y, z);

        Debug.Log($"GameObject moved to: X = {x}, Y = {y}, Z = {z}");
    }
}


