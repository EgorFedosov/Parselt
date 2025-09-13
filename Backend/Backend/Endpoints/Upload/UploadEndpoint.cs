namespace Backend.Endpoints.Upload;

using Infrastructure.Data;
using Core.Interfaces;

public static class UploadEndpoint
{
    public static void MapUpload(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/upload",
            async (HttpContext context, 
                ILoggingService loggingService,
                ApplicationContext db) =>
            {
                await UploadService.UploadFileAsync(context, loggingService, db);
            });
    }
}