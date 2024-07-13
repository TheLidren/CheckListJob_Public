using System.ComponentModel.DataAnnotations;

namespace CheckListJob.Models
{
    public class ShiftTask
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Данное поле не должно быть пустым")]
        [StringLength(100, ErrorMessage = "Введите корректно размерность", MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Укажите порядковый номер")]
        public int Number { get; set; }

        [Required(ErrorMessage = "Данное поле не должно быть пустым")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Данное поле не должно быть пустым")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Данное поле не должно быть пустым")]
        public TimeSpan FinishTime { get; set; }

        [Required(ErrorMessage = "Данное поле не должно быть пустым")]
        public int ShiftId { get; set; }
        public Shift? Shift { get; set; }
        public bool Status { get; set; }

        public bool NotifyUser { get; set; }

        public DateTime? LastAction { get; set; }

        public ICollection<ListLog>? ListLogs { get; set; }
        public ICollection<ShiftTaskHst>? ShiftTaskHsts { get; set; }
    }
}
