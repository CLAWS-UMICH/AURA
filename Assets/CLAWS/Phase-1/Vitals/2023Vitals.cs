// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;
// using System;

// public class SuitsControlController : MonoBehaviour
// {
//     private Subscription<VitalsUpdatedEvent> vitalsUpdateEvent;
//     private Subscription<FellowAstronautVitalsDataChangeEvent> fellowVitalsUpdateEvent;
//     private Subscription<DCUChanged> dcuChanged;

//     private GameObject astr1Board;
//     private GameObject astr1CritBoard;
//     private GameObject astr1SuitBoard;
//     private GameObject astr1BottomBoard;

//     private GameObject astrDCUBoard;
//     private GameObject astr2DCUBoard;

//     private GameObject astr2Board;
//     private GameObject astr2CritBoard;
//     private GameObject astr2SuitBoard;
//     private GameObject astr2BottomBoard;

//     // Battery Parameters
//     private float BATT_TIME_MAX = 10800.0f; // sec
//     private float BATT_TIME_MIN = 3600.0f; // sec

//     // Oxygen Parameters
//     private float OXY_STOR_MAX = 100.0f; // %
//     private float OXY_STOR_MIN = 20.0f; // %

//     private float OXY_PRES_MAX = 3000.0f; // PSI
//     private float OXY_PRES_MIN = 600.0f; // PSI

//     private float OXY_TIME_MAX = 21600.0f; // sec
//     private float OXY_TIME_MIN = 3600.0f; // sec

//     private float OXY_CONSUM_MAX = 0.15f; // psi/min
//     private float OXY_CONSUM_MIN = 0.05f; // psi/min

//     // CO2 Parameters
//     private float CO2_PROD_MAX = 0.15f; // psi/min
//     private float CO2_PROD_MIN = 0.05f; // psi/min

//     // Coolant Parameters
//     private float COOL_STOR_MAX = 100.0f; // %
//     private float COOL_STOR_MIN = 80.0f; // %

//     // Heart Rate Parameters
//     private float HEART_RATE_MAX = 160.0f; // bpm
//     private float HEART_RATE_MIN = 50.0f; // bpm

//     // Pressure Parameters
//     private float SUIT_PRES_OXY_MAX = 4.1f; // psi
//     private float SUIT_PRES_OXY_MIN = 3.5f; // psi

//     private float SUIT_PRES_CO2_MAX = 0.1f; // psi
//     private float SUIT_PRES_OTHER_MAX = 0.5f; // psi
//     private float SUIT_PRES_TOTAL_MAX = 4.5f; // psi
//     private float SUIT_PRES_TOTAL_MIN = 3.5f; // psi

//     private float HELMET_PRES_CO2_MAX = 0.15f; // psi

//     // Fan Parameters
//     private float FAN_SPEED_MAX = 30000.0f; // rpm
//     private float FAN_SPEED_MIN = 20000.0f; // rpm

//     // Scrubber Parameters
//     private float SCRUBBER_CO2_STOR_MAX = 60.0f; // %

//     // Temperature Parameters
//     private float TEMP_MAX = 90.0f; // farhenheit
//     private float TEMP_MIN = 50.0f; // farhenheit

//     // Coolant Parameters
//     private float COOL_LIQ_MAX = 700.0f; // psi
//     private float COOL_LIQ_MIN = 100.0f; // psi
//     private float COOL_GAS_MAX = 700.0f; // psi

//     //1st astronaut
//     GameObject heartRate, temp, oxyCons, co2Prod, priOxyPres, priOxyStor, secOxyPres,
//                helmetCO2Pres, otherSuitPres, batt, scrubberA, secFan, scrubberB, coolant,
//                suitPresOxy, suitTotPres, suitPresCO2, secOxyStor, priFan, coolLiquidPres,
//                coolGasPres, powerTime, oxyTime;

//     //2nd astronaut
//     GameObject heartRate2, temp2, oxyCons2, co2Prod2, priOxyPres2, helmetCO2Pres2, secOxyPres2,
//                priOxyStor2, secFan2, otherSuitPres2, batt2, scrubberA2, scrubberB2, coolant2,
//                suitPresOxy2, suitTotPres2, suitPresCO22, secOxyStor2, priFan2, coolLiquidPres2,
//                coolGasPres2, powerTime2, oxyTime2;

