using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrontEndButton : MonoBehaviour
{
    
    [SerializeField]private GameObject messageObject;
    [SerializeField]private GameObject LMCCgc;
    [SerializeField]private GameObject A2gc;
    [SerializeField]private GameObject A2andLMCCgc;
    [SerializeField]private Sprite thumbsUp;
    [SerializeField]private Sprite thumbsDown;
    [SerializeField]private Sprite warning;
    private string messageText;
    private MessagingBackend messagingBackend;

    void Start () 
    {
        LMCCgc.SetActive(false);
        A2andLMCCgc.SetActive(false);
        A2gc.SetActive(true);
    }

    void sendMessageButton() 
    {
        messageText = messageObject.transform.Find("Message").GetComponent<TextMeshPro>().text;
        int messageCount = messagingBackend.allMessage.FindAll(m => m.from == (AstronautInstance.User.id + 1)).Count + 1;
        Message newMessage;
        if (LMCCgc.activeSelf)
        {
            newMessage = new Message
            {
                message_id = messageCount,
                sent_to = 3,
                message = messageText,
                from = AstronautInstance.User.id + 1
            };
            EventBus.Publish(new MessageSentEvent(newMessage));
        }
        else if (A2gc.activeSelf)
        {
            if (AstronautInstance.User.id == 0) 
            {
                newMessage = new Message
                {
                    message_id = messageCount,
                    sent_to = 2,
                    message = messageText,
                    from = AstronautInstance.User.id + 1
                };
                EventBus.Publish(new MessageSentEvent(newMessage));
            }
            if (AstronautInstance.User.id == 1)
            {
                newMessage = new Message
                {
                    message_id = messageCount,
                    sent_to = 1,
                    message = messageText,
                    from = AstronautInstance.User.id + 1
                };
                EventBus.Publish(new MessageSentEvent(newMessage));
            }
        }
        else 
        {
            newMessage = new Message
            {
                message_id = messageCount,
                sent_to = 4,
                message = messageText,
                from = AstronautInstance.User.id + 1
            };
            EventBus.Publish(new MessageSentEvent(newMessage));
        }
    }


    void deleteButton()
    {
        messageText = messageObject.transform.Find("Message").GetComponent<TextMeshPro>().text;
        messageText = "";
    }


    void sendReaction(string JSONreaction)
    {
        string messageText = JSONreaction;
        int messageCount = messagingBackend.allMessage.FindAll(m => m.from == (AstronautInstance.User.id + 1)).Count + 1;
        Message newMessage;
        if (LMCCgc.activeSelf)
        {
            newMessage = new Message
            {
                message_id = messageCount,
                sent_to = 3,
                message = messageText,
                from = AstronautInstance.User.id + 1
            };
            EventBus.Publish(new MessageSentEvent(newMessage));
        }
        else if (A2gc.activeSelf)
        {
            if (AstronautInstance.User.id == 0) 
            {
                newMessage = new Message
                {
                    message_id = messageCount,
                    sent_to = 2,
                    message = messageText,
                    from = AstronautInstance.User.id + 1
                };
                EventBus.Publish(new MessageSentEvent(newMessage));
            }
            if (AstronautInstance.User.id == 1)
            {
                newMessage = new Message
                {
                    message_id = messageCount,
                    sent_to = 1,
                    message = messageText,
                    from = AstronautInstance.User.id + 1
                };
                EventBus.Publish(new MessageSentEvent(newMessage));
            }
        }
        else 
        {
            newMessage = new Message
            {
                message_id = messageCount,
                sent_to = 4,
                message = messageText,
                from = AstronautInstance.User.id + 1
            };
            EventBus.Publish(new MessageSentEvent(newMessage));
        }
    }


    void sendThumbsUp()
    {
        ImageMessage encodedImage = new ImageMessage(thumbsUp);
        string jsonString = JsonUtility.ToJson(encodedImage);
        sendReaction(jsonString);
    }


    void sendThumbsDown()
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


    void lmccGC() 
    {
        LMCCgc.SetActive(true);
        A2andLMCCgc.SetActive(false);
        A2gc.SetActive(false);
    }


    void a2GC() 
    {
        LMCCgc.SetActive(false);
        A2andLMCCgc.SetActive(false);
        A2gc.SetActive(true);
    }


    void a2andlmccGC()
    {
        LMCCgc.SetActive(false);
        A2andLMCCgc.SetActive(true);
        A2gc.SetActive(false);
    }


    // void upButton() 
    // {
    //     // call scroll handler
    // }


    // void downButton()
    // {
    //     // call scroll handler
    // }

}




// <prefab>.transform.Find("CompressableButtonVisuals".Find("IconAndText").Find("Message").GetComponent<TextMeshPro>().text;