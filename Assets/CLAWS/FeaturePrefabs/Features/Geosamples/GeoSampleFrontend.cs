using System;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit;
using MixedReality.Toolkit.UX;
using UnityEngine;
using TMPro;
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
            string name = "";
            int id = 0;
            List<Composition> compositions = new List<Composition>();
            if (AstronautInstance.User.id == 1)
            {
                name = AstronautInstance.User.spec.spec.eva1.name;
                id = AstronautInstance.User.spec.spec.eva1.id;
                compositions.Add(new Composition("SiO2", (float)AstronautInstance.User.spec.spec.eva1.data.SiO2));
                compositions.Add(new Composition("TiO2", (float)AstronautInstance.User.spec.spec.eva1.data.TiO2));
                compositions.Add(new Composition("Al2O3", (float)AstronautInstance.User.spec.spec.eva1.data.Al2O3));
                compositions.Add(new Composition("FeO", (float)AstronautInstance.User.spec.spec.eva1.data.FeO));
                compositions.Add(new Composition("MnO", (float)AstronautInstance.User.spec.spec.eva1.data.MnO));
                compositions.Add(new Composition("MgO", (float)AstronautInstance.User.spec.spec.eva1.data.MgO));
                compositions.Add(new Composition("CaO", (float)AstronautInstance.User.spec.spec.eva1.data.CaO));
                compositions.Add(new Composition("K2O", (float)AstronautInstance.User.spec.spec.eva1.data.K2O));
                compositions.Add(new Composition("P2O3", (float)AstronautInstance.User.spec.spec.eva1.data.P2O3));
            }
            else
            {
                name = AstronautInstance.User.spec.spec.eva2.name;
                id = AstronautInstance.User.spec.spec.eva2.id;
                compositions.Add(new Composition("SiO2", (float)AstronautInstance.User.spec.spec.eva2.data.SiO2));
                compositions.Add(new Composition("TiO2", (float)AstronautInstance.User.spec.spec.eva2.data.TiO2));
                compositions.Add(new Composition("Al2O3", (float)AstronautInstance.User.spec.spec.eva2.data.Al2O3));
                compositions.Add(new Composition("FeO", (float)AstronautInstance.User.spec.spec.eva2.data.FeO));
                compositions.Add(new Composition("MnO", (float)AstronautInstance.User.spec.spec.eva2.data.MnO));
                compositions.Add(new Composition("MgO", (float)AstronautInstance.User.spec.spec.eva2.data.MgO));
                compositions.Add(new Composition("CaO", (float)AstronautInstance.User.spec.spec.eva2.data.CaO));
                compositions.Add(new Composition("K2O", (float)AstronautInstance.User.spec.spec.eva2.data.K2O));
                compositions.Add(new Composition("P2O3", (float)AstronautInstance.User.spec.spec.eva2.data.P2O3));
            }
            //public string name;
            //public string zone;
            //public string shape;
            //public string color;
            //public string texture;
            //public string note;
            //public int id;  // NASA GIVEN ID
            //public bool isSignificant;
            //public List<Composition> comp;
            sample.name = geoSampleController.GeosampleSelectionScreen.transform.Find("SampleName").Find("GeoNameText").gameObject.GetComponent<TextMeshPro>().text;
            sample.id = id;
            sample.comp = new List<Composition>(compositions);
            EventBus.Publish(new GeoSampleAddedToZoneEvent(sample));
        }
    }


    public void openHueSelectionMenu()
    {

    }

    public void openVoiceNotesDictation()
    {
        closeGeoSampleFeature();
        geoSampleController.voiceNotesDictation.SetActive(true);
    }

    // TEXTURES //
    public void fineTexture()
    {
        sample.texture = "FINE";
        textureSelected = true;
        secondLayerDoneCheck();
        geoSampleController.TextureSelectScreen.SetActive(false);
        restoreBackplate();
    }
    public void mediumTexture()
    {
        sample.texture = "MEDIUMM";
        textureSelected = true;
        secondLayerDoneCheck();
        geoSampleController.TextureSelectScreen.SetActive(false);
        restoreBackplate();
    }
    public void coarseTexture()
    {
        sample.texture = "COARSE";
        textureSelected = true;
        secondLayerDoneCheck();
        geoSampleController.TextureSelectScreen.SetActive(false);
        restoreBackplate();
    }


    // SHAPES //
    public void polygon()
    {
        sample.shape = "POLYGON";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();
    }
    public void sphere()
    {
        sample.shape = "SPHERE";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();
    }
    public void cube()
    {
        sample.shape = "CUBE";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();
    }
    public void cylinder()
    {
        sample.shape = "CYLINDER";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();
    }
    public void irregular()
    {
        sample.shape = "IRREGULAR";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();
    }
    public void cone()
    {
        sample.shape = "CONE";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();
    }
    public void elipsisoid()
    {
        sample.shape = "ELIPSOID";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();
    }
    public void other()
    {
        sample.shape = "OTHER";
        shapeSelected = true;
        secondLayerDoneCheck();
        geoSampleController.ShapeSelectScreen.SetActive(false);
        restoreBackplate();
    }
}