//     private void Start()
//     {
//         vitalsUpdateEvent = EventBus.Subscribe<VitalsUpdatedEvent>(onVitalsUpdate);
//         fellowVitalsUpdateEvent = EventBus.Subscribe<FellowAstronautVitalsDataChangeEvent>(onFellowVitalsUpdate);
//         dcuChanged = EventBus.Subscribe<DCUChanged>(onDCUChanged);

//         //Astr 1
//         astr1Board = transform.Find("SuitsControlScreen1").gameObject;
//         astr1CritBoard = astr1Board.transform.Find("ScreenCrit").gameObject;
//         astr1SuitBoard = astr1Board.transform.Find("Screen1").gameObject;
//         astr1BottomBoard = astr1Board.transform.Find("Screen2").gameObject;
//         astrDCUBoard = astr1Board.transform.Find("DCU").gameObject;

//         oxyTime = astr1Board.transform.Find("O2priOxygen").gameObject;
//         powerTime = astr1Board.transform.Find("PriBattery").gameObject;
//         heartRate = astr1CritBoard.transform.Find("HeartRate").gameObject;
//         oxyCons = astr1CritBoard.transform.Find("O2con").gameObject;
//         co2Prod = astr1CritBoard.transform.Find("CO2prod").gameObject;
//         temp = astr1CritBoard.transform.Find("Temp").gameObject;
//         priOxyStor = astr1SuitBoard.transform.Find("O2priStor").gameObject;
//         secOxyStor = astr1SuitBoard.transform.Find("O2secStor").gameObject;
//         batt = astr1SuitBoard.transform.Find("Battery").gameObject;
//         coolant = astr1SuitBoard.transform.Find("Coolant").gameObject;
//         scrubberA = astr1SuitBoard.transform.Find("ScrubberA").gameObject;
//         scrubberB = astr1SuitBoard.transform.Find("ScrubberB").gameObject;
//         priOxyPres = astr1BottomBoard.transform.Find("O2priPress").gameObject;
//         secOxyPres = astr1BottomBoard.transform.Find("O2secPress").gameObject;
//         suitPresOxy = astr1BottomBoard.transform.Find("O2suitPress").gameObject;
//         suitTotPres = astr1BottomBoard.transform.Find("SuitTotPress").gameObject;
//         suitPresCO2 = astr1BottomBoard.transform.Find("CO2suitPress").gameObject;
//         otherSuitPres = astr1BottomBoard.transform.Find("OtherSuitPress").gameObject;
//         priFan = astr1SuitBoard.transform.Find("FanPri").gameObject;
//         secFan = astr1SuitBoard.transform.Find("FanSec").gameObject;
//         helmetCO2Pres = astr1BottomBoard.transform.Find("HelCO2press").gameObject;
//         coolLiquidPres = astr1BottomBoard.transform.Find("CoolLiqPress").gameObject;
//         coolGasPres = astr1BottomBoard.transform.Find("CoolGasPress").gameObject;

//         //Astr 2
//         astr2Board = transform.Find("SuitsControlScreen2").gameObject;
//         astr2CritBoard = astr2Board.transform.Find("ScreenCrit").gameObject;
//         astr2SuitBoard = astr2Board.transform.Find("Screen1").gameObject;
//         astr2BottomBoard = astr2Board.transform.Find("Screen2").gameObject;
//         astr2DCUBoard = astr2Board.transform.Find("DCU").gameObject;

