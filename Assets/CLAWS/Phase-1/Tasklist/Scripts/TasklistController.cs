using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasklistController : MonoBehaviour
{
    [SerializeField] private GameObject tasklist;

    public void ToggleTasklistScreen()
    {
        //Debug.Log($"Current Screen: {StateMachine.Instance.CurrScreen}");
        tasklist.SetActive(true);
        //EventBus.Publish(new ScreenChangedEvent(Screens.VitalsSecondAstronaut));
    }

    public void CloseTasklistScreen()
    {
        tasklist.SetActive(false);
    }
}
