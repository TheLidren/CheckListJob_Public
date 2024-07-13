namespace CheckListJob.Models
{
    public class ShiftTaskHst
    {
        public int Id { get; set; }

        public int ShiftTaskId {  get; set; }
        public ShiftTask? ShiftTask { get; set; }
        public DateTime? LastNotify { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }
    }
}
