using Newtonsoft.Json;

namespace TimerBot.Bot.Configurations
{
    public struct ConfigJson
    {
        [JsonProperty("token")] public string Token { get; set; }

        [JsonProperty("botowner")] public string BotOwner { get; set; }

        [JsonProperty("commandprefix")] public string CommandPrefix { get; set; }

        [JsonProperty("baseserver")] public string BaseServer { get; set; }

        [JsonProperty("exceptionchannel")] public string ExceptionChannel { get; set; }
    }
}
