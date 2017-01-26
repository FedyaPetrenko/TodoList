using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TodoList.Models;

namespace TodoList.Entities
{
    [Table("ToDoTasks")]
    public class ToDoTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        public bool IsDone { get; set; }

        public virtual Project Project { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}