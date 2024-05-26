using System;

[Serializable]
public class MessageEntry
{
    public string messageName;
    public string message;
    public MessageEntry(string messageName, string message)
    {
        this.messageName = messageName;
        this.message = message;
    }
}
