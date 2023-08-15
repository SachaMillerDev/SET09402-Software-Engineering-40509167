using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;


public class TweetMessage : Message
{
    public string TwitterID { get; set; }
    public string Content { get; set; }
    public List<string> Hashtags { get; set; } = new List<string>();
    public List<string> MentionedTwitterIDs { get; set; } = new List<string>();

    Dictionary<string, int> hashtagsDictionary = new Dictionary<string, int>();
    Dictionary<string, int> mentionsDictionary = new Dictionary<string, int>();

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
            if (hashtagsDictionary.ContainsKey(hashtag))
            {
                hashtagsDictionary[hashtag]++;
            }
            else
            {
                hashtagsDictionary.Add(hashtag, 1);
            }
        }
        foreach (var mention in MentionedTwitterIDs)
        {
            if (mentionsDictionary.ContainsKey(mention))
            {
                mentionsDictionary[mention]++;
            }
            else
            {
                mentionsDictionary.Add(mention, 1);
            }
        }
    }

    public override void Process()
    {
        Content = ExpandTextspeak(Content);
        ProcessHashtags();
        ProcessTwitterIDs();
    }
}
