namespace Backend.Application.DTOs.Response;

public class CsvPreviewRawRowsDto
{
    public List<RawRowDto> Rows { get; set; } = new();
}