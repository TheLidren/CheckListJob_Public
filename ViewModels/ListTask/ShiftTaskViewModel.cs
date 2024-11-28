using CheckListJob.Models;

namespace CheckListJob.ViewModels.ListTask;
public class ShiftTaskViewModel
{
    public IEnumerable<ShiftTask> ShiftTasks { get; set; }

    public ShiftTaskSort ShiftTaskSort { get; set; }

    public int ShiftId { get; set; }
}
