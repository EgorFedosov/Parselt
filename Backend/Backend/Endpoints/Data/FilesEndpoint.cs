using Backend.Application.DTOs.Response;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Endpoints.Data;

public class FilesEndpoint
{
    public static void MapGetFiles(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/files", async (ApplicationContext db, string? fileName) =>
        {
            var query = db.FileMetaData
                .Where(f => string.IsNullOrEmpty(fileName) || f.FileName.Contains(fileName))
                .Select(f => new
                {
                    fileName = f.FileName,
                    uploadedAt = f.UploadedAt,
                    size = f.Size,
                    id = f.Id
                });

            var files = await query.ToListAsync();

            if (!files.Any())
                return Results.NotFound("Files not found");

            return Results.Ok(files);
        });
    }

    public static void MapGetRaw(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/files/{fileId}/raw", async (ApplicationContext db, uint fileId) =>
        {
            var rawCells = await db.CsvRawCells
                .Where(c => c.FileMetaDataId == fileId)
                .Select(c => c.Value)
                .ToListAsync();

            if (!rawCells.Any())
                return Results.NotFound("Files not found");

            return Results.Ok(new
            {
                rawRows = rawCells
            });
        });
    }

    public static void MapDownloadRaw(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/files/{fileId}/download-row",
            async (ApplicationContext db, HttpContext http, uint fileId) =>
            {
                var file = await db.FileMetaData.FirstOrDefaultAsync(f => f.Id == fileId);
                if (file == null)
                    return Results.NotFound(new { message = "File not found" });

                var rows = await db.CsvRawCells
                    .Where(c => c.FileMetaDataId == fileId)
                    .Select(c => c.Value)
                    .ToListAsync();

                var csv = string.Join("\n", rows);
                var fileName = file.FileName ?? "file.csv";

                http.Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");
                return Results.File(
                    System.Text.Encoding.UTF8.GetBytes(csv),
                    "text/csv",
                    fileName
                );
            });
    }


    public static void MapDownloadResult(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/files/{operationId}/download-result",
            async (ApplicationContext db, HttpContext http, Guid operationId) =>
            {
                var rowsWithFile = await db.CsvParsedCells
                    .AsNoTracking()
                    .Where(c => c.OperationId == operationId)
                    .Select(c => new
                    {
                        c.RowIndex,
                        c.ColumnName,
                        c.Value,
                        FileName = c.FileMetaData.FileName
                    })
                    .ToListAsync();

                if (!rowsWithFile.Any())
                    return Results.NotFound();

                var fileName = rowsWithFile[0].FileName;
                var csv = string.Join("\n", rowsWithFile.Select(r => $"{r.RowIndex},{r.ColumnName},{r.Value}"));

                http.Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");

                return Results.File(
                    System.Text.Encoding.UTF8.GetBytes(csv),
                    "text/csv",
                    fileName
                );
            });
    }

    public static void MapGetLogs(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/files/{fileId}/logs", async (ApplicationContext db, uint fileId) =>
        {
            var logs = await db.OperationLogs
                .Where(l => l.FileMetaDataId == fileId)
                .Select(l => new
                {
                    id = l.Id,
                    type = l.Type,
                    status = l.Status,
                    startAt = l.StartedAt,
                    finishAt = l.FinishedAt,
                    totalRows = l.TotalRows,
                    operationId = l.OperationId,
                })
                .ToListAsync();

            if (!logs.Any())
                return Results.NotFound("Logs not found");

            return Results.Ok(new { fileId, operations = logs });
        });
    }

    public static void MapGetLogsErrors(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/files/{operationId}/logs/errors",
            async (ApplicationContext db, Guid operationId) =>
            {
                var operation = await db.OperationLogs
                    .Include(l => l.FileMetaData)
                    .FirstOrDefaultAsync(l => l.OperationId == operationId);

                if (operation == null)
                {
                    return Results.NotFound("Operation not found");
                }

                var errors = await db.ErrorLogs
                    .Where(l => l.OperationLogId == operation.Id)
                    .Select(l => new
                    {
                        message = l.Message,
                    }).ToListAsync();

                return Results.Ok(new { operationId, errors, operation.FileMetaData.FileName });
            });
    }

    public static void MapGetResults(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/files/{operationId}/results",
            async (ApplicationContext db, Guid operationId) =>
            {
                var cells = await db.CsvParsedCells
                    .Where((c) => c.OperationId == operationId)
                    .ToListAsync();
                var grouped = cells.GroupBy(c => c.RowIndex);
                var result = grouped.Select(group => new CsvParsedRowDto
                {
                    RowIndex = group.Key,
                    OperationId = group.First().OperationId,
                    ParsedValues = group.ToDictionary(
                        cell => cell.ColumnName,
                        cell => (object?)cell.Value
                    ),
                    IsValid = true
                }).ToList();

                return Results.Ok(result);
            });
    }
}