using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit;

public class MenuState : MonoBehaviour
{

    // [SerializeField] private bool isIRISMenuOpen;
    // [SerializeField] private bool isIRISModeSelected;

    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject TasksButton;
    [SerializeField] private GameObject NavigationButton;
    [SerializeField] private GameObject MessagesButton;
    [SerializeField] private GameObject SamplesButton;
    [SerializeField] private GameObject VitalsButton; 

    // are we doing the modes button??
    // [SerializeField] private GameObject ModesButton;

    // [SerializeField] private GameObject IRISMenu;
    // [SerializeField] private GameObject IRISSamplingButton;
    // [SerializeField] private GameObject IRISNavigationButton;
    // [SerializeField] private GameObject IRISRouteButton;
    // [SerializeField] private GameObject IRISEgressButton;
    // [SerializeField] private GameObject IRISAirlockButton;
    // [SerializeField] private GameObject IRISCloseButton;

    // [SerializeField] private GameObject HideMenuButton;
    // [SerializeField] private GameObject IRISHideMenuButton;
    // [SerializeField] private GameObject ShowMenuButton;
    // [SerializeField] private GameObject ExitIRISModeButton;

    // [SerializeField] private Material regularModes;
    // [SerializeField] private Material highlightedModes;
    //[SerializeField] private TMP_Text mode;

    private Subscription<ModeChangedEvent> modeChangedSubscription;

    private void Start()
    {
        //EventBus.Subscribe<GeosampleModeEndedEvent>((e) => ClickIRISClose());
        modeChangedSubscription = EventBus.Subscribe<ModeChangedEvent>(SwitchMode);
    }

    private void SwitchMode(ModeChangedEvent e)
    {
        switch (e.Mode)
        {
            case Modes.Normal:
                // ClickIRISClose();
                break;
            case Modes.Sampling:
                //ClickIRISSampling();
                break;
            case Modes.Navigation:
                //ClickIRISNavigation();
                break;
            case Modes.Egress:
                //ClickIRISEgress();
                break;
            default:
                // ClickIRISClose();
                break;
        }
    }

    public void ClickTasks()
    {
        //transform.parent.Find("Main_TaskList").GetComponent<TaskListScreenHandler>().OpenTaskListMain();
        //ClickIRISClose(
    }

    public void ClickNavigation()
    {
        //transform.parent.Find("Navigation").Find("NavController").GetComponent<NavScreenHandler>().OpenNavScreen();
        //ClickIRISClose();
    }

    public void ClickMessages()
    {
        //ClickIRISClose();
    }

    public void ClickSamples()
    {
        //ClickIRISClose();
    }
    public void ClickVitals()
    {
        transform.parent.Find("Vitals").GetComponent<VitalsController>().ToggleVitalsScreen();
        // ClickIRISClose();
    }

    public void ClickUIA()
    {
        // TODO: Open Egress

    }

    // public void ClickModes()
    // {

    //     // Make IRIS Menu visible
    //     IRISMenu.SetActive(true);
    //     isIRISMenuOpen = true;

    //     // Change hide menu buttons
    //     HideMenuButton.SetActive(false);
    //     IRISHideMenuButton.SetActive(true);

    //     // Show the exit IRIS mode button if an IRIS mode is currently selected
    //     if (isIRISModeSelected)
    //     {
    //         ExitIRISModeButton.SetActive(true);
    //     }

    //     // Change the backplate transparency of top menu icons
    //     Material mat = TasksButton.transform.Find("Button").transform.Find("BackPlate").transform.Find("Quad").transform.GetComponent<MeshRenderer>().material;
    //     mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.4f);

    //     mat = NavigationButton.transform.Find("Button").transform.Find("BackPlate").transform.Find("Quad").transform.GetComponent<MeshRenderer>().material;
    //     mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.4f);

    //     mat = MessagesButton.transform.Find("Button").transform.Find("BackPlate").transform.Find("Quad").transform.GetComponent<MeshRenderer>().material;
    //     mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.4f);

