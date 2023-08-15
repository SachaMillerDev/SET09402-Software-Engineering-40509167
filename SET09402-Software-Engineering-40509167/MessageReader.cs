using System;
using System.IO;

public class MessageReader
{
    public string InputFile { get; set; }
    private StreamReader reader;

    public MessageReader(string inputFile)
    {
        InputFile = inputFile;
        reader = new StreamReader(InputFile);
    }

    public Message ReadMessage()
    {
        string line = reader.ReadLine();
        if (line != null)
        {
            if (line.StartsWith("SMS:"))
            {
                return new SMSMessage { Text = line.Substring(4) };
            }
            else if (line.StartsWith("Email:"))
            {
                {
                string[] parts = line.Substring(6).Split(';');
                if (parts.Length >= 2)
                {
                    return new EmailMessage
                    {
                        Subject = parts[0].Replace("Subject: ", "").Trim(),
                        Body = parts[1].Replace("Body: ", "").Trim()
                    };
                }
                else
                {
                    // Handle the error or return a default value
                    return null;
                }
            }
            }
            else if (line.StartsWith("Tweet:"))
            {
                return new TweetMessage { Content = line.Substring(6) };
            }
        }
        reader.Close();
        return null;
    }
}