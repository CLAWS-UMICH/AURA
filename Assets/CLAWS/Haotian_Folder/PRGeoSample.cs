using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PRGeoSample : MonoBehaviour
{
    [SerializeField]private GameObject toggleObject; 
    [SerializeField]private LMCCWebSocketClient webSocketClient; // Reference to the web socket client

    private Subscription<GeoSampleAddedToZoneEvent> geosampleEvent;
    private GeoSample currentGeosample;
    private bool isSubscribed = false;

    // Start is called before the first frame update
    void Start()
    {
        geosampleEvent = EventBus.Subscribe<GeoSampleAddedToZoneEvent>(OnSampleReceived);
        
        // Find the web socket client if not assigned
        if (webSocketClient == null)
        {
            webSocketClient = FindObjectOfType<LMCCWebSocketClient>();
        }
    }

    public void toggleGeoSampleSubscription()
    {
        if (webSocketClient == null)
        {
            Debug.LogError("WebSocketClient reference is missing!");
            return;
        }

        if (currentGeosample == null)
        {
            Debug.LogWarning("No geosample data available to toggle");
            return;
        }

        // Create dictionary with message data
        Dictionary<string, object> geosampleData = new Dictionary<string, object>
        {

            { "name", currentGeosample.name },
            { "zone", currentGeosample.zone },
            { "type", currentGeosample.type },
            { "shape", currentGeosample.shape },
            { "color", currentGeosample.color },
            { "texture", currentGeosample.texture },
            { "note", currentGeosample.note },
            { "id", currentGeosample.id },
            { "isSignificant", currentGeosample.isSignificant },
            { "comp", currentGeosample.comp }
        
        };

        webSocketClient.SendJsonData(geosampleData, "GEOSAMPLES", 3); // 3 is for PR client
    }

    private void OnSampleReceived(GeoSampleAddedToZoneEvent e)
    {
        currentGeosample = e.newGeoSample;
        Debug.Log($"Received message");

    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(geosampleEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
