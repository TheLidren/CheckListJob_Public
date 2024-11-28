namespace CheckListJob.ViewModels.AdminShift;
public class AdminShiftSort(AdminShiftEnum adminShiftEnum)
{
    public AdminShiftEnum StartTimeSort { get; set; } = adminShiftEnum == AdminShiftEnum.StartTimeAsc ? AdminShiftEnum.StartTimeDesc : AdminShiftEnum.StartTimeAsc;
    public AdminShiftEnum ShiftSort { get; set; } = adminShiftEnum == AdminShiftEnum.ShiftAsc ? AdminShiftEnum.ShiftDesc : AdminShiftEnum.ShiftAsc;
    public AdminShiftEnum StatusSort { get; set; } = adminShiftEnum == AdminShiftEnum.StatusAsc ? AdminShiftEnum.StatusDesc : AdminShiftEnum.StatusAsc;
    public AdminShiftEnum NotifySort { get; set; } = adminShiftEnum == AdminShiftEnum.NotifyAsc ? AdminShiftEnum.NotifyDesc : AdminShiftEnum.NotifyAsc;
    public AdminShiftEnum NumberSort { get; set; } = adminShiftEnum == AdminShiftEnum.NumberAsc ? AdminShiftEnum.NumberDesc : AdminShiftEnum.NumberAsc;

    public AdminShiftEnum CurrentSort { get; set; } = adminShiftEnum;
}

public enum AdminShiftEnum
{
    StartTimeAsc, StartTimeDesc, 
    ShiftAsc, ShiftDesc,
    StatusAsc, StatusDesc,
    NotifyAsc, NotifyDesc,
    NumberAsc, NumberDesc
}