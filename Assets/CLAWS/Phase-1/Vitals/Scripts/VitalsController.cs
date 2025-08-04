using UnityEngine;
using TMPro;
using System.Collections;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit;
using System.Collections.Generic;
using Unity.VisualScripting;


[System.Serializable]
public class EVAGroup
{
    [Header("Vitals Group")]
    public GameObject heartRate;
    public GameObject temp;
    public GameObject oxyCons;
    public GameObject co2Prod;
    public GameObject priOxyPres;
    public GameObject priOxyStor;
    public GameObject secOxyPres;
    public GameObject helmetCO2Pres;
    public GameObject otherSuitPres;
    public GameObject scrubberA;
    public GameObject secFan;
    public GameObject scrubberB;
    public GameObject coolant;
    public GameObject suitPresOxy;
    public GameObject suitTotPres;
    public GameObject suitPresCO2;
    public GameObject secOxyStor;
    public GameObject priFan;
    public GameObject coolLiquidPres;
    public GameObject coolGasPres;
    public GameObject powerTime;
    public GameObject oxyTime;
    public Slider oxySlider;
    public Slider battSlider;
}

public class VitalsData
{
    public string room;
    public string use;
    public Vitals data;
}

public class VitalsController : MonoBehaviour
{
    [SerializeField] private GameObject vitals;    
    [SerializeField] private GameObject vitalsFirstAstronautScreen;
    [SerializeField] private GameObject vitalsSecondAstronautScreen;
    [SerializeField] private GameObject selfAlert;
    //[SerializeField] private GameObject otherAlert;

    [SerializeField] private GameObject alerts;
    public DCUController dcuControllerInstance;
    private Subscription<UpdatedVitalsEvent> vitalsUpdateEvent;
    private Subscription<UpdatedFellowAstronautVitalsEvent> fellowVitalsUpdateEvent;
    [SerializeField] private EVAGroup eva1;
    [SerializeField] private EVAGroup eva2;
    [SerializeField] private LMCCWebSocketClient LMCCwebSocketClient;
    [SerializeField] private ToggleCollection toggleCollection;


    private string message;
    // Suit Resources Parameters
    private float BATT_TIME_MIN = 3600.0f; // sec
    private float BATT_TIME_MAX = 10800.0f; // sec

    private float OXY_STOR_MIN = 20.0f; // %
    private float OXY_STOR_MAX = 100.0f; // %

    private float OXY_PRES_MIN = 600.0f; // PSI
    private float OXY_PRES_MAX = 3000.0f; // PSI

    private float OXY_TIME_MIN = 3600.0f; // sec
    private float OXY_TIME_MAX = 21600.0f; // sec

    private float COOL_STOR_MIN = 80.0f; // %
    private float COOL_STOR_MAX = 100.0f; // %

    // Suit Atmosphere Parameters
    private float HEART_RATE_MIN = 50.0f; // bpm
    private float HEART_RATE_MAX = 160.0f; // bpm

    private float OXY_CONSUM_MIN = 0.05f; // psi/min
    private float OXY_CONSUM_MAX = 0.15f; // psi/min

    private float CO2_PROD_MIN = 0.05f; // psi/min
    private float CO2_PROD_MAX = 0.15f; // psi/min

    private float SUIT_PRES_OXY_MIN = 3.5f; // psi
    private float SUIT_PRES_OXY_MAX = 4.1f; // psi

    private float SUIT_PRES_CO2_MAX = 0.1f; // psi

    private float SUIT_PRES_OTHER_MAX = 0.5f; // psi

    private float SUIT_PRES_TOTAL_MIN = 3.5f; // psi
    private float SUIT_PRES_TOTAL_MAX = 4.5f; // psi
    
    private float HELMET_PRES_CO2_MAX = 0.15f; // psi

    // Suit Fan Parameters
    private float FAN_SPEED_MIN = 19990.0f; // rpm
    private float FAN_SPEED_MAX = 30010.0f; // rpm

    // Suit Scrubber Parameters
    private float SCRUBBER_CO2_STOR_MAX = 60.0f; // %

