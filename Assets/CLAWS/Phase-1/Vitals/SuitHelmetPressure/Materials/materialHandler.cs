using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialHandler : MonoBehaviour
{
    private Material uniqueMaterial;

    void Start()
    {
        // Create a unique instance of the material for this quad
        uniqueMaterial = new Material(GetComponent<Renderer>().sharedMaterial);
        GetComponent<Renderer>().material = uniqueMaterial;
    }

    // Provide a public getter for the unique material
    public Material GetMaterial()
    {
        return uniqueMaterial;
    }
}


