namespace Backend.Infrastructure.Data.Entities;

public class CsvParsedCell
{
    public uint Id { get; set; }
    public uint RowIndex { get; set; }
    public string ColumnName { get; set; } = null!;
    public string? Value { get; set; }
    public Guid OperationId { get; set; }
    public uint FileMetaDataId { get; set; }
    public FileMetaData FileMetaData { get; set; } = null!;
}