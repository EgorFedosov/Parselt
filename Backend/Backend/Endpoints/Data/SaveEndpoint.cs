using Backend.Core.Interfaces;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Data.Entities;
using Backend.Infrastructure.Data.Uploads;
using Backend.Infrastructure.Utils;

namespace Backend.Endpoints.Data;

public static class SaveEndpoint
{
    public static void MapSaveResult(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/parse/save/result/{key}/{fileMetaId}/{operationId}",
            async (string key, uint fileMetaId, Guid operationId, IParseResultCache cache, ApplicationContext db,
                ILoggingService loggingService) =>
            {
                var result = cache.Retrieve(key);
                if (result == null)
                    return Results.BadRequest(new { error = "Parsed result not found or cache has expired." });
                if (result.Rows.Count == 0)
                    return Results.BadRequest(new { error = "No data to save." });

                var metaData = await db.FileMetaData.FindAsync(fileMetaId);
                if (metaData == null)
                    return Results.NotFound(new { error = "FileMetaData not found" });

                var mapper = new CsvParsedCellMapper();
                var allCells = new List<CsvParsedCell>();

                foreach (var row in result.Rows)
                {
                    var cells = mapper.Map(row, metaData);
                    allCells.AddRange(cells);
                }

                await db.CsvParsedCells.AddRangeAsync(allCells);

                var operation = loggingService.GetOperation(operationId);
                if (operation == null)
                    return Results.NotFound(new { error = "operation not found" });

                var operationEntity = new OperationLog
                {
                    Id = operation.Id,
                    Type = operation.Type,
                    Status = operation.Status,
                    StartedAt = operation.StartedAt,
                    FinishedAt = operation.FinishedAt,
                    TotalRows = operation.TotalRows,
                    Errors = operation.Errors?.Select(e => new ErrorLog
                    {
                        Message = e.Message,
                        OperationLogId = operation.Id
                    }).ToList() ?? new List<ErrorLog>(),
                    OperationId = operation.OperationId,
                    FileMetaDataId = metaData.Id,
                    FileMetaData = metaData
                };

                await db.OperationLogs.AddAsync(operationEntity);
                await db.SaveChangesAsync();

                cache.Remove(key);

                var fullPath = Path.Combine(FilePathService.UploadPath, metaData.FileName);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                return Results.Ok(new { message = $"Rows saved: {allCells.Count}" });
            });
    }
}