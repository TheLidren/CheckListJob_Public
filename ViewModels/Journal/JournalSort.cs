namespace CheckListJob.ViewModels.Journal;
public class JournalSort(JournalEnum journalEnum)
{
    public JournalEnum MarkActionSort { get; set; } = journalEnum == JournalEnum.MarkActionAsc ? JournalEnum.MarkActionDesc : JournalEnum.MarkActionAsc;
    public JournalEnum EmailSort { get; set; } = journalEnum == JournalEnum.EmailAsc ? JournalEnum.EmailDesc : JournalEnum.EmailAsc;
    public JournalEnum ShiftSort { get; set; } = journalEnum == JournalEnum.ShiftAsc ? JournalEnum.ShiftDesc : JournalEnum.ShiftAsc;

    public JournalEnum SelectedSort { get; set; } = journalEnum;
}
public enum JournalEnum
{
    MarkActionAsc, MarkActionDesc,
    EmailAsc, EmailDesc,
    ShiftAsc, ShiftDesc,
}
