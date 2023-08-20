using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Linq;
using System.IO;


namespace SET09402_Software_Engineering_40509167
{
    public partial class MainWindow : Window
    {
        private int messageIDCounter = 1;
        private Dictionary<string, string> abbreviations = new Dictionary<string, string>
        {
            { "LOL", "Laugh Out Loud" },
            { "BRB", "Be Right Back" },
            { "GTG", "Got To Go" },
            { "TTYL", "Talk To You Later" },
            { "OMG", "Oh My God" },
            { "IDK", "I Don't Know" },
            { "IMO", "In My Opinion" },
            { "IMHO", "In My Humble Opinion" },
            { "BFF", "Best Friends Forever" },
            { "FYI", "For Your Information" },
            { "ROFL", "Rolling On the Floor Laughing" },
            { "SMH", "Shaking My Head" },
            { "TMI", "Too Much Information" },
            { "YOLO", "You Only Live Once" },
            { "ICYMI", "In Case You Missed It" },
            { "FOMO", "Fear Of Missing Out" },
            { "TL;DR", "Too Long; Didn't Read" },
            { "BTW", "By The Way" },
            { "DM", "Direct Message" },
            { "NSFW", "Not Safe For Work" },
            { "TBT", "Throwback Thursday" },
            { "TGIF", "Thank God It's Friday" }
        };


        public MainWindow()
        {
            InitializeComponent();
            emailRecipientInput.Visibility = Visibility.Collapsed;
            emailSubjectInput.Visibility = Visibility.Collapsed;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (smsRadioButton.IsChecked == true)
            {
                if (!Regex.IsMatch(smsPhoneNumberInput.Text, @"^\+?\d{1,14}$") || smsPhoneNumberInput.Text.Length > 14)
                {
                    MessageBox.Show("Invalid phone number. Ensure it's numeric and up to 14 characters.");
                    return;
                }
            }

            if (tweetRadioButton.IsChecked == true)
            {
                if (!tweetUsernameInput.Text.StartsWith("@") || tweetUsernameInput.Text.Length > 14)
                {
                    MessageBox.Show("Invalid username. It should start with '@' and be up to 14 characters.");
                    return;
                }
            }

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

                if (incidentCheckBox.IsChecked == true && incidentComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select an incident type or uncheck the SIR box.");
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
                smsOutputList.Items.Insert(0, messageContent);
            }
            else if (emailRadioButton.IsChecked == true)
            {
                messageType = "Email:";
                messageContent = $"{messageType} {uniqueID} Recipient: {emailRecipientInput.Text}; Subject: {emailSubjectInput.Text}; Body: {ExpandAbbreviations(messageInput.Text)}";

                if (incidentCheckBox.IsChecked == true)
                {
                    messageContent += $"; Incident Type: {((ComboBoxItem)incidentComboBox.SelectedItem).Content}";
                    UpdateSIRList(((ComboBoxItem)incidentComboBox.SelectedItem).Content.ToString());

                    ListBoxItem listItem = new ListBoxItem();
                    listItem.Content = messageContent;
                    listItem.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 230, 230)); // Set the background color to light red
                    emailOutputList.Items.Insert(0, listItem);
                }
                else
                {
                    emailOutputList.Items.Insert(0, messageContent);
                }
            }
            else if (tweetRadioButton.IsChecked == true)
            {
                messageType = "Tweet:";
                messageContent = $"{messageType} {uniqueID} Username: {tweetUsernameInput.Text}; Message: {ExpandAbbreviations(messageInput.Text)}";
                CheckForMentionsAndHashtags(messageInput.Text);
                tweetOutputList.Items.Insert(0, messageContent);
            }

            smsPhoneNumberInput.Text = "Phone Number";
            tweetUsernameInput.Text = "Username";
            emailRecipientInput.Text = "Recipient";
            emailSubjectInput.Text = "Subject";
            messageInput.Text = "Body here";
            incidentComboBox.SelectedIndex = -1;
            incidentCheckBox.IsChecked = false; // Uncheck the incident checkbox after sending the message
            SaveMessagesToJson();
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

        private void UpdateSIRList(string incidentType)
        {
            bool found = false;
            foreach (var item in SIRList.Items)
            {
                string content = item.ToString();
                if (content.StartsWith(incidentType))
                {
                    string[] parts = content.Split(' ');
                    int count = int.Parse(parts[1]) + 1;
                    SIRList.Items.Remove(item);
                    SIRList.Items.Insert(0, incidentType + " " + count);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                SIRList.Items.Insert(0, incidentType + " 1");
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
            incidentComboBox.SelectedIndex = -1;
        }
        private void SaveMessagesToJson()
        {
            var messages = new
            {
                SMSMessages = smsOutputList.Items.Cast<string>().ToList(),
                EmailMessages = emailOutputList.Items.Cast<ListBoxItem>().Select(item => item.Content.ToString()).ToList(),
                TweetMessages = tweetOutputList.Items.Cast<string>().ToList()
            };

            string json = JsonConvert.SerializeObject(messages, Formatting.Indented);
            File.WriteAllText("messages.json", json);
        }
    }
}
