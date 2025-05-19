using UnityEngine;

public class PRMinimap : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;        // Reference to the player's transform
    public Camera minimapCamera;            // Reference to the minimap camera
    public Transform playerIcon;            // Reference to the player icon on the minimap
    public GameObject maskObject;           // Reference to the mask GameObject
    public GameObject borderObject;         // Reference to the border GameObject

    [Header("Settings")]
    public float cameraHeight = 50f;        // Height of the minimap camera
    public bool rotateWithPlayer = true;    // Whether the minimap should rotate with the player

    private void Start()
    {
        // Ensure the minimap camera is properly positioned
        if (minimapCamera != null)
        {
            minimapCamera.transform.position = new Vector3(0, cameraHeight, 0);
            minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    private void Update()
    {
        if (playerTransform == null || minimapCamera == null) return;

        // Update camera position to follow player
        Vector3 newCameraPosition = new Vector3(
            playerTransform.position.x,
            cameraHeight,
            playerTransform.position.z
        );
        minimapCamera.transform.position = newCameraPosition;

        // Update camera rotation if enabled
        if (rotateWithPlayer)
        {
            minimapCamera.transform.rotation = Quaternion.Euler(90f, playerTransform.eulerAngles.y, 0f);
        }

        // Update player icon position and rotation
        if (playerIcon != null)
        {
            // Position the icon at the center of the minimap
            playerIcon.position = new Vector3(
                playerTransform.position.x,
                playerIcon.position.y,
                playerTransform.position.z
            );

            // Rotate the icon to match player's rotation
            playerIcon.rotation = Quaternion.Euler(90f, 0f, -playerTransform.eulerAngles.y);
        }
    }
} 