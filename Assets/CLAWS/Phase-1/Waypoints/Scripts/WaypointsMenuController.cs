using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MixedReality.Toolkit.UX;
using TMPro;

public class WaypointsMenuController : MonoBehaviour
{
    [SerializeField] private GameObject waypointsMenu;
    [SerializeField] private GameObject createWaypointMenu;
    [SerializeField] private GameObject navigationMenu;

    [SerializeField] private GameObject waypointTypeText;

    [SerializeField] private GameObject companionsButton;
    [SerializeField] private GameObject stationsButton;
    [SerializeField] private GameObject samplesButton;
    [SerializeField] private GameObject dangerButton;
    [SerializeField] private GameObject interestButton;

    // Start is called before the first frame update
    void Start()
    {
        // Initalize menu gameobjects
        waypointsMenu = GameObject.Find("WaypointsMenu").gameObject;
        createWaypointMenu = GameObject.Find("CreateWaypointMenu").gameObject;
        navigationMenu = GameObject.Find("NavigationMenu").gameObject;
        createWaypointMenu.SetActive(false);
        navigationMenu.SetActive(false);

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

    public void OpenNavigationMenu()
    {
        waypointsMenu.SetActive(true);
        onClickChangeWaypointType(0);
    }

    // onClick function for "Add Waypoint"
    public void onClickAddWaypoint()
    {
        waypointsMenu.SetActive(false);
        createWaypointMenu.SetActive(true);
    }

    // onClick function for changing waypoint types with sidebar buttons
    public void onClickChangeWaypointType(float type)
    {
        // TODO: SPAWN WAYPOINT LIST FOR EACH WAYPOINT TYPE
        string clickedButton = EventSystem.current.currentSelectedGameObject.name;
        if (clickedButton.Equals("CompanionsButton") || type == 0)
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Companions";
            companionsButton.GetComponent<PressableButton>().ForceSetToggled(true);
        }
        else if (clickedButton.Equals("StationsButton") || type == 1)
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Stations";
            stationsButton.GetComponent<PressableButton>().ForceSetToggled(true);
        }
        else if (clickedButton.Equals("SamplesButton") || type == 2)
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Samples";
            samplesButton.GetComponent<PressableButton>().ForceSetToggled(true);
        }
        else if (clickedButton.Equals("DangerButton") || type == 3)
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Danger";
            dangerButton.GetComponent<PressableButton>().ForceSetToggled(true);
        }
        else if (clickedButton.Equals("InterestButton") || type == 4)
        {
            waypointTypeText.GetComponent<TextMeshPro>().text = "Interest";
            interestButton.GetComponent<PressableButton>().ForceSetToggled(true);
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
