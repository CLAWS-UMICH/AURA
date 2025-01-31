using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicMessagingPop : MonoBehaviour
{
    private Subscription<MessagesAddedEvent> messageAddedEvent;
    public GameObject[] prefabs; //the three different prefabs from where the message came from. self, other astronaut, LMCC (ORDER SENSITIVE!!)
    private List<Message> currentList; //the current list of message objects that need to be displayed (currently in use by the astronaut)
    private List<GameObject> clones = new List<GameObject>(); //clones of the prefab in a list--the clones turn the currentList into the right prefabs
    private int me; //your astronaut ID (necessary for switch case)
    private int them; //the other astronaut's ID

    void Start()
    {
        messageAddedEvent = EventBus.Subscribe<MessagesAddedEvent>(appendList); //list of prefabs to be displayed regenerated every time theres new message

        if (AstronautInstance.User.id == 1) { /*assigning the right ID's. messages from you will be displayed from the right side and use a different
        prefab, so it's important to discern who you are. prefabs will then be generated as sent by ME (you), and will always be on the right*/
            me = 1;
            them = 2;
        } else {
            them = 1;
            me = 2;
        }
    }

    void appendList() { //making the current list whatever the current active screen is
        if (LMCC.active == true) {
            currentList = LMCCChat;
        } else if (A2.active == true) {
            currentList = AstroChat;
        } else {
            currentList = GroupChat;
        }

        for (int i = 0; i < currentList.Count; i++) { //unoptimal but functioning, can be cleaned up by whoever later if necessary
            if (currentList[i].message_id <= 2 && currentList[i].message_id == me) { //"me" can be either 1 or 2 so the switch case checks if it's you
                GameObject clone = Instantiate(prefabs[0]); //prefab 0, the "you" prefab, will be instantiated by 1 or 2 but ultimately you
            } else if (currentList[i].message_id <= 2) {
                GameObject clone = Instantiate(prefabs[1]);
            } else {
                GameObject clone = Instantiate(prefabs[2]);
            }

            clones.Add(clone);
        }

        for (int j = 0; j < clones.Count; j++) { //iterate through clones editing them
            clones[j].transform.Find("CompressableButtonVisuals".Find("IconAndText").Find("Message").GetComponent<TextMeshPro>().text = currentList[j].message;
            //essentially, the message box of clones[j] prefab = currentList[j] json string data
        }
    }
}
