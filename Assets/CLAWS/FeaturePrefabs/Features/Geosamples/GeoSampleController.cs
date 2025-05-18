using MixedReality.Toolkit.UX;
using UnityEngine;

public class GeoSampleController : MonoBehaviour
{
    public NavigationController navigationController;
    [Header("Screens")]
    public GameObject geoSampleControllerScreen;
    // 1st layer start menu toggles 
    public GameObject startMenu_databaseScreen;
    public GameObject startMenu_zoneScreen;
    // 2nd layer -- database samplees per zone
    public GameObject ZoneA_databaseSamplesScreen;
    public GameObject ZoneB_databaseSamplesScreen;
    public GameObject ZoneC_databaseSamplesScreen;
    // 2nd layer -- zone navigation
    public GameObject zoneNavigationConfirmationScreen;
    // 2nd layer -- geosampling mode
    public GameObject geoSamplingModeSelectionScreen;

    // shape and texture selection screens
    public GameObject ShapeSelectScreen;
    public GameObject TextureSelectScreen;
    // color hue selection screens
    public GameObject colorHueSelectionScreen;
    public GameObject redHueSelectionScreen;
    

    public GameObject voiceNotesDictation;

    [Header("Description Panels for Each Zone")]
    public GameObject ZoneADescriptionScreen;
    public GameObject ZoneBDescriptionScreen;
    public GameObject ZoneCDescriptionScreen;
    
    [Header("Navigation screens")]
    public GameObject ZoneANavigationScreen;
    public GameObject ZoneBNavigationScreen;
    public GameObject ZoneCNavigationScreen;

    public Slider slider; 
     
    public GameObject ZoneMappingScreen;
    public GameObject GeosampleSelectionScreen;
    public  GameObject ColorHueSelectionScreen;
    public GameObject HueParentScreen;

    // buttons
    public GameObject buttonSideBar;
    public PressableButton mainMenuGeoButton;
    public ToggleCollection sideBarToggleCollection;
    public PressableButton procedureStep1Button;
    public PressableButton procedureStep2Button;
    public PressableButton procedureStep3Button;

    private Subscription<GeoSampleAddedToZoneEvent> geoSampleAddedToZoneEventSubscription;
    private Subscription<GeoSampleZoneAddedEvent> geoSampleZoneAddedEventSubscription;

    void Start()
    {
        geoSampleAddedToZoneEventSubscription = EventBus.Subscribe<GeoSampleAddedToZoneEvent>(OnGeoSampleAddedToZone);
        // geoSampleZoneAddedEventSubscription = EventBus.Subscribe<GeoSampleZoneAddedEvent>(OnGeoSampleZoneAdded);
    }

    void OnGeoSampleAddedToZone(GeoSampleAddedToZoneEvent e)
    {
        switch (e.newGeoSample.zone)
        {
            case "ZONE_A":
                AstronautInstance.User.geosampleZones[0].TotalGeoSamples.samples.Add(e.newGeoSample);
                break;
            case "ZONE_B":
                AstronautInstance.User.geosampleZones[1].TotalGeoSamples.samples.Add(e.newGeoSample);
                break;
            case "ZONE_C":
                AstronautInstance.User.geosampleZones[2].TotalGeoSamples.samples.Add(e.newGeoSample);
                break;
        }
    }

    // dynaamic zone creation -- may not be needed
    // void OnGeoSampleZoneAdded(GeoSampleZoneAddedEvent e)
    // {
        
    // }
}
