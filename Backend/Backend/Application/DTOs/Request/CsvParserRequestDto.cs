namespace Backend.Application.DTOs.Request;

public class CsvParserRequestDto
{
    public uint FileId { get; set; }
    public string FileName { get; set; } = null!;

    public List<ColumnRuleDto> Rules { get; set; } = [];
    public string Delimiter { get; set; } = ",";
}