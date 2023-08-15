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
            jsonOutput.WriteToJSON(processor.Messages);


            Message message = reader.ReadMessage();
            while (message != null)
            {
                processor.AddMessage(message);
                message = reader.ReadMessage();
            }

            jsonOutput.WriteToJSON(processor.Messages);
            string jsonContent = File.ReadAllText("Output.json");

            string newOutput = "New Message: " + jsonContent;
            outputTextBlock.Text = newOutput + "\n\n" + outputTextBlock.Text;
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
            if (tb.Text == "Text here" || tb.Text == "Recipient" || tb.Text == "Subject" || tb.Text == "Body here")
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
                if (tb == messageInput && emailRadioButton.IsChecked == true) tb.Text = "Body here";
                else if (tb == messageInput) tb.Text = "Text here";
                else if (tb == emailRecipientInput) tb.Text = "Recipient";
                else if (tb == emailSubjectInput) tb.Text = "Subject";
                tb.Foreground = Brushes.Gray;
            }
        }
    }
}
