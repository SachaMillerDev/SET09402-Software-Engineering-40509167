using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;
using CsvHelper;
using System.Reflection.PortableExecutable;
using Microsoft.Win32;



namespace SET09402_Software_Engineering_40509167
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, int> mentionsDictionary = new Dictionary<string, int>();
        private Dictionary<string, int> hashtagsDictionary = new Dictionary<string, int>();
        private Dictionary<string, int> quarantinedUrlsDictionary = new Dictionary<string, int>();
        private int sortCodeSegment1 = 0;
        private int sortCodeSegment2 = 0;
        private int sortCodeSegment3 = 0;
        private int smsMessageIDCounter = 1;
        private int emailMessageIDCounter = 1;
        private int tweetMessageIDCounter = 1;
        private Dictionary<string, string> abbreviations = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
            LoadAbbreviationsFromCSV();
            MessagesData messagesData = ReadMessagesFromJson();

            if (messagesData != null)
            {
                sortCodeSegment1 = messagesData.SortCodeSegment1 ?? sortCodeSegment1;
                sortCodeSegment2 = messagesData.SortCodeSegment2 ?? sortCodeSegment2;
                sortCodeSegment3 = messagesData.SortCodeSegment3 ?? sortCodeSegment3;
                smsMessageIDCounter = messagesData.SmsMessageIDCounter ?? smsMessageIDCounter;
                emailMessageIDCounter = messagesData.EmailMessageIDCounter ?? emailMessageIDCounter;
                tweetMessageIDCounter = messagesData.TweetMessageIDCounter ?? tweetMessageIDCounter;


                smsOutputList.Items.Clear();
                foreach (var message in messagesData.SMSMessages)
                {
                    smsOutputList.Items.Add(message);
                }

                emailOutputList.Items.Clear();
                foreach (var msgObj in messagesData.EmailMessages)

                {
                    string msgString = msgObj.ToString();
                    if (msgString.Contains("Incident Type:"))
                    {
                        ListBoxItem listItem = new ListBoxItem();
                        listItem.Content = msgObj;
                        listItem.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 230, 230));
                        emailOutputList.Items.Add(listItem);
                    }
                    else
                    {
                        emailOutputList.Items.Add(msgObj);
                    }
                }


                tweetOutputList.Items.Clear();
                foreach (var message in messagesData.TweetMessages)
                {
                    tweetOutputList.Items.Add(message);
                }

                TrendingList.Items.Clear();
                foreach (var hashtag in messagesData.Hashtags)
                {
                    var parts = hashtag.Split(' ');
                    if (parts.Length > 1 && int.TryParse(parts.Last().Trim('(', ')'), out int count))
                    {
                        hashtagsDictionary[parts[0]] = count;
                    }
                    TrendingList.Items.Add(hashtag);
                }

                MentionsList.Items.Clear();
                foreach (var mention in messagesData.Mentions)
                {
                    var parts = mention.Split(' ');
                    if (parts.Length > 1 && int.TryParse(parts.Last().Trim('(', ')'), out int count))
                    {
                        mentionsDictionary[parts[0]] = count;
                    }
                    MentionsList.Items.Add(mention);
                }


                SIRList.Items.Clear();
                foreach (var sir in messagesData.SIRList)
                {
                    SIRList.Items.Add(sir);
                }

                QuarantinedUrlsList.Items.Clear();
                foreach (var url in messagesData.QuarantinedUrls)
                {
                    var parts = url.Split(' ');
                    if (parts.Length > 1 && int.TryParse(parts.Last().Trim('(', ')'), out int count))
                    {
                        quarantinedUrlsDictionary[parts[0]] = count;
                    }
                    QuarantinedUrlsList.Items.Add(url);
                }
            }

            emailRecipientInput.Visibility = Visibility.Collapsed;
            emailSubjectInput.Visibility = Visibility.Collapsed;
            incidentCheckBox.Visibility = Visibility.Collapsed;
            incidentComboBox.Visibility = Visibility.Collapsed;
        }
        private MessagesData ReadMessagesFromJson()
        {
            string filePath = @"C:\Users\SachaMiller\Downloads\test\messages.json";
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<MessagesData>(json);
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
        private void ImportJsonButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string jsonContent = File.ReadAllText(openFileDialog.FileName);
                MessagesData importedData = JsonConvert.DeserializeObject<MessagesData>(jsonContent);

                // Update the counters based on the imported data
                int highestSMSId = smsMessageIDCounter;
                foreach (var item in importedData.SMSMessages)
                {
                    string idStr = item.Substring(1, 9); // Assuming the ID format is "Sxxxxxxxxx"
                    if (int.TryParse(idStr, out int id))
                    {
                        highestSMSId = Math.Max(highestSMSId, id);
                    }
                    if (!smsOutputList.Items.Contains(item))
                    {
                        smsOutputList.Items.Add(item);
                    }
                }
                smsMessageIDCounter = highestSMSId + 1;

                int highestEmailId = emailMessageIDCounter;
                foreach (var item in importedData.EmailMessages)
                {
                    string idStr = item.Substring(1, 9); // Assuming the ID format is "Exxxxxxxx"
                    if (int.TryParse(idStr, out int id))
                    {
                        highestEmailId = Math.Max(highestEmailId, id);
                    }
                    if (!emailOutputList.Items.Contains(item))
                    {
                        if (item.Contains("Incident Type:"))
                        {
                            ListBoxItem listItem = new ListBoxItem();
                            listItem.Content = item;
                            listItem.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 230, 230));
                            emailOutputList.Items.Add(listItem);
                        }
                        else
                        {
                            emailOutputList.Items.Add(item);
                        }
                    }
                }
                emailMessageIDCounter = highestEmailId + 1;

                int highestTweetId = tweetMessageIDCounter;
                foreach (var item in importedData.TweetMessages)
                {
                    string idStr = item.Substring(1, 9); // Assuming the ID format is "Txxxxxxxxx"
                    if (int.TryParse(idStr, out int id))
                    {
                        highestTweetId = Math.Max(highestTweetId, id);
                    }
                    if (!tweetOutputList.Items.Contains(item))
                    {
                        tweetOutputList.Items.Add(item);
                    }
                }
                tweetMessageIDCounter = highestTweetId + 1;

                // Update the sort code segments
                sortCodeSegment1 = Math.Max(sortCodeSegment1, importedData.SortCodeSegment1 ?? 0);
                sortCodeSegment2 = Math.Max(sortCodeSegment2, importedData.SortCodeSegment2 ?? 0);
                sortCodeSegment3 = Math.Max(sortCodeSegment3, importedData.SortCodeSegment3 ?? 0);

                // Merge the imported data with the existing data for other lists
                foreach (var item in importedData.Hashtags)
                {
                    if (!TrendingList.Items.Contains(item))
                    {
                        TrendingList.Items.Add(item);
                    }
                }

                foreach (var item in importedData.Mentions)
                {
                    if (!MentionsList.Items.Contains(item))
                    {
                        MentionsList.Items.Add(item);
                    }
                }

                foreach (var item in importedData.SIRList)
                {
                    if (!SIRList.Items.Contains(item))
                    {
                        SIRList.Items.Add(item);
                    }
                }

                foreach (var item in importedData.QuarantinedUrls)
                {
                    if (!QuarantinedUrlsList.Items.Contains(item))
                    {
                        QuarantinedUrlsList.Items.Add(item);
                    }
                }

                // Update the hashtags dictionary with imported data
                foreach (var item in importedData.Hashtags)
                {
                    var parts = item.Split(' ');
                    if (parts.Length > 1 && int.TryParse(parts.Last().Trim('(', ')'), out int count))
                    {
                        if (hashtagsDictionary.ContainsKey(parts[0]))
                        {
                            hashtagsDictionary[parts[0]] = Math.Max(hashtagsDictionary[parts[0]], count);
                        }
                        else
                        {
                            hashtagsDictionary[parts[0]] = count;
                        }
                    }
                }

                // Update the mentions dictionary with imported data
                foreach (var item in importedData.Mentions)
                {
                    var parts = item.Split(' ');
                    if (parts.Length > 1 && int.TryParse(parts.Last().Trim('(', ')'), out int count))
                    {
                        if (mentionsDictionary.ContainsKey(parts[0]))
                        {
                            mentionsDictionary[parts[0]] = Math.Max(mentionsDictionary[parts[0]], count);
                        }
                        else
                        {
                            mentionsDictionary[parts[0]] = count;
                        }
                    }
                }
            }
        }

        private void LoadAbbreviationsFromCSV()
        {
            string csvFilePath = @"C:\Users\SachaMiller\source\repos\SET09402-Software Engineering-SachaMiller-40509167\SET09402-Software Engineering-SachaMiller-40509167\AbbriviationsCurrent.csv";
            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<AbbreviationRecord>();
                foreach (var record in records)
                {
                    abbreviations[record.Abbreviation] = record.FullForm;
                }
            }
        }



        public class AbbreviationRecord
        {
            public string Abbreviation { get; set; }
            public string FullForm { get; set; }
        }
        private List<string> ExtractURLs(string message)
        {
            var urlPattern = @"\b(?:https?://)?(?:www\.)?[\w\-\.]+\.\w+\b";
            var matches = Regex.Matches(message, urlPattern);
            return matches.Cast<Match>().Select(m => m.Value).ToList();
        }



        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (smsRadioButton.IsChecked == true)
            {
                if (!Regex.IsMatch(smsPhoneNumberInput.Text, @"^(\+)?\d{1,14}$") || smsPhoneNumberInput.Text.Length > 14)
                {
                    MessageBox.Show("Invalid phone number. Ensure it's numeric and up to 14 characters.");
                    return;
                }
                // New SMS Message Body Validation
                if (messageInput.Text.Length > 140)
                {
                    MessageBox.Show("SMS message body should be a maximum of 140 characters long.");
                    return;
                }
            }


            if (tweetRadioButton.IsChecked == true)
            {
                if (!tweetUsernameInput.Text.StartsWith("@") || tweetUsernameInput.Text.Length > 15)
                {
                    MessageBox.Show("Invalid username. It should start with '@' and be up to 15 characters.");
                    return;
                }
                // New Tweet Message Body Validation
                if (messageInput.Text.Length > 140)
                {
                    MessageBox.Show("Tweet message body should be a maximum of 140 characters long.");
                    return;
                }
            }


            if (emailRadioButton.IsChecked == true)
            {
                if (!Regex.IsMatch(emailRecipientInput.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    MessageBox.Show("Invalid email format. expected format e.g. xxx@gmail.com ");
                    return;
                }
                // New Email Subject and Body Validation
                if (emailSubjectInput.Text.Length > 20)
                {
                    MessageBox.Show("Email subject should be a maximum of 20 characters long.");
                    return;
                }
                if (messageInput.Text.Length > 1028)
                {
                    MessageBox.Show("Email body should be a maximum of 1028 characters long.");
                    return;
                }
            }


            string messageType = "";
            string messageContent = "";
            string uniqueID = ""; // Initialize with an empty string
            if (emailRadioButton.IsChecked == true && incidentCheckBox.IsChecked == true)
            {
                uniqueID = "Sort Code: " + GenerateNextSortCode();
            }
            else
            {
                string prefix = "";
                if (smsRadioButton.IsChecked == true)
                {
                    prefix = "S";
                    uniqueID = prefix + smsMessageIDCounter++.ToString("D9");
                }
                else if (emailRadioButton.IsChecked == true)
                {
                    prefix = "E";
                    uniqueID = prefix + emailMessageIDCounter++.ToString("D9");
                }
                else if (tweetRadioButton.IsChecked == true)
                {
                    prefix = "T";
                    uniqueID = prefix + tweetMessageIDCounter++.ToString("D9");
                }
            }




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
                    string formattedUrl = $"{url} ({quarantinedUrlsDictionary[url]})";
                    var existingItem = QuarantinedUrlsList.Items.Cast<object>().Select(item => item.ToString()).FirstOrDefault(itemStr => itemStr.StartsWith(url));
                    if (existingItem != null)
                    {
                        QuarantinedUrlsList.Items.Remove(existingItem);
                    }
                    QuarantinedUrlsList.Items.Insert(0, formattedUrl);

                }
                var sortedUrls = QuarantinedUrlsList.Items.Cast<object>().Select(item => item.ToString())
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
                message = message.Replace(abbreviation.Key, $"{abbreviation.Key} <{abbreviation.Value}>");
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
            var sortedList = SIRList.Items.Cast<object>().Select(item => item.ToString())
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
        public class MessagesData
        {
            public List<string> SMSMessages { get; set; } = new List<string>();
            public List<string> EmailMessages { get; set; } = new List<string>();
            public List<string> TweetMessages { get; set; } = new List<string>();
            public List<string> Hashtags { get; set; } = new List<string>();
            public List<string> Mentions { get; set; } = new List<string>();
            public List<string> SIRList { get; set; } = new List<string>();
            public List<string> QuarantinedUrls { get; set; } = new List<string>();
            public int? SortCodeSegment1 { get; set; }
            public int? SortCodeSegment2 { get; set; }
            public int? SortCodeSegment3 { get; set; }
            public int? SmsMessageIDCounter { get; set; }
            public int? EmailMessageIDCounter { get; set; }
            public int? TweetMessageIDCounter { get; set; }
        }


        private void SaveMessagesToJson()
        {
            var smsMessages = smsOutputList.Items.Cast<object>().Select(item => item.ToString()).ToList();
            var emailMessages = emailOutputList.Items.Cast<object>().Select(item =>
            {
                if (item is ListBoxItem listBoxItem)
                {
                    return listBoxItem.Content as string;
                }
                return item as string;
            }).Where(email => !string.IsNullOrEmpty(email)).ToList();

            var tweetMessages = tweetOutputList.Items.Cast<object>().Select(item => item.ToString()).ToList();
            var hashtags = TrendingList.Items.Cast<object>().Select(item => item.ToString()).ToList();
            var mentions = MentionsList.Items.Cast<object>().Select(item => item.ToString()).ToList();
            var sirList = SIRList.Items.Cast<object>().Select(item => item.ToString()).ToList();
            var quarantinedUrls = QuarantinedUrlsList.Items.Cast<object>().Select(item => item.ToString()).ToList();

            MessagesData messages = new MessagesData
            {
                SMSMessages = smsMessages,
                EmailMessages = emailMessages,
                TweetMessages = tweetMessages,
                Hashtags = hashtags,
                Mentions = mentions,
                SIRList = sirList,
                QuarantinedUrls = quarantinedUrls,
                SortCodeSegment1 = sortCodeSegment1,
                SortCodeSegment2 = sortCodeSegment2,
                SortCodeSegment3 = sortCodeSegment3,
                SmsMessageIDCounter = smsMessageIDCounter,
                EmailMessageIDCounter = emailMessageIDCounter,
                TweetMessageIDCounter = tweetMessageIDCounter
            };

            string json = JsonConvert.SerializeObject(messages, Formatting.Indented);
            string filePath = @"C:\Users\SachaMiller\Downloads\test\messages.json";
            File.WriteAllText(filePath, json);
        }



    }
}
