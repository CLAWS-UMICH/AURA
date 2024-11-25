/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[Serializable]
public class SuitVariable
{
    public GameObject displayObject;
    public float max;
    public float min;
}

public class SuitsControlController : MonoBehaviour
{
    private Subscription<VitalsUpdatedEvent> vitalsUpdateEvent;

    public SuitVariable var_1;
    public SuitVariable var_2;
    public SuitVariable var_3;
    public SuitVariable var_4;
    public SuitVariable var_5;

    private void Start()
    {
        vitalsUpdateEvent = EventBus.Subscribe<VitalsUpdatedEvent>(onVitalsUpdate);
    }

    private void onVitalsUpdate(VitalsUpdatedEvent e)
    {
        UpdateVariable(var_1, e.vitals.value1);
        UpdateVariable(var_2, e.vitals.value2);
        UpdateVariable(var_3, e.vitals.value3);
        UpdateVariable(var_4, e.vitals.value4);
        UpdateVariable(var_5, e.vitals.value5);
    }

    private void UpdateVariable(SuitVariable variable, float currentValue)
    {
        float normalizedValue = Mathf.Clamp01((currentValue - variable.min) / (variable.max - variable.min));
        float arcValue = (1 - normalizedValue) * 302;

        variable.displayObject.transform.Find("RingFull").GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", arcValue);
        variable.displayObject.transform.Find("BodyText").GetComponent<TextMeshPro>().text = currentValue.ToString("F1");
    }
}*/
