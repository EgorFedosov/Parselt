using Backend.Infrastructure.Data.Entities;

namespace Backend.Infrastructure.Data;

using Application.DTOs.Logging;
using Core.Interfaces;
using Application.Validators;
using Utils;
using Core.Logging.Enums;
using Uploads;

public static class UploadService
{
    public static async Task UploadFileAsync(HttpContext context,
        ILoggingService loggingService,
        ApplicationContext db)
    {
        var response = context.Response;
        var request = context.Request;


        if (request.Method != "POST")
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            return;
        }

        if (!request.HasFormContentType)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "Content-Type must be form-data" });
            return;
        }

        IFormFileCollection files = request.Form.Files;

        if (files.Count == 0)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "No files were uploaded" });
            return;
        }

        var metaDataList = new List<FileMetaData>();
        var operation = new OperationLog
        {
            Type = OperationType.UploadFile,
            StartedAt = DateTime.Now
        };
        foreach (var file in files)
        {
            if (UploadFileValidator.IsCsv(file))
            {
                var metaData = new FileMetaData
                {
                    FileName = file.FileName,
                    Size = file.Length
                };
                await db.AddAsync(metaData);
                await db.SaveChangesAsync();
                Console.WriteLine("Meta Id:" + metaData.Id);
                operation.FileMetaDataId = metaData.Id;
                db.Add(operation);
                await db.SaveChangesAsync();

                loggingService.RegisterOperation(operation);
                loggingService.AttachMetaData(operation, metaData);

                await UploadFileToDataBase.AddRawFileToDataBase(file, loggingService, db, metaData);

                string fullPath = FilePathGenerator.GetUniqueFilePath(FilePathService.UploadPath, file.FileName);
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    await file.CopyToAsync(fileStream);


                db.Update(metaData);
                await db.SaveChangesAsync();
                metaDataList.Add(metaData);
            }
            else
            {
                loggingService.AddError(operation.OperationId, "The uploaded file format is invalid.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }


        var responseMetaData = metaDataList.Select(m => new
        {
            m.Id,
            m.FileName
        });

        await response.WriteAsJsonAsync(new { status = "Files uploaded successfully.", metaData = responseMetaData });
    }
    public static void CleanTempFiles(string tempPath, TimeSpan maxAge)
    {
        if (!Directory.Exists(tempPath)) return;

        foreach (var file in Directory.GetFiles(tempPath))
        {
            try
            {
                var creation = File.GetCreationTimeUtc(file);
                if (DateTime.UtcNow - creation > maxAge)
                    File.Delete(file);
            }
            catch
            {
                Console.WriteLine("Ошибка функции временной очистки");
            }
        }
    }

}