using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataStorage
{
    public string OutputFile { get; set; }

    //private void SaveMessagesToJson()
    //{
    //    // Extracting SMS messages
    //    var smsMessages = smsOutputList.Items.Cast<object>().Select(item => item.ToString()).ToList();


    //    // Extracting email messages
    //    var emailMessages = emailOutputList.Items.Cast<object>().Select(item =>
    //    {
    //        if (item is ListBoxItem listBoxItem)
    //        {
    //            return listBoxItem.Content as string;
    //        }
    //        return item as string;
    //    }).Where(email => !string.IsNullOrEmpty(email)).ToList();

    //    // Extracting tweet messages
    //    var tweetMessages = tweetOutputList.Items.Cast<object>().Select(item => item.ToString()).ToList();


    //    // Extracting hashtags, mentions, and SIR lists
    //    var hashtags = TrendingList.Items.Cast<object>().Select(item => item.ToString()).ToList();

    //    var mentions = MentionsList.Items.Cast<object>().Select(item => item.ToString()).ToList();

    //    var sirList = SIRList.Items.Cast<object>().Select(item => item.ToString()).ToList();

    //    var quarantinedUrls = QuarantinedUrlsList.Items.Cast<object>().Select(item => item.ToString()).ToList();


    //    // Creating the object to be serialized
    //    var messages = new
    //    {
    //        SMSMessages = smsMessages,
    //        EmailMessages = emailMessages,
    //        TweetMessages = tweetMessages,
    //        Hashtags = hashtags,
    //        Mentions = mentions,
    //        SIRList = sirList,
    //        QuarantinedUrls = quarantinedUrls,
    //        SortCodeSegment1 = sortCodeSegment1,
    //        SortCodeSegment2 = sortCodeSegment2,
    //        SortCodeSegment3 = sortCodeSegment3,
    //        SmsMessageIDCounter = smsMessageIDCounter,
    //        EmailMessageIDCounter = emailMessageIDCounter,
    //        TweetMessageIDCounter = tweetMessageIDCounter
    //    };


    //    // Serializing and writing to file
    //    string json = JsonConvert.SerializeObject(messages, Formatting.Indented);
    //    string filePath = @"C:\Users\SachaMiller\Downloads\test\messages.json";
    //    File.WriteAllText(filePath, json);
    //}

    //    private dynamic ReadMessagesFromJson()
    //    {
    //        string filePath = @"C:\Users\SachaMiller\Downloads\test\messages.json";
    //        if (!File.Exists(filePath))
    //            return null;

    //        string json = File.ReadAllText(filePath);
    //        return JsonConvert.DeserializeObject(json);
    //    }
}
