using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GeosampleClickHandler : MonoBehaviour
{
    public void onClick()
    {
        Debug.Log("Geosample clicked");
        GeoSampleFrontend geoSampleFrontend = FindObjectOfType<GeoSampleFrontend>();
        int index = 0;
        if (Int32.TryParse(transform.GetChild(2).GetChild(0).GetChild(0).GetChild(8).gameObject.GetComponent<TextMeshPro>().text.Substring(1), out index))
        {
            Debug.Log($"Geosample Index = {index}");
        }

        geoSampleFrontend.selectGeoSample(index);
    }
}
