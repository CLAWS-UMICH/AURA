using UnityEngine;
using TMPro;
using System;
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
    private Subscription<CloseEvent> closeSubscription;
    private Subscription<ScreenChangedEvent> screenChangedSubscription;
    [SerializeField] private EVAGroup eva1;
    [SerializeField] private EVAGroup eva2;
    private WebSocketClient webSocketClient;

    private float BATT_TIME_MAX = 10800.0f; // sec
    private float BATT_TIME_MIN = 3600.0f; // sec

    // Oxygen Parameters
    private float OXY_STOR_MAX = 100.0f; // %
    private float OXY_STOR_MIN = 20.0f; // %

    private float OXY_PRES_MAX = 3000.0f; // PSI
    private float OXY_PRES_MIN = 600.0f; // PSI

    private float OXY_TIME_MAX = 21600.0f; // sec
    private float OXY_TIME_MIN = 3600.0f; // sec

    private float OXY_CONSUM_MAX = 0.15f; // psi/min
    private float OXY_CONSUM_MIN = 0.05f; // psi/min

    // CO2 Parameters
    private float CO2_PROD_MAX = 0.15f; // psi/min
    private float CO2_PROD_MIN = 0.05f; // psi/min

    // Coolant Parameters
    private float COOL_STOR_MAX = 100.0f; // %
    private float COOL_STOR_MIN = 80.0f; // %

    // Heart Rate Parameters
    private float HEART_RATE_MAX = 160.0f; // bpm
    private float HEART_RATE_MIN = 50.0f; // bpm

    // Pressure Parameters
    private float SUIT_PRES_OXY_MAX = 4.1f; // psi
    private float SUIT_PRES_OXY_MIN = 3.5f; // psi

    private float SUIT_PRES_CO2_MAX = 0.1f; // psi
    private float SUIT_PRES_OTHER_MAX = 0.5f; // psi
    private float SUIT_PRES_TOTAL_MAX = 4.5f; // psi
    private float SUIT_PRES_TOTAL_MIN = 3.5f; // psi

    private float HELMET_PRES_CO2_MAX = 0.15f; // psi

    // Fan Parameters
    private float FAN_SPEED_MAX = 30000.0f; // rpm
    private float FAN_SPEED_MIN = 20000.0f; // rpm

    // Scrubber Parameters
    private float SCRUBBER_CO2_STOR_MAX = 60.0f; // %

    // Temperature Parameters
    private float TEMP_MAX = 90.0f; // farhenheit
    private float TEMP_MIN = 50.0f; // farhenheit

    // Coolant Parameters
    private float COOL_LIQ_MAX = 700.0f; // psi
    private float COOL_LIQ_MIN = 100.0f; // psi
    private float COOL_GAS_MAX = 700.0f; // psi

    private void Start() 
    {
        InitializeWebConnection();
        vitalsUpdateEvent = EventBus.Subscribe<UpdatedVitalsEvent>(onVitalsUpdate);
        fellowVitalsUpdateEvent = EventBus.Subscribe<FellowAstronautVitalsDataChangeEvent>(onFellowVitalsUpdate);
        closeSubscription = EventBus.Subscribe<CloseEvent>(HandleCloseScreen);
        screenChangedSubscription = EventBus.Subscribe<ScreenChangedEvent>(HandleScreenChange);
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
        if (StateMachine.Instance.CurrScreen == Screens.VitalsFirstAstronaut)
        {
            EventBus.Publish(new CloseEvent(Screens.VitalsFirstAstronaut));
            EventBus.Publish(new ScreenChangedEvent(Screens.VitalsSecondAstronaut)); 
        }
        else if (StateMachine.Instance.CurrScreen == Screens.VitalsSecondAstronaut)
        {
            EventBus.Publish(new CloseEvent(Screens.VitalsSecondAstronaut));
            EventBus.Publish(new ScreenChangedEvent(Screens.VitalsFirstAstronaut));
        }
    }


    public void CloseVitalScreen()
    {
        if (StateMachine.Instance.CurrScreen == Screens.VitalsFirstAstronaut)
        {
            EventBus.Publish(new CloseEvent(Screens.VitalsFirstAstronaut));
            Debug.Log("VitalsFirstAstronaut screen closed.");
        }
        else if (StateMachine.Instance.CurrScreen == Screens.VitalsSecondAstronaut)
        {
            EventBus.Publish(new CloseEvent(Screens.VitalsSecondAstronaut));
            Debug.Log("VitalsSecondAstronaut screen closed.");
        }
    }


    private void HandleScreenChange(ScreenChangedEvent e)
    {
        if (e.Screen == Screens.VitalsFirstAstronaut)
        {
            vitalsFirstAstronautScreen.SetActive(true);
            vitalsSecondAstronautScreen.SetActive(false);
            Debug.Log("VitalsSecondAstronaut screen changed to VitalsFirstAstronaut.");
        }
        else if (e.Screen == Screens.VitalsSecondAstronaut)
        {
            vitalsSecondAstronautScreen.SetActive(true);
            vitalsFirstAstronautScreen.SetActive(false);
            Debug.Log("VitalFirstAstronaut screen changed to VitalsSecondAstronaut.");
        }
    }

    private void HandleCloseScreen(CloseEvent e)
    {
        if (e.Screen == Screens.VitalsFirstAstronaut)
        {
            vitalsFirstAstronautScreen.SetActive(false);
            Debug.Log("VitalsFirstAstronaut screen closed.");
        }
        else if (e.Screen == Screens.VitalsSecondAstronaut)
        {
            vitalsSecondAstronautScreen.SetActive(false);
            Debug.Log("VitalsSecondAstronaut screen closed.");
        }
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
        eva1.suitPresCO2.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_co2.ToString("F1");
        eva1.otherSuitPres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_other / 0.5) * 302));
        eva1.otherSuitPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_other.ToString("F1");
        eva1.helmetCO2Pres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.helmet_pressure_co2 / 0.15) * 302));
        eva1.helmetCO2Pres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.helmet_pressure_co2.ToString("F1");

        eva1.priFan.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.fan_pri_rpm.ToString("N0");
        eva1.secFan.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.fan_sec_rpm.ToString("N0");
        eva1.scrubberA.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.scrubber_a_co2_storage.ToString("F0");
        eva1.scrubberB.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.scrubber_b_co2_storage.ToString("F0");
        eva1.coolLiquidPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.coolant_liquid_pressure.ToString("F0");
        eva1.coolGasPres.transform.Find("Value").GetComponent<TextMeshPro>().text = e.vitals.coolant_gas_pressure.ToString("F0");
        
        int oxyTimeLeftSeconds = e.vitals.oxy_time_left;
        int oxyHours = oxyTimeLeftSeconds / 3600;
        int oxyMinutes = oxyTimeLeftSeconds % 3600 / 60;
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
        Debug.Log(json);
    }

    private void onFellowVitalsUpdate(FellowAstronautVitalsDataChangeEvent e)
    {
        //astronaut 2

    }

    private void checkVitals(UpdatedVitalsEvent e) {

        if (e.vitals.heart_rate > HEART_RATE_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Heart, "Heart Rate High", $"{e.vitals.heart_rate} BPM, slow down"));
        }

        // suit pressure oxygen
        if (e.vitals.suit_pressure_oxy > SUIT_PRES_OXY_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "Switch OXY to SEC", $"O2 Suit Pressure High: {e.vitals.suit_pressure_oxy} PSI"));
        }
        if (e.vitals.suit_pressure_oxy < SUIT_PRES_OXY_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "Switch OXY to SEC", $"O2 Suit Pressure Low: {e.vitals.suit_pressure_oxy} PSI"));
        }
        if (e.vitals.oxy_pri_storage < OXY_STOR_MIN)
        {
            string desc = $"Primary Oxygen Low: {e.vitals.oxy_pri_storage.ToString()}%";
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "Switch OXY to SEC", desc));
        }

        // scrubbers
        if (e.vitals.scrubber_a_co2_storage > SCRUBBER_CO2_STOR_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Scrubber, "Switch CO2 to B", $"Scrubber A High: {e.vitals.scrubber_a_co2_storage.ToString()}%"));
        }
        if (e.vitals.scrubber_b_co2_storage > SCRUBBER_CO2_STOR_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Scrubber, "Switch CO2 to A", $"Scrubber B High: {e.vitals.scrubber_b_co2_storage.ToString()}%"));
        }
        if (e.vitals.suit_pressure_co2 > SUIT_PRES_CO2_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_CO2, "Switch CO2 to B", $"CO2 Suit Pressure High: {e.vitals.suit_pressure_co2.ToString()} PSI"));
        }


        // Fans
        if (e.vitals.fan_pri_rpm < FAN_SPEED_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Switch FAN to SEC", $"Fan Speed Low: {e.vitals.fan_pri_rpm.ToString()} RPM"));
        }
        if (e.vitals.fan_pri_rpm > FAN_SPEED_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Switch FAN to SEC", $"Fan Speed High: {e.vitals.fan_pri_rpm.ToString()} RPM"));
        }
        if (e.vitals.fan_sec_rpm < FAN_SPEED_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Switch FAN to PRI", $"Fan Speed Low: {e.vitals.fan_sec_rpm.ToString()} RPM"));
        }
        if (e.vitals.fan_sec_rpm > FAN_SPEED_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Switch FAN to PRI", $"Fan Speed High: {e.vitals.fan_sec_rpm.ToString()} RPM"));
        }
        if (e.vitals.helmet_pressure_co2 > HELMET_PRES_CO2_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Switch FAN to SEC", $"Helmet CO2 Pressure High: {e.vitals.helmet_pressure_co2.ToString()} PSI"));
        }

        // Temperature
        if (e.vitals.temperature > TEMP_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Temp, "Temperature High", $"{e.vitals.helmet_pressure_co2.ToString()} ?F. Slow Down"));
        }

        // Battery
        if (e.vitals.batt_time_left < BATT_TIME_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Battery, "Switch BATT to LOCAL", $"Battery Low: {e.vitals.batt_time_left.ToString()}%"));
        }

        ///////////////

        // Regular Notifs

        if (e.vitals.oxy_consumption > OXY_CONSUM_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "O2 Consumption High", $"{e.vitals.oxy_consumption.ToString()} PSI/m"));
        }
        if (e.vitals.oxy_pri_storage < OXY_STOR_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "O2 Pri Storage Low", $"{e.vitals.oxy_pri_storage.ToString()}%"));
        }
        if (e.vitals.oxy_sec_storage < OXY_STOR_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "O2 Sec Storage Low", $"{e.vitals.oxy_sec_storage.ToString()}%"));
        }
        if (e.vitals.coolant_m < COOL_STOR_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Coolant, "Coolant Low", $"{e.vitals.coolant_m.ToString()}%"));
        }
        if (e.vitals.scrubber_a_co2_storage > SCRUBBER_CO2_STOR_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Scrubber, "Scrubber A High", $"{e.vitals.scrubber_a_co2_storage.ToString()}%"));
        }
        if (e.vitals.scrubber_b_co2_storage > SCRUBBER_CO2_STOR_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Scrubber, "Scrubber B High", $"{e.vitals.scrubber_b_co2_storage.ToString()}%"));
        }
        if (e.vitals.co2_production > CO2_PROD_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_CO2, "CO2 Production High", $"{e.vitals.co2_production.ToString()} PSI/m"));
        }
        if (e.vitals.suit_pressure_other > SUIT_PRES_OTHER_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Other Suit Pressure High", $"{e.vitals.suit_pressure_other.ToString()} PSI"));
        }
        if (e.vitals.suit_pressure_total > SUIT_PRES_TOTAL_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Total Suit Pressure High", $"{e.vitals.suit_pressure_total.ToString()} PSI"));
        }
        if (e.vitals.suit_pressure_total < SUIT_PRES_TOTAL_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Total Suit Pressure Low", $"{e.vitals.suit_pressure_total.ToString()} PSI"));
        }
        if (e.vitals.oxy_pri_pressure > OXY_PRES_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Pri Pressure High", $"{e.vitals.oxy_pri_pressure.ToString()} PSI"));
        }
        if (e.vitals.oxy_pri_pressure < OXY_PRES_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Pri Pressure Low", $"{e.vitals.oxy_pri_pressure.ToString()} PSI"));
        }
        if (e.vitals.oxy_sec_pressure > OXY_PRES_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Sec Pressure High", $"{e.vitals.oxy_sec_pressure.ToString()} PSI"));
        }
        if (e.vitals.oxy_pri_pressure < OXY_PRES_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Sec Pressure Low", $"{e.vitals.oxy_sec_pressure.ToString()} PSI"));
        }
        if (e.vitals.suit_pressure_oxy > SUIT_PRES_OXY_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Suit Pressure High", $"{e.vitals.suit_pressure_oxy.ToString()} PSI"));
        }
        if (e.vitals.suit_pressure_oxy < SUIT_PRES_OXY_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Suit Pressure Low", $"{e.vitals.suit_pressure_oxy.ToString()} PSI"));
        }
        if (e.vitals.suit_pressure_co2 > SUIT_PRES_CO2_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "CO2 Suit Pressure High", $"{e.vitals.suit_pressure_co2.ToString()} PSI"));
        }
        if (e.vitals.helmet_pressure_co2 > HELMET_PRES_CO2_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Helmet CO2 Pressure High", $"{e.vitals.helmet_pressure_co2.ToString()} PSI"));
        }
        if (e.vitals.coolant_liquid_pressure > COOL_LIQ_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Coolant Liquid Pressure High", $"{e.vitals.coolant_liquid_pressure.ToString()} PSI"));
        }
        if (e.vitals.coolant_liquid_pressure < COOL_LIQ_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Coolant Liquid Pressure Low", $"{e.vitals.coolant_liquid_pressure.ToString()} PSI"));
        }
        if (e.vitals.coolant_gas_pressure > COOL_GAS_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Coolant Gas Pressure High", $"{e.vitals.coolant_gas_pressure.ToString()} PSI"));
        }

        if (e.vitals.fan_pri_rpm > FAN_SPEED_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Fan Pri Speed High", $"{e.vitals.fan_pri_rpm.ToString()} PSI"));
        }
        if (e.vitals.fan_pri_rpm < FAN_SPEED_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Fan Pri Speed Low", $"{e.vitals.fan_pri_rpm.ToString()} PSI"));
        }
        if (e.vitals.fan_sec_rpm > FAN_SPEED_MAX)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Fan Sec Speed High", $"{e.vitals.fan_sec_rpm.ToString()} PSI"));
        }
        if (e.vitals.fan_sec_rpm < FAN_SPEED_MIN)
        {
            EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Fan Sec Speed Low", $"{e.vitals.fan_sec_rpm.ToString()} PSI"));
        }
    }
    private void OnDestroy() 
    {
        EventBus.Unsubscribe(closeSubscription);
        EventBus.Unsubscribe(screenChangedSubscription);
        EventBus.Unsubscribe(vitalsUpdateEvent);
        EventBus.Unsubscribe(fellowVitalsUpdateEvent);
    }
}
