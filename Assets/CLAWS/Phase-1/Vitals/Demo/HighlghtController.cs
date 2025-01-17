//molly maloney
using UnityEngine;
using MixedReality.Toolkit.UX;

public class HighlightController : MonoBehaviour
{
    // IMPORTANT: put your total number of buttons for the screen 
    // and this will populate the highlights correctly 
    // (1 indexed, like speaking), "1, 2, 3, 4, 5"
    [SerializeField]private int totalButtons = 0;
    private GameObject[] buttons;
    private Material[] originalMaterials;

    void Start()
    {
        buttons = new GameObject[totalButtons];
        originalMaterials = new Material[totalButtons];

        for (int i = 0; i < totalButtons; ++i)
        {
            int adjusted = i + 1;
            string buttonName = "Button" + adjusted;
            Debug.Log(buttonName);

            buttons[i] = gameObject.transform.Find(buttonName)?.gameObject;
            if (buttons[i] != null)
            {
                var button =  buttons[i].GetComponent<PressableButton>();
                originalMaterials[i] = buttons[i].transform.GetChild(1).GetChild(1).Find("UX.Button.FrontplateHighlight").GetComponent<Renderer>().material;
                button.IsGazeHovered.OnEntered.AddListener((data) => OnButtonGazeEnter(i));
                button.IsGazeHovered.OnExited.AddListener((data) => OnButtonGazeExit(i));
            }
            else { Debug.Log("null button"); }
        }
    }


    private void OnButtonGazeEnter(int buttonIndex)
    {
        Debug.Log("setting:" + buttonIndex);
        SetButtonHighlight(buttons[buttonIndex], "UI/Default", Color.green);
    }


    private void OnButtonGazeExit(int buttonIndex)
    {
        Debug.Log("redo:" + buttonIndex);
        ResetButtonHighlight(buttons[buttonIndex], originalMaterials[buttonIndex]);
    }


    private void SetButtonHighlight(GameObject button, string shader, Color color)
    {
        var frontplateHighlight = button.transform.GetChild(1).GetChild(1).Find("UX.Button.FrontplateHighlight");
        if (frontplateHighlight != null)
        {
            var renderer = frontplateHighlight.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material newMaterial = new Material(Shader.Find("UI/default"));
                newMaterial.SetColor("_Color", color);
                renderer.material = newMaterial;
            }
        }
    }

    private void ResetButtonHighlight(GameObject button, Material originalMaterial)
    {
        var frontplateHighlight = button.transform.GetChild(1).GetChild(1).Find("UX.Button.FrontplateHighlight");
        if (frontplateHighlight != null)
        {
            var renderer = frontplateHighlight.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = originalMaterial;
            }
        }
    }
}
