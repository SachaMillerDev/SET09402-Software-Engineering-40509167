using System.Collections.Generic;

public class MessageProcessor
{
    public List<Message> Messages { get; set; } = new List<Message>();

    public void ProcessMessages()
    {
        foreach (Message message in Messages)
        {
            message.Process();
        }
    }

    public void AddMessage(Message message)
    {
        Messages.Add(message);
    }
}
