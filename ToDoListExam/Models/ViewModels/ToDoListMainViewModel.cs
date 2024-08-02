using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoListExam.ToDoList;

namespace ToDoListExam.Models.ViewModels
{
    public class ToDoListMainViewModel
    {
        public List<ToDoListItem>? Items { get; set; }
        public SelectList? Categories { get; set; }
        
        public int? SelectedCategoryId { get; set; }
        public string? SelectedEnded { get; set; }
        public string? SelectedSort { get; set; }
        public string? Search { get; set; }
    }
}
