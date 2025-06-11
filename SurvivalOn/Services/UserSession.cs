using SurvivalCL;

namespace SurvivalOn.Services
{
    public class UserSession
    {
        public string? Username { get; set; }
        public string? HashedPassword { get; set; }
        public Guid? CharGuid { get; set; }
        public Character? activeChar { get; set; }
        public GameState? stateOfGame { get; set; } 
    }
}
