using System;

namespace SET09402_Software_Engineering_SachaMiller_40509167
{
    internal class main
    {
        static void Main(string[] args)
        {
            MessageProcessor processor = new MessageProcessor();
            MessageReader reader = new MessageReader(@"C:\Users\SachaMiller\source\repos\SET09402-Software Engineering-SachaMiller-40509167\SET09402-Software Engineering-SachaMiller-40509167\TestFile.txt");
            JSONOutput jsonOutput = new JSONOutput { OutputFile = @"C:\Users\SachaMiller\source\repos\SET09402-Software Engineering-SachaMiller-40509167\SET09402-Software Engineering-SachaMiller-40509167\Output.json" };

            // Read and process messages
            Message message = reader.ReadMessage();
            while (message != null)
            {
                processor.AddMessage(message);
                message = reader.ReadMessage();
            }

            processor.ProcessMessages();

            // Debugging output
            Console.WriteLine($"Number of processed messages: {processor.Messages.Count}");
            if (processor.Messages.Count > 0)
            {
                Console.WriteLine($"First processed message: {processor.Messages[0]}");
                string jsonTest = Newtonsoft.Json.JsonConvert.SerializeObject(processor.Messages[0]);
                Console.WriteLine($"First message serialized to JSON: {jsonTest}");
            }

            // Output to JSON
            jsonOutput.WriteToJSON(processor.Messages);

            Console.WriteLine("Processing complete!");
        }

    }
}
