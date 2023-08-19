using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SET09402_Software_Engineering_40509167
{
    public partial class MainWindow : Window
    {
        private int messageIDCounter = 1;
        private Dictionary<string, string> abbreviations = new Dictionary<string, string>
        {
            { "LOL", "Laugh Out Loud" }
        };

        public MainWindow()
        {
            InitializeComponent();
            emailRecipientInput.Visibility = Visibility.Collapsed;
            emailSubjectInput.Visibility = Visibility.Collapsed;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // Validation logic
            // SMS Validation
            if (smsRadioButton.IsChecked == true)
            {
                if (!Regex.IsMatch(smsPhoneNumberInput.Text, @"^\+?\d{1,14}$"))
                {
                    MessageBox.Show("Invalid phone number. Ensure it's numeric and up to 14 characters.");
                    return;
                }
            }

            // Tweet Validation
            if (tweetRadioButton.IsChecked == true)
            {
                if (!tweetUsernameInput.Text.StartsWith("@") || tweetUsernameInput.Text.Length > 14)
                {
                    MessageBox.Show("Invalid username. It should start with '@' and be up to 14 characters.");
                    return;
                }
            }

            // Email Validation
            if (emailRadioButton.IsChecked == true)
            {
                if (!Regex.IsMatch(emailRecipientInput.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    MessageBox.Show("Invalid email format.");
                    return;
                }
                if (emailSubjectInput.Text.Length > 20)
                {
                    MessageBox.Show("Subject should be up to 20 characters.");
                    return;
                }
            }

            string messageType = "";
            string messageContent = "";
            string uniqueID = "ID: " + messageIDCounter++;

            if (smsRadioButton.IsChecked == true)
            {
                messageType = "SMS:";
                messageContent = $"{messageType} {uniqueID} Phone Number: {smsPhoneNumberInput.Text}; Message: {ExpandAbbreviations(messageInput.Text)}";
            }
            else if (emailRadioButton.IsChecked == true)
            {
                messageType = "Email:";
                messageContent = $"{messageType} {uniqueID} Recipient: {emailRecipientInput.Text}; Subject: {emailSubjectInput.Text}; Body: {ExpandAbbreviations(messageInput.Text)}";
                if (incidentCheckBox.IsChecked == true)
                {
                    messageContent += $"; Incident Type: {((ComboBoxItem)incidentComboBox.SelectedItem).Content}";
                    SIRList.Items.Insert(0, ((ComboBoxItem)incidentComboBox.SelectedItem).Content);
                }
            }
            else if (tweetRadioButton.IsChecked == true)
            {
                messageType = "Tweet:";
                messageContent = $"{messageType} {uniqueID} Username: {tweetUsernameInput.Text}; Message: {ExpandAbbreviations(messageInput.Text)}";
                CheckForMentionsAndHashtags(messageInput.Text);
            }

            File.AppendAllText("TestFile.txt", messageContent + Environment.NewLine);
            outputTextBlock.Text = "New Message: " + messageContent + "\n\n" + outputTextBlock.Text;

            // Clearing input boxes
            smsPhoneNumberInput.Text = "Phone Number";
            tweetUsernameInput.Text = "Username";
            emailRecipientInput.Text = "Recipient";
            emailSubjectInput.Text = "Subject";
            messageInput.Text = "Body here";
        }

        private string ExpandAbbreviations(string message)
        {
            foreach (var abbreviation in abbreviations)
            {
                message = message.Replace(abbreviation.Key, abbreviation.Value);
            }
            return message;
        }

        private void CheckForMentionsAndHashtags(string message)
        {
            var mentions = Regex.Matches(message, @"@\w+");
            var hashtags = Regex.Matches(message, @"#\w+");
            foreach (Match mention in mentions)
            {
                MentionsList.Items.Insert(0, mention.Value);
            }
            foreach (Match hashtag in hashtags)
            {
                TrendingList.Items.Insert(0, hashtag.Value);
            }
        }

        private void SmsRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            smsPhoneNumberInput.Visibility = Visibility.Visible;
            tweetUsernameInput.Visibility = Visibility.Collapsed;
            emailRecipientInput.Visibility = Visibility.Collapsed;
            emailSubjectInput.Visibility = Visibility.Collapsed;
            incidentCheckBox.Visibility = Visibility.Collapsed;
            incidentComboBox.Visibility = Visibility.Collapsed;
            messageInput.Text = "Text here";
            messageInput.Foreground = System.Windows.Media.Brushes.Gray;
        }

        private void EmailRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            smsPhoneNumberInput.Visibility = Visibility.Collapsed;
            tweetUsernameInput.Visibility = Visibility.Collapsed;
            emailRecipientInput.Visibility = Visibility.Visible;
            emailSubjectInput.Visibility = Visibility.Visible;
            incidentCheckBox.Visibility = Visibility.Visible;
            messageInput.Text = "Body here";
            messageInput.Foreground = System.Windows.Media.Brushes.Gray;
        }

        private void TweetRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            smsPhoneNumberInput.Visibility = Visibility.Collapsed;
            tweetUsernameInput.Visibility = Visibility.Visible;
            emailRecipientInput.Visibility = Visibility.Collapsed;
            emailSubjectInput.Visibility = Visibility.Collapsed;
            incidentCheckBox.Visibility = Visibility.Collapsed;
            incidentComboBox.Visibility = Visibility.Collapsed;
            messageInput.Text = "Text here";
            messageInput.Foreground = System.Windows.Media.Brushes.Gray;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text == "Text here" || tb.Text == "Recipient" || tb.Text == "Subject" || tb.Text == "Body here" || tb.Text == "Phone Number" || tb.Text == "Username")
            {
                tb.Text = "";
                tb.Foreground = System.Windows.Media.Brushes.Black;
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
                else if (tb == smsPhoneNumberInput) tb.Text = "Phone Number";
                else if (tb == tweetUsernameInput) tb.Text = "Username";
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void IncidentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            incidentComboBox.Visibility = Visibility.Visible;
        }

        private void IncidentCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            incidentComboBox.Visibility = Visibility.Collapsed;
        }
    }
}
