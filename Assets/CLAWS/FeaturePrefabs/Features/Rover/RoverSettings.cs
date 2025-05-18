using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;




public class PRVitalDataWrapper
{
    public string room;
    public string use;
    public PRVitalsData data;
}

public class RoverSettings : MonoBehaviour
{
    public GameObject roverSettingsScreen;
    [Header("Toggle Icons")]
    public GameObject messagingToggle;
    public GameObject waypointToggle;
    public GameObject samplingToggle;
    [Header("Map")]
    public GameObject map;
    private Subscription<PR_UpdateVitalsEvent> vitalsUpdateEvent; 
    [Header("Vitals")]
    public GameObject oxyTime; 
    public GameObject battTime; 


    void Start()
    {
        vitalsUpdateEvent = EventBus.Subscribe<PR_UpdateVitalsEvent>(updateVitals); 
    }

    public void openRoverSettingsScreen()
    {
        roverSettingsScreen.SetActive(true);
        foreach(Transform child in roverSettingsScreen.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    public void closeRoverSettingsScreen()
    {
        roverSettingsScreen.SetActive(false);
        foreach(Transform child in roverSettingsScreen.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void updateVitals(PR_UpdateVitalsEvent e)
    {
        oxyTime.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.o2_tank / 0.15) * 302));
        oxyTime.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.o2_tank.ToString("F2");

        battTime.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)(1 - e.vitals.battery_level/0.15) * 302);
        battTime.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.battery_level.ToString("F2");

        PRVitalsDataWrapper vitalsData = new PRVitalsDataWrapper
            {
                room = "PR_Vitals",
                use = "POST",
                data = new PRVitalsData { o2_tank = e.vitals.o2_tank, battery_level = e.vitals.battery_level }
            };

        // Convert vitalsData to Dictionary<string, object> for SendJsonData
        var message = new Dictionary<string, object>
        {
            { "o2_tank", e.vitals.o2_tank },
            { "battery_level", e.vitals.battery_level }
        };
        LMCCWebSocketClient.SendJsonData(message, "PR_Vitals", 3);
    }

    void OnDestroy() {
        EventBus.Unsubscribe(vitalsUpdateEvent);
    }
}
