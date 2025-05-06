using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    [SerializeField] private GameObject dangerPrefab;
    [SerializeField] private GameObject geoPrefab;
    [SerializeField] private GameObject stationPrefab;
    [SerializeField] private GameObject poiPrefab;
    [SerializeField] private GameObject Ev2;
    [SerializeField] private GameObject Rover;

    // add if 3d map added
    // [SerializeField] private GameObject dangerPrefab_3D;
    // [SerializeField] private GameObject geoPrefab_3D;
    // [SerializeField] private GameObject stationPrefab_3D;
    // [SerializeField] private GameObject poiPrefab_3D;
    // [SerializeField] private GameObject companionPrefab_3D;

    private Subscription<WaypointAddedEvent> waypointAddedSubscription;
    private Subscription<WaypointDeletedEvent> waypointRemovedSubscription;
    priv


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
