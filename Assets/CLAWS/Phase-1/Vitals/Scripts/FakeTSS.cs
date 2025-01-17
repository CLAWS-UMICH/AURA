using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class FakeTSS : MonoBehaviour
{
    [SerializeField] float secondsToUpdate = 3f;
    private float timer;

    private void Start()
    {
        Fake_Vitals();
    }


    public void Fake_Vitals()
    {
         Debug.Log("Starting vitals update coroutine...");
        StartCoroutine(UpdateVitals());
    }

    IEnumerator UpdateVitals()
    {
        while (true)
        {
            Debug.Log("Test"); 
            AstronautInstance.User.vitals.eva_time = 69;
            yield return new WaitForSeconds(secondsToUpdate);
            
            // Update vitals with random values
            AstronautInstance.User.vitals.co2_production = UnityEngine.Random.Range(0f, 0.2f);
            AstronautInstance.User.vitals.oxy_consumption = UnityEngine.Random.Range(0f, 0.2f);
            AstronautInstance.User.vitals.heart_rate = UnityEngine.Random.Range(20f, 200f);
            AstronautInstance.User.vitals.temperature = UnityEngine.Random.Range(30f, 100f);

            AstronautInstance.User.vitals.helmet_pressure_co2 = UnityEngine.Random.Range(0f, 0.2f);
            AstronautInstance.User.vitals.suit_pressure_co2 = UnityEngine.Random.Range(0f, 0.1f);
            AstronautInstance.User.vitals.suit_pressure_oxy = UnityEngine.Random.Range(3.5f, 4.7f);
            AstronautInstance.User.vitals.suit_pressure_total = UnityEngine.Random.Range(3.5f, 4.7f);
            AstronautInstance.User.vitals.suit_pressure_other = UnityEngine.Random.Range(0f, 0.7f);

            AstronautInstance.User.vitals.oxy_pri_storage = UnityEngine.Random.Range(0f, 100f);            
            AstronautInstance.User.vitals.oxy_sec_storage = UnityEngine.Random.Range(0f, 100f);
            AstronautInstance.User.vitals.oxy_pri_pressure = UnityEngine.Random.Range(400f, 3200f);
            AstronautInstance.User.vitals.oxy_sec_pressure = UnityEngine.Random.Range(400f, 3200f);
            AstronautInstance.User.vitals.coolant_m = UnityEngine.Random.Range(70f, 100f);
        
            AstronautInstance.User.vitals.fan_pri_rpm = UnityEngine.Random.Range(19000f, 32000f);
            AstronautInstance.User.vitals.fan_sec_rpm = UnityEngine.Random.Range(19000f, 32000f);
            AstronautInstance.User.vitals.scrubber_a_co2_storage = UnityEngine.Random.Range(0f, 70f);
            AstronautInstance.User.vitals.scrubber_b_co2_storage = UnityEngine.Random.Range(0f, 70f);
            AstronautInstance.User.vitals.coolant_liquid_pressure = UnityEngine.Random.Range(80f, 750f);
            AstronautInstance.User.vitals.coolant_gas_pressure = UnityEngine.Random.Range(80f, 750f);

             timer += secondsToUpdate;

            // Calculate time left for battery and oxygen
            TimeSpan batteryTime = TimeSpan.FromMinutes(5) - TimeSpan.FromSeconds(timer);
            TimeSpan o2Time = TimeSpan.FromMinutes(5) - TimeSpan.FromSeconds(timer);

            AstronautInstance.User.vitals.batt_time_left = 3000;//(int)batteryTime.TotalSeconds;
            AstronautInstance.User.vitals.oxy_time_left = 3000;//(int)o2Time.TotalSeconds;

            // Publish VitalsUpdatedEvent
            EventBus.Publish<UpdatedVitalsEvent>(new UpdatedVitalsEvent(AstronautInstance.User.vitals));

        }
    }

}
