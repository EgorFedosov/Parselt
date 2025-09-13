using Backend.Core.Interfaces;
using Backend.Core.Logging.Enums;
using Backend.Infrastructure.Data.Entities;

namespace Backend.Infrastructure.Logging;

public class LoggingService : ILoggingService
{
    private readonly List<OperationLog> _operations = new();

    public void RegisterOperation(OperationLog operation)
    {
        _operations.Add(operation);
    }

    public void AttachMetaData(OperationLog operation, FileMetaData metaData)
    {
        operation.FileMetaData = metaData;
        operation.FileMetaDataId = metaData.Id;
        metaData.OperationLogs.Add(operation);
    }


    public void AddError(Guid operationId, string message)
    {
        var operation = _operations.FirstOrDefault(operation => operation.OperationId == operationId);
        if (operation != null)
        {
            operation.Errors?.Add(new ErrorLog()
            {
                Message = message
            });
            operation.Status = OperationStatus.Partial;
        }
    }

    public OperationLog? GetOperation(Guid operationId)
    {
        return _operations.FirstOrDefault(op => op.OperationId == operationId);
    }
}