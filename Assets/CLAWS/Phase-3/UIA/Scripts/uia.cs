using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit;

public class UIAState : MonoBehaviour
{

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
        transform.parent.Find("Navigation").gameObject.SetActive(true);
        transform.parent.Find("Navigation").GetComponent<WaypointsMenuController>().openWaypoints();
        transform.parent.Find("Navigation").GetComponent<WaypointsMenuController>().onClickChangeWaypointType(0);
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
}
