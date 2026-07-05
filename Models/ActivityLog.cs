namespace TaskManagementSystem.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Action { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
            = DateTime.Now;

        public User User { get; set; }

        public string Module { get; set; } = string.Empty;
    }
}
