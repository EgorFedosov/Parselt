using Backend.Core.Interfaces;

namespace Backend.Application.DTOs.Response;

public class ParseResultDto: IParseResultDto
{
    public List<CsvParsedRowDto> Rows { get; set; } = new();
    public Guid OperationId { get; set; }

}