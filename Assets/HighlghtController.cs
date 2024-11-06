using UnityEngine;

public class HighlightController : MonoBehaviour
{
    private GameObject button1;
    private Material renderer1;
    private GameObject button2;
    private Material renderer2;
    private GameObject button3;
    private Material renderer3;
    private GameObject button4;
    private Material renderer4;

    void Start()
    {
        // Get the button objects and their child GameObjects
        button1 = gameObject.transform.Find("Button1").gameObject;
        button2 = gameObject.transform.Find("Button2").gameObject;
        button3 = gameObject.transform.Find("Button3").gameObject;
        button4 = gameObject.transform.Find("Button4").gameObject;

        // Get the original materials for each button
        renderer1 = button1.transform.GetChild(1).GetChild(1).Find("UX.Button.FrontplateHighlight").GetComponent<Renderer>().material;
        renderer2 = button2.transform.GetChild(1).GetChild(1).Find("UX.Button.FrontplateHighlight").GetComponent<Renderer>().material;
        renderer3 = button3.transform.GetChild(1).GetChild(1).Find("UX.Button.FrontplateHighlight").GetComponent<Renderer>().material;
        renderer4 = button4.transform.GetChild(1).GetChild(1).Find("UX.Button.FrontplateHighlight").GetComponent<Renderer>().material;
    }

    public void button1_highlight()
    {
        SetButtonHighlight(button1, "Graphics Tools/Non-Canvas/Frontplate", Color.red);
    }

    public void button1_NO_highlight()
    {
        ResetButtonHighlight(button1, renderer1);
    }

    public void button2_highlight()
    {
        SetButtonHighlight(button2, "Graphics Tools/Non-Canvas/Frontplate", Color.green);
    }

    public void button2_NO_highlight()
    {
        ResetButtonHighlight(button2, renderer2);
    }

    public void button3_highlight()
    {
        SetButtonHighlight(button3, "Graphics Tools/Non-Canvas/Frontplate", Color.yellow);
    }

    public void button3_NO_highlight()
    {
        ResetButtonHighlight(button3, renderer3);
    }

    public void button4_highlight()
    {
        SetButtonHighlight(button4, "Graphics Tools/Non-Canvas/Frontplate", Color.black);
    }

    public void button4_NO_highlight()
    {
        ResetButtonHighlight(button4, renderer4);
    }

    private void SetButtonHighlight(GameObject button, string shaderName, Color color)
    {
        var frontplateHighlight = button.transform.GetChild(1).GetChild(1).Find("UX.Button.FrontplateHighlight");
        if (frontplateHighlight != null)
        {
            var renderer = frontplateHighlight.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material newMaterial = new Material(Shader.Find("UI/Default"));
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
