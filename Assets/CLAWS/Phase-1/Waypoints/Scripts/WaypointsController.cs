// /*
// Need to update with waypoint prefabs.
// Also possibly remove the 3d way points if we are not doing that anymore?
// */
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;

// public class WaypointsController : MonoBehaviour
// {
//     [SerializeField] private GameObject dangerPrefab;
//     [SerializeField] private GameObject geoPrefab;
//     [SerializeField] private GameObject stationPrefab;
//     [SerializeField] private GameObject poiPrefab;
//     [SerializeField] private GameObject companionPrefab;

//     [SerializeField] private GameObject dangerPrefab_3D;
//     [SerializeField] private GameObject geoPrefab_3D;
//     [SerializeField] private GameObject stationPrefab_3D;
//     [SerializeField] private GameObject poiPrefab_3D;
//     [SerializeField] private GameObject companionPrefab_3D;

//     private GameObject parentObject;

//     private Subscription<WaypointsAddedEvent> waypointsAddedEvent;
//     private Subscription<WaypointsDeletedEvent> waypointsDeletedEvent;
//     private Subscription<WaypointsEditedEvent> waypointsEditedEvent; 
//     private Subscription<WaypointToDelete> waypointToDeleteEvent;
//     private Subscription<WaypointToAdd> waypointToAdd;
//     private Subscription<FellowIMUChanged> imuChangedEvent;

//     private Dictionary<int, Waypoint> waypointDict = new Dictionary<int, Waypoint>();
//     private Dictionary<int, GameObject> waypointObjDic = new Dictionary<int, GameObject>();
//     private Dictionary<int, GameObject> waypointObjDic_3D = new Dictionary<int, GameObject>();
//     private Dictionary<int, GameObject> waypointButtonDic = new Dictionary<int, GameObject>();

//     private TextMeshPro _danger_title, _danger_letter, _danger_minimap_letter, _geo_title, _geo_letter, _geo_minimap_letter, _nav_title, _nav_letter, _nav_minimap_letter, _station_title, _station_letter, _station_minimap_letter, _comp_title, _comp_letter, _comp_minimap_letter;
//     private TextMeshPro _danger_title_3D, _danger_letter_3D, _geo_title_3D, _geo_letter_3D, _nav_title_3D, _nav_letter_3D, _station_title_3D, _station_letter_3D, _comp_title_3D, _comp_letter_3D;

//     private WebsocketDataHandler wd;
//     private NavScreenHandler screenHandler;

//     // Start is called before the first frame update
//     void Start()
//     {
//         parentObject = GameObject.Find("WaypointParent").gameObject;
//         wd = GameObject.Find("Controller").GetComponent<WebsocketDataHandler>();

//         // TODO: Update waypoints based on prefabs to be added

//         // Main Waypoints
//         _danger_title = dangerPrefab.transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _danger_letter = dangerPrefab.transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _danger_minimap_letter = dangerPrefab.transform.Find("MiniMap").Find("Letter").gameObject.GetComponent<TextMeshPro>();

//         _geo_title = geoPrefab.transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _geo_letter = geoPrefab.transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _geo_minimap_letter = geoPrefab.transform.Find("MiniMap").Find("Letter").gameObject.GetComponent<TextMeshPro>();

//         _nav_title = poiPrefab.transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _nav_letter = poiPrefab.transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _nav_minimap_letter = poiPrefab.transform.Find("MiniMap").Find("Letter").gameObject.GetComponent<TextMeshPro>();

//         _station_title = stationPrefab.transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _station_letter = stationPrefab.transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _station_minimap_letter = stationPrefab.transform.Find("MiniMap").Find("Letter").gameObject.GetComponent<TextMeshPro>();

//         _comp_title = companionPrefab.transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _comp_letter = companionPrefab.transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _comp_minimap_letter = companionPrefab.transform.Find("MiniMap").Find("Letter").gameObject.GetComponent<TextMeshPro>();

//         // 3D Map Waypoints
//         _danger_title_3D = dangerPrefab_3D.transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _danger_letter_3D = dangerPrefab_3D.transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();

//         _geo_title_3D = geoPrefab_3D.transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _geo_letter_3D = geoPrefab_3D.transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();

//         _nav_title_3D = poiPrefab_3D.transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _nav_letter_3D = poiPrefab_3D.transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();

//         _station_title_3D = stationPrefab_3D.transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _station_letter_3D = stationPrefab_3D.transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();

//         _comp_title_3D = companionPrefab_3D.transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
//         _comp_letter_3D = companionPrefab_3D.transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();


