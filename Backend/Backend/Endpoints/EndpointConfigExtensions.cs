namespace Backend.Endpoints;

using Backend.Infrastructure.Data;
using Upload;
using Parse;
using Data;

public static class EndpointConfigExtensions
{
    public static void MapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        UploadEndpoint.MapUpload(endpoints);
        ParseEndpoint.MapPreview(endpoints);
        ParseEndpoint.MapParse(endpoints);
        ParseEndpoint.MapGetLog(endpoints);
        SaveEndpoint.MapSaveResult(endpoints);

        FilesEndpoint.MapGetFiles(endpoints);
        FilesEndpoint.MapGetRaw(endpoints);
        FilesEndpoint.MapGetResults(endpoints);

        FilesEndpoint.MapDownloadRaw(endpoints);
        FilesEndpoint.MapGetLogs(endpoints);
        FilesEndpoint.MapGetLogsErrors(endpoints);
        FilesEndpoint.MapDownloadResult(endpoints);

        endpoints.MapPost("/api/database/delete", async (ApplicationContext db) =>
        {
            await db.Database.EnsureDeletedAsync();
            return Results.Ok(new { message = "Data base deleted." });
        });

        endpoints.MapFallback(() => Results.Json(new { error = "Map not found." }, statusCode: 404));
    }
}