//         oxyTime2 = astr2Board.transform.Find("O2priOxygen").gameObject;
//         powerTime2 = astr2Board.transform.Find("PriBattery").gameObject;
//         heartRate2 = astr2CritBoard.transform.Find("HeartRate").gameObject;
//         oxyCons2 = astr2CritBoard.transform.Find("O2con").gameObject;
//         co2Prod2 = astr2CritBoard.transform.Find("CO2prod").gameObject;
//         temp2 = astr2CritBoard.transform.Find("Temp").gameObject;
//         priOxyStor2 = astr2SuitBoard.transform.Find("O2priStor").gameObject;
//         secOxyStor2 = astr2SuitBoard.transform.Find("O2secStor").gameObject;
//         batt2 = astr2SuitBoard.transform.Find("Battery").gameObject;
//         coolant2 = astr2SuitBoard.transform.Find("Coolant").gameObject;
//         scrubberA2 = astr2SuitBoard.transform.Find("ScrubberA").gameObject;
//         scrubberB2 = astr2SuitBoard.transform.Find("ScrubberB").gameObject;
//         priOxyPres2 = astr2BottomBoard.transform.Find("O2priPress").gameObject;
//         secOxyPres2 = astr2BottomBoard.transform.Find("O2secPress").gameObject;
//         suitPresOxy2 = astr2BottomBoard.transform.Find("O2suitPress").gameObject;
//         suitTotPres2 = astr2BottomBoard.transform.Find("SuitTotPress").gameObject;
//         suitPresCO22 = astr2BottomBoard.transform.Find("CO2suitPress").gameObject;
//         otherSuitPres2 = astr2BottomBoard.transform.Find("OtherSuitPress").gameObject;
//         priFan2 = astr2SuitBoard.transform.Find("FanPri").gameObject;
//         secFan2 = astr2SuitBoard.transform.Find("FanSec").gameObject;
//         helmetCO2Pres2 = astr2BottomBoard.transform.Find("HelCO2press").gameObject;
//         coolLiquidPres2 = astr2BottomBoard.transform.Find("CoolLiqPress").gameObject;
//         coolGasPres2 = astr2BottomBoard.transform.Find("CoolGasPress").gameObject;

//     }


//     private void onVitalsUpdate(VitalsUpdatedEvent e)
//     {
//         //astr 1
//         oxyTime.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_time_left.ToString("F0");
//         powerTime.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.batt_time_left.ToString("F0");

//         heartRate.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.heart_rate / 160) * 302));
//         heartRate.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.heart_rate.ToString("F0");
//         //Debug.Log(heartRate.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.GetFloat("_Arc1"));

//         oxyCons.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.oxy_consumption / 0.15) * 302));
//         oxyCons.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_consumption.ToString("F1");

//         co2Prod.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.co2_production / 0.15) * 302));
//         co2Prod.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.co2_production.ToString("F1");

//         temp.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.temperature / 90) * 302));
//         temp.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.temperature.ToString("F0");

//         priOxyStor.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.oxy_pri_storage / 100) * 302));
//         priOxyStor.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_pri_storage.ToString("F0");

//         secOxyStor.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.oxy_sec_storage / 100) * 302));
//         secOxyStor.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_sec_storage.ToString("F0");

//         batt.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.batt_percentage / 100) * 302));
//         batt.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.batt_percentage.ToString("F0");

//         coolant.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.coolant_m / 100) * 302));
//         coolant.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.coolant_m.ToString("F0");

//         scrubberA.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.scrubber_a_co2_storage / 100) * 302));
//         scrubberA.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.scrubber_a_co2_storage.ToString("F0");

//         scrubberB.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.scrubber_b_co2_storage / 100) * 302));
//         scrubberB.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.scrubber_b_co2_storage.ToString("F0");

//         priFan.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.fan_pri_rpm / 30000) * 302));
//         priFan.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.fan_pri_rpm.ToString().Substring(0, Mathf.Min(2, e.vitals.fan_pri_rpm.ToString().Length)) + "k";

//         secFan.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.fan_sec_rpm / 30000) * 302));
//         secFan.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.fan_sec_rpm.ToString().Substring(0, Mathf.Min(2, e.vitals.fan_sec_rpm.ToString().Length)) + "k";

//         priOxyPres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.oxy_pri_pressure / 3000) * 302));
//         priOxyPres.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_pri_pressure.ToString("F0");

//         secOxyPres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.oxy_sec_pressure / 3000) * 302));
//         secOxyPres.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_sec_pressure.ToString("F0");

//         suitTotPres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_total / 4.5) * 302));
//         suitTotPres.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_total.ToString("F1");

//         suitPresOxy.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_oxy / 4.1) * 302));
//         suitPresOxy.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_oxy.ToString("F1");

//         suitPresCO2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_co2 / 0.1) * 302));
//         suitPresCO2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_co2.ToString("F1");

