using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TimerBot.Bot.Configurations;

namespace TimerBot.Bot.Services
{
    public class StartupService
    {
        private readonly CommandService _commands;
        private readonly DiscordShardedClient _discord;
        private readonly IServiceProvider _provider;

        public StartupService(
            IServiceProvider provider,
            DiscordShardedClient discord,
            CommandService commands)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Starting bot...");

            var discordToken = ConfigData.Data.Token; // Get the discord token from the config file
            if (string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("Please enter your bots token into the `/Configs/ConfigData.json` file.");

            await _discord.LoginAsync(TokenType.Bot, discordToken); // Login to discord
            await _discord.StartAsync(); // Connect to the websocket

            await _discord.SetGameAsync("🎶 Say " + ConfigData.Data.CommandPrefix + "fmhelp to use 🎶");
            await _discord.SetStatusAsync(UserStatus.DoNotDisturb);

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(),
                _provider); // Load commands and modules into the command service
        }
    }
}
