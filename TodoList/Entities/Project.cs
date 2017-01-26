﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;
using TodoList.Models;

namespace TodoList.Entities
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        public virtual IList<ToDoTask> Tasks { get; set; }

        public virtual ApplicationUser User { get; set; }

        public Project()
        {
            Tasks = new List<ToDoTask>();
        }
    }
}