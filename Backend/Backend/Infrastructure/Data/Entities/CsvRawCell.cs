namespace Backend.Infrastructure.Data.Entities;

public class CsvRawCell
{
    public uint Id { get; set; }
    public uint FileMetaDataId { get; set; }
    public FileMetaData FileMetaData { get; set; } = null!;
    public string? Value { get; set; }
}   