using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrontEndButton : MonoBehaviour
{
    
    [SerializeField]private GameObject messageObject;
    [SerializeField]private GameObject PRgc;
    [SerializeField]private GameObject A2gc;
    [SerializeField]private GameObject A2andPRgc;
    [SerializeField]private Sprite thumbsUp;
    [SerializeField]private Sprite thumbsDown;
    [SerializeField]private Sprite warning;
    private string messageText;
    public MessagingBackend messagingBackend;

    void Start () 
    {
        PRgc.SetActive(true);
        A2andPRgc.SetActive(false);
        A2gc.SetActive(false);
    }

    void Update()
    {
        // Debug.Log("astro count: " + messagingBackend.AstroChat.Count);
        // Debug.Log("lmcc count: " + messagingBackend.LMCCChat.Count);
        // Debug.Log("group count: " + messagingBackend.GroupChat.Count);
        // Debug.Log("astro seen: " + messagingBackend.a2MessagesSeen);
        // Debug.Log("lmcc seen: " + messagingBackend.lmccMessagesSeen);
        // Debug.Log("group seen: " + messagingBackend.gcMessagesSeen);
        if (messagingBackend.allMessage.Count - messagingBackend.totalMessagesSeen > 0) 
        {
            messagingBackend.chat.SetActive(true);
        }
        if (messagingBackend.allMessage.Count - messagingBackend.totalMessagesSeen == 0) 
        {
            messagingBackend.chat.SetActive(false);
        }
        
        if (messagingBackend.AstroChat.Count > 0 || messagingBackend.a2MessagesSeen > 0) 
        {
            messagingBackend.a2chat.SetActive(true);
        }
        if (messagingBackend.AstroChat.Count == 0 || messagingBackend.a2MessagesSeen == 0) 
        {
            messagingBackend.a2chat.SetActive(false);
        }
        
        if (messagingBackend.LMCCChat.Count > 0 || messagingBackend.lmccMessagesSeen > 0) 
        {
            messagingBackend.lmccChat.SetActive(true);
        }
        if (messagingBackend.LMCCChat.Count == 0 || messagingBackend.lmccMessagesSeen == 0) 
        {
            messagingBackend.lmccChat.SetActive(false);
        }

        if (messagingBackend.GroupChat.Count > 0 || messagingBackend.gcMessagesSeen > 0) 
        {
            messagingBackend.gcChat.SetActive(true);
        }
        if (messagingBackend.GroupChat.Count == 0 || messagingBackend.gcMessagesSeen == 0) 
        {
            messagingBackend.gcChat.SetActive(false);
        }
    }

    public void sendMessageButton() 
    {
        messageText = messageObject.transform.Find("Message").GetComponent<TextMeshPro>().text;
        int messageCount = messagingBackend.allMessage.FindAll(m => m.from == AstronautInstance.User.id).Count + 1;
        Message newMessage;
        if (PRgc.activeSelf)
        {
            if (AstronautInstance.User.id == 1) 
            {
                newMessage = new Message
                {
                    message_id = messageCount,
                    sent_to = 2,
                    message = messageText,
                    from = AstronautInstance.User.id
                };
                Debug.Log(newMessage);
                EventBus.Publish(new MessageSentEvent(newMessage));
                EventBus.Publish(new MessagesAddedEvent(new List<Message> { newMessage }));
            }
            if (AstronautInstance.User.id == 2)
            {
                newMessage = new Message
                {
                    message_id = messageCount,
                    sent_to = 1,
                    message = messageText,
                    from = AstronautInstance.User.id
                };
                Debug.Log(newMessage);
                EventBus.Publish(new MessageSentEvent(newMessage));
                EventBus.Publish(new MessagesAddedEvent(new List<Message> { newMessage }));
            }
        }
        else if (A2gc.activeSelf)
        {
            newMessage = new Message
            {
                message_id = messageCount,
                sent_to = 4,
                message = messageText,
                from = AstronautInstance.User.id
            };
            Debug.Log(newMessage);
            EventBus.Publish(new MessageSentEvent(newMessage));
            EventBus.Publish(new MessagesAddedEvent(new List<Message> { newMessage }));
        }
        else 
        {
            Debug.Log("entered");
            newMessage = new Message
            {
                message_id = messageCount,
                sent_to = 3,
                message = messageText,
                from = AstronautInstance.User.id
            };
            EventBus.Publish(new MessageSentEvent(newMessage));
            EventBus.Publish(new MessagesAddedEvent(new List<Message> { newMessage }));
        }
    }


    public void deleteButton()
    {
        messageText = messageObject.transform.Find("Message").GetComponent<TextMeshPro>().text;
        messageText = "";
    }


    public void sendReaction(string JSONreaction)
    {
        string messageText = JSONreaction;
        int messageCount = messagingBackend.allMessage.FindAll(m => m.from == (AstronautInstance.User.id + 1)).Count + 1;
        Message newMessage;
        if (PRgc.activeSelf)
        {
            newMessage = new Message
            {
                message_id = messageCount,
                sent_to = 3,
                message = messageText,
                from = AstronautInstance.User.id
            };
            EventBus.Publish(new MessageSentEvent(newMessage));
        }
        else if (A2gc.activeSelf)
        {
            newMessage = new Message
            {
                message_id = messageCount,
                sent_to = 4,
                message = messageText,
                from = AstronautInstance.User.id
            };
            EventBus.Publish(new MessageSentEvent(newMessage));
        }
        else 
        {
            if (AstronautInstance.User.id == 1) 
            {
                newMessage = new Message
                {
                    message_id = messageCount,
                    sent_to = 2,
                    message = messageText,
                    from = AstronautInstance.User.id
                };
                EventBus.Publish(new MessageSentEvent(newMessage));
            }
            if (AstronautInstance.User.id == 2)
            {
                newMessage = new Message
                {
                    message_id = messageCount,
                    sent_to = 1,
                    message = messageText,
                    from = AstronautInstance.User.id
                };
                EventBus.Publish(new MessageSentEvent(newMessage));
            }
        }
    }


    public void sendThumbsUp()
    {
        ImageMessage encodedImage = new ImageMessage(thumbsUp);
        string jsonString = JsonUtility.ToJson(encodedImage);
        sendReaction(jsonString);
    }


    public void sendThumbsDown()
    {
        ImageMessage encodedImage = new ImageMessage(thumbsDown);
        string jsonString = JsonUtility.ToJson(encodedImage);
        sendReaction(jsonString);
    }


    void sendWarning()
    {
        ImageMessage encodedImage = new ImageMessage(warning);
        string jsonString = JsonUtility.ToJson(encodedImage);
        sendReaction(jsonString);
    }


    public void switchPRgc() 
    {
        messagingBackend.lmccMessagesSeen = messagingBackend.LMCCChat.Count;
        PRgc.SetActive(true);
        A2andPRgc.SetActive(false);
        A2gc.SetActive(false);
    }


    public void a2GC() 
    {
        messagingBackend.a2MessagesSeen = messagingBackend.AstroChat.Count;
        Debug.Log(messagingBackend.a2MessagesSeen);
        PRgc.SetActive(false);
        A2andPRgc.SetActive(false);
        A2gc.SetActive(true);
    }


    public void a2andPRgc()
    {
        messagingBackend.gcMessagesSeen = messagingBackend.GroupChat.Count;
        PRgc.SetActive(false);
        A2andPRgc.SetActive(true);
        A2gc.SetActive(false);
    }


    public void closeFeature()
    {
        PRgc.SetActive(false);
        A2andPRgc.SetActive(false);
        A2gc.SetActive(false);
        Debug.Log("{tranform} " + transform);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }


    public void openFeatureScreen()
    {
        // First activate the main messaging screen
        transform.gameObject.SetActive(true);
        
        // Reset all chat views
        PRgc.SetActive(false);
        A2andPRgc.SetActive(false);
        A2gc.SetActive(false);
        
        // Activate the default chat view (A2gc)
        A2gc.SetActive(true);
        
        // Activate all child objects
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        
        // Update the seen messages count for the default view
        messagingBackend.a2MessagesSeen = messagingBackend.AstroChat.Count;
    }
}




// <prefab>.transform.Find("CompressableButtonVisuals".Find("IconAndText").Find("Message").GetComponent<TextMeshPro>().text;