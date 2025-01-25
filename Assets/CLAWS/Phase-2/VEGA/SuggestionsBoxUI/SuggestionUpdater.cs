using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* 
Script made by Kevin M
phase 2 voice assistant
changes the text of the voice assistant suggestions 
*/

public class SuggestionUpdater : MonoBehaviour
{
    [SerializeField] private string[] textOptions; // Array of the diff voice assistant suggestions
    private TextMeshPro[] textElements; // Array to store child TextMeshPro components

    private void Awake()
    {
        List<TextMeshPro> suggestionTexts = new List<TextMeshPro>();

        // get textmeshpro obj named "suggestion"
        foreach (TextMeshPro tmp in GetComponentsInChildren<TextMeshPro>(true))
        {
            if (tmp.gameObject.name == "suggestion")
            {
                suggestionTexts.Add(tmp);
                
                // for some reason the container holding the text would move itself on start,
                // so this block just forces the correct position of it 
                Transform parent = tmp.transform.parent;
                if (parent != null && parent.gameObject.name == "textContainer")
                {
                    Vector3 newPos = parent.localPosition;
                    newPos.x = -0.0168f;
                    parent.localPosition = newPos;
                }
            }
        }

        textElements = suggestionTexts.ToArray();

        // Make sure its only 5 objects
        if (textElements.Length != 5)
        {
            Debug.LogError($"expected 5 TextMeshPro children, found {textElements.Length}", this);
        }
    }

    // Update the text of the 5 TextMeshPro components
    public void UpdateTextElements(int[] indexes)
    {
        if (indexes.Length != 5)
        {
            Debug.LogError("input array should only have  5 indexes", this);
            return;
        }

        // Update each text element
        for (int i = 0; i < 5; i++)
        {
            if (indexes[i] < 0 || indexes[i] >= textOptions.Length)
            {
                Debug.LogError($"invalid index {indexes[i]} at position {i}", this);
                continue;
            }

            textElements[i].text = textOptions[indexes[i]];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Example of how of how to use func
        int[] indexes = {0, 1, 2, 3, 4};
        UpdateTextElements(indexes);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
