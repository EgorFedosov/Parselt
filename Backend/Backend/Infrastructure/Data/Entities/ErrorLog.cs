namespace Backend.Infrastructure.Data.Entities;

public class ErrorLog
{
    public int Id { get; set; }
    public uint OperationLogId { get; set; }
    public OperationLog OperationLog { get; set; } = null!;
    public string Message { get; set; } = string.Empty;
}
