using UnityEngine;
using UnityEngine.UI;

public class ScreenChangeButton : MonoBehaviour
{
    public Screens TargetScreen;

    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ChangeScreen);
        }
        else
        {
            Debug.LogError("No Button component found on the GameObject.");
        }
    }

    private void ChangeScreen()
    {
        EventBus.Publish(new ScreenChangedEvent(TargetScreen));
    }
}
