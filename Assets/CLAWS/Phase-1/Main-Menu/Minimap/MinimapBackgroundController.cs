using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapBackgroundController : MonoBehaviour
{
    public Transform playerIcon; // Reference to the player icon (at 0,0,0 on the map)
    public Camera minimapCamera; // Reference to the minimap camera

    void Update()
    {
       Vector3 newCameraPosition = new Vector3(playerIcon.position.x, minimapCamera.transform.position.y, playerIcon.position.z);
        minimapCamera.transform.position = newCameraPosition;
        minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
