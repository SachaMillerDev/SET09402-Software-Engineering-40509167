using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class EmailMessage : Message
{
    public string SenderEmail { get; set; }
    public string Subject { get; set; }
    public string Text { get; set; }
    public List<string> QuarantineList { get; set; } = new List<string>();
    public string SortCode { get; set; }
    public string NatureOfIncident { get; set; }

    public static List<EmailMessage> SIRList = new List<EmailMessage>();


    public override string DetectType() => "Email";

    public void RemoveURLs()
    {
        foreach (Match match in Regex.Matches(Text, @"https?:"))
        {
            QuarantineList.Add(match.Value);
            Text = Text.Replace(match.Value, "<URL Quarantined>");
        }
    }

    public override void Process()
    {
        RemoveURLs();
        ExtractSIRDetails();
    }

    public void ExtractSIRDetails()
    {
        if (IsSIR())
        {
            SortCode = Regex.Match(Text, @"Sort Code: (\d{2}-\d{2}-\d{2})").Groups[1].Value;
            NatureOfIncident = Regex.Match(Text, @"Nature of Incident: (.+)").Groups[1].Value.Trim();
            SIRList.Add(this);
        }
    }
}

