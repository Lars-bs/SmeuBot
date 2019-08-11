using Discord;
using SimpleSettings;

namespace SmeuArchief
{
    public class Settings
    {
        [Description("Enter here your discord bot token.")]
        public string Token { get; set; }

        [Description("How much log messages do you want to receive?"), Default(LogSeverity.Debug)]
        public LogSeverity LogLevel { get; set; }

        [Description("Use this character combination to indicate that the message is a command"), Default("c!")]
        public string CommandPrefix { get; set; }
    }
}
