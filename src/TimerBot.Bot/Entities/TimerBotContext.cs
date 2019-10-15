using Microsoft.EntityFrameworkCore;

namespace TimerBot.Bot.Entities
{
    public class TimerBotContext : DbContext
    {
        public TimerBotContext(DbContextOptions<TimerBotContext> options)
            : base(options)
        {
        }
    }
}
