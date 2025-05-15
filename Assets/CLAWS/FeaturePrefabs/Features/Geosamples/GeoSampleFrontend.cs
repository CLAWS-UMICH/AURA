using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoSampleFrontend : MonoBehaviour
{
    public GeoSampleController geoSampleController;
    // Start is called before the first frame update
    void Start()
    {
        geoSampleController.menuToggleCollection.OnToggleSelected.AddListener(OnToggleChanged);
    }


    private void OnToggleChanged(int index)
    {
        if (index == 0)
        {
            openSample();
        }
        else if (index == 1)
        {
            open_ZONEs();
        }
    }

    public void openSample()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            if (screen.name != "GeoSampleControllerScreen")
            {
                screen.gameObject.SetActive(false);
            }
        }
        geoSampleController.GeosampleSelectionScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.151099995f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
    }


    public void open_ZONEs()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            if (screen.name != "GeoSampleControllerScreen")
            {
                screen.gameObject.SetActive(false);
            }
        }
        geoSampleController.ZoneMappingScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.151099995f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
    }


    public void openFeatureScreen()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            if (screen.name != "GeoSampleControllerScreen")
            {
                screen.gameObject.SetActive(false);
            }
        }
        geoSampleController.startScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.151099995f,-0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
    }


    public void closeGeoSampleFeature()
    {
       foreach (Transform screen in geoSampleController.transform)
        {
            if (screen.name != "GeoSampleControllerScreen")
            {
                screen.gameObject.SetActive(false);
            }
        }
        geoSampleController.geoSampleControllerScreen.SetActive(false);
    }


    public void open_ZONEA_screen()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.ZoneA_databaseSamplesScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.151099995f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
    }


    public void open_ZONEB_screen()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.ZoneB_databaseSamplesScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.151099995f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
    }


    public void open_ZONEC_screen()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.ZoneC_databaseSamplesScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.151099995f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
    }



}