    //     mat = SamplesButton.transform.Find("Button").transform.Find("BackPlate").transform.Find("Quad").transform.GetComponent<MeshRenderer>().material;
    //     mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.4f);

    //     mat = VitalsButton.transform.Find("Button").transform.Find("BackPlate").transform.Find("Quad").transform.GetComponent<MeshRenderer>().material;
    //     mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.4f);
    // }

    // public void ClickIRISSampling()
    // {
    //     ClickHideMenu();

    //     // Update exit IRIS mode button text
    //     mode.text = "Exit Sampling Mode";
    //     isIRISModeSelected = true;
    // }

    // public void ClickIRISNavigation()
    // {
    //     ClickHideMenu();

    //     // Update exit IRIS mode button text
    //     mode.text = "Exit Navigation Mode";
    //     isIRISModeSelected = true;
    // }

    // public void ClickIRISRoute()
    // {
    //     ClickHideMenu();

    //     // Update exit IRIS mode button text
    //     mode.text = "Exit Route Mode";
    //     isIRISModeSelected = true;
    // }

    // public void ClickIRISEgress()
    // {
    //     ClickHideMenu();

    //     // Update exit IRIS mode button text
    //     mode.text = "Exit Egress Mode";
    //     isIRISModeSelected = true;
    // }

    // public void ClickIRISAirlock()
    // {
    //     ClickHideMenu();

    //     // Update exit IRIS mode button text
    //     mode.text = "Exit Airlock Mode";
    //     isIRISModeSelected = true;
    // }

    // public void ClickIRISClose()
    // {
    //     // Make IRIS Menu invisible
    //     IRISMenu.SetActive(false);
    //     isIRISMenuOpen = false;
    //     ExitIRISModeButton.SetActive(false);

    //     // Change hide menu buttons
    //     HideMenuButton.SetActive(true);
    //     IRISHideMenuButton.SetActive(false);

    //     ClickShowMenu();

    //     // Change the backplate transparency of top menu icons back to full opacity

    //     Material mat = TasksButton.transform.Find("Button").transform.Find("BackPlate").transform.Find("Quad").transform.GetComponent<MeshRenderer>().material;
    //     mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1f);

    //     mat = NavigationButton.transform.Find("Button").transform.Find("BackPlate").transform.Find("Quad").transform.GetComponent<MeshRenderer>().material;
    //     mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1f);

    //     mat = MessagesButton.transform.Find("Button").transform.Find("BackPlate").transform.Find("Quad").transform.GetComponent<MeshRenderer>().material;
    //     mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1f);

    //     mat = SamplesButton.transform.Find("Button").transform.Find("BackPlate").transform.Find("Quad").transform.GetComponent<MeshRenderer>().material;
    //     mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1f);

    //     mat = VitalsButton.transform.Find("Button").transform.Find("BackPlate").transform.Find("Quad").transform.GetComponent<MeshRenderer>().material;
    //     mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1f);
    // }

    public void ClickHideMenu()
    {
        MainMenu.SetActive(false);
        // IRISMenu.SetActive(false);
        // HideMenuButton.SetActive(false);
        // IRISHideMenuButton.SetActive(false);
        // ExitIRISModeButton.SetActive(false);
        // ShowMenuButton.SetActive(true);
    }

    // public void ClickShowMenu()
    // {
    //     MainMenu.SetActive(true);
    //     ShowMenuButton.SetActive(false);
    //     if (isIRISMenuOpen)
    //     {
    //         IRISMenu.SetActive(true);
    //         IRISHideMenuButton.SetActive(true);
            
    //         if (isIRISModeSelected)
    //         {
    //             ExitIRISModeButton.SetActive(true);
    //         }
    //     }
    //     else
    //     {
    //         HideMenuButton.SetActive(true);
    //     }
    // }

    // public void ClickExitIRISMode()
    // {
    //     isIRISModeSelected = false;
    //     ExitIRISModeButton.SetActive(false);

    // }

}
