using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace SET09402_Software_Engineering_40509167
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            emailRecipientInput.Visibility = Visibility.Collapsed;
            emailSubjectInput.Visibility = Visibility.Collapsed;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string messageType = "";
            string messageContent = "";
            if (smsRadioButton.IsChecked == true)
            {
                messageType = "SMS:";
                messageContent = messageType + messageInput.Text;
            }
            else if (emailRadioButton.IsChecked == true)
            {
                messageType = "Email:";
                messageContent = messageType + "Recipient: " + emailRecipientInput.Text + "; Subject: " + emailSubjectInput.Text + "; Body: " + messageInput.Text;
            }
            else if (tweetRadioButton.IsChecked == true)
            {
                messageType = "Tweet:";
                messageContent = messageType + messageInput.Text;
            }

            File.AppendAllText("TestFile.txt", messageContent + Environment.NewLine);

            MessageProcessor processor = new MessageProcessor();
            MessageReader reader = new MessageReader("TestFile.txt");
            JSONOutput jsonOutput = new JSONOutput { OutputFile = "Output.json" };

            Message message = reader.ReadMessage();
            while (message != null)
            {
                processor.AddMessage(message);
                message = reader.ReadMessage();
            }

            jsonOutput.SaveToJson(processor.Messages);

            string jsonContent = File.ReadAllText("Output.json");
            outputTextBlock.Text = "Output: " + jsonContent;
        }

        private void SmsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            emailRecipientInput.Visibility = Visibility.Collapsed;
            emailSubjectInput.Visibility = Visibility.Collapsed;
            messageInput.Text = "Text here";
            messageInput.Foreground = Brushes.Gray;
        }

        private void EmailRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            emailRecipientInput.Visibility = Visibility.Visible;
            emailSubjectInput.Visibility = Visibility.Visible;
            messageInput.Text = "Body here";
            messageInput.Foreground = Brushes.Gray;
        }

        private void TweetRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            emailRecipientInput.Visibility = Visibility.Collapsed;
            emailSubjectInput.Visibility = Visibility.Collapsed;
            messageInput.Text = "Text here";
            messageInput.Foreground = Brushes.Gray;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "Text here" || tb.Text == "Recipient" || tb.Text == "Subject")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                if (tb == messageInput) tb.Text = "Text here";
                else if (tb == emailRecipientInput) tb.Text = "Recipient";
                else if (tb == emailSubjectInput) tb.Text = "Subject";
                tb.Foreground = Brushes.Gray;
            }
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
            // You can add any specific processing for Email messages here.
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
                        return null;
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
