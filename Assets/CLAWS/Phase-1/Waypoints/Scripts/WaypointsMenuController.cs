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

    //[SerializeField] private GameObject companionsHighlight;
    //[SerializeField] private GameObject stationsHighlight;
    //[SerializeField] private GameObject samplesHighlight;
    //[SerializeField] private GameObject dangerHighlight;
    //[SerializeField] private GameObject interestHighlight;

    [SerializeField] private GameObject waypointTypeText;

    [SerializeField] private static List<Waypoint> waypointsList = Waypoints.WaypointsList;

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
        //GameObject buttons = waypointsMenu.transform.Find("VerticalAppBar").Find("Buttons").gameObject;
        //companionsHighlight = buttons.transform.Find("CompanionsButton").Find("HighlightedBackplate").gameObject;
        //stationsHighlight = buttons.transform.Find("StationsButton").Find("HighlightedBackplate").gameObject;
        //samplesHighlight = buttons.transform.Find("SamplesButton").Find("HighlightedBackplate").gameObject;
        //dangerHighlight = buttons.transform.Find("DangerButton").Find("HighlightedBackplate").gameObject;
        //interestHighlight = buttons.transform.Find("InterestButton").Find("HighlightedBackplate").gameObject;
        //stationsHighlight.SetActive(false);
        //samplesHighlight.SetActive(false);
        //dangerHighlight.SetActive(false);
        //interestHighlight.SetActive(false);

        // Initialize current selected waypoint type
        //companionsHighlight.SetActive(true);        // TODO: SPAWN WAYPOINT LIST FOR COMPANIONS
        //stationsHighlight.SetActive(false);
        //samplesHighlight.SetActive(false);
        //dangerHighlight.SetActive(false);
        //interestHighlight.SetActive(false);

        // Initialize waypoint type text
        waypointTypeText = waypointsMenu.transform.Find("WaypointTypeText").gameObject;
    }

    private void Update()
    {
        // FIXME: Optimize so the list is not constantly updating (only update when waypoint is added, edited, or deleted)
        waypointsList = Waypoints.WaypointsList;
    }

    public void openWaypoints()
    {
        waypointsMenu.SetActive(true);
        createWaypointMenu.SetActive(false);
        navigationMenu.SetActive(false);
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
    public void onClickChangeWaypointType(int buttonIndex)
    {
        // TODO: SPAWN WAYPOINT LIST FOR EACH WAYPOINT TYPE
        string clickedButton = EventSystem.current.currentSelectedGameObject.name;
        if (clickedButton.Equals("CompanionsButton") || buttonIndex == 0)
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Companions";
            foreach (Waypoint w in waypointsList)
            {
                if (w.Type == "Companion")
                {
                    w.WaypointObject.transform.Find("Focused").gameObject.SetActive(true);
                    w.WaypointObject.transform.Find("Minimized").gameObject.SetActive(false);
                }
                else
                {
                    w.WaypointObject.transform.Find("Focused").gameObject.SetActive(false);
                    w.WaypointObject.transform.Find("Minimized").gameObject.SetActive(true);
                }
            }
            //companionsHighlight.SetActive(true);
            //stationsHighlight.SetActive(false);
            //samplesHighlight.SetActive(false);
            //dangerHighlight.SetActive(false);
            //interestHighlight.SetActive(false);
        }
        else if (clickedButton.Equals("StationsButton") || buttonIndex == 1)
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Stations";
            foreach (Waypoint w in waypointsList)
            {
                if (w.Type == "Station")
                {
                    w.WaypointObject.transform.Find("Focused").gameObject.SetActive(true);
                    w.WaypointObject.transform.Find("Minimized").gameObject.SetActive(false);
                }
                else
                {
                    w.WaypointObject.transform.Find("Focused").gameObject.SetActive(false);
                    w.WaypointObject.transform.Find("Minimized").gameObject.SetActive(true);
                }
            }
            //companionsHighlight.SetActive(false);
            //stationsHighlight.SetActive(true);
            //samplesHighlight.SetActive(false);
            //dangerHighlight.SetActive(false);
            //interestHighlight.SetActive(false);
        }
        else if (clickedButton.Equals("SamplesButton") || buttonIndex == 2)
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Samples";
            foreach (Waypoint w in waypointsList)
            {
                if (w.Type == "Sample")
                {
                    w.WaypointObject.transform.Find("Focused").gameObject.SetActive(true);
                    w.WaypointObject.transform.Find("Minimized").gameObject.SetActive(false);
                }
                else
                {
                    w.WaypointObject.transform.Find("Focused").gameObject.SetActive(false);
                    w.WaypointObject.transform.Find("Minimized").gameObject.SetActive(true);
                }
            }
            //companionsHighlight.SetActive(false);
            //stationsHighlight.SetActive(false);
            //samplesHighlight.SetActive(true);
            //dangerHighlight.SetActive(false);
            //interestHighlight.SetActive(false);
        }
        else if (clickedButton.Equals("DangerButton") || buttonIndex == 3)
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Danger";
            foreach (Waypoint w in waypointsList)
            {
                if (w.Type == "Danger")
                {
                    w.WaypointObject.transform.Find("Focused").gameObject.SetActive(true);
                    w.WaypointObject.transform.Find("Minimized").gameObject.SetActive(false);
                }
                else
                {
                    w.WaypointObject.transform.Find("Focused").gameObject.SetActive(false);
                    w.WaypointObject.transform.Find("Minimized").gameObject.SetActive(true);
                }
            }
            //companionsHighlight.SetActive(false);
            //stationsHighlight.SetActive(false);
            //samplesHighlight.SetActive(false);
            //dangerHighlight.SetActive(true);
            //interestHighlight.SetActive(false);
        }
        else if (clickedButton.Equals("InterestButton") || buttonIndex == 4)
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Interest";
            foreach (Waypoint w in waypointsList)
            {
                if (w.Type == "Interest")
                {
                    w.WaypointObject.transform.Find("Focused").gameObject.SetActive(true);
                    w.WaypointObject.transform.Find("Minimized").gameObject.SetActive(false);
                }
                else
                {
                    w.WaypointObject.transform.Find("Focused").gameObject.SetActive(false);
                    w.WaypointObject.transform.Find("Minimized").gameObject.SetActive(true);
                }
            }
            //companionsHighlight.SetActive(false);
            //stationsHighlight.SetActive(false);
            //samplesHighlight.SetActive(false);
            //dangerHighlight.SetActive(false);
            //interestHighlight.SetActive(true);
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
