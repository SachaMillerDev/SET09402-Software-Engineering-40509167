using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.IO;

namespace SET09402_Software_Engineering_40509167
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, int> mentionsDictionary = new Dictionary<string, int>();
        private Dictionary<string, int> hashtagsDictionary = new Dictionary<string, int>();
        private Dictionary<string, int> quarantinedUrlsDictionary = new Dictionary<string, int>();
        private int messageIDCounter = 1;
        private int sortCodeSegment1 = 0;
        private int sortCodeSegment2 = 0;
        private int sortCodeSegment3 = 0;

        private Dictionary<string, string> abbreviations = new Dictionary<string, string>
        {
            {"LOL", "Laugh Out Loud"},
            {"BRB", "Be Right Back"},
            {"GTG", "Got To Go"},
            {"TTYL", "Talk To You Later"},
            {"OMG", "Oh My God"},
            {"IDK", "I Don't Know"},
            {"IMO", "In My Opinion"},
            {"IMHO", "In My Humble Opinion"},
            {"BFF", "Best Friends Forever"},
            {"FYI", "For Your Information"},
            {"ROFL", "Rolling On the Floor Laughing"},
            {"SMH", "Shaking My Head"},
            {"TMI", "Too Much Information"},
            {"YOLO", "You Only Live Once"},
            {"ICYMI", "In Case You Missed It"},
            {"FOMO", "Fear Of Missing Out"},
            {"TL;DR", "Too Long; Didn't Read"},
            {"BTW", "By The Way"},
            {"DM", "Direct Message"},
            {"NSFW", "Not Safe For Work"},
            {"TBT", "Throwback Thursday"},
            {"TGIF", "Thank God It's Friday"}
        };

        public MainWindow()
        {
            InitializeComponent();
            emailRecipientInput.Visibility = Visibility.Collapsed;
            emailSubjectInput.Visibility = Visibility.Collapsed;
            incidentCheckBox.Visibility = Visibility.Collapsed;
            incidentComboBox.Visibility = Visibility.Collapsed;
        }

        private string GenerateNextSortCode()
        {
            sortCodeSegment3++;
            if (sortCodeSegment3 > 99)
            {
                sortCodeSegment3 = 0;
                sortCodeSegment2++;
                if (sortCodeSegment2 > 99)
                {
                    sortCodeSegment2 = 0;
                    sortCodeSegment1++;
                    if (sortCodeSegment1 > 99)
                    {
                        MessageBox.Show("No more unique sort code available. Please speak to an admin.");
                        return null;
                    }
                }
            }
            return $"{sortCodeSegment1:00}-{sortCodeSegment2:00}-{sortCodeSegment3:00}";
        }
        private List<string> ExtractURLs(string message)
        {
            var urlPattern = @"https://www\.[\w\-\.]+";
            var matches = Regex.Matches(message, urlPattern);
            return matches.Cast<Match>().Select(m => m.Value).ToList();
        }


        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (smsRadioButton.IsChecked == true)
            {
                if (!Regex.IsMatch(smsPhoneNumberInput.Text, @"^\+\d{1,14}$") || smsPhoneNumberInput.Text.Length > 14)
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

            if (emailRadioButton.IsChecked == true)
            {
                EmailMessage emailMessage = new EmailMessage
                {
                    Subject = emailSubjectInput.Text,
                    Body = messageInput.Text
                };

                // Sanitize URLs in the email body
                var urls = ExtractURLs(emailMessage.Body);
                foreach (var url in urls)
                {
                    emailMessage.Body = emailMessage.Body.Replace(url, "[URL Quarantined]");

                    // Update the dictionary
                    if (quarantinedUrlsDictionary.ContainsKey(url))
                    {
                        quarantinedUrlsDictionary[url]++;
                    }
                    else
                    {
                        quarantinedUrlsDictionary[url] = 1;
                    }

                    // Add the URL with its count to the QuarantinedUrlsList
                    QuarantinedUrlsList.Items.Insert(0, $"{url} ({quarantinedUrlsDictionary[url]})");
                }
                var sortedUrls = QuarantinedUrlsList.Items.Cast<string>()
                .OrderByDescending(item =>
                {
                    var parts = item.Split(' ');
                    return parts.Length > 1 && int.TryParse(parts.Last().Trim('(', ')'), out int count) ? count : 0;
                }).ToList();

                QuarantinedUrlsList.Items.Clear();
                foreach (var url in sortedUrls)
                {
                    QuarantinedUrlsList.Items.Add(url);
                }


                messageType = "Email:";
                messageContent = $"{messageType} {uniqueID} Recipient: {emailRecipientInput.Text}; Subject: {emailSubjectInput.Text}; Body: {ExpandAbbreviations(emailMessage.Body)}";

                if (incidentCheckBox.IsChecked == true)
                {
                    messageContent += $"; Incident Type: {((ComboBoxItem)incidentComboBox.SelectedItem).Content}";
                    UpdateSIRList(((ComboBoxItem)incidentComboBox.SelectedItem).Content.ToString());
                    ListBoxItem listItem = new ListBoxItem();
                    listItem.Content = messageContent;
                    listItem.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 230, 230));
                    emailOutputList.Items.Insert(0, listItem);
                }
                else
                {
                    emailOutputList.Items.Insert(0, messageContent);
                }
            }
            else if (smsRadioButton.IsChecked == true)
            {
                messageType = "SMS:";
                messageContent = $"{messageType} {uniqueID} Phone Number: {smsPhoneNumberInput.Text}; Message: {ExpandAbbreviations(messageInput.Text)}";
                smsOutputList.Items.Insert(0, messageContent);
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
            incidentCheckBox.IsChecked = false;
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
            // Extracting mentions and hashtags
            var mentions = Regex.Matches(message, @"@\w+").Cast<Match>().Select(m => m.Value).ToList();
            var hashtags = Regex.Matches(message, @"#\w+").Cast<Match>().Select(m => m.Value).ToList();

            // Updating mentions dictionary and list
            foreach (var mention in mentions)
            {
                if (mentionsDictionary.ContainsKey(mention))
                {
                    mentionsDictionary[mention]++;
                }
                else
                {
                    mentionsDictionary[mention] = 1;
                }
            }
            MentionsList.Items.Clear();
            foreach (var item in mentionsDictionary.OrderByDescending(x => x.Value))
            {
                MentionsList.Items.Add($"{item.Key} ({item.Value})");
            }

            // Updating hashtags dictionary and list
            foreach (var hashtag in hashtags)
            {
                if (hashtagsDictionary.ContainsKey(hashtag))
                {
                    hashtagsDictionary[hashtag]++;
                }
                else
                {
                    hashtagsDictionary[hashtag] = 1;
                }
            }
            TrendingList.Items.Clear();
            foreach (var item in hashtagsDictionary.OrderByDescending(x => x.Value))
            {
                TrendingList.Items.Add($"{item.Key} ({item.Value})");
            }
        }

        private void UpdateSIRList(string incidentType)
        {
            bool found = false;
            foreach (var item in SIRList.Items)
            {
                string content = item.ToString();
                var parts = content.Split(' ');
                if (parts.Length == 2 && parts[0] == incidentType)
                {
                    if (int.TryParse(parts[1], out int count))
                    {
                        SIRList.Items.Remove(item);
                        SIRList.Items.Insert(0, incidentType + " " + (count + 1));
                        found = true;
                        break;
                    }
                }
            }
            if (!found)
            {
                SIRList.Items.Insert(0, incidentType + " 1");
            }

            // Sort the SIR list
            var sortedList = SIRList.Items.Cast<string>()
                .OrderByDescending(item =>
                {
                    var splitItem = item.Split(' ');
                    return splitItem.Length > 1 && int.TryParse(splitItem[1], out int tally) ? tally : 0;
                }).ToList();

            SIRList.Items.Clear();
            foreach (var sir in sortedList)
            {
                SIRList.Items.Add(sir);
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
            if (incidentCheckBox.IsChecked == true)
            {
                incidentComboBox.Visibility = Visibility.Visible;
            }
            else
            {
                incidentComboBox.Visibility = Visibility.Collapsed;
            }
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
            messageInput.Text = "Tweet here";
            messageInput.Foreground = System.Windows.Media.Brushes.Gray;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = "";
            textBox.Foreground = System.Windows.Media.Brushes.Black;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                switch (textBox.Name)
                {
                    case "smsPhoneNumberInput":
                        textBox.Text = "Phone Number";
                        break;
                    case "tweetUsernameInput":
                        textBox.Text = "Username";
                        break;
                    case "emailRecipientInput":
                        textBox.Text = "Recipient";
                        break;
                    case "emailSubjectInput":
                        textBox.Text = "Subject";
                        break;
                    case "messageInput":
                        if (smsRadioButton.IsChecked == true)
                        {
                            textBox.Text = "Text here";
                        }
                        else if (emailRadioButton.IsChecked == true)
                        {
                            textBox.Text = "Body here";
                        }
                        else if (tweetRadioButton.IsChecked == true)
                        {
                            textBox.Text = "Tweet here";
                        }
                        break;
                }
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
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
            // Extracting SMS messages
            var smsMessages = smsOutputList.Items.Cast<string>().ToList();

            // Extracting email messages
            var emailMessages = emailOutputList.Items.Cast<object>().Select(item =>
            {
                if (item is ListBoxItem listBoxItem)
                {
                    return listBoxItem.Content as string;
                }
                return item as string;
            }).Where(email => !string.IsNullOrEmpty(email)).ToList();

            // Extracting tweet messages
            var tweetMessages = tweetOutputList.Items.Cast<string>().ToList();

            // Extracting hashtags, mentions, and SIR lists
            var hashtags = TrendingList.Items.Cast<string>().ToList();
            var mentions = MentionsList.Items.Cast<string>().ToList();
            var sirList = SIRList.Items.Cast<string>().ToList();
            var quarantinedUrls = QuarantinedUrlsList.Items.Cast<string>().ToList();

            // Creating the object to be serialized
            var messages = new
            {
                SMSMessages = smsMessages,
                EmailMessages = emailMessages,
                TweetMessages = tweetMessages,
                Hashtags = hashtags,
                Mentions = mentions,
                SIRList = sirList,
                QuarantinedUrls = quarantinedUrls
            };

            // Serializing and writing to file
            string json = JsonConvert.SerializeObject(messages, Formatting.Indented);
            string filePath = @"C:\Users\SachaMiller\Downloads\test\messages.json";
            File.WriteAllText(filePath, json);
        }



    }
}
