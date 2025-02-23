using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//script added to MessagingController Object
public class DynamicMessagingPop : MonoBehaviour
{
    private Subscription<MessagesAddedEvent> messageAddedEvent;
    public GameObject[] prefabs; //the three different prefabs from where the message came from. self, other astronaut, LMCC (ORDER SENSITIVE!!)
    private List<Message> currentList; //the current list of message objects that need to be displayed (currently in use by the astronaut)
    public List<Transform> clones = new List<Transform>(); //clones of the prefab in a list--the clones turn the currentList into the right prefabs
    private int me; //your astronaut ID (necessary for switch case)
    private int them; //the other astronaut's ID

    public GameObject LMCC; //the LMCC chat screen
    public GameObject A2; //the other astronaut's chat screen

    public List<Message> LMCCChat; //the LMCC chat list
    public List<Message> AstroChat; //the other astronaut's chat list
    public List<Message> GroupChat; //the group chat list

    void Start()
    {
        messageAddedEvent = EventBus.Subscribe<MessagesAddedEvent>(e => appendList()); //list of prefabs to be displayed regenerated every time theres new message

        if (AstronautInstance.User.id == 1) { /*assigning the right ID's. messages from you will be displayed from the right side and use a different
        prefab, so it's important to discern who you are. prefabs will then be generated as sent by ME (you), and will always be on the right*/
            me = 1;
            them = 2;
        } else {
            them = 1;
            me = 2;
        }
    }

    public void appendList() { //making the current list whatever the current active screen is
        if (LMCC.activeSelf == true) {
            currentList = LMCCChat;
        } else if (A2.activeSelf == true) {
            currentList = AstroChat;
        } else {
            currentList = GroupChat;
        }

        for (int i = 0; i < currentList.Count; i++) { //unoptimal but functioning, can be cleaned up by whoever later if necessary
            if (currentList[i].message_id <= 2 && currentList[i].message_id == me) { //"me" can be either 1 or 2 so the switch case checks if it's you
                Transform clone = Instantiate(prefabs[0]).transform; //prefab 0, the "you" prefab, will be instantiated by 1 or 2 but ultimately you
                clones.Add(clone); 
            } else if (currentList[i].message_id <= 2) {
                Transform clone = Instantiate(prefabs[1]).transform; //prefab 1, the "them" prefab, will be instantiated by 1 or 2 but ultimately the other astronaut
                clones.Add(clone);
            } else {
                Transform clone = Instantiate(prefabs[2]).transform;
                clones.Add(clone);
            }
        }

        for (int j = 0; j < clones.Count; j++) { //iterate through clones editing them
            clones[j].transform.Find("CompressableButtonVisuals").Find("IconAndText").Find("Message").GetComponent<TextMeshPro>().text = currentList[j].message;
            //essentially, the message box of clones[j] prefab = currentList[j] json string data
        }
    }
}
