using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class WaypointChar : MonoBehaviour
{
    [SerializeField] private string currentText = "A";
    private float textDepth = 0.05f; // Default depth set to 0.05
    private TextMeshPro textMesh;
    private const float TEXT_SIZE = 5f; // Larger fixed size

    void Start()
    {
        SetupTextMesh();
    }

    void SetupTextMesh()
    {
        GameObject textObject = new GameObject("TextMeshPro");
        textObject.transform.SetParent(transform, false);

        textMesh = textObject.AddComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = TEXT_SIZE;
        
        // Set the layer to 'Minimap Only'
        textObject.layer = LayerMask.NameToLayer("Minimap Only");
        
        UpdateText(currentText);
        PositionTextOnQuad();
    }

    void PositionTextOnQuad()
    {
        if (textMesh != null)
        {
            textMesh.transform.localPosition = new Vector3(0, 0, -textDepth);
            textMesh.transform.localRotation = Quaternion.identity;
        }
    }

    public void UpdateText(string newText)
    {
        currentText = newText;
        if (textMesh != null)
        {
            textMesh.text = currentText;
        }
    }

    private void OnValidate()
    {
        if (textMesh != null)
        {
            UpdateText(currentText);
            PositionTextOnQuad();
        }
    }
}
