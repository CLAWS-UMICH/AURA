// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class WaypointsBackend : MonoBehaviour
// {
//     public List<Waypoint> WaypointsList = new List<Waypoint>();

//     private Subscription<WaypointAddedEvent> waypointAddedEvent;
//     private Subscription<WaypointsEditedEvent> waypointEditedEvent;
//     private Subscription<WaypointDeletedEvent> waypointDeletedEvent;

//     [SerializeField] private WebSocketClient webSocketClient;

//     // Start is called before the first frame update
//     void Start()
//     {
//         waypointAddedEvent = EventBus.Subscribe<WaypointAddedEvent>(AddWaypoint);
//         waypointEditedEvent = EventBus.Subscribe<WaypointsEditedEvent>(EditWaypoint);
//         waypointDeletedEvent = EventBus.Subscribe<WaypointDeletedEvent>(DeleteWaypoint);
//     }

//      public void AddWaypoint(WaypointAddedEvent e)
//     {
//         Waypoint newWaypoint = e.NewAddedWaypoint;
//         WaypointsList.Add(newWaypoint);

//         string json = JsonUtility.ToJson(newWaypoint);
//         webSocketClient.SendJsonData(json, "WAYPOINTS");
//         Debug.Log($"Waypoint added: {newWaypoint.Title}");
//     }

//     public void EditWaypoint(WaypointsEditedEvent e)
//     {
//         Waypoint waypoint = WaypointsList.Find(w => w.Id == e.WaypointId);
//         if (waypoint != null)
//         {
//             waypoint.X = e.NewX;
//             waypoint.Y = e.NewY;
//             waypoint.Z = e.NewZ;
//             waypoint.Type = e.NewType;

//             string json = JsonUtility.ToJson(waypoint);
//             webSocketClient.SendJsonData(json, "WAYPOINTS");
//             Debug.Log($"Waypoint edited: {waypoint.Name}");
//         }
//     }

//     public void DeleteWaypoint(WaypointDeletedEvent e)
//     {
//         Waypoint waypoint = WaypointsList.Find(w => w.Id == e.WaypointId);
//         if (waypoint != null)
//         {
//             WaypointsList.Remove(waypoint);

//             string json = JsonUtility.ToJson(waypoint);
//             webSocketClient.SendJsonData(json, "WAYPOINTS");
//             Debug.Log($"Waypoint deleted: {waypoint.Name}");
//         }
//     }

//     void OnDestroy()
//     {
//         EventBus.Unsubscribe(waypointAddedEvent);
//         EventBus.Unsubscribe(waypointEditedEvent);
//         EventBus.Unsubscribe(waypointDeletedEvent);
//     }
// }