//         otherSuitPres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_other / 0.5) * 302));
//         otherSuitPres.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_other.ToString("F1");

//         helmetCO2Pres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.helmet_pressure_co2 / 0.15) * 302));
//         helmetCO2Pres.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.helmet_pressure_co2.ToString("F1");

//         coolLiquidPres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.coolant_liquid_pressure / 700) * 302));
//         coolLiquidPres.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.coolant_liquid_pressure.ToString("F0");

//         coolGasPres.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.coolant_gas_pressure / 700) * 302));
//         coolGasPres.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.coolant_gas_pressure.ToString("F0");

//         //checkVitals(e);
//     }



//     private void onFellowVitalsUpdate(FellowAstronautVitalsDataChangeEvent e)
//     {
//         //astr 2
//         oxyTime2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_time_left.ToString("F0");
//         powerTime2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.batt_time_left.ToString("F0");

//         heartRate2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.heart_rate / 160) * 302));
//         heartRate2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.heart_rate.ToString("F0");

//         oxyCons2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.oxy_consumption / 0.15) * 302));
//         oxyCons2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_consumption.ToString("F1");

//         co2Prod2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.co2_production / 0.15) * 302));
//         co2Prod2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.co2_production.ToString("F1");

//         temp2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.temperature / 90) * 302));
//         temp2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.temperature.ToString("F0");

//         priOxyStor2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.oxy_pri_storage / 100) * 302));
//         priOxyStor2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_pri_storage.ToString("F0");

//         secOxyStor2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.oxy_sec_storage / 100) * 302));
//         secOxyStor2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_sec_storage.ToString("F0");

//         batt2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.batt_percentage / 100) * 302));
//         batt2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.batt_percentage.ToString("F0");

//         coolant2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.coolant_m / 100) * 302));
//         coolant2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.coolant_m.ToString("F0");

//         scrubberA2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.scrubber_a_co2_storage / 100) * 302));
//         scrubberA2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.scrubber_a_co2_storage.ToString("F0");

//         scrubberB2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.scrubber_b_co2_storage / 100) * 302));
//         scrubberB2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.scrubber_b_co2_storage.ToString("F0");

//         priFan2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.fan_pri_rpm / 30000) * 302));
//         priFan2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.fan_pri_rpm.ToString().Substring(0, Mathf.Min(2, e.vitals.fan_pri_rpm.ToString().Length)) + "k";

//         secFan2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.fan_sec_rpm / 30000) * 302));
//         secFan2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.fan_sec_rpm.ToString().Substring(0, Mathf.Min(2, e.vitals.fan_sec_rpm.ToString().Length)) + "k";

//         priOxyPres2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.oxy_pri_pressure / 3000) * 302));
//         priOxyPres2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_pri_pressure.ToString("F0");

//         secOxyPres2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.oxy_sec_pressure / 3000) * 302));
//         secOxyPres2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_sec_pressure.ToString("F0");

//         suitTotPres2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_total / 4.5) * 302));
//         suitTotPres2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_total.ToString("F1");

//         suitPresOxy2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_oxy / 4.1) * 302));
//         suitPresOxy2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_oxy.ToString("F1");

//         suitPresCO22.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_co2 / 0.1) * 302));
//         suitPresCO22.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_co2.ToString("F1");

//         otherSuitPres2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.suit_pressure_other / 0.5) * 302));
//         otherSuitPres2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.suit_pressure_other.ToString("F1");

//         helmetCO2Pres2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.helmet_pressure_co2 / 0.15) * 302));
//         helmetCO2Pres2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.helmet_pressure_co2.ToString("F1");

//         coolLiquidPres2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.coolant_liquid_pressure / 700) * 302));
//         coolLiquidPres2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.coolant_liquid_pressure.ToString("F0");

//         coolGasPres2.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (float)((1 - e.vitals.coolant_gas_pressure / 700) * 302));
//         coolGasPres2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.coolant_gas_pressure.ToString("F0");

//     }



//     private void onDCUChanged(DCUChanged e)
//     {

