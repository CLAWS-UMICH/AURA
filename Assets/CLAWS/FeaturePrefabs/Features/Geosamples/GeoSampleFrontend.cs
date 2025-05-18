using System;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit;
using MixedReality.Toolkit.UX;
using UnityEngine;
using TMPro;
using System.Linq;

public class GeoSampleFrontend : MonoBehaviour
{
    public GeoSampleController geoSampleController;
    public GeoSample sample;

    private List<GameObject> lastActiveScreens = new List<GameObject>();

    // flags for layers
    public bool cameraDone = false;
    public bool xrfScanDone = false;
    public bool colorSelected = false;
    public bool shapeSelected = false;
    public bool textureSelected = false;
    public bool voiceNotesDone = false;

    private Subscription<XRFScanEvent> scanEvent;

    // Start is called before the first frame update
    void Start()
    {
        geoSampleController.sideBarToggleCollection.OnToggleSelected.AddListener(OnToggleChanged);
        geoSampleController.transform.Find("ButtonSideBar").GetChild(0).GetComponent<PressableButton>().ForceSetToggled(true);
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
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.133499995f, 0.0759999976f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
        // Check if the toggle is already selected
    }


    public void openStart_ZONES()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.startMenu_zoneScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.252799988f, 0.0706999972f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);
    }


    public void openFeatureScreen()
    {
        openStart_DB();
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
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.279f, 0.076f, 0.014f);
        geoSampleController.buttonSideBar.SetActive(true);
        foreach (Transform child in geoSampleController.ZoneA_databaseSamplesScreen.transform)
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
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.279f, 0.076f, 0.014f);
        geoSampleController.buttonSideBar.SetActive(true);
    }


    public void open_ZONEC_screen()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.ZoneC_databaseSamplesScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.279f, 0.076f, 0.014f);
        geoSampleController.buttonSideBar.SetActive(true);
    }



    public void getLastScreenBeforeGeoSampling()
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }

        foreach (GameObject screen in lastActiveScreens)
        {
            if (screen != null)
            {
                screen.SetActive(true);
            }
        }
        Debug.Log("Reactivated all last active screens before geo-sampling.");
    }

    public void enterGeoSamplingMode()
    {
        lastActiveScreens.Clear();
        foreach (Transform screen in geoSampleController.transform)
        {
            if (screen.gameObject.activeSelf)
            {
                lastActiveScreens.Add(screen.gameObject);
            }
        }

        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.geoSamplingModeSelectionScreen.SetActive(true);
        currentZoneCheck();
        int count = 0;
        char zone = ' ';
        if (sample.zone != null)
        {
            if (sample.zone == "ZONE_A")
            {
                zone = 'A';
                count = AstronautInstance.User.geosampleZones[0].TotalGeoSamples.samples.Count;
            }
            else if (sample.zone == "ZONE_B")
            {
                zone = 'B';
                count = AstronautInstance.User.geosampleZones[1].TotalGeoSamples.samples.Count;
            }
            else if (sample.zone == "ZONE_C")
            {
                zone = 'C';
                count = AstronautInstance.User.geosampleZones[2].TotalGeoSamples.samples.Count;
            }

            geoSampleController.geoSamplingModeSelectionScreen.transform.Find("SampleName").Find("GeoNumText").GetComponent<TextMeshPro>().text = zone + count.ToString();
        }
        else
        {
            Debug.LogError("Sample zone is null. Cannot set sample name.");
            geoSampleController.geoSamplingModeSelectionScreen.transform.Find("SampleName").Find("GeoNumText").GetComponent<TextMeshPro>().text = "ERR";
        }

        // Reset states when entering the geosampling mode
        sample = new GeoSample();
        cameraDone = false;
        xrfScanDone = false;
        colorSelected = false;
        shapeSelected = false;
        textureSelected = false;
        voiceNotesDone = false;

        // TODO: Reset UI (to be created) when entering the geosampling mode
        geoSampleController.XRFInitUI.SetActive(true);
        geoSampleController.XRFCompleteUI.SetActive(false);
        geoSampleController.colorInitUI.SetActive(true);
        geoSampleController.colorCompleteUI.SetActive(false);
        geoSampleController.shapeInitUI.SetActive(true);
        geoSampleController.shapeCompleteUI.SetActive(false);
        geoSampleController.textureInitUI.SetActive(true);
        geoSampleController.textureCompleteUI.SetActive(false);
        geoSampleController.voiceNotesInitUI.SetActive(true);
        geoSampleController.voiceNotesCompleteUI.SetActive(false);
        Debug.Log("set XRFInitUI to true: " + geoSampleController.XRFInitUI.activeSelf);
    }

    public void currentZoneCheck()
    {
        Location currentLocation = AstronautInstance.User.current;
        if (currentLocation.posX < AstronautInstance.User.geosampleZones[0].origin.posX + 5 ||
            currentLocation.posX > AstronautInstance.User.geosampleZones[0].origin.posX - 5 ||
            currentLocation.posY < AstronautInstance.User.geosampleZones[0].origin.posY + 5 ||
            currentLocation.posY > AstronautInstance.User.geosampleZones[0].origin.posY - 5)
        {
            sample.zone = "ZONE_A";
        }
        else if (currentLocation.posX < AstronautInstance.User.geosampleZones[1].origin.posX + 5 ||
                 currentLocation.posX > AstronautInstance.User.geosampleZones[1].origin.posX - 5 ||
                 currentLocation.posY < AstronautInstance.User.geosampleZones[1].origin.posY + 5 ||
                 currentLocation.posY > AstronautInstance.User.geosampleZones[1].origin.posY - 5)
        {
            sample.zone = "ZONE_B";
        }
        else if (currentLocation.posX < AstronautInstance.User.geosampleZones[2].origin.posX + 5 ||
                 currentLocation.posX > AstronautInstance.User.geosampleZones[2].origin.posX - 5 ||
                 currentLocation.posY < AstronautInstance.User.geosampleZones[2].origin.posY + 5 ||
                 currentLocation.posY > AstronautInstance.User.geosampleZones[2].origin.posY - 5)
        {
            sample.zone = "ZONE_C";
        }
        else
        {
            // add logic to handle a new zone if we have time/want to
        }
    }

    public void openNavigationToZoneA(int index)
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.ZoneANavigationScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.231099993f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);

        // navigation to zome A logic
    }


    public void openNavigationToZoneB(int index)
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.ZoneBNavigationScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.231099993f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);

        // navigation to zome B logic
    }


    public void openNavigationToZoneC(int index)
    {
        foreach (Transform screen in geoSampleController.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.ZoneCNavigationScreen.SetActive(true);
        geoSampleController.buttonSideBar.transform.localPosition = new Vector3(-0.231099993f, -0.00680000009f, 0.00314010005f);
        geoSampleController.buttonSideBar.SetActive(true);

        // navigation to zome C logic
    }

    // called in each layer buttons to then trigger when result achieved (after camera or xrf)
    public bool firstLayerDoneCheck()
    {
        if (cameraDone && xrfScanDone)
        {
            geoSampleController.geoSamplingModeSelectionScreen.transform.Find("1st_Layer").GetChild(0).GetComponent<PressableButton>().ForceSetToggled(true);
            return true;
        }
        return false;
    }


    public bool secondLayerDoneCheck()
    {
        if (shapeSelected && colorSelected && textureSelected)
        {
            geoSampleController.geoSamplingModeSelectionScreen.transform.Find("2nd_Layer").GetChild(0).GetComponent<PressableButton>().ForceSetToggled(true);
            return true;
        }
        return false;
    }


    public bool thirdLayerDoneCheck()
    {
        if (voiceNotesDone)
        {
            geoSampleController.geoSamplingModeSelectionScreen.transform.Find("3rd_Layer").GetChild(0).GetComponent<PressableButton>().ForceSetToggled(true);
            return true;
        }
        return false;
    }


    public void expandBackplateForShape()
    {
        geoSampleController.geoSamplingModeSelectionScreen.transform.localPosition = new Vector3(0.0285940003f, 0.136000007f, -0.00317399995f);
        geoSampleController.ShapeSelectScreen.transform.localPosition = new Vector3(-0.2869f, -0.056f, 0);
        geoSampleController.TextureSelectScreen.SetActive(false);
        geoSampleController.ShapeSelectScreen.SetActive(true);

        GameObject backplate = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("UIBackplate").GetChild(0).gameObject;
        backplate.transform.localPosition = new Vector3(-0.02037f, -0.079f, 0);
        backplate.transform.localScale = new Vector3(0.3678901f, 0.4016f, 0.02f);

        GameObject sideLine = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("MovingLine").gameObject;
        sideLine.transform.localPosition = new Vector3(-0.2834f, -0.3287f, 0.0182f);

        GameObject dividerLine = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("Divider").gameObject;
        dividerLine.transform.localPosition = new Vector3(-0.10228f, -0.3794f, 0.0163f);

        GameObject thirdLayer = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("3rd_Layer").gameObject;
        thirdLayer.transform.localPosition = new Vector3(0, -0.2335f, 0);

        GameObject bottomBar = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("BottomBar").gameObject;
        bottomBar.transform.localPosition = new Vector3(-0.0413f, -0.1343f, 0.083f);
    }

    public void restoreBackplate()
    {
        geoSampleController.ShapeSelectScreen.SetActive(false);
        geoSampleController.TextureSelectScreen.SetActive(false);
        geoSampleController.geoSamplingModeSelectionScreen.transform.localPosition = new Vector3(0.028594f, 0.136f, -0.003174f);
        GameObject backplate = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("UIBackplate").GetChild(0).gameObject;
        backplate.transform.localPosition = new Vector3(-0.02037f, -0.03013f, 0);
        backplate.transform.localScale = new Vector3(0.3678f, 0.303689f, 0.02f);
   
        GameObject sideLine = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("MovingLine").gameObject;
        sideLine.transform.localPosition = new Vector3(-0.2834f, -0.2536f, 0.0182f);

        GameObject dividerLine = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("Divider").gameObject;
        dividerLine.transform.localPosition = new Vector3(-0.10228f, -0.2818f, 0.0163f);

        GameObject thirdLayer = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("3rd_Layer").gameObject;
        thirdLayer.transform.localPosition = new Vector3(0, -0.1365f, 0);

        GameObject bottomBar = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("BottomBar").gameObject;
        bottomBar.transform.localPosition = new Vector3(-0.0453f, -0.0358f, 0.0841f);
    }

    public void expandBackplateForTexture()
    {
        geoSampleController.TextureSelectScreen.transform.localPosition = new Vector3(-0.285f, -0.0598f, 0);
        geoSampleController.geoSamplingModeSelectionScreen.transform.localPosition = new Vector3(0.0285940003f, 0.136000007f, -0.00317399995f);
        geoSampleController.ShapeSelectScreen.SetActive(false);
        geoSampleController.TextureSelectScreen.SetActive(true);

        GameObject backplate = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("UIBackplate").GetChild(0).gameObject;
        backplate.transform.localPosition = new Vector3(-0.02037f, -0.0524f, 0);
        backplate.transform.localScale = new Vector3(0.3678901f, 0.3484932f, 0.02f);

        GameObject sideLine = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("MovingLine").gameObject;
        sideLine.transform.localPosition = new Vector3(-0.2834f, -0.308f, 0.0182f);

        GameObject dividerLine = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("Divider").gameObject;
        dividerLine.transform.localPosition = new Vector3(-0.10228f, -0.3237f, 0.0163f);

        GameObject thirdLayer = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("3rd_Layer").gameObject;
        thirdLayer.transform.localPosition = new Vector3(0, -0.1802f, 0);

        GameObject bottomBar = geoSampleController.geoSamplingModeSelectionScreen.transform.Find("BottomBar").gameObject;
        bottomBar.transform.localPosition = new Vector3(-0.0422f, -0.0807f, 0.0805f);
    }


    public void addGeoSampleToDB()
    {
        // If save is complete, then add geosample into the DB and go back to the zone screen
        // Prevents needing to check whether geosample already exists
        if (firstLayerDoneCheck() && secondLayerDoneCheck() && thirdLayerDoneCheck())
        {
            // Retrieve eva data from the astronaut
            string type;
            int id;
            //List<Composition> compositions = new List<Composition>();
            if (AstronautInstance.User.id == 1)
            {
                type = AstronautInstance.User.spec.spec.eva1.name;
                id = AstronautInstance.User.spec.spec.eva1.id;
                //compositions.Add(new Composition("SiO2", (float)AstronautInstance.User.spec.spec.eva1.data.SiO2));
                //compositions.Add(new Composition("TiO2", (float)AstronautInstance.User.spec.spec.eva1.data.TiO2));
                //compositions.Add(new Composition("Al2O3", (float)AstronautInstance.User.spec.spec.eva1.data.Al2O3));
                //compositions.Add(new Composition("FeO", (float)AstronautInstance.User.spec.spec.eva1.data.FeO));
                //compositions.Add(new Composition("MnO", (float)AstronautInstance.User.spec.spec.eva1.data.MnO));
                //compositions.Add(new Composition("MgO", (float)AstronautInstance.User.spec.spec.eva1.data.MgO));
                //compositions.Add(new Composition("CaO", (float)AstronautInstance.User.spec.spec.eva1.data.CaO));
                //compositions.Add(new Composition("K2O", (float)AstronautInstance.User.spec.spec.eva1.data.K2O));
                //compositions.Add(new Composition("P2O3", (float)AstronautInstance.User.spec.spec.eva1.data.P2O3));
                //compositions.Add(new Composition("other", (float)AstronautInstance.User.spec.spec.eva1.data.other));
            }
            else
            {
                type = AstronautInstance.User.spec.spec.eva2.name;
                id = AstronautInstance.User.spec.spec.eva2.id;
                //compositions.Add(new Composition("SiO2", (float)AstronautInstance.User.spec.spec.eva2.data.SiO2));
                //compositions.Add(new Composition("TiO2", (float)AstronautInstance.User.spec.spec.eva2.data.TiO2));
                //compositions.Add(new Composition("Al2O3", (float)AstronautInstance.User.spec.spec.eva2.data.Al2O3));
                //compositions.Add(new Composition("FeO", (float)AstronautInstance.User.spec.spec.eva2.data.FeO));
                //compositions.Add(new Composition("MnO", (float)AstronautInstance.User.spec.spec.eva2.data.MnO));
                //compositions.Add(new Composition("MgO", (float)AstronautInstance.User.spec.spec.eva2.data.MgO));
                //compositions.Add(new Composition("CaO", (float)AstronautInstance.User.spec.spec.eva2.data.CaO));
                //compositions.Add(new Composition("K2O", (float)AstronautInstance.User.spec.spec.eva2.data.K2O));
                //compositions.Add(new Composition("P2O3", (float)AstronautInstance.User.spec.spec.eva2.data.P2O3));
                //compositions.Add(new Composition("other", (float)AstronautInstance.User.spec.spec.eva2.data.other));
            }

            // Update sample data
            sample.name = geoSampleController.GeosampleSelectionScreen.transform.Find("SampleName").Find("GeoNameText").gameObject.GetComponent<TextMeshPro>().text;
            sample.type = type;
            sample.id = id;
            //sample.comp = new List<Composition>(compositions);

            // Check if the geosample is significant
            sample.isSignificant = false;
            foreach (Composition comp in sample.comp)
            {
                switch (comp.element)
                {
                    case "SiO2":
                        if (comp.amount < 30f)
                        {
                            sample.isSignificant = true;
                        }
                        break;
                    case "TiO2":
                        if (comp.amount > 10f)
                        {
                            sample.isSignificant = true;
                        }
                        break;
                    case "Al2O3":
                        if (comp.amount > 25f)
                        {
                            sample.isSignificant = true;
                        }
                        break;
                    case "FeO":
                        if (comp.amount > 20f)
                        {
                            sample.isSignificant = true;
                        }
                        break;
                    case "MnO":
                        if (comp.amount > 0.5f)
                        {
                            sample.isSignificant = true;
                        }
                        break;
                    case "MgO":
                        if (comp.amount > 10f)
                        {
                            sample.isSignificant = true;
                        }
                        break;
                    case "CaO":
                        if (comp.amount < 5f)
                        {
                            sample.isSignificant = true;
                        }
                        break;
                    case "K2O":
                        if (comp.amount > 1f)
                        {
                            sample.isSignificant = true;
                        }
                        break;
                    case "P2O3":
                        if (comp.amount > 1f)
                        {
                            sample.isSignificant = true;
                        }
                        break;
                    case "other":
                        if (comp.amount > 50f)
                        {
                            sample.isSignificant = true;
                        }
                        break;
                }

                if (sample.isSignificant)
                {
                    break;
                }
            }

            // Add geosample to the corresponding zone's list
            // TODO: Add geosample to the scrolling list
            switch (sample.zone)
            {
                case "ZONE_A":
                    AstronautInstance.User.geosampleZones[0].TotalGeoSamples.samples.Add(sample);
                    break;
                case "ZONE_B":
                    AstronautInstance.User.geosampleZones[1].TotalGeoSamples.samples.Add(sample);
                    break;
                case "ZONE_C":
                    AstronautInstance.User.geosampleZones[2].TotalGeoSamples.samples.Add(sample);
                    break;
            }

            // Return to zone screen
            getLastScreenBeforeGeoSampling();
        }
    }

    public void selectGeoSample(int index)
    {
        // Just works for zone A right now
        GeoSample geosample = AstronautInstance.User.geosampleZones[0].TotalGeoSamples.samples[index];
        GameObject updatedState = geoSampleController.ZoneA_databaseSamplesScreen.transform.GetChild(3).Find("UpdatedState").gameObject;

        // Set screens to show detailed information of geosample
        geoSampleController.ZoneA_databaseSamplesScreen.transform.GetChild(3).Find("InitialState").gameObject.SetActive(false);
        updatedState.SetActive(true);

        // Update composition data
        int i = 0;
        foreach (Transform cell in updatedState.transform.Find("MineralCells").transform)
        {
            cell.Find("Type").GetComponent<TextMeshPro>().text = geosample.comp[i].element;
            cell.Find("Value").GetComponent<TextMeshPro>().text = geosample.comp[i].amount.ToString();
            i++;
        }

        // Update type, shape, and color data
        GameObject typingCells = updatedState.transform.Find("TypingCells").gameObject;
        typingCells.transform.Find("Type").Find("Value").GetComponent<TextMeshPro>().text = geosample.type;
        typingCells.transform.Find("Shape").Find("Value").GetComponent<TextMeshPro>().text = geosample.shape;
        typingCells.transform.Find("Color").Find("Value").GetComponent<TextMeshPro>().text = geosample.color;

        // Update voice notes data
        updatedState.transform.Find("VoiceNotes").Find("Title").GetComponent<TextMeshPro>().text = geosample.note;
    }

    public void takeXRFScan()
    {
        closeGeoSampleFeature();
        geoSampleController.xrfScreen.SetActive(true);

        // TODO: Use endpoint to get XRF scan data and update text
        geoSampleController.xrfScreen.transform.Find("Waiting").gameObject.SetActive(true);
        geoSampleController.xrfScreen.transform.Find("Compositions").gameObject.SetActive(false);
        scanEvent = EventBus.Subscribe<XRFScanEvent>(updateXRFScan);
    }

    public void updateXRFScan(XRFScanEvent e)
    {
        if (geoSampleController.xrfScreen.activeSelf)
        {
            geoSampleController.xrfScreen.transform.Find("Waiting").gameObject.SetActive(false);
            geoSampleController.xrfScreen.transform.Find("Compositions").gameObject.SetActive(true);
            int i = 0;
            List<Composition> compositions = new List<Composition>();
            foreach (Transform composition in geoSampleController.xrfScreen.transform.Find("Compositions").transform)
            {
                switch (i)
                {
                    case 0:
                        composition.Find("Type").GetComponent<TextMeshPro>().text = "SiO2";
                        composition.Find("Value").GetComponent<TextMeshPro>().text = e.compositions.SiO2.ToString();
                        compositions.Add(new Composition("SiO2", (float)e.compositions.SiO2));
                        break;
                    case 1:
                        composition.Find("Type").GetComponent<TextMeshPro>().text = "TiO2";
                        composition.Find("Value").GetComponent<TextMeshPro>().text = e.compositions.TiO2.ToString();
                        compositions.Add(new Composition("TiO2", (float)e.compositions.TiO2));
                        break;
                    case 2:
                        composition.Find("Type").GetComponent<TextMeshPro>().text = "Al2O3";
                        composition.Find("Value").GetComponent<TextMeshPro>().text = e.compositions.Al2O3.ToString();
                        compositions.Add(new Composition("Al2O3", (float)e.compositions.Al2O3));
                        break;
                    case 3:
                        composition.Find("Type").GetComponent<TextMeshPro>().text = "FeO";
                        composition.Find("Value").GetComponent<TextMeshPro>().text = e.compositions.FeO.ToString();
                        compositions.Add(new Composition("FeO", (float)e.compositions.FeO));
                        break;
                    case 4:
                        composition.Find("Type").GetComponent<TextMeshPro>().text = "MnO";
                        composition.Find("Value").GetComponent<TextMeshPro>().text = e.compositions.MnO.ToString();
                        compositions.Add(new Composition("MnO", (float)e.compositions.MnO));
                        break;
                    case 5:
                        composition.Find("Type").GetComponent<TextMeshPro>().text = "MgO";
                        composition.Find("Value").GetComponent<TextMeshPro>().text = e.compositions.MgO.ToString();
                        compositions.Add(new Composition("MgO", (float)e.compositions.MgO));
                        break;
                    case 6:
                        composition.Find("Type").GetComponent<TextMeshPro>().text = "CaO";
                        composition.Find("Value").GetComponent<TextMeshPro>().text = e.compositions.CaO.ToString();
                        compositions.Add(new Composition("CaO", (float)e.compositions.CaO));
                        break;
                    case 7:
                        composition.Find("Type").GetComponent<TextMeshPro>().text = "K2O";
                        composition.Find("Value").GetComponent<TextMeshPro>().text = e.compositions.K2O.ToString();
                        compositions.Add(new Composition("K2O", (float)e.compositions.K2O));
                        break;
                    case 8:
                        composition.Find("Type").GetComponent<TextMeshPro>().text = "P2O3";
                        composition.Find("Value").GetComponent<TextMeshPro>().text = e.compositions.P2O3.ToString();
                        compositions.Add(new Composition("P2O3", (float)e.compositions.P2O3));
                        break;
                    case 9:
                        composition.Find("Type").GetComponent<TextMeshPro>().text = "other";
                        composition.Find("Value").GetComponent<TextMeshPro>().text = e.compositions.other.ToString();
                        compositions.Add(new Composition("other", (float)e.compositions.other));
                        break;
                }
                i++;
            }
            sample.comp = new List<Composition>(compositions);
            xrfScanDone = true;
        }

        EventBus.Unsubscribe(scanEvent);
    }

    public void confirmXRFScan()
    {
        geoSampleController.xrfScreen.SetActive(false);
        geoSampleController.geoSamplingModeSelectionScreen.SetActive(true);
        EventBus.Unsubscribe(scanEvent);

        if (xrfScanDone)
        {
            geoSampleController.XRFInitUI.SetActive(false);
            geoSampleController.XRFCompleteUI.SetActive(true);
            List<Composition> top4 = sample.comp.OrderByDescending(comp => comp.amount).Take(4).ToList();
            int i = 0;
            foreach (Transform comp in geoSampleController.XRFCompleteUI.transform)
            {
                comp.Find("Type").GetComponent<TextMeshPro>().text = top4[i].element;
                comp.Find("Value").GetComponent<TextMeshPro>().text = top4[i].amount.ToString();
                i++;
            }
        }
    }

    public void takePhoto()
    {
        // TODO: Implement taking a photo
        cameraDone = true;
    }

    public void openHueSelectionMenu()
    {
        closeGeoSampleFeature();
        geoSampleController.ColorHueSelectionScreen.SetActive(true);
    }

    public void openVoiceNotesDictation()
    {
        closeGeoSampleFeature();
        geoSampleController.voiceNotesDictation.SetActive(true);
    }

    public void startRecording()
    {
        geoSampleController.voiceNotesDictation.transform.Find("StartRecordingButton").gameObject.SetActive(false);
        geoSampleController.voiceNotesDictation.transform.Find("StopRecordingButton").gameObject.SetActive(true);
    }

    public void stopRecording()
    {
        voiceNotesDone = true;
        geoSampleController.voiceNotesDictation.transform.Find("StartRecordingButton").gameObject.SetActive(true);
        geoSampleController.voiceNotesDictation.transform.Find("StopRecordingButton").gameObject.SetActive(false);
        sample.note = geoSampleController.voiceNotesDictation.transform.Find("VoiceNotes").GetComponent<TextMeshPro>().text;
    }

    public void closeVoiceNotesDictation()
    {
        geoSampleController.voiceNotesDictation.SetActive(false);
        geoSampleController.geoSamplingModeSelectionScreen.SetActive(true);
        if (voiceNotesDone)
        {
            geoSampleController.voiceNotesInitUI.SetActive(false);
            geoSampleController.voiceNotesCompleteUI.SetActive(true);
            geoSampleController.voiceNotesCompleteUI.GetComponent<TextMeshPro>().text = sample.note;
        }
    }

    // COLORS //
    public void selectHue()
    {
        geoSampleController.ColorHueSelectionScreen.SetActive(false);
        geoSampleController.HueParentScreen.SetActive(true);
        foreach (Transform screen in geoSampleController.HueParentScreen.transform)
        {
            screen.gameObject.SetActive(false);
        }
    }
    public void closeColors()
    {
        foreach (Transform screen in geoSampleController.HueParentScreen.transform)
        {
            screen.gameObject.SetActive(false);
        }
        geoSampleController.geoSamplingModeSelectionScreen.SetActive(true);

        // TODO: Make and update UI for geosample selection button to display 
    }
    public void yellow()
    {
        selectHue();
        geoSampleController.yellowScreen.SetActive(true);
    }
    public void yellowGreen()
    {
        selectHue();
        geoSampleController.yellowGreenScreen.SetActive(true);
    }
    public void green()
    {
        selectHue();
        geoSampleController.greenScreen.SetActive(true);
    }
    public void blueGreen()
    {
        selectHue();
        geoSampleController.blueGreenScreen.SetActive(true);
    }
    public void blue()
    {
        selectHue();
        geoSampleController.blueScreen.SetActive(true);
    }
    public void bluePurple()
    {
        selectHue();
        geoSampleController.bluePurpleScreen.SetActive(true);
    }
    public void purple()
    {
        selectHue();
        geoSampleController.purpleScreen.SetActive(true);
    }
    public void pink()
    {
        selectHue();
        geoSampleController.pinkScreen.SetActive(true);
    }
    public void red()
    {
        selectHue();
        geoSampleController.redScreen.SetActive(true);
    }
    public void orange()
    {
        selectHue();
        geoSampleController.orangeScreen.SetActive(true);
    }

    // TEXTURES //
    public void fineTexture()
    {
        sample.texture = "FINE";
        textureSelected = true;
        secondLayerDoneCheck();
        geoSampleController.TextureSelectScreen.SetActive(false);
        restoreBackplate();

        // Update texture UI
        geoSampleController.textureInitUI.SetActive(false);
        geoSampleController.textureCompleteUI.SetActive(true);
        foreach (Transform texture in geoSampleController.textureCompleteUI.transform)
        {
            texture.gameObject.SetActive(false);
        }
        geoSampleController.textureCompleteUI.transform.Find("Fine").gameObject.SetActive(true);
        geoSampleController.textureCompleteUI.transform.Find("TextureText").gameObject.SetActive(true);
        geoSampleController.textureCompleteUI.transform.Find("TextureText").GetComponent<TextMeshPro>().text = "Fine";
    }
    public void mediumTexture()
    {
        sample.texture = "MEDIUM";
        textureSelected = true;
        secondLayerDoneCheck();
        geoSampleController.TextureSelectScreen.SetActive(false);
        restoreBackplate();

        // Update texture UI
        geoSampleController.textureInitUI.SetActive(false);
        geoSampleController.textureCompleteUI.SetActive(true);
        foreach (Transform texture in geoSampleController.textureCompleteUI.transform)
        {
            texture.gameObject.SetActive(false);
        }
        geoSampleController.textureCompleteUI.transform.Find("Medium").gameObject.SetActive(true);
        geoSampleController.textureCompleteUI.transform.Find("TextureText").gameObject.SetActive(true);
        geoSampleController.textureCompleteUI.transform.Find("TextureText").GetComponent<TextMeshPro>().text = "Medium";
    }
    public void coarseTexture()
    {
        sample.texture = "COARSE";
        textureSelected = true;
        secondLayerDoneCheck();
        geoSampleController.TextureSelectScreen.SetActive(false);
        restoreBackplate();

        // Update texture UI
        geoSampleController.textureInitUI.SetActive(false);
        geoSampleController.textureCompleteUI.SetActive(true);
        foreach (Transform texture in geoSampleController.textureCompleteUI.transform)
        {
            texture.gameObject.SetActive(false);
        }
        geoSampleController.textureCompleteUI.transform.Find("Coarse").gameObject.SetActive(true);
        geoSampleController.textureCompleteUI.transform.Find("TextureText").gameObject.SetActive(true);
        geoSampleController.textureCompleteUI.transform.Find("TextureText").GetComponent<TextMeshPro>().text = "Coarse";
    }

    // SHAPES //
    public void polygon()
    {
        sample.shape = "POLYGON";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();

        // Update shape UI
        geoSampleController.shapeInitUI.SetActive(false);
        geoSampleController.shapeCompleteUI.SetActive(true);
        foreach (Transform shape in geoSampleController.shapeCompleteUI.transform)
        {
            shape.gameObject.SetActive(false);
        }
        geoSampleController.shapeCompleteUI.transform.Find("Polygon").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").GetComponent<TextMeshPro>().text = "Polygon";
    }
    public void sphere()
    {
        sample.shape = "SPHERE";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();

        // Update shape UI
        geoSampleController.shapeInitUI.SetActive(false);
        geoSampleController.shapeCompleteUI.SetActive(true);
        foreach (Transform shape in geoSampleController.shapeCompleteUI.transform)
        {
            shape.gameObject.SetActive(false);
        }
        geoSampleController.shapeCompleteUI.transform.Find("Sphere").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").GetComponent<TextMeshPro>().text = "Sphere";
    }
    public void cube()
    {
        sample.shape = "CUBE";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();

        // Update shape UI
        geoSampleController.shapeInitUI.SetActive(false);
        geoSampleController.shapeCompleteUI.SetActive(true);
        foreach (Transform shape in geoSampleController.shapeCompleteUI.transform)
        {
            shape.gameObject.SetActive(false);
        }
        geoSampleController.shapeCompleteUI.transform.Find("Cube").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").GetComponent<TextMeshPro>().text = "Cube";
    }
    public void cylinder()
    {
        sample.shape = "CYLINDER";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();

        // Update shape UI
        geoSampleController.shapeInitUI.SetActive(false);
        geoSampleController.shapeCompleteUI.SetActive(true);
        foreach (Transform shape in geoSampleController.shapeCompleteUI.transform)
        {
            shape.gameObject.SetActive(false);
        }
        geoSampleController.shapeCompleteUI.transform.Find("Cylinder").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").GetComponent<TextMeshPro>().text = "Cylinder";
    }
    public void irregular()
    {
        sample.shape = "IRREGULAR";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();

        // Update shape UI
        geoSampleController.shapeInitUI.SetActive(false);
        geoSampleController.shapeCompleteUI.SetActive(true);
        foreach (Transform shape in geoSampleController.shapeCompleteUI.transform)
        {
            shape.gameObject.SetActive(false);
        }
        geoSampleController.shapeCompleteUI.transform.Find("Irregular").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").GetComponent<TextMeshPro>().text = "Irregular";
    }
    public void cone()
    {
        sample.shape = "CONE";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();

        // Update shape UI
        geoSampleController.shapeInitUI.SetActive(false);
        geoSampleController.shapeCompleteUI.SetActive(true);
        foreach (Transform shape in geoSampleController.shapeCompleteUI.transform)
        {
            shape.gameObject.SetActive(false);
        }
        geoSampleController.shapeCompleteUI.transform.Find("Cone").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").GetComponent<TextMeshPro>().text = "Cone";
    }
    public void elipsisoid()
    {
        sample.shape = "ELIPSOID";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();

        // Update shape UI
        geoSampleController.shapeInitUI.SetActive(false);
        geoSampleController.shapeCompleteUI.SetActive(true);
        foreach (Transform shape in geoSampleController.shapeCompleteUI.transform)
        {
            shape.gameObject.SetActive(false);
        }
        geoSampleController.shapeCompleteUI.transform.Find("Ellipsoid").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").GetComponent<TextMeshPro>().text = "Ellipsoid";
    }
    public void other()
    {
        sample.shape = "OTHER";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();

        // Update shape UI
        geoSampleController.shapeInitUI.SetActive(false);
        geoSampleController.shapeCompleteUI.SetActive(true);
        foreach (Transform shape in geoSampleController.shapeCompleteUI.transform)
        {
            shape.gameObject.SetActive(false);
        }
        geoSampleController.shapeCompleteUI.transform.Find("Other1").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("Other2").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").gameObject.SetActive(true);
        geoSampleController.shapeCompleteUI.transform.Find("ShapeText").GetComponent<TextMeshPro>().text = "Other";
    }
}
