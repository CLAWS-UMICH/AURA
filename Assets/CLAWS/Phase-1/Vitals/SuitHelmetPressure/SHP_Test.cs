using System.Collections;
using UnityEngine;
using TMPro;
using System;

[Serializable]
public class SuitVariable
{
    public GameObject displayObject; // Parent GameObject containing material and text
}

public class SHP_Test : MonoBehaviour
{
    public SuitVariable var_1;
    public SuitVariable var_2;
    public SuitVariable var_3;
    public SuitVariable var_4;
    public SuitVariable var_5;

    private void Start()
    {
        // Initialize all variables to a default state
        SetValues(var_1, 200f, 5.0f);
        SetValues(var_2, 200f, 5.0f);
        SetValues(var_3, 200f, 5.0f);
        SetValues(var_4, 200f, 5.0f);
        SetValues(var_5, 200f, 5.0f);
    }

    private void Update()
    {
        // Generate random values for demonstration purposes
        float randomArcValue = UnityEngine.Random.Range(0f, 300f);
        float randomBodyTextValue = UnityEngine.Random.Range(0f, 10f);

        // Update the values dynamically
        UpdateValues(var_1, randomArcValue, randomBodyTextValue);
        UpdateValues(var_2, randomArcValue, randomBodyTextValue);
        UpdateValues(var_3, randomArcValue, randomBodyTextValue);
        UpdateValues(var_4, randomArcValue, randomBodyTextValue);
        UpdateValues(var_5, randomArcValue, randomBodyTextValue);
    }

    private void SetValues(SuitVariable variable, float arcValue, float bodyTextValue)
    {
        if (variable == null || variable.displayObject == null)
        {
            Debug.LogError("SuitVariable or displayObject is null!");
            return;
        }

        // Set Arc Point 1 value
        var ringFull = variable.displayObject.transform.Find("RingFull");
        if (ringFull != null)
        {
            var materialHandler = ringFull.GetComponent<materialHandler>();
            if (materialHandler != null)
            {
                Material material = materialHandler.GetMaterial();
                if (material != null)
                {
                    material.SetFloat("_Arc1", arcValue);
                    Debug.Log($"Set _Arc1 for {variable.displayObject.name} to {arcValue}");
                }
                else
                {
                    Debug.LogError($"Material is missing or not assigned on {ringFull.name}");
                }
            }
            else
            {
                Debug.LogError($"MaterialHandler script is missing on {ringFull.name}");
            }
        }
        else
        {
            Debug.LogError($"RingFull child is missing in {variable.displayObject.name}");
        }

        // Set BodyText value
        var bodyText = variable.displayObject.transform.Find("BodyText");
        if (bodyText != null && bodyText.GetComponent<TextMeshPro>() != null)
        {
            bodyText.GetComponent<TextMeshPro>().text = bodyTextValue.ToString("F1");
            Debug.Log($"Set BodyText for {variable.displayObject.name} to {bodyTextValue}");
        }
        else
        {
            Debug.LogError($"BodyText is missing or not set up correctly on {variable.displayObject.name}");
        }
    }

    private void UpdateValues(SuitVariable variable, float arcValue, float bodyTextValue)
    {
        // Reuse SetValues for the update logic
        SetValues(variable, arcValue, bodyTextValue);
    }
}




