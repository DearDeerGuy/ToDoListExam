using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListExam.ToDoList
{
    public class ToDoListItem
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;

        public string Name { get; set; } = default!;
        
        public string Description { get; set; } = default!;

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; } = default!;

        public bool IsEnded { get; set; } = false;

        private int priority = 1;
        public int Priority { get { return priority; } set { priority = (value >= 1 && value <= 5 ) ? value : 3; } }

        public DateTime CreationDate { get; set; } = DateTime.Now;
        
        private DateTime completeDate = DateTime.Now.AddDays(1);
        public DateTime CompleteDate { get { return completeDate; } set { completeDate = (value < CreationDate) ? DateTime.Now : value; } }
    }
}
