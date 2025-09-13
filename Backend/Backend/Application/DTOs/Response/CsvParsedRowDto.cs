namespace Backend.Application.DTOs.Response;

public class CsvParsedRowDto
{
    public uint RowIndex { get; set; }
    public Dictionary<string, object?> ParsedValues { get; set; } = new();
    public bool IsValid { get; set; }
    public Guid OperationId { get; set; }   
}