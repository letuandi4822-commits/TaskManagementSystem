using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.ViewModel
{
    public class EditProjectViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } 
        [Required]
        public string Description { get; set; } 
    }
}
