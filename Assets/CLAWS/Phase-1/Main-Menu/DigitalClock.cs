using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DigitalClock : MonoBehaviour
{
    private TextMeshProUGUI textDisplay;
    string hour, minute, second;
    DateTime timeNow;
    
    // Start is called before the first frame update
    void Start()
    {
        textDisplay = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        timeNow = DateTime.Now;
        hour = timeNow.Hour.ToString().PadLeft(2, '0');
        minute = timeNow.Minute.ToString().PadLeft(2, '0');
        second = timeNow.Second.ToString().PadLeft(2, '0');

        textDisplay.text = hour + ":" + minute + ":" + second;
    }
}
