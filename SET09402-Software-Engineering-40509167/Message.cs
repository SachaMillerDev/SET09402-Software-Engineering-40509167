public abstract class Message
{
    public string MessageID { get; set; }
    public string Body { get; set; }

    public abstract string DetectType();
    public abstract void Process();
}
