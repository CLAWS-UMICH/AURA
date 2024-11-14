using UnityEngine;
using MixedReality.Toolkit.UX;

public class ScreenChangeButton : MonoBehaviour
{
    public Screens TargetScreen;
    public PressableButton pressableButton;

    private void Start()
    {
        if (pressableButton != null)
        {
            pressableButton.OnClicked.AddListener(ChangeScreen);
        }
        else
        {
            Debug.LogError("No PressableButton component found on the GameObject.");
        }
    }

    private void ChangeScreen()
    {

        EventBus.Publish(new CloseEvent(StateMachine.Instance.CurrScreen));
        EventBus.Publish(new ScreenChangedEvent(TargetScreen));

        Debug.Log("Changing screen from " + StateMachine.Instance.CurrScreen.ToString() + " to " + TargetScreen.ToString());
    }
}
