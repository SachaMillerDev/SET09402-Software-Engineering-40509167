using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

public class JSONOutput
{
    public string OutputFile { get; set; }

    public void WriteToJSON(List<Message> messages) // Accepts a list of messages
    {
        string json = JsonConvert.SerializeObject(messages);
        File.WriteAllText(OutputFile, json);
    }
}
