using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageMenu : MonoBehaviour
{

    public TMP_Text messageNameText;
    public TMP_Text messageText;
    public GameObject messageUIPositionPrefab;
    public GameObject gridGameobject;

    public List<MessageEntry> messageData = new List<MessageEntry>();
    // Start is called before the first frame update
    void Start()
    {
        messageData = FileHandler.ReadListFromJSON<MessageEntry>("message/message.json");

        foreach (MessageEntry MessageEntry in messageData)
        {
            GameObject newMessage = Instantiate(messageUIPositionPrefab, gridGameobject.transform);
            MessageObject messageObject = newMessage.GetComponent<MessageObject>();
            messageObject.Initialize(MessageEntry.messageName, MessageEntry.message);
        }
    }

    public void AddMessage()
    {
        if(messageNameText.text.Length > 1 && messageText.text.Length > 1)
        {
            GameObject newMessage = Instantiate(messageUIPositionPrefab, gridGameobject.transform);    
            MessageObject messageObject = newMessage.GetComponent<MessageObject>();    
            messageObject.Initialize(messageNameText.text, messageText.text);    
            messageData.Add(new MessageEntry(messageNameText.text,messageText.text));    
            messageNameText.text = "";
            messageText.text = "";
            FileHandler.SaveToJSON(messageData, "message/message.json");
        }
    }
}
