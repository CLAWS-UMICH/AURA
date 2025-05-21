using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//script added to MessagingController Object
public class DynamicMessagingPop : MonoBehaviour
{
    private Subscription<MessagesAppendedEvent> messagesAppendedEvent;

    public MessagingBackend MessagingBackend;
    public GameObject[] prefabs; //the three different prefabs from where the message came from. self, other astronaut, LMCC (ORDER SENSITIVE!!)
    private List<Message> currentList; //the current list of message objects that need to be displayed (currently in use by the astronaut)
    public List<Transform> lmccClones = new List<Transform>();
    public List<Transform> a2Clones = new List<Transform>();
    public List<Transform> gcClones = new List<Transform>();  // clones of the prefab in a list
    public float LastCloneY;
    private int me; //your astronaut ID (necessary for switch case)
    private int them; //the other astronaut's ID
    public MessagingScrollHandler scrollHandler; 

    public GameObject LMCC; //the LMCC chat screen
        public GameObject A2; //the other astronaut's chat screen
    public GameObject GC;
        [SerializeField] private Renderer BoundsRenderer; // Renderer to define the bounds

    void Start()
    {
        Debug.Log("DynamicMessagingPop script started");
        messagesAppendedEvent = EventBus.Subscribe<MessagesAppendedEvent>(appendList);
        LastCloneY = 0.00659999996f;
        Debug.Log("id: " + AstronautInstance.User.id);
        if (AstronautInstance.User.id == 1) { /*assigning the right ID's. messages from you will be displayed from the right side and use a different
        prefab, so it's important to discern who you are. prefabs will then be generated as sent by ME (you), and will always be on the right*/
            me = 1;
            them = 2;
        } 
        else {
            them = 1;
            me = 2;
        }
    }


    private void PopulateChat(List<Message> messages, Transform parent, List<Transform> cloneList)
    {
        // Clear old clones
        foreach (Transform clone in cloneList)
        {
            Destroy(clone.gameObject);
        }
        cloneList.Clear();

        float bottomBound = -0.041f;
        float currentYPosition = 0.00659999996f;
        float yPlus = 0.0185992f;
        float chatPlus = 0.0184f;

        for (int i = 0; i < messages.Count; i++)
        {
            Transform clone;
            if (currentYPosition < bottomBound)
            {
                parent.localPosition = new Vector3(0, chatPlus, 0);
                chatPlus += 0.0184f;
            }
            Debug.Log("me:" + me);
            Debug.Log("From:" + messages[i].from);
            if (messages[i].from == me)
            {
                Debug.Log("Instantiating in parent: " + parent.name);
                clone = Instantiate(prefabs[0], parent).transform;
                clone.localPosition = new Vector3(0.121200003f, currentYPosition, 0.00270000007f);
            }
            else if (messages[i].from <= 2 && messages[i].from != me)
            {
                Debug.Log("Instantiating in parent: " + parent.name);
                clone = Instantiate(prefabs[1], parent).transform;
                clone.localPosition = new Vector3(0.0868000016f, currentYPosition, 0.00270000007f);
            }
            else
            {
                Debug.Log("Instantiating in parent: " + parent.name);
                clone = Instantiate(prefabs[2], parent).transform;
                clone.localPosition = new Vector3(0.0868000016f, currentYPosition, 0.00270000007f);
            }

            cloneList.Add(clone);
            currentYPosition -= yPlus;
        }

        for (int j = 0; j < cloneList.Count; j++)
        {
            cloneList[j].transform.Find("CompressableButtonVisuals").Find("IconAndText").Find("Time").GetComponent<TextMeshPro>().text = System.DateTime.Now.ToString("hh:mm tt");
            cloneList[j].transform.Find("CompressableButtonVisuals").Find("IconAndText").Find("Message").GetComponent<TextMeshPro>().text = messages[j].message;
            cloneList[j].gameObject.SetActive(true);
        }
    }


    public void appendList(MessagesAppendedEvent e)
    {
        Debug.Log("Appending chats for all panels");
        if (AstronautInstance.User.id == 1) { /*assigning the right ID's. messages from you will be displayed from the right side and use a different
        prefab, so it's important to discern who you are. prefabs will then be generated as sent by ME (you), and will always be on the right*/
            me = 1;
            them = 2;
        } 
        else {
            them = 1;
            me = 2;
        }
        // Update all 3 screens
        PopulateChat(MessagingBackend.LMCCChat, LMCC.transform, lmccClones);
        PopulateChat(MessagingBackend.AstroChat, A2.transform, a2Clones);
        PopulateChat(MessagingBackend.GroupChat, GC.transform, gcClones);
    }
}