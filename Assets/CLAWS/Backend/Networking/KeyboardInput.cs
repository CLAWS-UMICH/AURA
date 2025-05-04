using TMPro;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    private TouchScreenKeyboard keyboard;
    [SerializeField] private GameObject keyboardObject;

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    private void Update()
    {
        if (keyboard != null)
        {
            // Check if enter key is pressed or the keyboard is closed
            if (keyboard.status == TouchScreenKeyboard.Status.Done || keyboard.status == TouchScreenKeyboard.Status.Canceled)
            {
                // Update the TextMeshPro text with the final input
                keyboardObject.GetComponent<TextMeshPro>().text = keyboard.text;
                keyboard = null;
            }
            else
            {
                // Update the TextMeshPro text in real-time as the user types
                keyboardObject.GetComponent<TextMeshPro>().text = keyboard.text;
            }
        }
    }
}
