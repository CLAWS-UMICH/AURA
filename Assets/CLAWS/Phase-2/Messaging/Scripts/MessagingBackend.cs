using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation.VisualScripting;

public class MessagingBackend : MonoBehaviour
{
    Messaging msgList;
    public List<Message> allMessage = new List<Message>();
    List<Message> AstroChat = new List<Message>();
    List<Message> LMCCChat = new List<Message>();
    List<Message> GroupChat = new List<Message>();
    private Subscription<MessagesAddedEvent> messageAddedEvent;
    private Subscription<MessageSentEvent> messageSentEvent;
    private Subscription<MessageReactionEvent> messageReactionEvent;
    private WebSocketClient webSocketClient;


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