//         waypointsAddedEvent = EventBus.Subscribe<WaypointsAddedEvent>(onWaypointsAdded);
//         waypointsDeletedEvent = EventBus.Subscribe<WaypointsDeletedEvent>(onWaypointsDeleted);
//         waypointsEditedEvent = EventBus.Subscribe<WaypointsEditedEvent>(onWaypointsEdited);
//         waypointToDeleteEvent = EventBus.Subscribe<WaypointToDelete>(onWaypointToDelete);
//         waypointToAdd = EventBus.Subscribe<WaypointToAdd>(onWaypointToAdd);
//         imuChangedEvent = EventBus.Subscribe<FellowIMUChanged>(onIMUChanged);

//         screenHandler = transform.parent.Find("NavController").gameObject.GetComponent<NavScreenHandler>();
//     }

//     public void onWaypointsAdded(WaypointsAddedEvent e)
//     {
//         List<Waypoint> addedWaypoints = e.NewAddedWaypoints;
//         foreach(Waypoint waypoint in addedWaypoints)
//         {
//             waypointDict[waypoint.waypoint_id] = waypoint;
//             AstronautInstance.User.WaypointData.AllWaypoints.Add(waypoint);

//             if ((AstronautInstance.User.id == 0 && waypoint.waypoint_id == 23) || (AstronautInstance.User.id == 1 && waypoint.waypoint_id == 24))
//             {
//                 continue;
//             } 

//             SpawnWaypoint(waypoint, waypoint.waypoint_id);

//             GameObject button = screenHandler.AddButton(waypoint); //Set the function to add button with screen handler
//             waypointButtonDic[waypoint.waypoint_id] = button;
//         }
//     }
//     public void onWaypointToAdd(WaypointToAdd e)
//     {
//         CreateNew(e.location, e.type, e.description);
//     }

//     public void onWaypointsDeleted(WaypointsDeletedEvent e)
//     {
//         List<Waypoint> deletedWaypoints = e.DeletedWaypoints;
//         foreach(Waypoint waypoint in deletedWaypoints)
//         {
//             if ((AstronautInstance.User.id == 0 && waypoint.waypoint_id == 23) || (AstronautInstance.User.id == 1 && waypoint.waypoint_id == 24))
//             {
//                 continue;
//             }
//             if (waypointDict.ContainsKey(waypoint.waypoint_id))
//             {
//                 waypointDict.Remove(waypoint.waypoint_id);

//                 GameObject gm = waypointObjDic[waypoint.waypoint_id];
//                 waypointObjDic.Remove(waypoint.waypoint_id);

//                 GameObject gm_3D = waypointObjDic_3D[waypoint.waypoint_id];
//                 waypointObjDic_3D.Remove(waypoint.waypoint_id);

//                 GameObject gmButton = waypointButtonDic[waypoint.waypoint_id];
//                 screenHandler.DeleteButton(gmButton, waypoint.type);
//                 waypointButtonDic.Remove(waypoint.waypoint_id);

//                 Destroy(gm);
//                 Destroy(gm_3D);
//             }

//             AstronautInstance.User.WaypointData.AllWaypoints.Remove(waypoint);
//         }
//     }

//     public void onWaypointToDelete(WaypointToDelete e)
//     {
//         int indexToDelete = e.waypointIndexToDelete;

//         if (waypointDict.ContainsKey(indexToDelete))
//         {
//             Waypoint w = waypointDict[indexToDelete];
//             waypointDict.Remove(indexToDelete);

//             GameObject gm = waypointObjDic[indexToDelete];
//             waypointObjDic.Remove(indexToDelete);

//             GameObject gm_3D = waypointObjDic_3D[indexToDelete];
//             waypointObjDic_3D.Remove(indexToDelete);

//             GameObject gmButton = waypointButtonDic[indexToDelete];
//             screenHandler.DeleteButton(gmButton, w.type);
//             waypointButtonDic.Remove(indexToDelete);

//             Destroy(gm);
//             Destroy(gm_3D);
//             AstronautInstance.User.WaypointData.AllWaypoints.Remove(w);

//             wd.SendWaypointData();
//         }
//     }

//     public void onWaypointsEdited(WaypointsEditedEvent e)
//     {
//         List<Waypoint> editedWaypoints = e.EditedWaypoints;
//         foreach(Waypoint waypoint in editedWaypoints)
//         {
//             if ((AstronautInstance.User.id == 0 && waypoint.waypoint_id == 23) || (AstronautInstance.User.id == 1 && waypoint.waypoint_id == 24))
//             {
//                 continue;
//             }
//             if (waypointDict.ContainsKey(waypoint.waypoint_id))
//             {
//                 waypointDict[waypoint.waypoint_id].location = waypoint.location;
//                 waypointDict[waypoint.waypoint_id].type = waypoint.type;
//                 waypointDict[waypoint.waypoint_id].description = waypoint.description;
//                 waypointDict[waypoint.waypoint_id].waypoint_letter = waypoint.waypoint_letter;

