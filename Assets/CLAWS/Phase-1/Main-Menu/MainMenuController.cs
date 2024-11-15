using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject taskList;
    [SerializeField] private GameObject navigation;
    [SerializeField] private GameObject messages;
    [SerializeField] private GameObject samples;
    [SerializeField] private GameObject vitals;
    [SerializeField] private GameObject UIA;
    private Subscription<CloseEvent> closeSubscription;
    private Subscription<ScreenChangedEvent> screenChangedSubscription;
     
    // Start is called before the first frame update
    private void Start()
    {
       closeSubscription = EventBus.Subscribe<CloseEvent>(HandleCloseScreen);
       screenChangedSubscription = EventBus.Subscribe<ScreenChangedEvent>(HandleScreenChange);
       taskList.SetActive(false);
       navigation.SetActive(false);
       messages.SetActive(false);
       samples.SetActive(false);
       vitals.SetActive(false);
       UIA.SetActive(false); 
    }

    [ContextMenu("HandleScreenChange")]
    public void HandleScreenChange(ScreenChangedEvent e){
        if(e.Screen == Screens.taskList){
            taskList.SetActive(true);
            navigation.SetActive(false);
            messages.SetActive(false);
            samples.SetActive(false);
            vitals.SetActive(false);
            UIA.SetActive(false);
        }else if(e.Screen == Screens.navigation){
            taskList.SetActive(false);
            navigation.SetActive(true);
            messages.SetActive(false);
            samples.SetActive(false);
            vitals.SetActive(false);
            UIA.SetActive(false);
        }else if(e.Screen == Screens.messages){
            taskList.SetActive(false);
            navigation.SetActive(false);
            messages.SetActive(true);
            samples.SetActive(false);
            vitals.SetActive(false);
            UIA.SetActive(false);
        }else if(e.Screen == Screens.samples){
            taskList.SetActive(false);
            navigation.SetActive(false);
            messages.SetActive(false);
            samples.SetActive(true);
            vitals.SetActive(false);
            UIA.SetActive(false);
        }else if(e.Screen == Screens.vitals){
            taskList.SetActive(false);
            navigation.SetActive(false);
            messages.SetActive(false);
            samples.SetActive(false);
            vitals.SetActive(true);
            UIA.SetActive(false);
        }else if(e.Screen == Screens.UIA){
            taskList.SetActive(false);
            navigation.SetActive(false);
            messages.SetActive(false);
            samples.SetActive(false);
            vitals.SetActive(false);
            UIA.SetActive(true);
        }
    }

    public void HandleCloseScreen(CloseEvent e){
        if(e.Screen == Screens.taskList){
            taskList.SetActive(false);
        }else if(e.Screen == Screens.navigation){
            navigation.SetActive(false);
        }else if(e.Screen == Screens.messages){
            messages.SetActive(false);
        }else if(e.Screen == Screens.samples){
            samples.SetActive(false);
        }else if(e.Screen == Screens.vitals){
            vitals.SetActive(false);
        }else if(e.Screen == Screens.UIA){
            UIA.SetActive(false);
        }
    }
    

    // Update is called once per frame
    private void OnDestroy()
    {
        EventBus.Unsubscribe(closeSubscription);
        EventBus.Unsubscribe(screenChangedSubscription);
    }
}
