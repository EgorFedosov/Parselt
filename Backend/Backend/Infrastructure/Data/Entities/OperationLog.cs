using System.Text.Json.Serialization;

namespace Backend.Infrastructure.Data.Entities;
using Core.Logging.Enums;


public class OperationLog
{
    public uint Id { get; set; }

    public Guid OperationId { get; set; } = Guid.NewGuid();
    public OperationType Type { get; set; }
    public OperationStatus Status { get; set; } = OperationStatus.Success;
    public DateTime StartedAt { get; set; } = DateTime.Now;
    public DateTime? FinishedAt { get; set; }
    public int? TotalRows { get; set; }
    public List<ErrorLog>? Errors { get; set; } = new();
    public uint FileMetaDataId { get; set; }
    [JsonIgnore]
    public FileMetaData FileMetaData { get; set; } = null!;
}

