/* 
 * 
 * starting backend connection 
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[Serializable]
public class SuitVariable
{
    public GameObject displayObject; // UI Object to update
    public float max; // Maximum value of the variable
    public float min; // Minimum value of the variable
}

public class SuitsControlController : MonoBehaviour
{
    private Subscription<VitalsUpdatedEvent> vitalsUpdateEvent;

    // Define the five SuitVariables
    public SuitVariable var_1;
    public SuitVariable var_2;
    public SuitVariable var_3;
    public SuitVariable var_4;
    public SuitVariable var_5;

    private void Start()
    {
        // Subscribe to the vitals update event
        vitalsUpdateEvent = EventBus.Subscribe<VitalsUpdatedEvent>(onVitalsUpdate);
    }

    private void onVitalsUpdate(VitalsUpdatedEvent e)
    {
        // Update each variable with the current value from the event
        UpdateVariable(var_1, e.vitals.value1);
        UpdateVariable(var_2, e.vitals.value2);
        UpdateVariable(var_3, e.vitals.value3);
        UpdateVariable(var_4, e.vitals.value4);
        UpdateVariable(var_5, e.vitals.value5);
    }

    private void UpdateVariable(SuitVariable variable, float currentValue)
    {
        // Normalize the value to a 0-1 range based on min and max
        float normalizedValue = Mathf.Clamp01((currentValue - variable.min) / (variable.max - variable.min));

        // Calculate the arc value for the ring's display
        float arcValue = (1 - normalizedValue) * 302;

        // Update the visual ring arc to match the current value
        variable.displayObject.transform.Find("RingFull")
            .GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", arcValue);

        // Update the text display with the current value formatted to one decimal place
        variable.displayObject.transform.Find("BodyText")
            .GetComponent<TextMeshPro>().text = currentValue.ToString("F1");
    }
} */
