using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*

For two buttons, Button1 and Button2, that bring up their respective plate (Plate1, Plate2) while hiding the other. 
Ian Kim 1/11/2025 

*/

public class PlateToggle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject plate1;
    [SerializeField] private GameObject plate2;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;

    private int current = 0;

    //"current" is the current plate being displayed

    void Start()
    {
        canvas = GameObject.Find("Canvas");

        plate1 = canvas.transform.Find("Plate1").gameObject;
        plate2 = canvas.transform.Find("Plate2").gameObject;

        button1 = canvas.transform.Find("Button1").gameObject.GetComponent<Button>();
        button2 = canvas.transform.Find("Button2").gameObject.GetComponent<Button>();

        button1.onClick.AddListener(button1Clicked);
        button2.onClick.AddListener(button2Clicked);

        plate2.SetActive(false);

    }

    public void button1Clicked() {
        if (current == 2) {
            plate2.SetActive(false);
        }

        current = 1;
        plate1.SetActive(true);
    }

    public void button2Clicked() {
        if (current == 1) {
            plate1.SetActive(false);
        }

        current = 2;
        plate2.SetActive(true);
    }
}
