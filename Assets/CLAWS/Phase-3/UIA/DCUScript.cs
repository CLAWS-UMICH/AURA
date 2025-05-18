using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DCUScript : MonoBehaviour
{
    // Public field to hold a single game object
    public GameObject battToggleObject;
    public GameObject oxyToggleObject;
    public GameObject commToggleObject;
    public GameObject fanToggleObject;
    public GameObject pumpToggleObject;
    public GameObject co2ToggleObject;
    
    public TextMeshPro UMB;
    public TextMeshPro LOCAL;
    public TextMeshPro OXY_PRI;
    public TextMeshPro OXY_SEC;
    public TextMeshPro COMM_A;
    public TextMeshPro COMM_B;
    public TextMeshPro FAN_PRI;
    public TextMeshPro FAN_SEC;
    public TextMeshPro OPEN;
    public TextMeshPro CLOSE;
    public TextMeshPro CO2_A;
    public TextMeshPro CO2_B;

    string battState;
    string oxyState;
    string commState;
    string fanState;
    string pumpState;
    string co2State;

    // Start is called before the first frame update
    void Start()
    {
        print("starting DCUScript");
        print(battToggleObject.name);
        print(oxyToggleObject.name);
        print(commToggleObject.name);
        print(fanToggleObject.name);
        print(pumpToggleObject.name);
        print(co2ToggleObject.name); 
        print("DCUScript started");
        battState = "UMB";
        oxyState = "PRI";
        commState = "A";
        fanState = "PRI";
        pumpState = "OPEN";
        co2State = "A";
    }

    // Update is called once per frame
    void Update()
    {
        //battState
        if (battState == "UMB"){
            battToggleObject.transform.eulerAngles = new Vector3(0, 0, 180);
            UMB.GetComponent<TextMeshPro>().color = Color.white;
            LOCAL.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray
        }
        else if (battState == "LOCAL"){
            battToggleObject.transform.eulerAngles = new Vector3(0, 0, 0);
            UMB.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray
            LOCAL.GetComponent<TextMeshPro>().color = Color.white;
        }
        
        //oxyState
        if (oxyState == "PRI"){
            oxyToggleObject.transform.eulerAngles = new Vector3(0, 0, 180);
            OXY_PRI.GetComponent<TextMeshPro>().color = Color.white;
            OXY_SEC.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray

        }
        else if (oxyState == "SEC"){
            oxyToggleObject.transform.eulerAngles = new Vector3(0, 0, 0);
            OXY_PRI.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray
            OXY_SEC.GetComponent<TextMeshPro>().color = Color.white;
        }
        
        //commState
        if (commState == "A"){
            commToggleObject.transform.eulerAngles = new Vector3(0, 0, 180);
            COMM_A.GetComponent<TextMeshPro>().color = Color.white;
            COMM_B.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray
        }
        else if (commState == "B"){
            commToggleObject.transform.eulerAngles = new Vector3(0, 0, 0);
            COMM_A.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray
            COMM_B.GetComponent<TextMeshPro>().color = Color.white;
        }

        //fanState
        if (fanState == "PRI"){
            fanToggleObject.transform.eulerAngles = new Vector3(0, 0, 180);
            FAN_PRI.GetComponent<TextMeshPro>().color = Color.white;
            FAN_SEC.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray
        }
        else if (fanState == "SEC"){
            fanToggleObject.transform.eulerAngles = new Vector3(0, 0, 0);
            FAN_PRI.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray
            FAN_SEC.GetComponent<TextMeshPro>().color = Color.white;
        }
        
        //pumpState
        if (pumpState == "OPEN"){
            pumpToggleObject.transform.eulerAngles = new Vector3(0, 0, 180);
            OPEN.GetComponent<TextMeshPro>().color = Color.white;
            CLOSE.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray
        }
        else if (pumpState == "CLOSE"){
            pumpToggleObject.transform.eulerAngles = new Vector3(0, 0, 0);
            OPEN.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray
            CLOSE.GetComponent<TextMeshPro>().color = Color.white;
        }
        
        //co2State
        if (co2State == "A"){
            co2ToggleObject.transform.eulerAngles = new Vector3(0, 0, 180);
            CO2_A.GetComponent<TextMeshPro>().color = Color.white;
            CO2_B.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray
        }
        else if (co2State == "B"){
            co2ToggleObject.transform.eulerAngles = new Vector3(0, 0, 0);
            CO2_A.GetComponent<TextMeshPro>().color = new Color(0.15f, 0.15f, 0.15f); // Even darker gray
            CO2_B.GetComponent<TextMeshPro>().color = Color.white;
        }
    }
}
