using Backend.Infrastructure.Data;

namespace Backend.Endpoints.Parse;

using Application.DTOs.Request;
using Core.Interfaces;

public class ParseEndpoint
{
    public static void MapPreview(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("/api/preview/{fileName}",
            async (ICsvDynamicParser parser, string fileName) =>
            {
                try
                {
                    var previewResponse = await parser.PreviewAsync(fileName);
                    return Results.Ok(previewResponse);
                }
                catch (FileNotFoundException ex)
                {
                    return Results.NotFound(new { error = ex.Message });
                }
                catch (Exception)
                {
                    return Results.StatusCode(500);
                }
            });
    }

    public static void MapParse(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("/api/parse",
            async (CsvParserRequestDto request, ICsvDynamicParser parser, ILoggingService loggingService,
                IParseResultCache cache, ApplicationContext db) =>
            {
                try
                {
                    var (result, operation) = await parser.ParseCsvAsync(request, loggingService);
                    var metaData = await db.FileMetaData.FindAsync(request.FileId);

                    if (metaData == null)
                        return Results.NotFound(new
                        {
                            code = "FILE_NOT_FOUND",
                            message = $"File with id {request.FileId} was not found"
                        });
                    
                    loggingService.AttachMetaData(operation, metaData);
                    var key = cache.Store(result);
                    return Results.Ok(new { result, key });
                }
                catch (FileNotFoundException exception)
                {
                    return Results.NotFound(new { error = exception.Message });
                }
                catch (FormatException exception)
                {
                    return Results.BadRequest(new { error = exception.Message });
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return Results.Problem(exception.InnerException?.Message ?? exception.Message);
                }
            });
    }

    public static void MapGetLog(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("api/log/{operationId}",
            (Guid operationId, ILoggingService loggingService) =>
            {
                var log = loggingService.GetOperation(operationId);
                if (log == null)
                    return Results.NotFound();

                return Results.Ok(log);
            });
    }
}