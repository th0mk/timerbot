using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hangfire;
using TimerBot.Bot.Configurations;
using TimerBot.Bot.Extensions;

namespace TimerBot.Bot.Commands
{
    public class StaticCommands : ModuleBase
    {
        private readonly CommandService _service;

        public StaticCommands(CommandService service)
        {
            _service = service;
        }


        [Command("test")]
        [Summary("Invites the bot to a server")]
        public async Task testAsync()
        {
            try
            {
                RecurringJob.AddOrUpdate(() => Console.WriteLine("Minutely Job"), Cron.Minutely);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            var builder = new EmbedBuilder();

            builder.AddField("Aan het spanjolen..", "asdf");

            await Context.Channel.SendMessageAsync("", false, builder.Build());

            //Recurring
            //RecurringJob.AddOrUpdate(() => Context.Channel.SendMessageAsync("", false, builder.Build()), Cron.Minutely);
        }


        [Command("invite")]
        [Summary("Invites the bot to a server")]
        [Alias("fmserver")]
        public async Task inviteAsync()
        {
            var builder = new EmbedBuilder();

            var SelfID = Context.Client.CurrentUser.Id.ToString();

            builder.AddField("Invite the bot to your own server with the link below:",
                "https://discordapp.com/oauth2/authorize?client_id=" + SelfID + "&scope=bot&permissions=268707926");

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("status")]
        [Summary("Displays bot stats.")]
        public async Task statusAsync()
        {
            var SelfUser = Context.Client.CurrentUser;

            var eab = new EmbedAuthorBuilder
            {
                IconUrl = SelfUser.GetAvatarUrl(),
                Name = SelfUser.Username
            };

            var builder = new EmbedBuilder();
            builder.WithAuthor(eab);

            var startTime = DateTime.Now - Process.GetCurrentProcess().StartTime;

            var client = Context.Client as DiscordShardedClient;

            var SocketSelf = Context.Client.CurrentUser as SocketSelfUser;

            var status = "Online";

            switch (SocketSelf.Status)
            {
                case UserStatus.Offline:
                    status = "Offline";
                    break;
                case UserStatus.Online:
                    status = "Online";
                    break;
                case UserStatus.Idle:
                    status = "Idle";
                    break;
                case UserStatus.AFK:
                    status = "AFK";
                    break;
                case UserStatus.DoNotDisturb:
                    status = "Do Not Disturb";
                    break;
                case UserStatus.Invisible:
                    status = "Invisible/Offline";
                    break;
            }

            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            builder.AddField("Bot Uptime: ", startTime.ToReadableString(), true);
            builder.AddField("Discord usercount: ", client.Guilds.Select(s => s.MemberCount).Sum(), true);
            builder.AddField("Servercount: ", client.Guilds.Count, true);
            builder.AddField("Bot status: ", status, true);
            builder.AddField("Latency: ", client.Latency + "ms", true);
            builder.AddField("Shards: ", client.Shards.Count, true);
            builder.AddField("Bot version: ", assemblyVersion, true);

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }


        [Command("help")]
        [Summary("Quick help summary to get started.")]
        [Alias("TimerBot")]
        public async Task fmhelpAsync()
        {
            var prefix = ConfigData.Data.CommandPrefix;

            var builder = new EmbedBuilder
            {
                Title = prefix + "TimerBot Quick Start Guide"
            };

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }


        [Command("fullhelp")]
        [Summary("Displays this list.")]
        public async Task fmfullhelpAsync()
        {
            var prefix = ConfigData.Data.CommandPrefix;

            var SelfUser = Context.Client.CurrentUser;

            string description = null;
            var length = 0;

            var builder = new EmbedBuilder();

            foreach (var module in _service.Modules.OrderByDescending(o => o.Commands.Count()).Where(w =>
                !w.Name.Contains("SecretCommands") && !w.Name.Contains("OwnerCommands") &&
                !w.Name.Contains("AdminCommands") && !w.Name.Contains("GuildCommands")))
            {
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                    {
                        if (!string.IsNullOrWhiteSpace(cmd.Summary))
                            description += $"{prefix}{cmd.Aliases.First()} - {cmd.Summary}\n";
                        else
                            description += $"{prefix}{cmd.Aliases.First()}\n";
                    }
                }


                if (description.Length < 1024)
                    builder.AddField
                    (module.Name + (module.Summary != null ? " - " + module.Summary : ""),
                        description != null ? description : "");


                length += description.Length;
                description = null;

                if (length < 1990)
                {
                    await Context.User.SendMessageAsync("", false, builder.Build());

                    builder = new EmbedBuilder();
                    length = 0;
                }
            }

            await Context.User.SendMessageAsync("", false, builder.Build());
        }
    }
}
