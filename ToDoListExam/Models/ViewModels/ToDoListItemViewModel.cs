using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoListExam.ToDoList;

namespace ToDoListExam.Models.ViewModels
{
    public class ToDoListItemViewModel
    {
        public ToDoListItem ToDoListItem { get; set; }
        public SelectList? Categories { get; set; }
    }
}
