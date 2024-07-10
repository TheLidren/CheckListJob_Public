using System.ComponentModel.DataAnnotations;

namespace CheckListJob.Models
{
    public class Shift
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указано название")]
        [RegularExpression(@"^([А-ЯЁA-Zа-яёa-z0-9\s]{1,50})$", ErrorMessage = "Некорректное название")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Данное поле не должно быть пустым")]
        public string Description { get; set; } = null!;


        [Required(ErrorMessage = "Данное поле не должно быть пустым")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Данное поле не должно быть пустым")]
        public TimeSpan FinishTime { get; set; }

        public ICollection<ShiftTask>? ShiftTasks { get; set; }
    }
}
