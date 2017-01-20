using System.Collections.Generic;

namespace TodoList.Entities
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual IList<ToDoTask> Tasks { get; set; }

        public virtual IList<Project> Projects { get; set; }

        public Project()
        {
            Tasks = new List<ToDoTask>();
            Projects = new List<Project>();
        }
    }
}