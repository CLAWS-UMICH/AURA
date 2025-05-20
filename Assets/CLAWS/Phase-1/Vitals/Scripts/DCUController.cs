using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class DCUGroup
{
    [Header("Batt Local")]
    public GameObject batt_loc;

    [Header("Batt UMB")]
    public GameObject batt_umb;

    [Header("Oxy sec")]
    public GameObject oxy_sec;

    [Header("Oxy Pri")]
    public GameObject oxy_pri;

    [Header("Comm B")]
    public GameObject comm_b;

    [Header("Comm A")]
    public GameObject comm_a;

    [Header("Fan Sec")]
    public GameObject fan_sec;

    [Header("Fan Pri")]
    public GameObject fan_pri;

    [Header("Pump Close")]
    public GameObject pump_close;

    [Header("Pump Open")]
    public GameObject pump_open;

    [Header("CO2 B")]
    public GameObject co2_b;

    [Header("CO2 A")]
    public GameObject co2_a;
}


public class DCUController : MonoBehaviour
{
    private Subscription<DCUChangedEvent> dcuChangedEvent;
    private Subscription<FellowDCUChangedEvent> fellowDcuChangedEvent;
    private Subscription<DCUErrorEvent> dcuErrorEvent;
    [SerializeField] public DCUGroup dcu1;
    [SerializeField] public DCUGroup dcu2;




    // Start is called before the first frame update
    void Start()
    {
        dcuChangedEvent = EventBus.Subscribe<DCUChangedEvent>(onDcuChange);
        fellowDcuChangedEvent = EventBus.Subscribe<FellowDCUChangedEvent>(onFellowDcuChange);
        dcuErrorEvent = EventBus.Subscribe<DCUErrorEvent>(onDcuError);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void swapDcuController()
    {
        bool state;
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        switch (buttonName)
        {
            case "LOCAL_1":
            case "UMB_1":
                state = dcu1.batt_umb.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu1.batt_umb.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                dcu1.batt_loc.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                break;

            case "LOCAL_2":
            case "UMB_2":
                state = dcu2.batt_loc.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu2.batt_umb.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                dcu2.batt_loc.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                break;


            case "PRI_OXY_1":
            case "SEC_OXY_1":
                state = dcu1.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu1.oxy_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                dcu1.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                break;

            case "PRI_OXY_2":
            case "SEC_OXY_2":
                state = dcu2.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu2.oxy_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                dcu2.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                break;

            case "A_COMMS_1":
            case "B_COMMS_1":
                state = dcu1.comm_b.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu1.comm_a.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                dcu1.comm_b.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                break;

            case "A_COMMS_2":
            case "B_COMMS_2":
                state = dcu2.comm_b.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu2.comm_a.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                dcu2.comm_b.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                break;

            case "PRI_FAN_1":
            case "SEC_FAN_1":
                state = dcu1.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu1.fan_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                dcu1.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                break;

            case "PRI_FAN_2":
            case "SEC_FAN_2":
                state = dcu2.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu2.fan_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                dcu2.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                break;

            case "Open_1":
            case "Close_1":
                state = dcu1.pump_close.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu1.pump_open.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                dcu1.pump_close.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                break;

            case "Open_2":
            case "Close_2":
                state = dcu2.pump_close.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu2.pump_open.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                dcu2.pump_close.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                break;

            case "A_CO2_1":
            case "B_CO2_1":
                state = dcu1.co2_b.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu1.co2_a.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                dcu1.co2_b.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                break;

            case "A_CO2_2":
            case "B_CO2_2":
                state = dcu2.co2_b.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
                dcu2.co2_a.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(state);
                dcu2.co2_b.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!state);
                break;

            default:
                Debug.LogWarning("Unknown DCU button pressed: " + buttonName);
                break;
        }

    }

    private void onDcuChange(DCUChangedEvent e)
    {
        // DCU 1

        // battery
        dcu1.batt_umb.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.batt);
        dcu1.batt_loc.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.batt);

        // oxygen
        dcu1.oxy_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.oxy);
        dcu1.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.oxy);

        // commms
        dcu1.comm_a.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.comm);
        dcu1.comm_b.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.comm);

        // fan
        dcu1.fan_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.fan);
        dcu1.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.fan);

        // pump
        dcu1.pump_open.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.pump);
        dcu1.pump_close.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.pump);

        // CO2
        dcu1.co2_a.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.co2);
        dcu1.co2_b.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.co2);

    }

    private void onFellowDcuChange(FellowDCUChangedEvent e)
    {
        // battery
        dcu2.batt_umb.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.batt);
        dcu2.batt_loc.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.batt);

        // oxygen
        dcu2.oxy_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.oxy);
        dcu2.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.oxy);

        // commms
        dcu2.comm_a.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.comm);
        dcu2.comm_b.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.comm);

        // fan
        dcu2.fan_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.fan);
        dcu2.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.fan);

        // pump
        dcu2.pump_open.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.pump);
        dcu2.pump_close.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.pump);

        // CO2
        dcu2.co2_a.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(e.eva.co2);
        dcu2.co2_b.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!e.eva.co2);

    }



    private void onDcuError(DCUErrorEvent e)
    {
        // oxygen DCU error
        if (e.err.oxy)
        {
            // dcu1 oxygen error toggle
            bool dcu1_oxy = dcu1.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
            dcu1.oxy_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(dcu1_oxy);
            dcu1.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!dcu1_oxy);

            // dcu2 oxygen error toggle
            bool dcu2_oxy = dcu2.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
            dcu2.oxy_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(dcu2_oxy);
            dcu2.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!dcu2_oxy);
        }

        // fan DCU error
        if (e.err.fan)
        {
            // dcu1 fan error toggle
            bool dcu1_fan = dcu1.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
            dcu1.fan_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(dcu1_fan);
            dcu1.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!dcu1_fan);

            // dcu1 fan error toggle
            bool dcu2_fan = dcu1.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
            dcu2.fan_pri.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(dcu2_fan);
            dcu2.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(!dcu2_fan);
        }

        // pump DCU error
        if (e.err.pump)
        {
            // dcu1 pump error toggle
            bool dcu1_pump = dcu1.pump_open.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
            if (dcu1_pump)
            {
                dcu1.pump_open.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(false);
                dcu1.pump_close.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(true);
            }

            // dcu1 pump error toggle
            bool dcu2_pump = dcu2.pump_open.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf;
            if (dcu2_pump)
            {
                dcu2.pump_open.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(false);
                dcu2.pump_close.transform.Find("UIBackplateToggleQuad").gameObject.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(dcuChangedEvent);
        EventBus.Unsubscribe(fellowDcuChangedEvent);
        EventBus.Unsubscribe(dcuErrorEvent);
    }
}
