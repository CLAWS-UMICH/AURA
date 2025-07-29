using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class MessagingBackend : MonoBehaviour
{
    Messaging msgList;
    public List<Message> allMessage = new List<Message>();
    public GameObject chat;
    public int totalMessagesSeen = 0;
    public List<Message> AstroChat = new List<Message>();
    public GameObject a2chat;
    public int a2MessagesSeen = 0;
    public List<Message> LMCCChat = new List<Message>();
    public GameObject lmccChat;
    public int lmccMessagesSeen = 0;
    public List<Message> GroupChat = new List<Message>();
    public GameObject gcChat;
    public int gcMessagesSeen = 0;

    [SerializeField] private GameObject messageObject;
    [SerializeField] public GameObject LMCCgc;
    [SerializeField] private GameObject A2gc;
    [SerializeField] private GameObject A2andLMCCgc;
    [SerializeField] private Sprite thumbsUp;
    [SerializeField] private Sprite thumbsDown;
    [SerializeField] private Sprite warning;

    [SerializeField] private GameObject fakeMessage;
    private string messageText;


    private Subscription<MessagesAddedEvent> messageAddedEvent;
    private Subscription<MessageSentEvent> messageSentEvent;
    private Subscription<MessageReactionEvent> messageReactionEvent;
    [SerializeField] private LMCCWebSocketClient webSocketClient;
    [SerializeField] private GameObject controllerObject;
    void Start()
    {
        msgList = new Messaging();
        allMessage = msgList.AllMessages;

        messageAddedEvent = EventBus.Subscribe<MessagesAddedEvent>(appendList);
        messageSentEvent = EventBus.Subscribe<MessageSentEvent>(sendMessage);
        messageReactionEvent = EventBus.Subscribe<MessageReactionEvent>(sendReaction);

        InitializeWebConnection();
    }

    public void fakeSendMessage()
    {
        fakeMessage.SetActive(true);
    }


    private void InitializeWebConnection()
    {

        if (webSocketClient != null)
        {
            Debug.Log("Successfully connected to the existing WebSocketClient from Controller.");
        }
        else
        {
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
            totalMessagesSeen = a2MessagesSeen + lmccMessagesSeen + gcMessagesSeen;
            if (allMessage.Count - totalMessagesSeen == 1)
            {
                chat.GetComponent<TextMeshPro>().text = (allMessage.Count - totalMessagesSeen).ToString() + " pending chat";
            }
            else
            {
                chat.GetComponent<TextMeshPro>().text = (allMessage.Count - totalMessagesSeen).ToString() + " pending chats";
            }

            //Astronaut1 = 1, Astronaut2 = 2, GC = 3, PR = 4

            if ((m.from == 4 && m.sent_to != 3) || (m.from == 1 && m.sent_to == 4) || (m.from == 2 && m.sent_to == 4))
            {
                AstroChat.Add(m);
                a2chat.transform.Find("TextMeshPro").GetComponent<TextMeshPro>().text = (AstroChat.Count - a2MessagesSeen).ToString();
            }
            else if ((m.sent_to == 3))
            {
                Debug.Log("GC ADDED");
                GroupChat.Add(m);
                gcChat.transform.Find("TextMeshPro").GetComponent<TextMeshPro>().text = (GroupChat.Count - gcMessagesSeen).ToString();
            }
            else if ((m.from == 1 && m.sent_to == 2) || (m.from == 2 && m.sent_to == 1))
            {
                LMCCChat.Add(m);
                lmccChat.transform.Find("TextMeshPro").GetComponent<TextMeshPro>().text = (LMCCChat.Count - lmccMessagesSeen).ToString();
            }

        }
        Debug.Log("Publishing MessagesAppendedEvent...");
        EventBus.Publish(new MessagesAppendedEvent());
    }


    void sendMessage(MessageSentEvent e)
    {
        var jsonData = new Dictionary<string, object>
        {
            { "message_id", e.NewMadeMessage.message_id},
            { "sent_to", e.NewMadeMessage.sent_to },
            { "message", e.NewMadeMessage.message },
            { "from", e.NewMadeMessage.from },
        };
        webSocketClient.SendJsonData(jsonData, "MESSAGING", 4);
    }


    void sendReaction(MessageReactionEvent e)
    {
        Message reaction = e.NewReactionMessage;
        string json = JsonUtility.ToJson(reaction);
        // webSocketClient.SendJsonData(json, "MESSAGING");
        // Debug.Log(json);
    }


    void OnDestroy()
    {
        EventBus.Unsubscribe(messageAddedEvent);
        EventBus.Unsubscribe(messageSentEvent);
        EventBus.Unsubscribe(messageReactionEvent);
    }
}