using CheckListJob.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CheckListJob.ViewModels.AdminShift
{
    public class AdminShiftViewModel
    {

        public AdminShiftViewModel(List<Shift> shifts)
        {
            shifts.Insert(0, new Shift { Id = 0, Name = "Все" });
            Shifts = new SelectList(shifts, "Id", "Name");
        }

        public IEnumerable<ShiftTask> ShiftTasks { get; set; }
        public string? Title { get; set; }
        public SelectList Shifts { get; set; }

        public short SelectedShift { get; set; }

        public AdminShiftSort AdminShiftSort { get; set; }
    }
}
