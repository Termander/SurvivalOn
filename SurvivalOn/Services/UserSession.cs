namespace SurvivalOn.Services
{
    public class UserSession
    {
        public string? Username { get; set; }
        public string? HashedPassword { get; set; }
        public Guid? CharGuid { get; set; }

    }
}
