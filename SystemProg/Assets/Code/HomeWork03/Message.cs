public struct Message
{
    internal string _text;
    internal MessageType _messageType;

    public Message(string text, MessageType messageType)
    {
        _text = text;
        _messageType = messageType;
    }

    public void Clear()
    {
        _text = "";
    }

}
