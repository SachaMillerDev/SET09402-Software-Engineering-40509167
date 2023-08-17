using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class TweetMessage : Message
{
    public string TwitterID { get; set; }
    public string Content { get; set; }
    public List<string> Hashtags { get; set; } = new List<string>();
    public List<string> MentionedTwitterIDs { get; set; } = new List<string>();

    public static Dictionary<string, int> HashtagsDictionary = new Dictionary<string, int>();
    public static Dictionary<string, int> MentionsDictionary = new Dictionary<string, int>();

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

    public void UpdateHashtagsAndMentions()
    {
        foreach (var hashtag in Hashtags)
        {
            if (HashtagsDictionary.ContainsKey(hashtag))
            {
                HashtagsDictionary[hashtag]++;
            }
            else
            {
                HashtagsDictionary.Add(hashtag, 1);
            }
        }
        foreach (var mention in MentionedTwitterIDs)
        {
            if (MentionsDictionary.ContainsKey(mention))
            {
                MentionsDictionary[mention]++;
            }
            else
            {
                MentionsDictionary.Add(mention, 1);
            }
        }
    }

    public override void Process()
    {
        Content = ExpandTextspeak(Content);
        ProcessHashtags();
        ProcessTwitterIDs();
        UpdateHashtagsAndMentions();
    }
}
