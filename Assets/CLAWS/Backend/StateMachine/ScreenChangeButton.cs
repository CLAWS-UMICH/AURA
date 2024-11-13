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
        // Close the current screen first
        EventBus.Publish(new CloseEvent(StateMachine.Instance.CurrScreen));

        // Then, publish the ScreenChangedEvent to switch to the target screen
        EventBus.Publish(new ScreenChangedEvent(TargetScreen));

        // Optionally log the screen change
        Debug.Log("Changing screen from " + StateMachine.Instance.CurrScreen.ToString() + " to " + TargetScreen.ToString());
    }
}