//                 waypointObjDic[waypoint.waypoint_id].transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>().text = waypoint.description;
//                 waypointObjDic[waypoint.waypoint_id].transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>().text = waypoint.waypoint_letter;

//                 waypointObjDic_3D[waypoint.waypoint_id].transform.Find("Body").Find("Title").gameObject.transform.Find("IconAndText").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>().text = waypoint.description;
//                 waypointObjDic_3D[waypoint.waypoint_id].transform.Find("Body").Find("Quad").gameObject.transform.Find("Text").gameObject.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>().text = waypoint.waypoint_letter;
//             }
//         }
//     }

//     void OnDestroy()
//     {
//         // Unsubscribe when the script is destroyed
//         if (waypointsDeletedEvent != null)
//         {
//             EventBus.Unsubscribe(waypointsDeletedEvent);
//         }
//         if (waypointsEditedEvent != null)
//         {
//             EventBus.Unsubscribe(waypointsEditedEvent);
//         }
//         if (waypointsAddedEvent != null)
//         {
//             EventBus.Unsubscribe(waypointsAddedEvent);
//         }
//     }

//     public void SpawnWaypoint(Waypoint waypoint, int num)
//     {
//         GameObject instantiatedObject = new GameObject();
//         GameObject instantiatedObject_3D = new GameObject();
//         switch (waypoint.type)
//         {
//             case 0: // station
//                 _station_letter.text = waypoint.waypoint_letter.ToString();
//                 _station_title.text = waypoint.description.ToString();
//                 _station_minimap_letter.text = waypoint.waypoint_letter.ToString();
//                 instantiatedObject = Instantiate(stationPrefab, GPSUtils.GPSCoordsToAppPosition(waypoint.location), Quaternion.identity);
//                 instantiatedObject.transform.parent = parentObject.transform;
//                 waypointObjDic[waypoint.waypoint_id] = instantiatedObject;

//                 _station_letter_3D.text = waypoint.waypoint_letter.ToString();
//                 _station_title_3D.text = waypoint.description.ToString();
//                 instantiatedObject_3D = Map3DController.SpawnWaypoint(stationPrefab_3D, GPSUtils.GPSCoordsToAppPosition(waypoint.location));
//                 waypointObjDic_3D[waypoint.waypoint_id] = instantiatedObject_3D;
//                 break;

//             case 1: // poi
//                 Debug.Log(waypoint);
//                 _nav_letter.text = waypoint.waypoint_letter.ToString();
//                 _nav_title.text = waypoint.description.ToString();
//                 _nav_minimap_letter.text = waypoint.waypoint_letter.ToString();
//                 instantiatedObject = Instantiate(poiPrefab, GPSUtils.GPSCoordsToAppPosition(waypoint.location), Quaternion.identity);
//                 instantiatedObject.transform.parent = parentObject.transform;
//                 waypointObjDic[waypoint.waypoint_id] = instantiatedObject;

//                 _nav_letter_3D.text = waypoint.waypoint_letter.ToString();
//                 _nav_title_3D.text = waypoint.description.ToString();
//                 instantiatedObject_3D = Map3DController.SpawnWaypoint(poiPrefab_3D, GPSUtils.GPSCoordsToAppPosition(waypoint.location));
//                 waypointObjDic_3D[waypoint.waypoint_id] = instantiatedObject_3D;
//                 break;

//             case 2: // geo
//                 _geo_letter.text = waypoint.waypoint_letter.ToString();
//                 _geo_title.text = waypoint.description.ToString();
//                 _geo_minimap_letter.text = waypoint.waypoint_letter.ToString();
//                 instantiatedObject = Instantiate(geoPrefab, GPSUtils.GPSCoordsToAppPosition(waypoint.location), Quaternion.identity);
//                 instantiatedObject.transform.parent = parentObject.transform;
//                 waypointObjDic[waypoint.waypoint_id] = instantiatedObject;

//                 _geo_letter_3D.text = waypoint.waypoint_letter.ToString();
//                 _geo_title_3D.text = waypoint.description.ToString();
//                 instantiatedObject_3D = Map3DController.SpawnWaypoint(geoPrefab_3D, GPSUtils.GPSCoordsToAppPosition(waypoint.location));
//                 waypointObjDic_3D[waypoint.waypoint_id] = instantiatedObject_3D;
//                 break;

//             case 3: // danger
//                 _danger_letter.text = waypoint.waypoint_letter.ToString();
//                 _danger_title.text = waypoint.description.ToString();
//                 _danger_minimap_letter.text = waypoint.waypoint_letter.ToString();
//                 instantiatedObject = Instantiate(dangerPrefab, GPSUtils.GPSCoordsToAppPosition(waypoint.location), Quaternion.identity);
//                 instantiatedObject.transform.parent = parentObject.transform;
//                 waypointObjDic[waypoint.waypoint_id] = instantiatedObject;

