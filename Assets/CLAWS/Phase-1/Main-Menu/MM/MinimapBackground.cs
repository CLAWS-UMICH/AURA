using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapBackgroundController : MonoBehaviour
{
    public RectTransform backgroundImage; // Background RectTransform
    public Transform player;              // Reference to the player
    public float mapScale = 0.1f;         // Scale factor for movement

    void Update()
    {
        if (backgroundImage == null || player == null) return;

        // **Position Update**
        // Convert player's world position to a 2D position for the minimap
        Vector2 playerPosition = new Vector2(player.position.x, player.position.z);

        // Offset the background's position to simulate player movement
        Vector2 backgroundPosition = -playerPosition * mapScale;

        // Apply the new position to the background image
        backgroundImage.anchoredPosition = backgroundPosition;

        // **Rotation Update**
        // Rotate the map in the opposite direction of the player to create realistic minimap behavior
        float playerYRotation = player.eulerAngles.y;
        backgroundImage.localRotation = Quaternion.Euler(0, 0, playerYRotation);

        //Debug.Log($"Player Position: {playerPosition}, Background Position: {backgroundImage.anchoredPosition}, Rotation: {backgroundImage.localRotation}");
    }
}
