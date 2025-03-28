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
    public List<Transform> clones = new List<Transform>(); //clones of the prefab in a list--the clones turn the currentList into the right prefabs
    public float LastCloneY;
    private int me; //your astronaut ID (necessary for switch case)
    private int them; //the other astronaut's ID
    public MessagingScrollHandler scrollHandler; 

    public GameObject LMCC; //the LMCC chat screen
    public GameObject A2; //the other astronaut's chat screen
    [SerializeField] private Renderer BoundsRenderer; // Renderer to define the bounds

    void Start()
    {
        Debug.Log("DynamicMessagingPop script started");
        messagesAppendedEvent = EventBus.Subscribe<MessagesAppendedEvent>(appendList);
        LastCloneY = 0.00659999996f;

        if (AstronautInstance.User.id == 0) { /*assigning the right ID's. messages from you will be displayed from the right side and use a different
        prefab, so it's important to discern who you are. prefabs will then be generated as sent by ME (you), and will always be on the right*/
            me = 1;
            them = 2;
        } else {
            them = 1;
            me = 2;
        }
    }

    public void appendList(MessagesAppendedEvent e)
    {
        if (LMCC.activeSelf == true)
        {
            currentList = MessagingBackend.LMCCChat;
        }
        else if (A2.activeSelf == true)
        {
            currentList = MessagingBackend.AstroChat;
        }
        else
        {
            currentList = MessagingBackend.GroupChat;
        }
        Debug.Log(currentList.Count);

        // clear prev chat clones
        foreach (Transform clone in clones)
        {
            Destroy(clone.gameObject);
        }
        clones.Clear();

        // bottom bound of qaud window
        float bottomBound = -0.041f;

        // y pos of last clone
        float currentYPosition = 0.00659999996f;
        float yPlus = 0.0185992f; // height of prefabs (difference)
        float chatPlus = 0.0184f; // height of chat window

        // instantiate clones under current
        for (int i = 0; i < currentList.Count; i++)
        {
            Transform clone;
            if (currentYPosition < bottomBound)
            {
                Debug.Log(chatPlus);
                // Move the parent GameObject up by the increment of one prefab (did math for prettiness rather than)
                transform.localPosition = new Vector3(0, chatPlus, 0);
                chatPlus += 0.0184f;
            }
            if (currentList[i].from == me)
            {
                clone = Instantiate(prefabs[0], transform).transform; // A1 = 1;
                clone.localPosition = new Vector3(0.121200003f, currentYPosition, 0.00270000007f);
            }
            else if (currentList[i].from <= 2 && currentList[i].from != me)
            {
                clone = Instantiate(prefabs[1], transform).transform; // A2 = 2
                clone.localPosition = new Vector3(0.0868000016f, currentYPosition, 0.00270000007f);
            }
            else
            {
                clone = Instantiate(prefabs[2], transform).transform; // LMCC = 3
                clone.localPosition = new Vector3(0.0868000016f, currentYPosition, 0.00270000007f);
            }

            // Add the clone to the list
            clones.Add(clone);

            // Increment the Y position for the next prefab
            // Check if the next prefab will exceed the bottom bound
            currentYPosition -= yPlus;
            Debug.Log(currentYPosition);
            Debug.Log(bottomBound);
        }

        // Update the text of each clone
        for (int j = 0; j < clones.Count; j++)
        {
            clones[j].transform.Find("CompressableButtonVisuals").Find("IconAndText").Find("Time").GetComponent<TextMeshPro>().text = System.DateTime.Now.ToString("hh:mm tt");
            clones[j].transform.Find("CompressableButtonVisuals").Find("IconAndText").Find("Message").GetComponent<TextMeshPro>().text = currentList[j].message;
            clones[j].gameObject.SetActive(true);
        }
    }
}