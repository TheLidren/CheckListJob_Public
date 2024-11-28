using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckListJob.Models;
public class User
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Не указано имя")]
    [RegularExpression(@"^([А-ЯЁA-Z]{1}[а-яёa-z]{1,50})$", ErrorMessage = "Латиница или кирилица с большой буквы")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Не указана фамилия")]
    [RegularExpression(@"^([А-ЯЁA-Z]{1}[а-яёa-z]{1,50})$", ErrorMessage = "Латиница или кирилица с большой буквы")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
    public string Surname { get; set; } = null!;

    [Required(ErrorMessage = "Не указано отчество")]
    [RegularExpression(@"^([А-ЯЁA-Z]{1}[а-яёa-z]{1,50})$", ErrorMessage = "Латиница или кирилица с большой буквы")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
    public string Patronymic { get; set; } = null!;

    [Required(ErrorMessage = "Не указан логин")]
    [RegularExpression(@"[A-Za-z0-9._%+-]{1,50}$", ErrorMessage = "Некорректный логин")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Длина строки должна быть до 20 символов")]
    public string Login { get; set; } = null!;

    [Required(ErrorMessage = "Не указан пароль")]
    [StringLength(255, ErrorMessage = "Пароль должен содержать от 8 символов", MinimumLength = 8)]
    public string Password { get; set; } = null!;

    [NotMapped]
    [Required(ErrorMessage = "Не указан пароль")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string PasswordConfirm { get; set; } = null!;
    public int RoleId { get; set; }
    public Role? Role { get; set; }
    public bool Status { get; set; }

    public ICollection<ListLog>? ListLogs { get; set; }

}

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<User>? Users { get; set; }
}
