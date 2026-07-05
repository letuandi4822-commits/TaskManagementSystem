namespace TaskManagementSystem.Models
{
    public class TaskAttachment
    {
        public int Id { get; set; }

        public int TaskId { get; set; }

        public TaskItem Task { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}
