using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    private Subscription<DCUErrorEvent> dcuErrorEvent;
    [SerializeField] private DCUGroup dcu1;
    [SerializeField] private DCUGroup dcu2;




    // Start is called before the first frame update
    void Start()
    {
        dcuChangedEvent = EventBus.Subscribe<DCUChangedEvent>(onDcuChange);
        dcuErrorEvent = EventBus.Subscribe<DCUErrorEvent>(onDcuError);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onDcuChange(DCUChangedEvent e)
    {
        if (e.id == 1)
        {
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
        else
        {
            
        }
    }

    

    private void onDcuError(DCUErrorEvent e)
    {

    }
}
