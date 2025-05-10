using System.Collections;
using UnityEngine;

public class StartingScript : MonoBehaviour
{
    [SerializeField] private GameObject greetingScreen;

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
        // Debug.Log($"Latitude: {lat}, Longitude: {lon}");

        // // turn on for hololens button
        // Cursor.visible = false;
        transform.Find("Main").gameObject.SetActive(false);
        transform.Find("Screens").gameObject.SetActive(false);
        greetingScreen.SetActive(true);

        // Stop the location service if no longer needed
        // Input.location.Stop();
    }
}