    // Suit Temperature Parameters
    private float TEMP_MIN = 50.0f; // farhenheit
    private float TEMP_MAX = 90.0f; // farhenheit

    private float COOL_LIQ_MIN = 100.0f; // psi
    private float COOL_LIQ_MAX = 700.0f; // psi

    private float COOL_GAS_MAX = 700.0f; // psi

    private void Start() 
    {
        vitalsUpdateEvent = EventBus.Subscribe<UpdatedVitalsEvent>(vitalsEventHandler);
        fellowVitalsUpdateEvent = EventBus.Subscribe<UpdatedFellowAstronautVitalsEvent>(fellowVitalsEventHandler);
        vitalsSecondAstronautScreen.SetActive(false);
        selfAlert.SetActive(false);
        // otherAlert.SetActive(false);
    }


    public void openFeatureScreen()
    {
        vitals.SetActive(true);
        vitals.transform.Find("CloseButton").gameObject.SetActive(true);
        vitalsFirstAstronautScreen.SetActive(true);
        alerts.SetActive(true);
        vitalsSecondAstronautScreen.SetActive(false);
    }


    public void CloseVitalScreen()
    {
        transform.parent.gameObject.SetActive(false);
        
    }

    public void swapVitalScreens()
    {
        bool state = vitalsFirstAstronautScreen.gameObject.activeSelf;
        vitalsFirstAstronautScreen.SetActive(!state);
        vitalsSecondAstronautScreen.SetActive(state);
    }

    private void vitalsEventHandler(UpdatedVitalsEvent e)
    {
        onVitalsUpdate(e);
        // onVitalsErrorCheck(e);
    }

    private void fellowVitalsEventHandler(UpdatedFellowAstronautVitalsEvent e)
    {
        onFellowVitalsUpdate(e);
        onFellowVitalsErrorCheck(e);
    }