//         GameObject oxyBox = astrDCUBoard.transform.Find("OxyBox").gameObject;
//         GameObject co2Box = astrDCUBoard.transform.Find("CO2Box").gameObject;
//         GameObject PumpBox = astrDCUBoard.transform.Find("PumpBox").gameObject;
//         GameObject FanBox = astrDCUBoard.transform.Find("FanBox").gameObject;
//         GameObject BattBox = astrDCUBoard.transform.Find("BattBox").gameObject;
//         GameObject CommsBox = astrDCUBoard.transform.Find("CommsBox").gameObject;

//         GameObject oxyBox2 = astr2DCUBoard.transform.Find("OxyBox").gameObject;
//         GameObject co2Box2 = astr2DCUBoard.transform.Find("CO2Box").gameObject;
//         GameObject PumpBox2 = astr2DCUBoard.transform.Find("PumpBox").gameObject;
//         GameObject FanBox2 = astr2DCUBoard.transform.Find("FanBox").gameObject;
//         GameObject BattBox2 = astr2DCUBoard.transform.Find("BattBox").gameObject;
//         GameObject CommsBox2 = astr2DCUBoard.transform.Find("CommsBox").gameObject;

//         if (e.data.oxy)
//         {
//             oxyBox.transform.Find("OxyStats").GetComponent<TextMeshPro>().text = "PRI";
//             oxyBox2.transform.Find("OxyStats").GetComponent<TextMeshPro>().text = "PRI";
//         }
//         else
//         {
//             oxyBox.transform.Find("OxyStats").GetComponent<TextMeshPro>().text = "SEC";
//             oxyBox2.transform.Find("OxyStats").GetComponent<TextMeshPro>().text = "SEC";
//         }

//         if (e.data.batt)
//         {
//             BattBox.transform.Find("BattStats").GetComponent<TextMeshPro>().text = "LOCAL";
//             BattBox2.transform.Find("BattStats").GetComponent<TextMeshPro>().text = "LOCAL";
//         }
//         else
//         {
//             BattBox.transform.Find("BattStats").GetComponent<TextMeshPro>().text = "UIA";
//             BattBox2.transform.Find("BattStats").GetComponent<TextMeshPro>().text = "UIA";
//         }

//         if (e.data.comm)
//         {
//             CommsBox.transform.Find("CommsStats").GetComponent<TextMeshPro>().text = "A";
//             CommsBox2.transform.Find("CommsStats").GetComponent<TextMeshPro>().text = "A";
//         }
//         else
//         {
//             CommsBox.transform.Find("CommsStats").GetComponent<TextMeshPro>().text = "B";
//             CommsBox2.transform.Find("CommsStats").GetComponent<TextMeshPro>().text = "B";
//         }

//         if (e.data.fan)
//         {
//             FanBox.transform.Find("FanStats").GetComponent<TextMeshPro>().text = "PRI";
//             FanBox2.transform.Find("FanStats").GetComponent<TextMeshPro>().text = "PRI";
//         }
//         else
//         {
//             FanBox.transform.Find("FanStats").GetComponent<TextMeshPro>().text = "SEC";
//             FanBox2.transform.Find("FanStats").GetComponent<TextMeshPro>().text = "SEC";
//         }

//         if (e.data.pump)
//         {
//             PumpBox.transform.Find("PumpStats").GetComponent<TextMeshPro>().text = "OPEN";
//             PumpBox2.transform.Find("PumpStats").GetComponent<TextMeshPro>().text = "OPEN";
//         }
//         else
//         {
//             PumpBox.transform.Find("PumpStats").GetComponent<TextMeshPro>().text = "CLOSED";
//             PumpBox2.transform.Find("PumpStats").GetComponent<TextMeshPro>().text = "CLOSED";
//         }

//         if (e.data.co2)
//         {
//             co2Box.transform.Find("CO2Stats").GetComponent<TextMeshPro>().text = "A";
//             co2Box2.transform.Find("CO2Stats").GetComponent<TextMeshPro>().text = "A";
//         }
//         else
//         {
//             co2Box.transform.Find("CO2Stats").GetComponent<TextMeshPro>().text = "B";
//             co2Box2.transform.Find("CO2Stats").GetComponent<TextMeshPro>().text = "B";
//         }
//     }


//     private void checkVitals(VitalsUpdatedEvent e)
//     {
//         // Error Scenarios DCU

