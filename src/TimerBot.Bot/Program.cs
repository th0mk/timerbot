using System.Threading.Tasks;

namespace TimerBot.Bot
{
    internal class Program
    {
        public static Task Main(string[] args)
        {
            return Startup.RunAsync(args);
        }
    }
}
