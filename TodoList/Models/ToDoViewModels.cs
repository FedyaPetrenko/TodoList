using System.ComponentModel.DataAnnotations;
using TodoList.Entities;

namespace TodoList.Models
{
    public class ToDoTaskViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Description { get; set; }

        public bool IsDone { get; set; }

        [Display(Name = "Project name")]
        [StringLength(30, ErrorMessage = "The project name is required.", MinimumLength = 1)]
        public string ProjectName { get; set; }
    }

    public class ProjectViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Description { get; set; }
    }
}