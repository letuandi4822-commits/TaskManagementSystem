using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.ViewModel
{
    public class CreateProjectViewModel
    {
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string Description { get; set; } = " ";
    }
}
