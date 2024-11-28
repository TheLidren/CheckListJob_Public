using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CheckListJob.ViewModels;
public class ResetPasswordViewModel
{
    [Required]
    public string Login { get; set; } = null!;

    [Required(ErrorMessage = "Не указан пароль")]
    [StringLength(255, ErrorMessage = "Пароль должен содержать от 8 символов", MinimumLength = 8)]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Не указан пароль")]
    [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string PasswordConfirm { get; set; } = null!;
}
