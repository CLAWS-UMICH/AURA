using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

 public class MessagingBackend : MonoBehaviour
 {
    Messaging msgList;
    public List<Message> allMessage = new List<Message>();
    public List<Message> AstroChat = new List<Message>();
    public List<Message> LMCCChat = new List<Message>();
    public List<Message> GroupChat = new List<Message>();

    [SerializeField]private GameObject messageObject;
    [SerializeField]public GameObject LMCCgc;
    [SerializeField]private GameObject A2gc;
    [SerializeField]private GameObject A2andLMCCgc;
    [SerializeField]private Sprite thumbsUp;
    [SerializeField]private Sprite thumbsDown;
    [SerializeField]private Sprite warning;
    private string messageText;
    

    private Subscription<MessagesAddedEvent> messageAddedEvent;
    private Subscription<MessageSentEvent> messageSentEvent;
    private Subscription<MessageReactionEvent> messageReactionEvent;
    [SerializeField]private WebSocketClient webSocketClient;
    [SerializeField]private GameObject controllerObject;
    void Start()
    {
        msgList = new Messaging();
        allMessage = msgList.AllMessages;

        messageAddedEvent = EventBus.Subscribe<MessagesAddedEvent>(appendList);
        messageSentEvent = EventBus.Subscribe<MessageSentEvent>(sendMessage);
        messageReactionEvent = EventBus.Subscribe<MessageReactionEvent>(sendReaction);

        InitializeWebConnection();
    }


    private void  InitializeWebConnection()
    {

        if (webSocketClient != null){
            Debug.Log("Successfully connected to the existing WebSocketClient from Controller.");
        } else{
            Debug.LogWarning("WebSocketClient component not found on Controller.");
        }
    }


    void appendList(MessagesAddedEvent e)
    {
        Debug.Log("recieved new messages");
        foreach (Message m in e.NewAddedMessages)
        {
            Debug.Log(m.message);
            Debug.Log(m.from);
            Debug.Log(m.sent_to);
            allMessage.Add(m); // Add new messages instead of replacing the list

            //Astronaut1 = 1, Astronaut2 = 2, LMCC = 3, Group = 4
            if (m.sent_to == 4)
            {
                GroupChat.Add(m);
            }
            else if (m.from == 3)
            {
                LMCCChat.Add(m);
            }
            else if ((m.sent_to == AstronautInstance.User.id + 1) || (m.from == AstronautInstance.User.id + 1))
            {
                Debug.Log("adding to AstroChat");
                AstroChat.Add(m);
            }
        }
        Debug.Log("Publishing MessagesAppendedEvent...");
        EventBus.Publish(new MessagesAppendedEvent());
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
    }
}
