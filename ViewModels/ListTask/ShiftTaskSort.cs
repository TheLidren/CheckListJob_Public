namespace CheckListJob.ViewModels.ListTask;
public class  ShiftTaskSort(ShiftTaskEnum shiftTaskEnum)
{
    public ShiftTaskEnum StartTimeSort { get; set; } = shiftTaskEnum == ShiftTaskEnum.StartTimeAsc ? ShiftTaskEnum.StartTimeDesc : ShiftTaskEnum.StartTimeAsc;
    public ShiftTaskEnum FinishTimeSort { get; set; } = shiftTaskEnum == ShiftTaskEnum.FinishTimeAsc ? ShiftTaskEnum.FinishTimeDesc : ShiftTaskEnum.FinishTimeAsc;
    public ShiftTaskEnum NumberSort { get; set; } = shiftTaskEnum == ShiftTaskEnum.NumberAsc? ShiftTaskEnum.NumberDesc : ShiftTaskEnum.NumberAsc;
    public ShiftTaskEnum CurrentSort { get; set; } = shiftTaskEnum;
}
public enum ShiftTaskEnum
{
    StartTimeAsc, StartTimeDesc,
    FinishTimeAsc, FinishTimeDesc,
    NumberAsc, NumberDesc
}
