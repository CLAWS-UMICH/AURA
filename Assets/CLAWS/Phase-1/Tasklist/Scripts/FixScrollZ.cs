using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixScrollZ : MonoBehaviour
{
    [SerializeField] float z;

    void Start()
    {

    }

    void Update()
    {
        transform.localPosition = new Vector3(0, transform.localPosition.y, z);
    }
}

