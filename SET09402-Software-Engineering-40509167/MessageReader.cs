using System;
using System.IO;

public class MessageReader
{
    public string InputFile { get; set; }
    private StreamReader reader;

    public MessageReader(string inputFile) // Constructor that takes the input file path
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
                return new EmailMessage { Text = line.Substring(6) };
            }
            else if (line.StartsWith("Tweet:"))
            {
                return new TweetMessage { Content = line.Substring(6) };
            }
        }

        reader.Close(); // Close the reader if no more messages
        return null; // Return null if no more messages
    }

}

