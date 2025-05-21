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
        int seconds = AstronautInstance.User.vitals.eva_time;
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        textDisplay.text = time.ToString(@"hh\:mm\:ss");
    }
}
