
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationHandler : MonoBehaviour
{
    bool originSet = false;

    private GameObject mainCamera;
    private GameObject mainCameraHolder;
    private GameObject mrtkSceneContent;

    private GameObject breadcrumbParent;
    private GameObject waypointParent;

    void Awake()
    {
        mainCameraHolder = GameObject.Find("MixedRealityPlayspace");
        mainCamera = mainCameraHolder.transform.Find("Main Camera").gameObject;
        mrtkSceneContent = GameObject.Find("MixedRealitySceneContent");
        breadcrumbParent = transform.parent.Find("BreadCrumbParent").gameObject;
        waypointParent = transform.parent.Find("WaypointParent").gameObject;
        EventBus.Subscribe<UpdatedGPSEvent>(OnTSLocation);
    }

    void OnTSLocation(UpdatedGPSEvent e)
    {
        Debug.LogWarning("Recieved user location.");

        NewUserLocation(AstronautInstance.User.location);

        // To display the angle between coordinates
        // Debug.Log(LocationUtilities.DistanceAndAngleBetweenCoords(new Coords(locations.list[0].latitude, locations.list[0].latitude), new Coords(locations.list[1].latitude, locations.list[1].latitude)));
    }

    public void UpdateTS()
    {
        Debug.Log("Recieved user location.");
        NewUserLocation(AstronautInstance.User.location);
    }

    void NewUserLocation(Location location)
    {
        Location user_coordinates = new Location(
                        29.56459834,
                        -95.0814415
                    );
        // set the initial origin
        GPSUtils.ChangeOriginGPSCoords(user_coordinates);

        Debug.Log("Recalculating user location.\n" +
        "Origin is: " + GPSUtils.originGPSCoords.latitude + ' ' + GPSUtils.originGPSCoords.longitude +
        "\nUser location is: " + location.latitude + ", " + location.longitude);

        //AstronautInstance.User.location = new Location(29.56459834, -95.0814415);

        mainCameraHolder.transform.position = new Vector3(0f, 0f, 0f);
        mainCamera.transform.position = new Vector3(0f, 0f, 0f);

        // Update Main Map Camera
        Location loc = new Location(29.56491516, -95.08146507);
        GameObject.Find("BigCameraMap").transform.position = GPSUtils.GPSCoordsToAppPosition(loc);

        /*// Update Breadcrumbs
        foreach (Transform childTransform in breadcrumbParent.transform)
        {
            GameObject breadCrumb = childTransform.gameObject;

            float currentY = breadCrumb.transform.position.y;
            int breadIndex = int.Parse(breadCrumb.name);

            if (breadIndex < 0 || breadIndex >= AstronautInstance.User.BreadCrumbData.AllCrumbs.Count)
            {
                continue;
            }

            Location newLoc = AstronautInstance.User.BreadCrumbData.AllCrumbs[breadIndex].location;
            Vector3 newL = GPSUtils.GPSCoordsToAppPosition(newLoc);
            newL.y = currentY;

            childTransform.position = newL;
        }

        // Update Waypoints
        foreach (Transform childTransform in waypointParent.transform)
        {
            GameObject waypoint = childTransform.gameObject;

            float currentY = waypoint.transform.position.y;
            int wayIndex = int.Parse(waypoint.name);

            if (wayIndex < 0 || wayIndex >= AstronautInstance.User.WaypointData.AllWaypoints.Count)
            {
                continue;
            }

            Location newLoc = AstronautInstance.User.WaypointData.AllWaypoints[wayIndex].location;
            Vector3 newL = GPSUtils.GPSCoordsToAppPosition(newLoc);
            newL.y = currentY;

            childTransform.position = newL;
        }

        // Update Geosamples
        // TODO*/


    }

    /*public void Recalibrate()
    {
        GPSMsg location = Simulation.User.GPS;
        GPSCoords user_coordinates = new GPSCoords(
                        location.lat,
                        location.lon
                    );

        Debug.Log("Recalculating user location.\n" +
            "Origin is: " + GPSUtils.originGPSCoords.latitude + ' ' + GPSUtils.originGPSCoords.longitude +
            "\nUser location is: " + location.lat + ", " + location.lon);

        Vector3 NewCameraAppPosition = GPSUtils.GPSCoordsToAppPosition(user_coordinates);
        NewCameraAppPosition.y = 0f;

        Debug.Log(NewCameraAppPosition.ToString());

        mainCameraHolder.transform.position = NewCameraAppPosition;
    }*/

    public void SetNorth()
    {
        Debug.LogWarning("Recalculating North.");

        Vector3 mainCameraWorldPosition = mainCamera.transform.position;

        float mainCameraY = mainCamera.transform.localRotation.eulerAngles.y;

        mainCameraHolder.transform.eulerAngles = new Vector3(0f, mainCameraY, 0f);

        Vector3 mainCameraNewWorldPosition = mainCamera.transform.position;

        mainCameraHolder.transform.position = mainCameraHolder.transform.position + -(mainCameraNewWorldPosition - mainCameraWorldPosition);
    }


    // shifts everything around the player North
    public void ShiftNorth(int magnitude)
    {
        Vector3 change = mainCamera.transform.forward * magnitude;
        change.y = 0;
        mainCamera.transform.position += change;
    }

    public void ShiftSouth(int magnitude)
    {
        Vector3 change = mainCamera.transform.forward * magnitude;
        change.y = 0;
        mainCamera.transform.position -= change;
    }

    public void ShiftEast(int magnitude)
    {
        Vector3 change = mainCamera.transform.right * magnitude;
        change.y = 0;
        mainCamera.transform.position -= change;
    }

    public void ShiftWest(int magnitude)
    {
        Vector3 change = mainCamera.transform.right * magnitude;
        change.y = 0;
        mainCamera.transform.position += change;
    }

    public void ShiftRotationClockwise(int magnitude)
    {
        mainCameraHolder.transform.eulerAngles = mainCameraHolder.transform.eulerAngles - new Vector3(0, magnitude, 0);
    }

    public void ShiftRotationCounterClockwise(int magnitude)
    {
        mainCameraHolder.transform.eulerAngles = mainCameraHolder.transform.eulerAngles + new Vector3(0, magnitude, 0);
    }


}