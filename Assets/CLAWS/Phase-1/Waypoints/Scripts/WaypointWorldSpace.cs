using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.Input;
using UnityEngine;

public class WaypointWorldSpace : MonoBehaviour
{
    [SerializeField] private GameObject waypointMarkerPrefab;
    public FuzzyGazeInteractor gazeInteractor;
    public NavigationFrontend navigationFrontend;

    public GameObject currentCircle;
    private bool isPlaced = false;


    void Update()
    {
        if (currentCircle.activeSelf && !isPlaced)
        {
            RaycastHit hit = gazeInteractor.PreciseHitResult.raycastHit;
            // Vector3 groundPosition = GetGroundPosition(hit.point);
            currentCircle.transform.position = hit.point;
        }
        if (Input.GetKeyDown(KeyCode.Space) && !isPlaced)
        {
            PlaceWaypoint();
            isPlaced = true;
        }
    }

    public void PlaceWaypoint()
    {
        isPlaced = true;
        Debug.Log("Waypoint placed at: " + currentCircle.transform.position);
        navigationFrontend.IMUposX = currentCircle.transform.position.x + AstronautInstance.User.origin.posX;
        navigationFrontend.IMUposY = currentCircle.transform.position.z + AstronautInstance.User.origin.posY;
        navigationFrontend.addWaypoint();
    }

    // Helper: cast downward ray to find ground from given position
    private Vector3 GetGroundPosition(Vector3 position)
    {
        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 10f))
        {
            return hit.point;
        }
        else
        {
            position.y -= 1f;
            Debug.LogWarning("No ground detected, returning original position with Y offset.");
            return position;
        }
    }
}

