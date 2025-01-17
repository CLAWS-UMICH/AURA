using UnityEngine;
using TMPro;
using System.Collections;
using MixedReality.Toolkit.UX;

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
    public string type;
    public string use;
    public Vitals data;
}

public class VitalsController : MonoBehaviour
{
    [SerializeField] private GameObject vitalsFirstAstronautScreen;
    [SerializeField] private GameObject vitalsSecondAstronautScreen;
    private Subscription<UpdatedVitalsEvent> vitalsUpdateEvent;
    private Subscription<FellowAstronautVitalsDataChangeEvent> fellowVitalsUpdateEvent;
    [SerializeField] private EVAGroup eva1;
    [SerializeField] private EVAGroup eva2;
    private WebSocketClient webSocketClient;

    private void Start() 
    {
        InitializeWebConnection();
        vitalsUpdateEvent = EventBus.Subscribe<UpdatedVitalsEvent>(onVitalsUpdate);
        fellowVitalsUpdateEvent = EventBus.Subscribe<FellowAstronautVitalsDataChangeEvent>(onFellowVitalsUpdate);
        vitalsSecondAstronautScreen.SetActive(false);
    }


    private void  InitializeWebConnection()
    {
        GameObject controllerObject = GameObject.Find("VitalsController");
        if (controllerObject != null){
            webSocketClient = controllerObject.GetComponent<WebSocketClient>();
            if (webSocketClient != null){
                Debug.Log("Successfully connected to the existing WebSocketClient from Controller.");
            } else{
                Debug.LogWarning("WebSocketClient component not found on Controller.");
            }
        } else{
            Debug.LogError("Controller object not found in the scene.");
        }
    }


    public void ToggleVitalsScreen()
    {
        Debug.Log($"Current Screen: {StateMachine.Instance.CurrScreen}");
        if (vitalsFirstAstronautScreen.activeSelf)
        {
            vitalsFirstAstronautScreen.SetActive(false);
            vitalsSecondAstronautScreen.SetActive(true);
            EventBus.Publish(new ScreenChangedEvent(Screens.VitalsSecondAstronaut)); 
        }
        else 
        {
            vitalsSecondAstronautScreen.SetActive(false);
            vitalsFirstAstronautScreen.SetActive(true);
            EventBus.Publish(new ScreenChangedEvent(Screens.VitalsFirstAstronaut));
        }
    }


    public void CloseVitalScreen()
    {
        vitalsFirstAstronautScreen.SetActive(false);
        vitalsSecondAstronautScreen.SetActive(false);
        StateMachine.Instance.CurrScreen = Screens.Menu;
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
        Debug.Log(oxyTimeLeftSeconds);
        eva1.oxyTime.transform.Find("Value").GetComponent<TextMeshPro>().text = $"{oxyHours} hr {oxyMinutes} m";
        eva1.oxySlider.Value = e.vitals.oxy_time_left;

        int powerTimeLeftSeconds = e.vitals.batt_time_left;
        int powerHours = powerTimeLeftSeconds / 3600;
        int powerMinutes = powerTimeLeftSeconds % 3600 / 60;
        eva1.powerTime.transform.Find("Value").GetComponent<TextMeshPro>().text = $"{powerHours} hr {powerMinutes} m";
        eva1.battSlider.Value = e.vitals.batt_time_left;

        VitalsData vitalsData = new VitalsData
            {
                type = "VITALS",
                use = "POST",
                data = AstronautInstance.User.vitals
            };
        string json = JsonUtility.ToJson(vitalsData);
        webSocketClient.SendJsonData(json, "VITALS");
        //Debug.Log(json);
    }


    private void onFellowVitalsUpdate(FellowAstronautVitalsDataChangeEvent e)
    {
        //astronaut 2

    }

    private void OnDestroy() 
    {
        EventBus.Unsubscribe(vitalsUpdateEvent);
        EventBus.Unsubscribe(fellowVitalsUpdateEvent);
    }
}
