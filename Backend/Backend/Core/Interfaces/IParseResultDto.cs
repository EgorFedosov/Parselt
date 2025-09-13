using Backend.Application.DTOs.Response;

namespace Backend.Core.Interfaces;
using Application.DTOs;
public interface IParseResultDto
{
    public List<CsvParsedRowDto> Rows { get; set; } 
}