namespace ZoaIds.Server.Data;

public class ApplicationJob
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string Caller { get; set; }
    public string JobKey { get; set; }
    public string JobValue { get; set; }
    public JobExitStatus ExitStatus { get; set; }
}

public enum JobExitStatus
{
    Success,
    Failure,
}