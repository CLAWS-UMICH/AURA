using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessagingBackend : MonoBehaviour
{
    Messaging msgList;
    List<Message> allMessage = new List<Message>();
    List<Message> AstroChat = new List<Message>();
    List<Message> LMCCChat = new List<Message>();
    List<Message> GroupChat = new List<Message>();

    [SerializeField]private GameObject messageObject;
    [SerializeField]private GameObject LMCCgc;
    [SerializeField]private GameObject A2gc;
    [SerializeField]private GameObject A2andLMCCgc;
    [SerializeField]private Sprite thumbsUp;
    [SerializeField]private Sprite thumbsDown;
    [SerializeField]private Sprite warning;
    [SerializeField]private GameObject notificationPanel;
    [SerializeField]private TextMeshProUGUI notificationText;
    [SerializeField] private GameObject chatWindow;
    private string messageText;
    

    private Subscription<MessagesAddedEvent> messageAddedEvent;
    private Subscription<MessageSentEvent> messageSentEvent;
    private Subscription<MessageReactionEvent> messageReactionEvent;
    private Subscription<MessageNotificationEvent> messageNotifEvent;
    private WebSocketClient webSocketClient;
    void Start()
    {
        msgList = new Messaging();
        allMessage = msgList.AllMessages;

        messageAddedEvent = EventBus.Subscribe<MessagesAddedEvent>(appendList);
        messageSentEvent = EventBus.Subscribe<MessageSentEvent>(sendMessage);
        messageReactionEvent = EventBus.Subscribe<MessageReactionEvent>(sendReaction);
        messageNotifEvent = EventBus.Subscribe<MessageNotificationEvent>(onNewMessageReceived);

        InitializeWebConnection();
    }

    void onNewMessageReceived(MessageNotificationEvent e)
    {
        if (e.NewAddedMessages.Count > 0)
        {
            notificationText.text = "New message received!";    
            notificationPanel.SetActive(true);
        }
    }

    public class MessageNotificationEvent
    {
        public List<Message> NewAddedMessages { get; private set; }

        public MessageNotificationEvent(List<Message> newMessages)
        {
            NewAddedMessages = newMessages;
        }
    }

    public void CloseNotification()
    {
        notificationPanel.SetActive(false);
    }

    public void CloseChatWindow()
    {
        chatWindow.SetActive(false);
    }

    private void  InitializeWebConnection()
    {
        GameObject controllerObject = GameObject.Find("MessagingController");
        if (controllerObject != null){
            webSocketClient = controllerObject.GetComponent<WebSocketClient>();
            if (webSocketClient != null){
                Debug.Log("Successfully connected to the existing WebSocketClient from Controller.");
            } else{
                Debug.LogWarning("WebSocketClient component not found on Controller.");
            }
        } else{
            Debug.LogError("Controller object not found in the scene.");
        }
    }


    void sendMessageButton() 
    {
        messageText = messageObject.transform.Find("Message").GetComponent<TextMeshPro>().text;
        int messageCount = allMessage.FindAll(m => m.from == (AstronautInstance.User.id + 1)).Count + 1;
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
        int messageCount = allMessage.FindAll(m => m.from == (AstronautInstance.User.id + 1)).Count + 1;
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


    void appendList(MessagesAddedEvent e)
    {
        foreach (Message m in e.NewAddedMessages)
        {
            allMessage.Add(m); // Add new messages instead of replacing the list

            //Astronaut1 = 1, Astronaut2 = 2, LMCC = 3, Group = 4
            if (m.sent_to == 4 && m.from != AstronautInstance.User.id)
            {
                GroupChat.Add(m);
            }
            else if (m.from == 3)
            {
                LMCCChat.Add(m);
            }
            else if (m.sent_to == AstronautInstance.User.id)
            {
                AstroChat.Add(m);
            }
        }
        EventBus.Publish(new MessageNotificationEvent(e.NewAddedMessages));
    }


    void sendMessage(MessageSentEvent e)
    {
        Message message = e.NewMadeMessage;
        string json = JsonUtility.ToJson(message);
        webSocketClient.SendJsonData(json, "MESSAGING");
        // Debug.Log(json);
    }


    void sendReaction(MessageReactionEvent e) 
    {
        Message reaction = e.NewReactionMessage;
        string json = JsonUtility.ToJson(reaction);
        webSocketClient.SendJsonData(json, "MESSAGING");
        // Debug.Log(json);
    }


    void OnDestroy()
    {
        EventBus.Unsubscribe(messageAddedEvent);
        EventBus.Unsubscribe(messageSentEvent);
        EventBus.Unsubscribe(messageReactionEvent);
        EventBus.Unsubscribe(messageNotifEvent);
    }
}
