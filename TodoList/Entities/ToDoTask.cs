using TodoList.Models;

namespace TodoList.Entities
{
    public class ToDoTask
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public bool IsDone { get; set; }

        public virtual Project Project { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}