using TaskManagementSystem.Models;

namespace TaskManagementSystem.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "To Do";

        public DateTime DueDate { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public int AssignedUserId { get; set; }

        public User AssignedUser { get; set; }

        public ICollection<TaskAttachment> Attachment { get; set; } = new List<TaskAttachment>();
        
    }
}