//                 _danger_letter_3D.text = waypoint.waypoint_letter.ToString();
//                 _danger_title_3D.text = waypoint.description.ToString();
//                 instantiatedObject_3D = Map3DController.SpawnWaypoint(dangerPrefab_3D, GPSUtils.GPSCoordsToAppPosition(waypoint.location));
//                 waypointObjDic_3D[waypoint.waypoint_id] = instantiatedObject_3D;
//                 break;
//             case 4: // companions
//                 _comp_letter.text = waypoint.waypoint_letter.ToString();
//                 _comp_title.text = waypoint.description.ToString();
//                 _comp_minimap_letter.text = waypoint.waypoint_letter.ToString();
//                 instantiatedObject = Instantiate(companionPrefab, GPSUtils.GPSCoordsToAppPosition(waypoint.location), Quaternion.identity);
//                 instantiatedObject.transform.parent = parentObject.transform;
//                 waypointObjDic[waypoint.waypoint_id] = instantiatedObject;

//                 _comp_letter_3D.text = waypoint.waypoint_letter.ToString();
//                 _comp_title_3D.text = waypoint.description.ToString();
//                 instantiatedObject_3D = Map3DController.SpawnWaypoint(companionPrefab_3D, GPSUtils.GPSCoordsToAppPosition(waypoint.location));
//                 waypointObjDic_3D[waypoint.waypoint_id] = instantiatedObject_3D;
//                 break;

//             default:

//                 break;
//         }

//         instantiatedObject.name = "Way_" + num.ToString();
//         instantiatedObject_3D.name = "Way_" + num.ToString() +"_3D";


//     }


//     public void CreateNew(Location loc, int type, string desc)
//     {
//         Waypoint way = new Waypoint
//         {
//             waypoint_id = AstronautInstance.User.WaypointData.currentIndex,
//             waypoint_letter = getLetter(AstronautInstance.User.WaypointData.currentIndex),
//             location = loc,
//             type = type,
//             description = desc,
//             author = AstronautInstance.User.id
//         };

//         waypointDict[way.waypoint_id] = way;
//         AstronautInstance.User.WaypointData.AllWaypoints.Add(way);
//         SpawnWaypoint(way, way.waypoint_id);

//         GameObject button = screenHandler.AddButton(way);
//         waypointButtonDic[way.waypoint_id] = button;

//         while (true)
//         {
//             AstronautInstance.User.WaypointData.currentIndex += 1;

//             if (AstronautInstance.User.WaypointData.currentIndex != 17 &&
//                 AstronautInstance.User.WaypointData.currentIndex != 23 &&
//                 AstronautInstance.User.WaypointData.currentIndex != 24)
//             {
//                 break;
//             }
//         }

//         wd.SendWaypointData();
//     }

//     private string getLetter(int num)
//     {
//         string result = "";

//         while (num >= 0)
//         {
//             result = (char)('A' + num % 26) + result;
//             num = (num / 26) - 1;

//             if (num < 0)
//             {
//                 break;
//             }
//         }

//         return result;
//     }

//     public Location getLocationOfButton(string letter)
//     {
//         int index = getNumGivenString(letter);
//         if (waypointDict.ContainsKey(index))
//         {
//             return waypointDict[index].location;
//         }

//         return new Location();
//     }

//     private int getNumGivenString(string letter)
//     {
//         int result = 0;
//         int length = letter.Length;

//         for (int i = 0; i < length; i++)
//         {
//             char c = letter[i];
//             result = result * 26 + (c - 'A' + 1);
//         }

//         return result - 1;
//     }

//     public void UpdateLocationsOfWaypoints()
//     {
//         // Set the positions of the GameObjects based on the corrected locations
//         foreach (var entry in waypointDict)
//         {
//             int key = entry.Key;
//             Waypoint waypoint = entry.Value;

//             if (waypointObjDic.TryGetValue(key, out GameObject waypointObj))
//             {
//                 // Correct the location using the CorrectLocation method
//                 Vector3 correctedPosition = GPSUtils.GPSCoordsToAppPosition(waypoint.location);
//                 waypointObj.transform.position = correctedPosition;
//             }
//         }
//     }

//     public void onIMUChanged(FellowIMUChanged e)
//     {
//         int numToUpdate;
//         if (AstronautInstance.User.id == 0)
//         {
//             numToUpdate = 24;
//         } else
//         {
//             numToUpdate = 23;
//         }

//         if (waypointObjDic.ContainsKey(numToUpdate))
//         {
//             var latLong = CoordinateConverter.ToLatLon(e.data.posx, e.data.posy, 15, 'R', northern: null, strict: true);
//             waypointObjDic[numToUpdate].transform.position = GPSUtils.GPSCoordsToAppPosition(new Location(latLong.latitude, latLong.longitude));
//         }
//     }
// }