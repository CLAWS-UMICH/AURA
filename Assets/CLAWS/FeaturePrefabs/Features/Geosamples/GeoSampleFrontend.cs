using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoSampleFrontend : MonoBehaviour
{
    public GeoSampleController geoSampleController;
    public GeoSample sample;
    // Start is called before the first frame update
    void Start()
    {
        geoSampleController.sideBarToggleCollection.OnToggleSelected.AddListener(OnToggleChanged);
    }


    private void OnToggleChanged(int index)
    {
        if (index == 0)
        {
            openStart_DB();
        }
        else if (index == 1)
        {
            openStart_ZONES();
        }
    }

    public void openStart_DB()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.startMenu_databaseScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.126000002f, -0.0250000004f, 0.00314010005f);
        // Check if the toggle is already selected
        if (geoSampleController.sideBarToggleCollection.CurrentIndex != 0)
        {
            geoSampleController.sideBarToggleCollection.SetSelection(0, force: true);
        }
    }


    public void openStart_ZONES()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.startMenu_zoneScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.151099995f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
        if (geoSampleController.sideBarToggleCollection.CurrentIndex != 0)
        {
            geoSampleController.sideBarToggleCollection.SetSelection(0, force: true);
        }
    }


    public void openFeatureScreen()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.startMenu_databaseScreen.SetActive(true);
         geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.126000002f, -0.0250000004f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
        if (geoSampleController.sideBarToggleCollection.CurrentIndex != 0)
        {
            geoSampleController.sideBarToggleCollection.SetSelection(0, force: true);
        }
    }


    public void closeGeoSampleFeature()
    {
       foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.geoSampleControllerScreen.SetActive(true);
    }


    public void open_ZONEA_screen()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.ZoneA_databaseSamplesScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.231099993f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
        foreach(Transform child in geoSampleController.ZoneA_databaseSamplesScreen.transform)
        {
            child.gameObject.SetActive(true);
        }
        geoSampleController.ZoneA_databaseSamplesScreen.transform.GetChild(3).Find("UpdatedState").gameObject.SetActive(false);
        geoSampleController.ZoneA_databaseSamplesScreen.transform.GetChild(3).Find("InitialState").gameObject.SetActive(true);
    }


    public void open_ZONEB_screen()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.ZoneB_databaseSamplesScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.231099993f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
    }


    public void open_ZONEC_screen()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.ZoneC_databaseSamplesScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.231099993f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
    }


    public void enterGeoSamplingMode()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.geoSamplingModeSelectionScreen.SetActive(true);
        // geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.231099993f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
    }


    

    // button fuunctions for adding geosamples

}
