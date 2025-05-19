using UnityEngine;
using MixedReality.Toolkit.UX;

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
            string hexCode = ColorUtility.ToHtmlStringRGBA(color);
            Debug.Log($"Selected Toggle Index: {selectedIndex}, Hex Code: #{hexCode}");
        }
        else
        {
            Debug.LogWarning("Material or _Color property is missing.");
        }
    }
}