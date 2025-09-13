namespace Backend.Application.DTOs.Logging;
using Core.Logging.Enums;

public class OperationLogDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public OperationType Type { get; set; }
    public OperationStatus Status { get; set; } = OperationStatus.Success;
    public DateTime StartedAt { get; set; } = DateTime.Now;
    public DateTime? FinishedAt { get; set; }
    public int? TotalRows { get; set; }
    public List<ErrorLogDto>? Errors { get; set; } = new();
}
