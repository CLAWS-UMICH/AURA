using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.UX;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameObject vitals;
    public GameObject navigation;
    public GameObject PR;
    public GameObject UIA;
    public GameObject messaging;
    public GameObject geosample;
    public ToggleCollection menuToggleCollection;


    void Start()
    {
        menuToggleCollection.OnToggleSelected.AddListener(OnToggleChanged);
        transform.Find("Screens").gameObject.SetActive(true);
        DeactivateAllScreens();
    }


    private void OnToggleChanged(int index)
    {
        Debug.Log($"Toggle changed to index: {index}");
        openScreen(index);
    }


    public void openScreen(int index)
    {
        DeactivateAllScreens();
        switch (index)
        {
            case 0:
                Debug.Log("Opening UIA screen");
                UIA.SetActive(true);
                // TODO: activate the proper child of UIA
                break;
            case 1:
                Debug.Log("Opening Navigation screen");
                navigation.SetActive(true);
                navigation.GetComponent<NavigationFrontend>().openFeatureScreen();
                break;
            case 2:
                Debug.Log("Opening Messaging screen");
                messaging.SetActive(true);
                messaging.GetComponent<FrontEndButton>().openFeatureScreen();
                break;
            case 3:
                Debug.Log("Opening Geosample screen");
                geosample.SetActive(true);
                geosample.GetComponent<GeoSampleFrontend>().openFeatureScreen();
                break;
            case 4:
                Debug.Log("Opening Vitals screen");
                vitals.SetActive(true);
                vitals.GetComponent<VitalsController>().openFeatureScreen();
                break;
             case 5:
                Debug.Log("Opening PR screen");
                PR.SetActive(true);
                // TODO: activate the proper child of PR
                break;
        }
    }


    private void DeactivateAllScreens()
    {
        Debug.Log("Deactivating all screens");
        UIA.SetActive(true);
        foreach (Transform child in UIA.transform)
        {
            child.gameObject.SetActive(false);
        }
        navigation.SetActive(true);
        foreach (Transform child in navigation.transform)
        {
            child.gameObject.SetActive(false);
        }
        PR.SetActive(true);
        foreach (Transform child in PR.transform)
        {
            child.gameObject.SetActive(false);
        }
        messaging.SetActive(true);
        foreach (Transform child in messaging.transform)
        {
            child.gameObject.SetActive(false);
        }
        geosample.SetActive(true);
        foreach (Transform child in geosample.transform)
        {
            child.gameObject.SetActive(false);
        }
        vitals.SetActive(true);
        foreach (Transform child in vitals.transform)
        {
            child.gameObject.SetActive(false);
        }
    }


    private void OnDestroy()
    {
        if (menuToggleCollection != null)
        {
            menuToggleCollection.OnToggleSelected.RemoveListener(OnToggleChanged);
        }
    }
}
