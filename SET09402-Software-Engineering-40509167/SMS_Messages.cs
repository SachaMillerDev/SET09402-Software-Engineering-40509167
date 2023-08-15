﻿public class SMSMessage : Message
{
    public string SenderPhoneNumber { get; set; }
    public string Text { get; set; }

    public override string DetectType() => "SMS";

    public void ExpandTextspeak()
    {
        Text = Text.Replace("ROFL", "<Rolls on the floor laughing>");
        Text = Text.Replace("OMG", "Oh My God");
        Text = Text.Replace("LOL", "Laughing Out Loud");
        //future abriviations addded here and follow the convention of the above code 
    }

    public override void Process()
    {
        ExpandTextspeak();
    }
}
