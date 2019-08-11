using Discord;
using SimpleSettings;

namespace SmeuArchief
{
    public class Settings
    {
        [Description("Enter here your discord bot token.")]
        [Group("Discord")]
        public string Token { get; set; }

        [Description("How much log messages do you want to receive?"), Default(LogSeverity.Debug)]
        [Group("Discord")]
        public LogSeverity LogLevel { get; set; }

        [Description("Use this character combination to indicate that the message is a command"), Default("c!")]
        [Group("Discord")]
        public string CommandPrefix { get; set; }

        [Description("In which channel should the smeu bot watch for smeu?")]
        public ulong SmeuChannelId { get; set; }
    }
}
