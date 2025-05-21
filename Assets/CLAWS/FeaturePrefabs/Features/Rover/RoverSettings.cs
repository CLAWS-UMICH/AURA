using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.UX;
using UnityEngine;

public class RoverSettings : MonoBehaviour
{
    public GameObject roverSettingsScreen;
    public GameObject imu_pos;
    public ToggleCollection menuToggleCollection;
    [Header("Toggle Icons")]
    public GameObject messagingToggle;
    public GameObject waypointToggle;
    public GameObject samplingToggle;
    [Header("Map")]
    public GameObject map;
    private Subscription<PR_LocationUpdatedEvent> pr_LocationUpdatedEventSubscription;
    protected Subscription<prUpdatedVitalsEvent> pr_VitalsUpdatedEventSubscription;


    void Start()
    {
        pr_LocationUpdatedEventSubscription = EventBus.Subscribe<PR_LocationUpdatedEvent>(OnPRLocationUpdated);
        pr_VitalsUpdatedEventSubscription = EventBus.Subscribe<prUpdatedVitalsEvent>(OnPRVitalsUpdated);

    }


    public void OnPRLocationUpdated(PR_LocationUpdatedEvent e)
    {
        imu_pos.transform.Find("Title (2)").GetComponent<TMPro.TextMeshProUGUI>().text = "[" + e.data.posX + ", " + e.data.posZ + "]";
    }

    public void OnPRVitalsUpdated(prUpdatedVitalsEvent e)
    {
        //GameObject o2_tank = PR_o2Tank.transform.Find("O2Tank").gameObject;
        //GameObject battery_level = PR_batteryLevel.transform.Find("BatteryLevel").gameObject;
        //o2_tank.transform.Find("Value").GetComponent<TMPro.TextMeshProUGUI>().text = e.data.oxygen_tank;
        //battery_level.transform.Find("Value").GetComponent<TMPro.TextMeshProUGUI>().text = e.data.battery_level;
    }

    public void openFeatureScreen()
    {
        roverSettingsScreen.SetActive(true);
        foreach (Transform child in roverSettingsScreen.transform)
        {
            child.gameObject.SetActive(true);
        }
        menuToggleCollection.SetSelection(5);
    }


    public void closeRoverSettingsScreen()
    {
        roverSettingsScreen.SetActive(false);
        foreach (Transform child in roverSettingsScreen.transform)
        {
            child.gameObject.SetActive(false);
        }
        menuToggleCollection.SetSelection(6);
    }
}
