using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraScript : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        // Update the rotation of the minimap camera to match the player's Y-axis rotation
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
