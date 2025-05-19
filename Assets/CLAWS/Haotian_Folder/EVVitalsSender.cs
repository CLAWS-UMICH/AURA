using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

[System.Serializable]
public class EVLocation
{
    public double posx;
    public double posy;
}

[System.Serializable]
public class EVVitals
{
    public int ev_time;
    public EVLocation ev_location;
    public int oxy_time_left;
    public double batt_time_left;
    public int heart_rate;
    public double temperature;
}

[System.Serializable]
public class EVVitalsData
{
    public EVVitals eva1;
    public EVVitals eva2;
}

[System.Serializable]
public class EVVitalsMessage
{
    public string client_id = "pr_client";
    public string room = "EV_VITALS";
    public EVVitalsData data;
}

public class EVVitalsSender : MonoBehaviour
{
    [SerializeField] private LMCCWebSocketClient LMCCwebSocketClient;

    void Start()
    {
        // Start a coroutine to send vitals every second
        StartCoroutine(SendVitalsRoutine());
    }

    private IEnumerator SendVitalsRoutine()
    {
        while (true)
        {
            // Gather vitals for EV1 and EV2
            EVVitals ev1 = GetEV1Vitals();
            EVVitals ev2 = GetEV2Vitals();

            // Send to PR
            SendEVVitalsToPR(ev1, ev2, LMCCwebSocketClient);

            yield return new WaitForSeconds(1f);
        }
    }

    // Implement GetEV1Vitals and GetEV2Vitals to fetch your data
    private EVVitals GetEV1Vitals()
    {
        // Replace with actual data source for EV1
        var user = AstronautInstance.User; 

        return new EVVitals
        {
            ev_time = user.vitals.eva_time,
            ev_location = new EVLocation
            {
                posx = user.current.posX,
                posy = user.current.posY
            },
            oxy_time_left = user.vitals.oxy_time_left,
            batt_time_left = user.vitals.batt_time_left,
            heart_rate = (int)user.vitals.heart_rate,
            temperature = user.vitals.temperature
        };
    }

    private EVVitals GetEV2Vitals()
    {
        // Replace with actual data source for EV2
        var fellow = AstronautInstance.User.fellowAstronaut;

        return new EVVitals
        {
            ev_time = fellow.vitals.eva_time,
            ev_location = new EVLocation
            {
                posx = fellow.location.posX,
                posy = fellow.location.posY
            },
            oxy_time_left = fellow.vitals.oxy_time_left,
            batt_time_left = fellow.vitals.batt_time_left,
            heart_rate = (int)fellow.vitals.heart_rate,
            temperature = fellow.vitals.temperature
        };
    }

    public void SendEVVitalsToPR(EVVitals ev1, EVVitals ev2, LMCCWebSocketClient socketClient)
    {
        var message = new EVVitalsMessage
        {
            data = new EVVitalsData
            {
                eva1 = ev1,
                eva2 = ev2
            }
        };

        var messageDict = new Dictionary<string, object>
        {
            { "client_id", message.client_id },
            { "room", message.room },
            { "data", message.data }
        };

        socketClient.SendJsonData(messageDict, "EV_VITALS", 3); // 3 = PR client
    }
}