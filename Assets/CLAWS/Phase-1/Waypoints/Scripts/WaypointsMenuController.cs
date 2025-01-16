using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class WaypointsMenuController : MonoBehaviour
{
    [SerializeField] private GameObject waypointsMenu;
    [SerializeField] private GameObject createWaypointMenu;
    [SerializeField] private GameObject navigationMenu;

    [SerializeField] private GameObject companionsHighlight;
    [SerializeField] private GameObject stationsHighlight;
    [SerializeField] private GameObject samplesHighlight;
    [SerializeField] private GameObject dangerHighlight;
    [SerializeField] private GameObject interestHighlight;

    [SerializeField] private GameObject waypointTypeText;

    // Start is called before the first frame update
    void Start()
    {
        // Initalize menu gameobjects
        waypointsMenu = GameObject.Find("WaypointsMenu").gameObject;
        createWaypointMenu = GameObject.Find("CreateWaypointMenu").gameObject;
        navigationMenu = GameObject.Find("NavigationMenu").gameObject;
        createWaypointMenu.SetActive(false);
        navigationMenu.SetActive(false);

        // Initialize highlighted backplates for waypoint types
        GameObject buttons = waypointsMenu.transform.Find("VerticalAppBar").Find("Buttons").gameObject;
        companionsHighlight = buttons.transform.Find("CompanionsButton").Find("HighlightedBackplate").gameObject;
        stationsHighlight = buttons.transform.Find("StationsButton").Find("HighlightedBackplate").gameObject;
        samplesHighlight = buttons.transform.Find("SamplesButton").Find("HighlightedBackplate").gameObject;
        dangerHighlight = buttons.transform.Find("DangerButton").Find("HighlightedBackplate").gameObject;
        interestHighlight = buttons.transform.Find("InterestButton").Find("HighlightedBackplate").gameObject;
        stationsHighlight.SetActive(false);
        samplesHighlight.SetActive(false);
        dangerHighlight.SetActive(false);
        interestHighlight.SetActive(false);

        // Initialize current selected waypoint type
        companionsHighlight.SetActive(true);        // TODO: SPAWN WAYPOINT LIST FOR COMPANIONS
        stationsHighlight.SetActive(false);
        samplesHighlight.SetActive(false);
        dangerHighlight.SetActive(false);
        interestHighlight.SetActive(false);

        // Initialize waypoint type text
        waypointTypeText = waypointsMenu.transform.Find("WaypointTypeText").gameObject;
    }

    // onClick function for "X" button on menus
    public void onClickCloseMenu()
    {
        waypointsMenu.SetActive(false);
        createWaypointMenu.SetActive(false);
        navigationMenu.SetActive(false);
    }

    // onClick function for "Add Waypoint"
    public void onClickAddWaypoint()
    {
        waypointsMenu.SetActive(false);
        createWaypointMenu.SetActive(true);
    }

    // onClick function for changing waypoint types with sidebar buttons
    public void onClickChangeWaypointType()
    {
        // TODO: SPAWN WAYPOINT LIST FOR EACH WAYPOINT TYPE
        string clickedButton = EventSystem.current.currentSelectedGameObject.name;
        if (clickedButton.Equals("CompanionsButton"))
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Companions";
            companionsHighlight.SetActive(true);
            stationsHighlight.SetActive(false);
            samplesHighlight.SetActive(false);
            dangerHighlight.SetActive(false);
            interestHighlight.SetActive(false);
        }
        else if (clickedButton.Equals("StationsButton"))
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Stations";
            companionsHighlight.SetActive(false);
            stationsHighlight.SetActive(true);
            samplesHighlight.SetActive(false);
            dangerHighlight.SetActive(false);
            interestHighlight.SetActive(false);
        }
        else if (clickedButton.Equals("SamplesButton"))
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Samples";
            companionsHighlight.SetActive(false);
            stationsHighlight.SetActive(false);
            samplesHighlight.SetActive(true);
            dangerHighlight.SetActive(false);
            interestHighlight.SetActive(false);
        }
        else if (clickedButton.Equals("DangerButton"))
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Danger";
            companionsHighlight.SetActive(false);
            stationsHighlight.SetActive(false);
            samplesHighlight.SetActive(false);
            dangerHighlight.SetActive(true);
            interestHighlight.SetActive(false);
        }
        else if (clickedButton.Equals("InterestButton"))
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Interest";
            companionsHighlight.SetActive(false);
            stationsHighlight.SetActive(false);
            samplesHighlight.SetActive(false);
            dangerHighlight.SetActive(false);
            interestHighlight.SetActive(true);
        }
    }

    // onClick function for "Cancel" button on Create Waypoint menu
    public void onClickCancelCreateWaypoint()
    {
        waypointsMenu.SetActive(true);
        createWaypointMenu.SetActive(false);
    }

    // onClick function for "Confirm" button on Create Waypoint menu
    public void onClickConfirmCreateWaypoint()
    {
        // TODO: IMPLEMENT BELOW
    }

    // onClick function for waypoint name on Create Waypoint menu
    public void onClickWaypointName()
    {
        // TODO: IMPLEMENT BELOW
    }

    // onClick function for selecting waypoint type on Create Waypoint menu
    public void onClickSelectWaypointType()
    {
        // TODO: IMPLEMENT BELOW
    }

    // onClick function for "Back" button on Navigation menu
    public void onClickBackFromNavigation()
    {
        waypointsMenu.SetActive(true);
        navigationMenu.SetActive(false);
    }

    // onClick function for "Confirm" button on Navigation menu
    public void onClickConfirmNavigation()
    {
        // TODO: IMPLEMENT BELOW
    }
}
