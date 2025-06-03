using System;
using System.Collections.Generic;

namespace SurvivalOn.Models
{
    public class PlayerData
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // For demo only! Hash in production.
        public string Email { get; set; } = string.Empty;
        public List<Guid> PCIds { get; set; } = new();
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
    }
}