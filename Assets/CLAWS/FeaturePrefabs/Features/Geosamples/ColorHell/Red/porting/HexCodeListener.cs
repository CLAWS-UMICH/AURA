using UnityEngine;
using MixedReality.Toolkit.UX;
using TMPro;
using Unity.VisualScripting;

public class ToggleCollectionHandler : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The MRTK Toggle Collection to monitor.")]
    private ToggleCollection toggleCollection;

    [SerializeField]
    [Tooltip("The materials associated with each toggle button.")]
    private Material[] buttonMaterials;

    private void Start()
    {
        if (toggleCollection == null)
        {
            Debug.LogError("ToggleCollection is not assigned.");
            return;
        }

        // Subscribe to the OnToggleSelected event
        toggleCollection.OnToggleSelected.AddListener(HandleToggleSelected);
    }

    private void OnDestroy()
    {
        if (toggleCollection != null)
        {
            toggleCollection.OnToggleSelected.RemoveListener(HandleToggleSelected);
        }
    }

    private void HandleToggleSelected(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= buttonMaterials.Length)
        {
            Debug.LogWarning("Selected index is out of range.");
            return;
        }

        Material selectedMaterial = buttonMaterials[selectedIndex];
        if (selectedMaterial != null && selectedMaterial.HasProperty("_Color"))
        {
            // Extract the hex code from the material's color
            Color color = selectedMaterial.color;
            GeoSampleFrontend geoSampleFrontend = FindObjectOfType<GeoSampleFrontend>();

            // Set the currently constructed geosample's color to be the hexcode (format RRGGBB)
            geoSampleFrontend.sample.color = UnityEngine.ColorUtility.ToHtmlStringRGBA(color);
            geoSampleFrontend.sample.color = geoSampleFrontend.sample.color.Substring(0, geoSampleFrontend.sample.color.Length - 2);
            Debug.Log($"Selected Toggle Index: {selectedIndex}, Hex Code: #{geoSampleFrontend.sample.color}");

            // Update the complete geosampling UI for the color
            Color newColor;
            if (UnityEngine.ColorUtility.TryParseHtmlString("#" + geoSampleFrontend.sample.color, out newColor))
            {
                geoSampleFrontend.geoSampleController.colorCompleteUI.transform.Find("Color").GetComponent<SpriteRenderer>().color = newColor;
                Debug.Log("color changed: " + newColor.ToString());
            }
            geoSampleFrontend.geoSampleController.colorCompleteUI.transform.Find("ColorText").GetComponent<TextMeshPro>().text = "#" + geoSampleFrontend.sample.color;

            // Make new UI visible
            geoSampleFrontend.geoSampleController.colorInitUI.SetActive(false);
            geoSampleFrontend.geoSampleController.colorCompleteUI.SetActive(true);

            geoSampleFrontend.colorSelected = true;
        }
        else
        {
            Debug.LogWarning("Material or _Color property is missing.");
        }
    }
}