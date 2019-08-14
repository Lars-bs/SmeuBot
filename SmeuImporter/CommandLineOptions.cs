using CommandLine;

namespace SmeuImporter
{
    public class CommandLineOptions
    {
        [Option('w', "whatsappLogDirectory", Required = true,
            HelpText = "The directory with the images and log of the whatsappConversation")]
        public string WhatsappLogDirectory { get; set; } = string.Empty;
    }
}