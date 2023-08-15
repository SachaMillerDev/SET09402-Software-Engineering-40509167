using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TweetMessage : Message
{
    public string TwitterID { get; set; }
    public string Content { get; set; }
    public List<string> Hashtags { get; set; } = new List<string>();
    public List<string> MentionedTwitterIDs { get; set; } = new List<string>();

    public override string DetectType() => "Tweet";

    public void ProcessHashtags()
    {
        foreach (Match match in Regex.Matches(Content, @"#\w+"))
        {
            Hashtags.Add(match.Value);
        }
    }

    public void ProcessTwitterIDs()
    {
        foreach (Match match in Regex.Matches(Content, @"@\w+"))
        {
            MentionedTwitterIDs.Add(match.Value);
        }
    }

    public override void Process()
    {
        Content = ExpandTextspeak(Content);
        ProcessHashtags();
        ProcessTwitterIDs();
    }
}
