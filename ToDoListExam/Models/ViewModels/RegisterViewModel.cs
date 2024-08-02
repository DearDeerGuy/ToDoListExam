using System.ComponentModel.DataAnnotations;

namespace ToDoListExam.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Login")]
        [Required]
        public string Login { get; set; } = default!;


        [Display(Name = "Email")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;

        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;


        [Display(Name = "Confirm Password")]
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords should match!")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
