using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixScrollRotation : MonoBehaviour
{
    void Update()
    {
        transform.localRotation = Quaternion.identity;
    }
}

