using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.UX;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class GeoSampleController : MonoBehaviour
{
    public NavigationController navigationController;
    [Header("Screens")]
    public GameObject geoSampleControllerScreen;
    public GameObject ZoneA_databaseSamplesScreen;
    public GameObject ZoneB_databaseSamplesScreen;
    public GameObject ZoneC_databaseSamplesScreen;
    public GameObject startScreen; 
    public GameObject ZoneMappingScreen;
    public GameObject GeosampleSelectionScreen;
    public  GameObject ColorHueSelectionScreen;
    public GameObject HueParentScreen;
    public GameObject ShapeSelectionScreen;
    public GameObject TextureSelectionScreen;
    // buttons
    public GameObject buttonSideBar;
    public ToggleCollection menuToggleCollection;

    private Subscription<GeoSampleAddedToZoneEvent> geoSampleAddedToZoneEventSubscription;
    private Subscription<GeoSampleZoneAddedEvent> geoSampleZoneAddedEventSubscription;

    void Start()
    {
        geoSampleAddedToZoneEventSubscription = EventBus.Subscribe<GeoSampleAddedToZoneEvent>(OnGeoSampleAddedToZone);
        geoSampleZoneAddedEventSubscription = EventBus.Subscribe<GeoSampleZoneAddedEvent>(OnGeoSampleZoneAdded);
    }

    void OnGeoSampleAddedToZone(GeoSampleAddedToZoneEvent e)
    {
        switch (e.newGeoSample.zone)
        {
            case "ZONE_A":
                
                break;
            case "ZONE_B":

                break;
            case "ZONE_C":

                break;
        }
    }


    void OnGeoSampleZoneAdded(GeoSampleZoneAddedEvent e)
    {
        
    }
}