    private void onVitalsUpdate(UpdatedVitalsEvent e)
    {
        //astronaut 1
        eva1.oxyCons.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.oxy_consumption.ToString("F2");
        eva1.heartRate.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.heart_rate.ToString("F0");
        eva1.co2Prod.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.co2_production.ToString("F2");
        eva1.temp.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.temperature.ToString("F0");

        eva1.priOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.oxy_pri_pressure.ToString("F0");
        eva1.secOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.oxy_sec_pressure.ToString("F0");
        eva1.coolant.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.coolant_m.ToString("F0");
        eva1.priOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.oxy_pri_storage.ToString("F0");
        eva1.secOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.oxy_sec_storage.ToString("F0");

        eva1.suitTotPres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_total / 4.5) * 302));
        eva1.suitTotPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_total.ToString("F1");
        eva1.suitPresOxy.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_oxy / 4.1) * 302));
        eva1.suitPresOxy.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_oxy.ToString("F1");
        eva1.suitPresCO2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_co2 / 0.1) * 302));
        eva1.suitPresCO2.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_co2.ToString("F2");
        eva1.otherSuitPres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_other / 0.5) * 302));
        eva1.otherSuitPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_other.ToString("F1");
        eva1.helmetCO2Pres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.helmet_pressure_co2 / 0.15) * 302));
        eva1.helmetCO2Pres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.helmet_pressure_co2.ToString("F2");

        eva1.priFan.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.fan_pri_rpm.ToString("N0");
        eva1.secFan.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.fan_sec_rpm.ToString("N0");
        eva1.scrubberA.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.scrubber_a_co2_storage.ToString("F0");
        eva1.scrubberB.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.scrubber_b_co2_storage.ToString("F0");
        eva1.coolLiquidPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.coolant_liquid_pressure.ToString("F0");
        eva1.coolGasPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.coolant_gas_pressure.ToString("F0");

        int oxyTimeLeftSeconds = e.vitals.oxy_time_left;
        int oxyHours = oxyTimeLeftSeconds / 3600;
        int oxyMinutes = oxyTimeLeftSeconds % 3600 / 60;
        //Debug.Log(oxyTimeLeftSeconds);
        eva1.oxyTime.transform.Find("Value").GetComponent<TextMeshPro>().text = $"{oxyHours} hr {oxyMinutes} m";
        eva1.oxySlider.Value = e.vitals.oxy_time_left;

        double powerTimeLeftSeconds = e.vitals.batt_time_left;
        int powerHours = (int)(powerTimeLeftSeconds / 3600);
        int powerMinutes = (int)(powerTimeLeftSeconds % 3600 / 60);
        eva1.powerTime.transform.Find("Value").GetComponent<TextMeshPro>().text = $"{powerHours} hr {powerMinutes} m";
        eva1.battSlider.Value = (int)e.vitals.batt_time_left;

        VitalsData vitalsData = new VitalsData
        {
            room = "VITALS",
            use = "POST",
            data = AstronautInstance.User.vitals
        };
        string json = JsonUtility.ToJson(vitalsData);
        //LMCCwebSocketClient.SendJsonData(json, "VITALS");
        //Debug.Log(json);
    }

    // For Primary Astronaut
    // private void onVitalsErrorCheck(UpdatedVitalsEvent e)
    // {
    // Debug.Log("Checking Vitals for errors");
    //     // tracks number of errors
    //     int errorCount = 0;
    //     var data = new Dictionary<string, object>
    //     {
    //         { "battery", false },
    //         { "heartRate", false },
    //         { "temp", false },
    //         { "o2SwapToPrimary", false },
    //         { "o2SwapToSecondary", false },
    //         { "o2Time", false },
    //         { "o2Cons", false },
    //         { "o2SuitPres", false },
    //         { "fanSwapToPrimary", false },
    //         { "fanSwapToSecondary", false },
    //         { "co2SwapToA", false },
    //         { "co2SwapToB", false },
    //         { "co2Prod", false },
    //         { "co2SuitPres", false },
    //         { "co2HelmPres", false },
    //         { "coolantStor", false },
    //         { "coolantGasPres", false },
    //         { "coolantLiquidPres", false },
    //         { "suitPresOth", false },
    //         { "suitPresTot", false },
    //     };

    //     // battery time
    //     if (e.vitals.batt_time_left < BATT_TIME_MIN)
    //     {
    //         data["battery"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Battery Time Low";
    //         eva1.powerTime.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.powerTime.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;
    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["battery"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.powerTime.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.powerTime.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // o2 primary storage
    //     if (e.vitals.oxy_pri_storage < OXY_STOR_MIN && dcuControllerInstance.dcu1.oxy_pri.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
    //     {
    //         data["o2SwapToSecondary"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Primary Storage Low. Switch to Secondary";
    //         eva1.priOxyStor.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.priOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["o2SwapToSecondary"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.priOxyStor.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.priOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // o2 secondary storage
    //     if (e.vitals.oxy_sec_storage < OXY_STOR_MIN && dcuControllerInstance.dcu1.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
    //     {
    //         data["o2SwapToPrimary"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Secondary Storage Low. Switch to Primary";
    //         eva1.secOxyStor.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.secOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["o2SwapToPrimary"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.secOxyStor.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.secOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }
           
    //     // o2 primary pressure
    //     if ((e.vitals.oxy_pri_pressure < OXY_PRES_MIN || e.vitals.oxy_pri_pressure > OXY_PRES_MAX) && dcuControllerInstance.dcu1.oxy_pri.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
    //     {
    //         data["o2SwapToSecondary"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Primary Pressure Low. Switch to Secondary";
    //         eva1.priOxyPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.priOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["o2SwapToSecondary"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.priOxyPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.priOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // o2 secondary pressure
    //     if ((e.vitals.oxy_sec_pressure < OXY_PRES_MIN || e.vitals.oxy_sec_pressure > OXY_PRES_MAX) && dcuControllerInstance.dcu1.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
    //     {
    //         data["o2SwapToPrimary"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Secondary Pressure Low. Switch to Primary";
    //         eva1.secOxyPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.secOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["o2SwapToPrimary"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.secOxyPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.secOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // o2 time
    //     if (e.vitals.oxy_time_left < OXY_STOR_MIN)
    //     {
    //         data["o2Time"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Time Low";
    //         eva1.oxyTime.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.oxyTime.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["o2Time"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.oxyTime.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.oxyTime.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // coolant storage
    //     if (e.vitals.coolant_m < COOL_STOR_MIN)
    //     {
    //         data["coolantStor"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Coolant Storage Low";
    //         eva1.coolant.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.coolant.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["coolantStor"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.coolant.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.coolant.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // heart rate
    //     if (e.vitals.heart_rate < HEART_RATE_MIN || e.vitals.heart_rate > HEART_RATE_MAX)
    //     {
    //         data["heartRate"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Heart Rate Out of Nominal Range";
    //         eva1.heartRate.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.heartRate.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["heartRate"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.heartRate.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.heartRate.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // o2 consumption
    //     if (e.vitals.oxy_consumption < OXY_CONSUM_MIN || e.vitals.oxy_consumption > OXY_CONSUM_MAX)
    //     {
    //         data["o2Cons"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Consumption Out of Nominal Range";
    //         eva1.oxyCons.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.oxyCons.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["o2Cons"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.oxyCons.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.oxyCons.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // co2 production
    //     if (e.vitals.co2_production < CO2_PROD_MIN || e.vitals.co2_production > CO2_PROD_MAX)
    //     {
    //         data["co2Prod"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "CO2 Production Out of Nominal Range";
    //         eva1.co2Prod.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.co2Prod.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["co2Prod"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.co2Prod.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.co2Prod.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }
        
    //     // suit o2 pressure
    //     if (e.vitals.suit_pressure_oxy < SUIT_PRES_OXY_MIN || e.vitals.suit_pressure_oxy > SUIT_PRES_OXY_MAX)
    //     {
    //         data["o2SuitPres"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Suit O2 Pressure Out of Nominal Range";
    //         eva1.suitPresOxy.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.suitPresOxy.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["o2SuitPres"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.suitPresOxy.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.suitPresOxy.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // suit co2 pressure
    //     if (e.vitals.suit_pressure_co2 > SUIT_PRES_CO2_MAX)
    //     {
    //         data["co2SuitPres"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Suit CO2 Pressure Out of Nominal Range";
    //         eva1.suitPresCO2.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.suitPresCO2.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["co2SuitPres"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.suitPresCO2.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.suitPresCO2.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // suit other pressure
    //     if (e.vitals.suit_pressure_other > SUIT_PRES_OTHER_MAX)
    //     {
    //         data["suitPresOth"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Suit Other Pressure Out of Nominal Range";
    //         eva1.otherSuitPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.otherSuitPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["suitPresOth"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.otherSuitPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.otherSuitPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // suit pressure total
    //     if (e.vitals.suit_pressure_total < SUIT_PRES_TOTAL_MIN || e.vitals.suit_pressure_total > SUIT_PRES_TOTAL_MAX)
    //     {
    //         data["suitPresTot"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Suit Total Pressure Out of Nominal Range";
    //         eva1.suitTotPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.suitTotPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["suitPresTot"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.suitTotPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.suitTotPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // helment pressure
    //     if (e.vitals.helmet_pressure_co2 > HELMET_PRES_CO2_MAX)
    //     {
    //         data["co2HelmPres"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Helmet CO2 Pressure Out of Nominal Range";
    //         eva1.helmetCO2Pres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.helmetCO2Pres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["co2HelmPres"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.helmetCO2Pres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.helmetCO2Pres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // fan primary speed
    //     if ((e.vitals.fan_pri_rpm < FAN_SPEED_MIN || e.vitals.fan_pri_rpm > FAN_SPEED_MAX) && dcuControllerInstance.dcu1.fan_pri.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
    //     {
    //         data["fanSwapToSecondary"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Fan Primary Speed Low. Switch to Secondary";
    //         eva1.priFan.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.priFan.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["fanSwapToSecondary"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.priFan.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.priFan.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // fan secondary speed
    //     if ((e.vitals.fan_sec_rpm < FAN_SPEED_MIN || e.vitals.fan_sec_rpm > FAN_SPEED_MAX) && dcuControllerInstance.dcu1.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
    //     {
    //         data["fanSwapToPrimary"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Fan Secondary Speed Low. Switch to Primary";
    //         eva1.secFan.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.secFan.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["fanSwapToPrimary"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.secFan.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.secFan.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // co2 A storage
    //     if (e.vitals.scrubber_a_co2_storage > SCRUBBER_CO2_STOR_MAX && dcuControllerInstance.dcu1.co2_a.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
    //     {
    //         data["co2SwapToB"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "CO2 A Storage High. Switch to B";
    //         eva1.scrubberA.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.scrubberA.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["co2SwapToB"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.scrubberA.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.scrubberA.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // co2 B storage
    //     if (e.vitals.scrubber_b_co2_storage > SCRUBBER_CO2_STOR_MAX && dcuControllerInstance.dcu1.co2_b.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
    //     {
    //         data["co2SwapToA"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "CO2 B Storage High. Switch to A";
    //         eva1.scrubberB.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.scrubberB.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["co2SwapToA"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.scrubberB.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.scrubberB.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // temperature
    //     if (e.vitals.temperature < TEMP_MIN || e.vitals.temperature > TEMP_MAX)
    //     {
    //         data["temp"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Temperature Out of Nominal Range";
    //         eva1.temp.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.temp.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["temp"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.temp.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.temp.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // coolant liquid
    //     if (e.vitals.coolant_liquid_pressure < COOL_LIQ_MIN || e.vitals.coolant_liquid_pressure > COOL_LIQ_MAX)
    //     {
    //         data["coolantLiqPres"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Coolant Liquid Pressure Out of Nominal Range";
    //         eva1.coolLiquidPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.coolLiquidPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["coolantLiqPres"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.coolLiquidPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.coolLiquidPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }

    //     // coolant gas
    //     if (e.vitals.coolant_gas_pressure > COOL_GAS_MAX)
    //     {
    //         data["coolantGasPres"] = true;
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Coolant Gas Pressure Out of Nominal Range";
    //         eva1.coolGasPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
    //         eva1.coolGasPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

    //         errorCount++;
    //     }
    //     else
    //     {
    //         data["coolantGasPres"] = false;
    //         //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //         eva1.coolGasPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
    //         eva1.coolGasPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
    //     }
    //     if (errorCount == 0)
    //     {
    //         selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
    //     }


    //     Debug.Log("Data: " + data);
    //     LMCCwebSocketClient.SendJsonData(data, "VITALS", 3);
    //     LMCCwebSocketClient.SendJsonData(data, "VITALS", 4);
    //     if (errorCount > 0)
    //     {
    //         selfAlert.SetActive(true);
    //     }
    // }


    private void onFellowVitalsUpdate(UpdatedFellowAstronautVitalsEvent e)
    {
        //astronaut 2
        eva2.oxyCons.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.oxy_consumption.ToString("F2");
        eva2.heartRate.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.heart_rate.ToString("F0");
        eva2.co2Prod.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.co2_production.ToString("F2");
        eva2.temp.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.temperature.ToString("F0");

        eva2.priOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.oxy_pri_pressure.ToString("F0");
        eva2.secOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.oxy_sec_pressure.ToString("F0");
        eva2.coolant.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.coolant_m.ToString("F0");
        eva2.priOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.oxy_pri_storage.ToString("F0");
        eva2.secOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.oxy_sec_storage.ToString("F0");

        eva2.suitTotPres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_total / 4.5) * 302));
        eva2.suitTotPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_total.ToString("F1");
        eva2.suitPresOxy.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_oxy / 4.1) * 302));
        eva2.suitPresOxy.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_oxy.ToString("F1");
        eva2.suitPresCO2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_co2 / 0.1) * 302));
        eva2.suitPresCO2.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_co2.ToString("F2");
        eva2.otherSuitPres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_other / 0.5) * 302));
        eva2.otherSuitPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_other.ToString("F1");
        eva2.helmetCO2Pres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.helmet_pressure_co2 / 0.15) * 302));
        eva2.helmetCO2Pres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.helmet_pressure_co2.ToString("F2");

        eva2.priFan.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.fan_pri_rpm.ToString("N0");
        eva2.secFan.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.fan_sec_rpm.ToString("N0");
        eva2.scrubberA.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.scrubber_a_co2_storage.ToString("F0");
        eva2.scrubberB.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.scrubber_b_co2_storage.ToString("F0");
        eva2.coolLiquidPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.coolant_liquid_pressure.ToString("F0");
        eva2.coolGasPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.coolant_gas_pressure.ToString("F0");

        int oxyTimeLeftSeconds = e.vitals.oxy_time_left;
        int oxyHours = oxyTimeLeftSeconds / 3600;
        int oxyMinutes = oxyTimeLeftSeconds % 3600 / 60;
        Debug.Log(oxyTimeLeftSeconds);
        eva2.oxyTime.transform.Find("Value").GetComponent<TextMeshPro>().text = $"{oxyHours} hr {oxyMinutes} m";
        eva2.oxySlider.Value = e.vitals.oxy_time_left;

        double powerTimeLeftSeconds = e.vitals.batt_time_left;
        int powerHours = (int)(powerTimeLeftSeconds / 3600);
        int powerMinutes = (int)(powerTimeLeftSeconds % 3600 / 60);
        eva2.powerTime.transform.Find("Value").GetComponent<TextMeshPro>().text = $"{powerHours} hr {powerMinutes} m";
        eva2.battSlider.Value = (int)e.vitals.batt_time_left;

        // VitalsData vitalsData = new VitalsData
        //{
        //    room = "VITALS",
        //     use = "POST",
        //     data = AstronautInstance.User.vitals
        // };
        // string json = JsonUtility.ToJson(vitalsData);
        //LMCCwebSocketClient.SendJsonData(json, "VITALS");
        //Debug.Log(json);

    }

    // For Companion Astronaut
    private void onFellowVitalsErrorCheck(UpdatedFellowAstronautVitalsEvent e)
    {
    Debug.Log("Checking Vitals for errors");
        // tracks number of errors
        int errorCount = 0;
        var data = new Dictionary<string, object>
        {
            { "battery", false },
            { "heartRate", false },
            { "temp", false },
            { "o2SwapToPrimary", false },
            { "o2SwapToSecondary", false },
            { "o2Time", false },
            { "o2Cons", false },
            { "o2SuitPres", false },
            { "fanSwapToPrimary", false },
            { "fanSwapToSecondary", false },
            { "co2SwapToA", false },
            { "co2SwapToB", false },
            { "co2Prod", false },
            { "co2SuitPres", false },
            { "co2HelmPres", false },
            { "coolantStor", false },
            { "coolantGasPres", false },
            { "coolantLiquidPres", false },
            { "suitPresOth", false },
            { "suitPresTot", false },
        };

        // battery time
        if (e.vitals.batt_time_left < BATT_TIME_MIN)
        {
            data["battery"] = true;
        // otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Battery Time Low";
            eva2.powerTime.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.powerTime.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;
            errorCount++;
        }
        else
        {
            data["battery"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.powerTime.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.powerTime.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // o2 primary storage
        if (e.vitals.oxy_pri_storage < OXY_STOR_MIN && dcuControllerInstance.dcu2.oxy_pri.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
        {
            data["o2SwapToSecondary"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Primary Storage Low. Switch to Secondary";
            eva2.priOxyStor.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.priOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["o2SwapToSecondary"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.priOxyStor.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.priOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // o2 secondary storage
        if (e.vitals.oxy_sec_storage < OXY_STOR_MIN && dcuControllerInstance.dcu2.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
        {
            data["o2SwapToPrimary"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Secondary Storage Low. Switch to Primary";
            eva2.secOxyStor.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.secOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["o2SwapToPrimary"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.secOxyStor.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.secOxyStor.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }
           
        // o2 primary pressure
        if ((e.vitals.oxy_pri_pressure < OXY_PRES_MIN || e.vitals.oxy_pri_pressure > OXY_PRES_MAX) && dcuControllerInstance.dcu2.oxy_pri.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
        {
            data["o2SwapToSecondary"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Primary Pressure Low. Switch to Secondary";
            eva2.priOxyPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.priOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["o2SwapToSecondary"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.priOxyPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.priOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // o2 secondary pressure
        if ((e.vitals.oxy_sec_pressure < OXY_PRES_MIN || e.vitals.oxy_sec_pressure > OXY_PRES_MAX) && dcuControllerInstance.dcu2.oxy_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
        {
            data["o2SwapToPrimary"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Secondary Pressure Low. Switch to Primary";
            eva2.secOxyPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.secOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["o2SwapToPrimary"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.secOxyPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.secOxyPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // o2 time
        if (e.vitals.oxy_time_left < OXY_STOR_MIN)
        {
            data["o2Time"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Time Low";
            eva2.oxyTime.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.oxyTime.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["o2Time"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.oxyTime.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.oxyTime.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // coolant storage
        if (e.vitals.coolant_m < COOL_STOR_MIN)
        {
            data["coolantStor"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Coolant Storage Low";
            eva2.coolant.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.coolant.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["coolantStor"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.coolant.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.coolant.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // heart rate
        if (e.vitals.heart_rate < HEART_RATE_MIN || e.vitals.heart_rate > HEART_RATE_MAX)
        {
            data["heartRate"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Heart Rate Out of Nominal Range";
            eva2.heartRate.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.heartRate.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["heartRate"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.heartRate.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.heartRate.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // o2 consumption
        if (e.vitals.oxy_consumption < OXY_CONSUM_MIN || e.vitals.oxy_consumption > OXY_CONSUM_MAX)
        {
            data["o2Cons"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "O2 Consumption Out of Nominal Range";
            eva2.oxyCons.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.oxyCons.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["o2Cons"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.oxyCons.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.oxyCons.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // co2 production
        if (e.vitals.co2_production < CO2_PROD_MIN || e.vitals.co2_production > CO2_PROD_MAX)
        {
            data["co2Prod"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "CO2 Production Out of Nominal Range";
            eva2.co2Prod.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.co2Prod.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["co2Prod"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.co2Prod.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.co2Prod.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }
        
        // suit o2 pressure
        if (e.vitals.suit_pressure_oxy < SUIT_PRES_OXY_MIN || e.vitals.suit_pressure_oxy > SUIT_PRES_OXY_MAX)
        {
            data["o2SuitPres"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Suit O2 Pressure Out of Nominal Range";
            eva2.suitPresOxy.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.suitPresOxy.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["o2SuitPres"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.suitPresOxy.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.suitPresOxy.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // suit co2 pressure
        if (e.vitals.suit_pressure_co2 > SUIT_PRES_CO2_MAX)
        {
            data["co2SuitPres"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Suit CO2 Pressure Out of Nominal Range";
            eva2.suitPresCO2.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.suitPresCO2.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["co2SuitPres"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.suitPresCO2.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.suitPresCO2.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // suit other pressure
        if (e.vitals.suit_pressure_other > SUIT_PRES_OTHER_MAX)
        {
            data["suitPresOth"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Suit Other Pressure Out of Nominal Range";
            eva2.otherSuitPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.otherSuitPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["suitPresOth"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.otherSuitPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.otherSuitPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // suit pressure total
        if (e.vitals.suit_pressure_total < SUIT_PRES_TOTAL_MIN || e.vitals.suit_pressure_total > SUIT_PRES_TOTAL_MAX)
        {
            data["suitPresTot"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Suit Total Pressure Out of Nominal Range";
            eva2.suitTotPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.suitTotPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["suitPresTot"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.suitTotPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.suitTotPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // helment pressure
        if (e.vitals.helmet_pressure_co2 > HELMET_PRES_CO2_MAX)
        {
            data["co2HelmPres"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Helmet CO2 Pressure Out of Nominal Range";
            eva2.helmetCO2Pres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.helmetCO2Pres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["co2HelmPres"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.helmetCO2Pres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.helmetCO2Pres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // fan primary speed
        if ((e.vitals.fan_pri_rpm < FAN_SPEED_MIN || e.vitals.fan_pri_rpm > FAN_SPEED_MAX) && dcuControllerInstance.dcu2.fan_pri.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
        {
            data["fanSwapToSecondary"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Fan Primary Speed Low. Switch to Secondary";
            eva2.priFan.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.priFan.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["fanSwapToSecondary"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.priFan.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.priFan.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // fan secondary speed
        if ((e.vitals.fan_sec_rpm < FAN_SPEED_MIN || e.vitals.fan_sec_rpm > FAN_SPEED_MAX) && dcuControllerInstance.dcu2.fan_sec.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
        {
            data["fanSwapToPrimary"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Fan Secondary Speed Low. Switch to Primary";
            eva2.secFan.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.secFan.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["fanSwapToPrimary"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.secFan.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.secFan.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // co2 A storage
        if (e.vitals.scrubber_a_co2_storage > SCRUBBER_CO2_STOR_MAX && dcuControllerInstance.dcu2.co2_a.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
        {
            data["co2SwapToB"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "CO2 A Storage High. Switch to B";
            eva2.scrubberA.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.scrubberA.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["co2SwapToB"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.scrubberA.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.scrubberA.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // co2 B storage
        if (e.vitals.scrubber_b_co2_storage > SCRUBBER_CO2_STOR_MAX && dcuControllerInstance.dcu2.co2_b.transform.Find("UIBackplateToggleQuad").gameObject.activeSelf)
        {
            data["co2SwapToA"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "CO2 B Storage High. Switch to A";
            eva2.scrubberB.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.scrubberB.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["co2SwapToA"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.scrubberB.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.scrubberB.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // temperature
        if (e.vitals.temperature < TEMP_MIN || e.vitals.temperature > TEMP_MAX)
        {
            data["temp"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Temperature Out of Nominal Range";
            eva2.temp.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.temp.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["temp"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.temp.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.temp.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // coolant liquid
        if (e.vitals.coolant_liquid_pressure < COOL_LIQ_MIN || e.vitals.coolant_liquid_pressure > COOL_LIQ_MAX)
        {
            data["coolantLiqPres"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Coolant Liquid Pressure Out of Nominal Range";
            eva2.coolLiquidPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.coolLiquidPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["coolantLiqPres"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.coolLiquidPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.coolLiquidPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }

        // coolant gas
        if (e.vitals.coolant_gas_pressure > COOL_GAS_MAX)
        {
            data["coolantGasPres"] = true;
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "Coolant Gas Pressure Out of Nominal Range";
            eva2.coolGasPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.red;
            eva2.coolGasPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.red;

            errorCount++;
        }
        else
        {
            data["coolantGasPres"] = false;
            //selfAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
            eva2.coolGasPres.transform.Find("Title").GetComponent<TextMeshPro>().color = Color.white;
            eva2.coolGasPres.transform.Find("Value").GetComponent<TextMeshPro>().color = Color.white;
        }
        if (errorCount == 0)
        {
        //    otherAlert.transform.Find("Message").GetComponent<TextMeshPro>().text = "";
        }


        //Debug.Log("Data: " + data);
        LMCCwebSocketClient.SendJsonData(data, "VITALS", 3);
        LMCCwebSocketClient.SendJsonData(data, "VITALS", 4);
        if (errorCount > 0)
        {
        //    otherAlert.SetActive(true);
        }
    }


    private void OnDestroy() 
    {
        EventBus.Unsubscribe(vitalsUpdateEvent);
        EventBus.Unsubscribe(fellowVitalsUpdateEvent);
    }
}
