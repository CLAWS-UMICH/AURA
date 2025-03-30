using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CompanionScript : MonoBehaviour
{
    public float hoverHeight = 0.5f;
    public GameObject companionQuad; // The circular quad
    public Transform waypoint; // The position the quad hovers above
    public Transform target; // The target (e.g., player camera)
    public Camera mainCamera; // Reference to the main camera
    [SerializeField] private string title = "Companion"; // Editable title in Inspector

    private GameObject titleTextObject; // Title text object
    private GameObject distanceTextObject; // Distance text object
    private TextMeshPro titleTextMesh;
    private TextMeshPro distanceTextMesh;

    void Start()
    {
        // Create Title Text
        titleTextObject = new GameObject("TitleText");
        titleTextObject.transform.SetParent(companionQuad.transform);
        titleTextMesh = titleTextObject.AddComponent<TextMeshPro>();
        titleTextMesh.text = title;
        titleTextMesh.fontSize = 4.5f;
        titleTextMesh.alignment = TextAlignmentOptions.Center;

        // Position Title Text just below the circular quad
        titleTextObject.transform.localPosition = new Vector3(0, -0.35f, 0);

        // Create Distance Text
        distanceTextObject = new GameObject("DistanceText");
        distanceTextObject.transform.SetParent(companionQuad.transform);
        distanceTextMesh = distanceTextObject.AddComponent<TextMeshPro>();
        distanceTextMesh.text = "0m"; // Placeholder, will update dynamically
        distanceTextMesh.fontSize = 3f;
        distanceTextMesh.alignment = TextAlignmentOptions.Center;

        // Position Distance Text below the Title Text
        distanceTextObject.transform.localPosition = new Vector3(0, -0.5f, 0);
    }

    void Update()
    {
        if (target != null && companionQuad != null)
        {
            // Position above waypoint
            companionQuad.transform.position = waypoint.position + Vector3.up * hoverHeight;

            // Quad orientation faces target
            companionQuad.transform.LookAt(2 * companionQuad.transform.position - target.position);

            // Update Distance Text dynamically based on the distance from the main camera
            float distanceToCamera = Vector3.Distance(companionQuad.transform.position, mainCamera.transform.position);
            distanceTextMesh.text = $"{distanceToCamera:F2}m"; // Format to 2 decimal places

            // Make both text objects face the camera
            titleTextObject.transform.rotation = Quaternion.LookRotation(titleTextObject.transform.position - mainCamera.transform.position);
            distanceTextObject.transform.rotation = Quaternion.LookRotation(distanceTextObject.transform.position - mainCamera.transform.position);
        }
    }
}
