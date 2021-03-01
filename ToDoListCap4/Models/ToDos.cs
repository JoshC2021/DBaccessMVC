using System;
using System.Collections.Generic;

namespace ToDoListCap4.Models
{
    public partial class ToDos
    {
        public int Id { get; set; }
        public string ItemDesciption { get; set; }
        public DateTime DueDate { get; set; }
        public bool? IsComplete { get; set; }
        public string UserId { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
