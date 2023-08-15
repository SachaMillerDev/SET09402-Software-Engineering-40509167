using System;

public abstract class Message
{
    public string MessageID { get; set; }
    public string Body { get; set; }
    public abstract string DetectType();
    public abstract void Process();

    public static string ExpandTextspeak(string inputText)
    {
        inputText = inputText.Replace("ROFL", "<Rolls on the floor laughing>");
        inputText = inputText.Replace("OMG", "<Oh My God>");
        inputText = inputText.Replace("LOL", "<Laughing Out Loud>");
        // Add more abbreviations as needed
        return inputText;
    }
}
