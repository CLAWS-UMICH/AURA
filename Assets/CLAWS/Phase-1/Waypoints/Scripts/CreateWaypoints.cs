
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateWaypoint : MonoBehaviour
{
    GameObject createWaypointScreen;
    GameObject nameField;
    string name;
    string letter;
    int type;
    bool defaultName;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Fix dependencies to find the correct game object
        createWaypointScreen = transform.parent.Find("CreateWaypointScreen").gameObject;
        nameField = createWaypointScreen.transform.Find("NameField").gameObject.transform.Find("NameText").gameObject;
        createWaypointScreen.SetActive(false);
        type = -1;
        defaultName = true;
    }

    public void OpenCreateWaypointScreen()
    {
        defaultName = true;
        type = -1;
        // TODO: fix dependencies with the AstronautInsstance, either add the method or change the way we store WaypointData
        letter = getLetter(AstronautInstance.User.WaypointData.currentIndex);
        createWaypointScreen.transform.Find("Title").gameObject.GetComponent<TextMeshPro>().text = "Create Waypoint " + letter + ":";

        createWaypointScreen.SetActive(true);
    }

    private string getLetter(int num)
    {
        string result = "";

        while (num >= 0)
        {
            result = (char)('A' + num % 26) + result;
            num = (num / 26) - 1;

            if (num < 0)
            {
                break;
            }
        }

        return result;
    }

    public void SelectStation()
    {
        type = 0;
        if (defaultName)
        {
            name = "Station " + letter;
            nameField.GetComponent<TextMeshPro>().text = name;
        }
    }

    public void SelectInterest()
    {
        type = 1;
        if (defaultName)
        {
            name = "Interest " + letter;
            nameField.GetComponent<TextMeshPro>().text = name;
        }
    }

    public void SelectDanger()
    {
        type = 3;
        if (defaultName)
        {
            name = "Danger " + letter;
            nameField.GetComponent<TextMeshPro>().text = name;
        }
    }

    public void SelectGeo()
    {
        type = 2;
        if (defaultName)
        {
            name = "Geo " + letter;
            nameField.GetComponent<TextMeshPro>().text = name;
        }
    }

    public void CreateWay()
    {
        if (type != -1)
        {
            // TODO: Fix dependencies with the EventBus
            EventBus.Publish(new WaypointToAdd(AstronautInstance.User.location, type, name));
            CloseWaypointCreationScreen();
        } else
        {
            // TODO: If they haven't specified type yet ERROR check

        }

    }

    public void CloseWaypointCreationScreen()
    {
        createWaypointScreen.SetActive(false);
    }

}

