using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using System;
using System.IO;
using Microsoft.Win32;
using System.Linq;


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

            jsonOutput.WriteToJSON(processor.Messages);

            string jsonContent = File.ReadAllText("Output.json");
            string newOutput = "New Message: " + jsonContent;
            outputTextBlock.Text = newOutput + "\n\n" + outputTextBlock.Text;

            DisplayLists();
        }

        private void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // Read messages from the selected file and process them
                // ... code to read and process messages
                DisplayLists();
            }
        }

        private void DisplayLists()
        {
            // Update trending hashtags list
            trendingHashtagsList.Items.Clear();
            foreach (var item in TweetMessage.HashtagsDictionary.OrderByDescending(x => x.Value))
            {
                trendingHashtagsList.Items.Add(item.Key + ": " + item.Value);
            }

            // Update mentions list
            mentionsList.Items.Clear();
            foreach (var item in TweetMessage.MentionsDictionary.OrderByDescending(x => x.Value))
            {
                mentionsList.Items.Add(item.Key + ": " + item.Value);
            }

            // Update SIR list
            sirList.Items.Clear();
            foreach (var item in EmailMessage.SIRList)
            {
                sirList.Items.Add("Sort Code: " + item.SortCode + ", Nature of Incident: " + item.NatureOfIncident);
            }
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

