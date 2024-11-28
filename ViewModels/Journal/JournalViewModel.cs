using CheckListJob.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CheckListJob.ViewModels.Journal;
public class JournalViewModel
{
    public JournalViewModel(List<User> users, List<Shift> shifts)
    {
        shifts.Insert(0, new Shift { Id = 0, Name = "Все" });
        users.Insert(0, new User { Id = 0, Login = "Все" });
        Shifts = new SelectList(shifts, "Id", "Name");
        Users = new SelectList(users, "Id", "Login");
    }

    public IEnumerable<ListLog> Log { get; set; }
    public SelectList Users { get; set; }
    public SelectList Shifts { get; set; }
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }

    public int CountRows { get; set; }

    public JournalSort JournalSort { get; set; }

}
