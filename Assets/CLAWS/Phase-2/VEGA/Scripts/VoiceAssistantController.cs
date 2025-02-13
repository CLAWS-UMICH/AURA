using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceAssistantController : MonoBehaviour
{
    [SerializeField] private GameObject voiceAssistant;
    [SerializeField] private AIAssistantIcon assistantIcon;

    public void ToggleVoiceAssistant()
    {
        //Debug.Log($"Current Screen: {StateMachine.Instance.CurrScreen}");
        voiceAssistant.SetActive(true);
        assistantIcon.ToggleVoiceAssistant(true);
        assistantIcon.Listening();
        //EventBus.Publish(new ScreenChangedEvent(Screens.VitalsSecondAstronaut));
    }

    public void CloseVoiceAssistant()
    {
        assistantIcon.ToggleVoiceAssistant(false);
        voiceAssistant.SetActive(false);
    }
}
