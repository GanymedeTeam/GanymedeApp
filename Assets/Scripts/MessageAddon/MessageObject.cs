using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageObject : MonoBehaviour
{
    public TMP_Text messageNameText;
    public string message;
    public string messageName;
    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = message;
    }

    public void Initialize(string messageName, string message)
    {
        this.messageName = messageName;
        this.message = message;
        messageNameText.text = messageName;
    }

    public void RemoveMessage()
    {
        MessageMenu messageMenu = FindObjectOfType<MessageMenu>();
        messageMenu.messageData.Remove(messageMenu.messageData.Find(x => x.message == message));
        FileHandler.SaveToJSON(messageMenu.messageData, "message/message.json");
        Destroy(gameObject);
    }

}