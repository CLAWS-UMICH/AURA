using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRWaypoint : MonoBehaviour
{
    [SerializeField]private GameObject toggleObject; 
    [SerializeField]private LMCCWebSocketClient webSocketClient;

    private Subscription<WaypointAddedEvent> waypointEvent;
    private Waypoint currentWaypoint;

    void Start()
    {
        waypointEvent = EventBus.Subscribe<WaypointAddedEvent>(OnWaypointAdded);
    }

    public void togglePRWaypoint() 
    {
        if (webSocketClient == null)
        {
            Debug.LogError("WebSocketClient reference is missing!");
            return;
        }

        if (currentWaypoint == null)
        {
            Debug.LogWarning("No waypoint data available to toggle");
            return;
        }

        Dictionary<string, object> waypointData = new Dictionary<string, object>
        {
            { "Id", currentWaypoint.Id },
            { "Name", currentWaypoint.Name },
            { "IMUposX", currentWaypoint.IMUposX },
            { "IMUposY", currentWaypoint.IMUposY },
            { "Type", currentWaypoint.Type },
            { "Author", currentWaypoint.Author },
            { "Use", "ADD" }
        };

        webSocketClient.SendJsonData(waypointData, "WAYPOINTS", 3); // 3 is for PR client
    }

    private void OnWaypointAdded(WaypointAddedEvent e) 
    {
        currentWaypoint = e.NewAddedWaypoint;
        Debug.Log("Waypoint subscribed");
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(waypointEvent);
    }
}
