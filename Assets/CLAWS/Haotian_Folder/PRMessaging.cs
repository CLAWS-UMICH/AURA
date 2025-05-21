using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PRMessaging : MonoBehaviour
{
    [SerializeField]private GameObject toggleObject; 
    [SerializeField]private LMCCWebSocketClient webSocketClient; // Reference to the web socket client

    private Subscription<MessageSentEvent> messageEvent;
    private Message currentMessage;
    private bool isSubscribed = false;

    // Start is called before the first frame update
    void Start()
    {
        messageEvent = EventBus.Subscribe<MessageSentEvent>(OnMessageReceived);
        
        // Find the web socket client if not assigned
        if (webSocketClient == null)
        {
            webSocketClient = FindObjectOfType<LMCCWebSocketClient>();
        }
    }

    public void ToggleMessageSubscription()
    {
        if (webSocketClient == null)
        {
            Debug.LogError("WebSocketClient reference is missing!");
            return;
        }

        if (currentMessage == null)
        {
            Debug.LogWarning("No message data available to toggle");
            return;
        }

        // Create dictionary with message data
        Dictionary<string, object> messageData = new Dictionary<string, object>
        {
            { "message", currentMessage.message },
            { "from", currentMessage.from },
            { "message_id", currentMessage.message_id },
            { "sent_to", currentMessage.sent_to },
            { "use", "SEND" }  
        };

        webSocketClient.SendJsonData(messageData, "MESSAGING", 3); // 3 is for PR client
    }

    private void OnMessageReceived(MessageSentEvent evt)
    {
        currentMessage = evt.NewMadeMessage;
        Debug.Log($"Received message");
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(messageEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
