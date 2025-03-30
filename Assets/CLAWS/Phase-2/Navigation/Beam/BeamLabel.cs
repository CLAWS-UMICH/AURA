using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BeamLabel : MonoBehaviour
{
    public GameObject beam; // Reference to the beam GameObject
    public Transform player; // Reference to the player/camera
    public float labelHeightFactor = 0.5f; // Height factor (relative to beam scale)
    public float minLabelHeight = 0.2f; // Minimum height above beam
    [SerializeField] private string title = "Beam"; // Editable title in Inspector
    
    // Scaling parameters (should match the beam's parameters)
    public float minDistance = 2.0f;
    public float maxDistance = 20.0f;
    public float minScale = 0.5f;
    public float maxScale = 2.0f;
    
    private GameObject titleTextObject; // Title text object
    private GameObject distanceTextObject; // Distance text object
    private TextMeshPro titleTextMesh;
    private TextMeshPro distanceTextMesh;
    private Vector3 initialLabelScale;
    private Quaternion initialTextRotation;
    private MeshRenderer beamRenderer;
    
    void Start()
    {
        if (player == null)
            player = Camera.main.transform;
            
        if (beam == null)
        {
            Debug.LogError("Beam reference not set in BeamLabel script!");
            return;
        }
        
        // Get the beam's renderer to access its bounds
        beamRenderer = beam.GetComponent<MeshRenderer>();
        if (beamRenderer == null)
        {
            Debug.LogError("Beam does not have a MeshRenderer component!");
        }
        
        // Store the initial scale of this quad (which is the label quad)
        initialLabelScale = transform.localScale;
        
        // Create Title Text as a separate GameObject
        titleTextObject = new GameObject("TitleText");
        titleTextObject.transform.SetParent(transform.parent); // Parent to the same parent as this quad
        titleTextMesh = titleTextObject.AddComponent<TextMeshPro>();
        titleTextMesh.text = title;
        titleTextMesh.fontSize = 4.5f;
        titleTextMesh.alignment = TextAlignmentOptions.Center;
        initialTextRotation = titleTextObject.transform.rotation;
        
        // Create Distance Text as a separate GameObject
        distanceTextObject = new GameObject("DistanceText");
        distanceTextObject.transform.SetParent(transform.parent); // Parent to the same parent as this quad
        distanceTextMesh = distanceTextObject.AddComponent<TextMeshPro>();
        distanceTextMesh.text = "0m"; // Placeholder, will update dynamically
        distanceTextMesh.fontSize = 3f;
        distanceTextMesh.alignment = TextAlignmentOptions.Center;
    }
    
    void Update()
    {
        if (beam != null && player != null && beamRenderer != null)
        {
            // Get the distance between player and beam
            float distance = Vector3.Distance(beam.transform.position, player.position);
            
            // Calculate scale factor using the same logic as BeamScale
            float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
            float scaleFactor = Mathf.Lerp(minScale, maxScale, t);
            
            // Calculate the top position of the beam using its current bounds
            beamRenderer.bounds.Expand(0.01f); // Slightly expand bounds to ensure we get accurate measurements
            Vector3 beamTopPosition = new Vector3(
                beam.transform.position.x,
                beamRenderer.bounds.max.y, // This gives us the top Y position of the beam
                beam.transform.position.z
            );
            
            // Calculate beam height for proportional positioning
            float beamHeight = beamRenderer.bounds.size.y;
            
            // Calculate a proportional height offset based on the beam's current scale
            float dynamicLabelHeight = Mathf.Max(minLabelHeight, beamHeight * labelHeightFactor);
            
            // Position this quad (the label) above the TOP of the beam with proportional height
            transform.position = beamTopPosition + Vector3.up * dynamicLabelHeight;
            
            // Scale this quad using the same scale factor as the beam
            transform.localScale = initialLabelScale * scaleFactor;
            
            // Make this quad face the player (with Y-only rotation like the beam)
            Vector3 directionToCamera = player.position - transform.position;
            directionToCamera.y = 0;
            
            if (directionToCamera != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
                Vector3 eulerAngles = lookRotation.eulerAngles;
                
                transform.rotation = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,
                    eulerAngles.y + 180f,
                    transform.rotation.eulerAngles.z
                );
            }
            
            // Position the title text below this quad with proportional spacing
            titleTextObject.transform.position = transform.position + new Vector3(0, -0.55f * scaleFactor, 0);
            
            // Position the distance text below the title text with proportional spacing
            distanceTextObject.transform.position = transform.position + new Vector3(0, -0.9f * scaleFactor, 0);
            
            // Update the distance text
            distanceTextMesh.text = $"{distance:F2}m";
            
            // Make both text objects face the camera (Y-rotation only)
            Vector3 titleDirToCamera = player.position - titleTextObject.transform.position;
            Vector3 distanceDirToCamera = player.position - distanceTextObject.transform.position;
            
            titleDirToCamera.y = 0;
            distanceDirToCamera.y = 0;
            
            if (titleDirToCamera != Vector3.zero)
            {
                Quaternion titleLookRotation = Quaternion.LookRotation(titleDirToCamera);
                Vector3 titleEulerAngles = titleLookRotation.eulerAngles;
                
                titleTextObject.transform.rotation = Quaternion.Euler(
                    initialTextRotation.eulerAngles.x,
                    titleEulerAngles.y + 180f,
                    initialTextRotation.eulerAngles.z
                );
            }
            
            if (distanceDirToCamera != Vector3.zero)
            {
                Quaternion distanceLookRotation = Quaternion.LookRotation(distanceDirToCamera);
                Vector3 distanceEulerAngles = distanceLookRotation.eulerAngles;
                
                distanceTextObject.transform.rotation = Quaternion.Euler(
                    initialTextRotation.eulerAngles.x,
                    distanceEulerAngles.y + 180f,
                    initialTextRotation.eulerAngles.z
                );
            }
            
            // Scale the text based on the same scale factor
            titleTextObject.transform.localScale = Vector3.one * scaleFactor;
            distanceTextObject.transform.localScale = Vector3.one * scaleFactor;
        }
    }
}
