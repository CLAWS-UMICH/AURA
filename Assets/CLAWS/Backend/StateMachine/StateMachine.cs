// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [System.Serializable]
// public enum Screens
// {
//     Menu,
//     taskList,
//     navigation,
//     messages,
//     samples,
//     vitals,
//     UIA,
//     VitalsFirstAstronaut,
//     VitalsSecondAstronaut
// }

// public enum Modes
// {
//     Normal,
//     Sampling,
//     Navigation,
//     Egress
// }

// public class StateMachine : MonoBehaviour
// {
//     // Singleton instance
//     public static StateMachine Instance { get; private set; }

//     public Screens CurrScreen = Screens.Menu;
//     public Modes CurrMode = Modes.Normal;

//     private Subscription<ScreenChangedEvent> screenChangedSubscription;
//     private Subscription<ModeChangedEvent> modeChangedSubscription;

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//         }
//         else
//         {
//             Destroy(gameObject);
//         }

//         InitializeEventSubscriptions();
//     }

//     private void InitializeEventSubscriptions()
//     {
        
//     }

//     private void OnDestroy()
//     {
//         if (screenChangedSubscription != null)
//         {
//             EventBus.Unsubscribe(screenChangedSubscription);
//         }
//         if (modeChangedSubscription != null)
//         {
//             EventBus.Unsubscribe(modeChangedSubscription);
//         }
//     }

//     // Switch screen when ScreenChangedEvent is published
//     public void SwitchScreen(ScreenChangedEvent e)
//     {
//         Debug.Log($"{CurrScreen} -> {e.Screen}");
//         CurrScreen = e.Screen;
//     }

//     // Switch mode when ModeChangedEvent is published
//     public void SwitchMode(ModeChangedEvent e)
//     {
//         Debug.Log($"{CurrMode} -> {e.Mode}");
//         CurrMode = e.Mode;
//     }
    
//     // Close screen when called
//     public void CloseScreen(CloseEvent e)
//     {
//         // Debug log for closing the screen
//         Debug.Log("Closing screen: " + e.Screen.ToString());
//         // currScreen = Screen.Menu;
//     }
// }

