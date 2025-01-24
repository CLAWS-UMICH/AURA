using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class MessagingBackend : MonoBehaviour
{
    List<Message> allMessage;
    List<Message> AstroChat;
    List<Message> LMCCChat;
    List<Message> GroupChat;
    FellowAstronaut fa;

    void Start{
        msgList = new Messaging();
        allMessage = msgList.AllMessages;
        AstroChat = new List<Message>();
        LMCCChat = new List<Message>();
        GroupChat = new List<Message>();

        fa = AstronautInstance.User.FellowAstronautsData;


        EventBus.Subscribe<MessagesAddedEvent>(appendList);

    }

     void appendList(MessagesAddedEvent e)
    {
        allMessage = e.NewAddedMessages;

        //Astrounaut1 = 1, Astronaut2 = 2, LMCC = 3, Group = 4

        //Need to check for self astronaut id

       foreach (Message m in allMessage)
       {
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


}