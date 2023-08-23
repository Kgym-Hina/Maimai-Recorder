namespace RecordAppBlazor.Data;

public class Recording
{
    public string? Code { get; set; }
    public string? Path { get; set; }
    public FileStatus Status { get; set; }
}

public enum FileStatus
{
    Uploading,
    Ready
}