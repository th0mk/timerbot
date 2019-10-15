using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimerBot.Bot.Handlers;
using TimerBot.Bot.Services;

namespace TimerBot.Bot
{
    public class Startup
    {
        public Startup(string[] args)
        {
            var builder = new ConfigurationBuilder(); // Create a new instance of the config builder
            Configuration = builder.Build(); // Build the configuration
        }

        private IConfigurationRoot Configuration { get; }

        public static async Task RunAsync(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            var startup = new Startup(args);
            await startup.RunAsync();
        }

        private async Task RunAsync()
        {
            var services = new ServiceCollection(); // Create a new instance of a service collection

            ConfigureServices(services);

            var provider = services.BuildServiceProvider(); // Build the service provider
            //provider.GetRequiredService<LoggingService>();      // Start the logging service
            provider.GetRequiredService<CommandHandler>(); // Start the command handler service

            await provider.GetRequiredService<StartupService>().StartAsync(); // Start the startup service
            await Task.Delay(-1); // Keep the program alive
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var discordClient = new DiscordShardedClient(new DiscordSocketConfig
            {
                // Add discord to the collection
                LogLevel = LogSeverity.Verbose, // Tell the logger to give Verbose amount of info
                MessageCacheSize = 0
            });

            var logger = new Logger.Logger();

            services
                .AddSingleton(discordClient)
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    DefaultRunMode = RunMode.Async
                }))
                .AddSingleton<CommandHandler>()
                .AddSingleton<StartupService>()
                .AddSingleton(logger)

                //.AddSingleton<LoggingService>()         // Add loggingservice to the collection
                .AddSingleton<Random>()
                .AddSingleton(Configuration);


            // Dit is de job shit. Raak het niet aan, ik ga het nog al fixen
            // Voor de tussentijd kan je gewoon met timers kutten

            //GlobalConfiguration.Configuration
            //    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //    .UseColouredConsoleLogProvider()
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseRecommendedSerializerSettings()
            //    .UsePostgreSqlStorage("Host=localhost;Port=5432;Username=postgres;Password=password;Database=timerbot;Command Timeout=15;Timeout=30", new PostgreSqlStorageOptions
            //    {
            //        PrepareSchemaIfNecessary = true
            //    });

            ////Fire-and-Forget
            //BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget"));
            ////Delayed
            //BackgroundJob.Schedule(() => Console.WriteLine("Delayed"), TimeSpan.FromDays(1));
            ////Recurring
            //RecurringJob.AddOrUpdate(() => Console.WriteLine("Minutely Job"), Cron.Minutely);
            ////Continuation
            //var id = BackgroundJob.Enqueue(() => Console.WriteLine("Hello, "));
            //BackgroundJob.ContinueWith(id, () => Console.WriteLine("world!"));
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Unhandled exception! \n \n" + e.ExceptionObject + "\n", ConsoleColor.Red);

            var logger = new Logger.Logger();
            logger.Log("UnhandledException! \n \n" + e.ExceptionObject + "\n");
        }
    }
}
