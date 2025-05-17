using System.Collections;
using UnityEngine;

public class StartingScript : MonoBehaviour
{
    [SerializeField] private GameObject greetingScreen;
    [SerializeField] private GameObject navigationScreen;
    private NavigationController navigationController;
    private NavigationFrontend navigationFrontend;

    void Start()
    {
        // if (!Input.location.isEnabledByUser)
        // {
        //     Debug.LogError("Location services are not enabled by the user.");
        //     yield break;
        // }

        // // Start the location service
        // Input.location.Start();
        // int maxWait = 20;
        // while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        // {
        //     yield return new WaitForSeconds(1);
        //     maxWait--;
        // }

        // // Check if the service timed out
        // if (maxWait <= 0)
        // {
        //     Debug.LogError("Timed out while initializing location services.");
        //     yield break;
        // }

        // // Check if the service failed
        // if (Input.location.status == LocationServiceStatus.Failed)
        // {
        //     Debug.LogError("Unable to determine device location.");
        //     yield break;
        // }

        // // Access the location data
        // double lat = Input.location.lastData.latitude;
        // double lon = Input.location.lastData.longitude;
        // Debug.Log($"Latitude: {lat}, Longitude: {lon}")  


        // // turn on for hololens button
        // Cursor.visible = false;
        AstronautInstance.User.origin.posX = -5670f;
        AstronautInstance.User.origin.posY = -10010f;
        AstronautInstance.User.origin.posZ = 0.2f;
        transform.Find("Main").gameObject.SetActive(false);
        greetingScreen.SetActive(true);

        //turn on navigation to instantiate
        navigationScreen.SetActive(true);
        foreach (Transform child in navigationScreen.transform)
        {
            child.gameObject.SetActive(false);
        }
        navigationController = navigationScreen.GetComponent<NavigationController>();
        navigationFrontend = navigationScreen.GetComponent<NavigationFrontend>();
        navigationController.enabled = true;
        navigationFrontend.enabled = true;
        navigationScreen.SetActive(true);
        Debug.Log("StartingScript Start");
        Debug.Log($"navigationScreen active: {navigationScreen.activeInHierarchy}");
        Debug.Log($"navigationController enabled: {navigationController.enabled}");
        StartCoroutine(InitializeNavigation());
        initializeGeoSamples();

        
        // Stop the location service if no longer needed
        // Input.location.Stop();
    }

    IEnumerator InitializeNavigation()
    {
        // Set up greeting screen
        greetingScreen.SetActive(true);

        // Activate navigation screen
        navigationScreen.SetActive(true);

        // Wait for one frame to ensure Unity processes the activation
        yield return null;

        // Initialize navigation components
        navigationController = navigationScreen.GetComponent<NavigationController>();
        navigationFrontend = navigationScreen.GetComponent<NavigationFrontend>();

        if (navigationController == null || navigationFrontend == null)
        {
            Debug.LogError("Navigation components are missing.");
            yield break;
        }

        navigationController.enabled = true;
        navigationFrontend.enabled = true;

        Debug.Log("Navigation screen and components initialized.");

        // INSTANTIATE STATION WAYPOINTS
        Waypoint station1 = new Waypoint {
            Use = "ADD",
            Id = navigationController.StationWaypointList.Count,
            Name = "Station 1",
            IMUposX = -5616f,
            IMUposY = -10005f,
            Type = WaypointType.STATION,
            Author = AstronautInstance.User.id == 1 ? AuthorType.EV1 : AuthorType.EV2,
        };

        Waypoint station2 = new Waypoint {
            Use = "ADD",
            Id = navigationController.StationWaypointList.Count,
            Name = "Station 2",
            IMUposX = -5643f,
            IMUposY = -9970f,
            Type = WaypointType.STATION,
            Author = AstronautInstance.User.id == 1 ? AuthorType.EV1 : AuthorType.EV2,
        };

        Waypoint station3 = new Waypoint {
            Use = "ADD",
            Id = navigationController.StationWaypointList.Count,
            Name = "Station 3",
            IMUposX = -5608f,
            IMUposY = -9988f,
            Type = WaypointType.STATION,
            Author = AstronautInstance.User.id == 1 ? AuthorType.EV1 : AuthorType.EV2,
        };
         // Wait another frame to ensure all components are fully active
        yield return null;
        Debug.Log("Instantiating Station Waypoints...");
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(station1));
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(station2));
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(station3));    

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // INSTANTIATE LTV WAYPOINTS ---- HARD CODED FOR TESTING  ------ WILL SEE IF WE CAN GET THE DATA FROM THE SERVER //
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        Waypoint ltv1 = new Waypoint {
            Use = "ADD",
            Id = navigationController.StationWaypointList.Count,
            Name = "Waypoint A",
            IMUposX = -5635f,
            IMUposY = -9970f,
            Type = WaypointType.POI,
            Author = AuthorType.PR
        };
        Waypoint ltv2 = new Waypoint {
            Use = "ADD",
            Id = navigationController.StationWaypointList.Count,
            Name = "Waypoint B",
            IMUposX = -5610f,
            IMUposY = -9971f,
            Type = WaypointType.POI,
            Author = AuthorType.PR
        };
        Waypoint ltv3 = new Waypoint {
            Use = "ADD",
            Id = navigationController.StationWaypointList.Count,
            Name = "Waypoint C",
            IMUposX = -5615f,
            IMUposY = -9995f,
            Type = WaypointType.POI,
            Author = AuthorType.PR,
        };
        Debug.Log("Instantiating LTV Waypoints...");
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(ltv1));
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(ltv2));
        EventBus.Publish<WaypointAddedEvent>(new WaypointAddedEvent(ltv3)); 
    }

    void initializeGeoSamples()
    {
        GeosampleZone newGeoSampleZone1 = new GeosampleZone {
            id = 0,
            origin = new Location {
                posX = -5635f - AstronautInstance.User.origin.posX,
                posY = 0.2f,
                posZ = -9970f - AstronautInstance.User.origin.posY,
            },
        };

        GeosampleZone newGeoSampleZone2 = new GeosampleZone {
            id = 0,
            origin = new Location {
                posX = -5610f - AstronautInstance.User.origin.posX,
                posY = 0.2f,
                posZ = -9971f - AstronautInstance.User.origin.posY,
            },
        };

        GeosampleZone newGeoSampleZone3 = new GeosampleZone {
            id = 0,
            origin = new Location {
                posX = -5615f - AstronautInstance.User.origin.posX,
                posY = 0.2f,
                posZ = -9995f - AstronautInstance.User.origin.posY,
            },
        };

        AstronautInstance.User.geosampleZones.Add(newGeoSampleZone1);
        AstronautInstance.User.geosampleZones.Add(newGeoSampleZone2);
        AstronautInstance.User.geosampleZones.Add(newGeoSampleZone3);
        Debug.Log("Instantiating Geosample Zones...");
    }
}
