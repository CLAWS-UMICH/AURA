using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This gameobject will be deleted on Start() if 
/// running on Universal Windows Platform (HoloLens)
/// </summary>
public class ConditionallyCompile : MonoBehaviour
{

#if UNITY_WSA && !UNITY_EDITOR
    void Start()
    {
        Destroy(gameObject);
    }
#endif

}