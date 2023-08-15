using System.Collections.Generic;
using System.Text.RegularExpressions;

public class EmailMessage : Message
{
    public string SenderEmail { get; set; }
    public string Subject { get; set; }
    public string Text { get; set; }
    public List<string> QuarantineList { get; set; } = new List<string>();

    public override string DetectType() => "Email";

    public void RemoveURLs()
    {
        foreach (Match match in Regex.Matches(Text, @"https?:"))
        {
            QuarantineList.Add(match.Value);
            Text = Text.Replace(match.Value, "<URL Quarantined>");
        }
    }

    public override void Process()
    {
        RemoveURLs();
    }
}
