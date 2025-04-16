using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class StartingScript : MonoBehaviour
{
    [SerializeField] private GameObject greetingScreen;

    // Start is called before the first frame update
    void Start()
    {
        transform.Find("Main").gameObject.SetActive(false);
        transform.Find("Screens").gameObject.SetActive(false);
        greetingScreen.SetActive(true);
    }
}
