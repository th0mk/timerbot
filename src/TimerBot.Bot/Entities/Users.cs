namespace TimerBot.Bot.Entities
{
    public class User
    {
        public long UserID { get; set; }

        public bool? Blacklisted { get; set; }

        public UserType UserType { get; set; }

        public string UserNameLastFM { get; set; }
    }
}
