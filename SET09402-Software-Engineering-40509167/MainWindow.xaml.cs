using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Windows;

namespace SET09402_Software_Engineering_40509167
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string messageType = "";
            if (smsRadioButton.IsChecked == true) messageType = "SMS:";
            else if (emailRadioButton.IsChecked == true) messageType = "Email:";
            else if (tweetRadioButton.IsChecked == true) messageType = "Tweet:";

            string messageContent = messageType + messageInput.Text;

            // Store the message in a file
            File.AppendAllText("TestFile.txt", messageContent + Environment.NewLine);

            // Process the message
            MessageProcessor processor = new MessageProcessor();
            MessageReader reader = new MessageReader("TestFile.txt");
            JSONOutput jsonOutput = new JSONOutput { OutputFile = "Output.json" };

            Message message = reader.ReadMessage();
            while (message != null)
            {
                processor.AddMessage(message);
                message = reader.ReadMessage();
            }

            // Convert messages to JSON and save to OutputFile
            jsonOutput.SaveToJson(processor.Messages);

            // Read the JSON output and display
            string jsonContent = File.ReadAllText("Output.json");
            outputTextBlock.Text = "Output: " + jsonContent;
        }
    }

    public class MessageProcessor
    {
        public List<Message> Messages { get; set; } = new List<Message>();

        public void ProcessMessages()
        {
            foreach (Message message in Messages)
            {
                message.Process();
            }
        }

        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }
    }

    public abstract class Message
    {
        public string MessageID { get; set; }
        public string Body { get; set; }

        public abstract string DetectType();
        public abstract void Process();
    }

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
            ProcessHashtags();
            ProcessTwitterIDs();
        }
    }

    public class SMSMessage : Message
    {
        public string SenderPhoneNumber { get; set; }
        public string Text { get; set; }

        public override string DetectType() => "SMS";

        public void ExpandTextspeak()
        {
            Text = Text.Replace("ROFL", "<Rolls on the floor laughing>");
            Text = Text.Replace("OMG", "Oh My God");
            Text = Text.Replace("LOL", "Laughing Out Loud");
            // Add more replacements as needed
        }

        public override void Process()
        {
            ExpandTextspeak();
        }
    }

    public class EmailMessage : Message
    {
        public string Subject { get; set; }
        public string SenderEmail { get; set; }

        public override string DetectType() => "Email";

        public override void Process()
        {
            // Add any specific processing for emails if needed
        }
    }

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
                    // Example logic for reading Email messages
                    string[] parts = line.Substring(6).Split(';');
                    return new EmailMessage { Subject = parts[0].Replace("Subject: ", "").Trim(), Body = parts[1].Replace("Body: ", "").Trim() };
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

    public class JSONOutput
    {
        public string OutputFile { get; set; }

        public void SaveToJson(List<Message> messages)
        {
            string json = JsonConvert.SerializeObject(messages, Formatting.Indented);
            File.WriteAllText(OutputFile, json);
        }
    }
}
