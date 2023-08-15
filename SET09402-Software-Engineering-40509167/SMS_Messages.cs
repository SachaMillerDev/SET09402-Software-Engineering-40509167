public class SMSMessage : Message
{
    public string SenderPhoneNumber { get; set; }
    public string Text { get; set; }

    public override string DetectType() => "SMS";

    public override void Process()
    {
        Text = ExpandTextspeak(Text);
    }
}
