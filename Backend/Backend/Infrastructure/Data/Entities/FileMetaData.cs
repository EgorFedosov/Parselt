namespace Backend.Infrastructure.Data.Entities;

public class FileMetaData
{
    public uint Id { get; set; }
    public List<CsvRawCell> CsvRawCells { get; set; } = [];
    public List<CsvParsedCell>CsvParsedCells { get; set; } = [];
    public List<OperationLog> OperationLogs { get; set; } = [];
    public string FileName { get; set; } 
    public DateTime UploadedAt { get; set; } = DateTime.Now;
    public long Size { get; set; }
}