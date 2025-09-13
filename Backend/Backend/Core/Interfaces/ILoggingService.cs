using Backend.Application.DTOs.Logging;
using Backend.Infrastructure.Data.Entities;

namespace Backend.Core.Interfaces;

public interface ILoggingService
{
    public void RegisterOperation(OperationLog operation);
    public void AttachMetaData(OperationLog operation, FileMetaData metaData);
    public void AddError(Guid operationId, string message);
    OperationLog? GetOperation(Guid operationId);
}