//         if (e.vitals.heart_rate > HEART_RATE_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Heart, "Heart Rate High", $"{e.vitals.heart_rate} BPM, slow down"));
//         }

//         // suit pressure oxygen
//         if (e.vitals.suit_pressure_oxy > SUIT_PRES_OXY_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "Switch OXY to SEC", $"O2 Suit Pressure High: {e.vitals.suit_pressure_oxy} PSI"));
//         }
//         if (e.vitals.suit_pressure_oxy < SUIT_PRES_OXY_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "Switch OXY to SEC", $"O2 Suit Pressure Low: {e.vitals.suit_pressure_oxy} PSI"));
//         }
//         if (e.vitals.oxy_pri_storage < OXY_STOR_MIN)
//         {
//             string desc = $"Primary Oxygen Low: {e.vitals.oxy_percentage.ToString()}%";
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "Switch OXY to SEC", desc));
//         }

//         // scrubbers
//         if (e.vitals.scrubber_a_co2_storage > SCRUBBER_CO2_STOR_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Scrubber, "Switch CO2 to B", $"Scrubber A High: {e.vitals.scrubber_a_co2_storage.ToString()}%"));
//         }
//         if (e.vitals.scrubber_b_co2_storage > SCRUBBER_CO2_STOR_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Scrubber, "Switch CO2 to A", $"Scrubber B High: {e.vitals.scrubber_b_co2_storage.ToString()}%"));
//         }
//         if (e.vitals.suit_pressure_co2 > SUIT_PRES_CO2_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_CO2, "Switch CO2 to B", $"CO2 Suit Pressure High: {e.vitals.suit_pressure_co2.ToString()} PSI"));
//         }


//         // Fans
//         if (e.vitals.fan_pri_rpm < FAN_SPEED_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Switch FAN to SEC", $"Fan Speed Low: {e.vitals.fan_pri_rpm.ToString()} RPM"));
//         }
//         if (e.vitals.fan_pri_rpm > FAN_SPEED_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Switch FAN to SEC", $"Fan Speed High: {e.vitals.fan_pri_rpm.ToString()} RPM"));
//         }
//         if (e.vitals.fan_sec_rpm < FAN_SPEED_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Switch FAN to PRI", $"Fan Speed Low: {e.vitals.fan_sec_rpm.ToString()} RPM"));
//         }
//         if (e.vitals.fan_sec_rpm > FAN_SPEED_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Switch FAN to PRI", $"Fan Speed High: {e.vitals.fan_sec_rpm.ToString()} RPM"));
//         }
//         if (e.vitals.helmet_pressure_co2 > HELMET_PRES_CO2_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Switch FAN to SEC", $"Helmet CO2 Pressure High: {e.vitals.helmet_pressure_co2.ToString()} PSI"));
//         }

//         // Temperature
//         if (e.vitals.temperature > TEMP_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Temp, "Temperature High", $"{e.vitals.helmet_pressure_co2.ToString()} ?F. Slow Down"));
//         }

//         // Battery
//         if (e.vitals.batt_time_left < BATT_TIME_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Battery, "Switch BATT to LOCAL", $"Battery Low: {e.vitals.batt_percentage.ToString()}%"));
//         }

//         ///////////////

//         // Regular Notifs

