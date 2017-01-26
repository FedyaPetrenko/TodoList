using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
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

        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
    }
}