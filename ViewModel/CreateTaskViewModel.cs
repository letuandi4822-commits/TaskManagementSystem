using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.ViewModel
{
    public class CreateTaskViewModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Status {  get; set; }

    

        public DateTime DueDate { get; set; }

        public int AssignedUserId { get; set; }

    }
}