//         if (e.vitals.oxy_consumption > OXY_CONSUM_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "O2 Consumption High", $"{e.vitals.oxy_consumption.ToString()} PSI/m"));
//         }
//         if (e.vitals.oxy_pri_storage < OXY_STOR_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "O2 Pri Storage Low", $"{e.vitals.oxy_pri_storage.ToString()}%"));
//         }
//         if (e.vitals.oxy_sec_storage < OXY_STOR_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_O2, "O2 Sec Storage Low", $"{e.vitals.oxy_sec_storage.ToString()}%"));
//         }
//         if (e.vitals.coolant_m < COOL_STOR_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Coolant, "Coolant Low", $"{e.vitals.coolant_m.ToString()}%"));
//         }
//         if (e.vitals.scrubber_a_co2_storage > SCRUBBER_CO2_STOR_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Scrubber, "Scrubber A High", $"{e.vitals.scrubber_a_co2_storage.ToString()}%"));
//         }
//         if (e.vitals.scrubber_b_co2_storage > SCRUBBER_CO2_STOR_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Scrubber, "Scrubber B High", $"{e.vitals.scrubber_b_co2_storage.ToString()}%"));
//         }
//         if (e.vitals.co2_production > CO2_PROD_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_CO2, "CO2 Production High", $"{e.vitals.co2_production.ToString()} PSI/m"));
//         }
//         if (e.vitals.suit_pressure_other > SUIT_PRES_OTHER_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Other Suit Pressure High", $"{e.vitals.suit_pressure_other.ToString()} PSI"));
//         }
//         if (e.vitals.suit_pressure_total > SUIT_PRES_TOTAL_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Total Suit Pressure High", $"{e.vitals.suit_pressure_total.ToString()} PSI"));
//         }
//         if (e.vitals.suit_pressure_total < SUIT_PRES_TOTAL_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Total Suit Pressure Low", $"{e.vitals.suit_pressure_total.ToString()} PSI"));
//         }
//         if (e.vitals.oxy_pri_pressure > OXY_PRES_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Pri Pressure High", $"{e.vitals.oxy_pri_pressure.ToString()} PSI"));
//         }
//         if (e.vitals.oxy_pri_pressure < OXY_PRES_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Pri Pressure Low", $"{e.vitals.oxy_pri_pressure.ToString()} PSI"));
//         }
//         if (e.vitals.oxy_sec_pressure > OXY_PRES_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Sec Pressure High", $"{e.vitals.oxy_sec_pressure.ToString()} PSI"));
//         }
//         if (e.vitals.oxy_pri_pressure < OXY_PRES_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Sec Pressure Low", $"{e.vitals.oxy_sec_pressure.ToString()} PSI"));
//         }
//         if (e.vitals.suit_pressure_oxy > SUIT_PRES_OXY_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Suit Pressure High", $"{e.vitals.suit_pressure_oxy.ToString()} PSI"));
//         }
//         if (e.vitals.suit_pressure_oxy < SUIT_PRES_OXY_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "O2 Suit Pressure Low", $"{e.vitals.suit_pressure_oxy.ToString()} PSI"));
//         }
//         if (e.vitals.suit_pressure_co2 > SUIT_PRES_CO2_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "CO2 Suit Pressure High", $"{e.vitals.suit_pressure_co2.ToString()} PSI"));
//         }
//         if (e.vitals.helmet_pressure_co2 > HELMET_PRES_CO2_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Helmet CO2 Pressure High", $"{e.vitals.helmet_pressure_co2.ToString()} PSI"));
//         }
//         if (e.vitals.coolant_liquid_pressure > COOL_LIQ_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Coolant Liquid Pressure High", $"{e.vitals.coolant_liquid_pressure.ToString()} PSI"));
//         }
//         if (e.vitals.coolant_liquid_pressure < COOL_LIQ_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Coolant Liquid Pressure Low", $"{e.vitals.coolant_liquid_pressure.ToString()} PSI"));
//         }
//         if (e.vitals.coolant_gas_pressure > COOL_GAS_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Pressure, "Coolant Gas Pressure High", $"{e.vitals.coolant_gas_pressure.ToString()} PSI"));
//         }

//         if (e.vitals.fan_pri_rpm > FAN_SPEED_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Fan Pri Speed High", $"{e.vitals.fan_pri_rpm.ToString()} PSI"));
//         }
//         if (e.vitals.fan_pri_rpm < FAN_SPEED_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Fan Pri Speed Low", $"{e.vitals.fan_pri_rpm.ToString()} PSI"));
//         }
//         if (e.vitals.fan_sec_rpm > FAN_SPEED_MAX)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Fan Sec Speed High", $"{e.vitals.fan_sec_rpm.ToString()} PSI"));
//         }
//         if (e.vitals.fan_sec_rpm < FAN_SPEED_MIN)
//         {
//             EventBus.Publish<CreateAlert>(new CreateAlert(AlertEnum.Vital_Fan, "Fan Sec Speed Low", $"{e.vitals.fan_sec_rpm.ToString()} PSI"));
//         }
//     }

// };
