using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

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
                messageContent = messageType + " Phone Number: " + smsPhoneNumberInput.Text + "; Message: " + messageInput.Text;
            }
            else if (emailRadioButton.IsChecked == true)
            {
                messageType = "Email:";
                messageContent = messageType + " Recipient: " + emailRecipientInput.Text + "; Subject: " + emailSubjectInput.Text + "; Body: " + messageInput.Text;
                if (incidentCheckBox.IsChecked == true)
                {
                    messageContent += "; Incident Type: " + ((ComboBoxItem)incidentComboBox.SelectedItem).Content;
                }
            }
            else if (tweetRadioButton.IsChecked == true)
            {
                messageType = "Tweet:";
                messageContent = messageType + " Username: " + tweetUsernameInput.Text + "; Message: " + messageInput.Text;
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
                DisplayLists();
            }
        }

        private void DisplayLists()
        {
            TrendingList.Items.Clear();
            foreach (var item in TweetMessage.HashtagsDictionary.OrderByDescending(x => x.Value))
            {
                TrendingList.Items.Add(item.Key + ": " + item.Value);
            }

            MentionsList.Items.Clear();
            foreach (var item in TweetMessage.MentionsDictionary.OrderByDescending(x => x.Value))
            {
                MentionsList.Items.Add(item.Key + ": " + item.Value);
            }

            SIRList.Items.Clear();
            foreach (var item in EmailMessage.SIRList)
            {
                SIRList.Items.Add("Sort Code: " + item.SortCode + ", Nature of Incident: " + item.NatureOfIncident);
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
