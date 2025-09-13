namespace Backend.Infrastructure.Data.Uploads;

using Entities;
using Core.Interfaces;
using Core.Logging.Enums;

public static class UploadFileToDataBase
{
    public async static Task AddRawFileToDataBase
    (IFormFile file,
        ILoggingService loggingService,
        ApplicationContext db,
        FileMetaData metaData)
    {
        var operation = new OperationLog
        {
            Type = OperationType.SaveToDatabaseRawFile,
        };
        loggingService.RegisterOperation(operation);
        loggingService.AttachMetaData(operation, metaData);
        db.Add(operation);
        await db.SaveChangesAsync(); // !!!

        using var reader = new StreamReader(file.OpenReadStream());
        var allCsvRawCells = new List<CsvRawCell>();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            allCsvRawCells.Add(new CsvRawCell
            {
                FileMetaData = metaData,
                FileMetaDataId = metaData.Id,
                Value = line,
            });
        }

        operation.TotalRows = allCsvRawCells.Count;
        operation.FinishedAt = DateTime.Now;
        await db.CsvRawCells.AddRangeAsync(allCsvRawCells); 
    }
}