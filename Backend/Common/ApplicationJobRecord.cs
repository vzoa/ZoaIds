namespace ZoaIdsBackend.Common;

public class ApplicationJobRecord
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string Caller { get; set; } = string.Empty;
    public string JobKey { get; set; } = string.Empty;
    public string JobValue { get; set; } = string.Empty;
    public ExitStatus Status { get; set; }

    public enum ExitStatus
    {
        Success,
        Failure
    }
}


